using System.Text.RegularExpressions;

namespace CamCare.Services
{
    /// <summary>
    /// Zerlegt eine E-Mail-Vorlage in ihre Bausteine: unveränderter Text, Platzhalter,
    /// Bedingungsblöcke und Kommentare.
    /// </summary>
    /// <remarks>
    /// Unterstützte Schreibweisen:
    /// <list type="bullet">
    ///   <item><description><c>{{Token}}</c> – Wert des Platzhalters (leer, wenn kein Wert vorhanden ist)</description></item>
    ///   <item><description><c>{{Token|Ersatzwert}}</c> – Ersatzwert, wenn der Wert leer ist</description></item>
    ///   <item><description><c>{{#if Token}} … {{/if}}</c> – Inhalt nur bei gefülltem Wert, optional mit <c>{{else}}</c></description></item>
    ///   <item><description><c>{{#unless Token}} … {{/unless}}</c> – Inhalt nur bei leerem Wert</description></item>
    ///   <item><description><c>{{! Kommentar}}</c> – wird nicht ausgegeben</description></item>
    /// </list>
    /// Als Abschluss eines Blockes werden <c>{{/if}}</c> und <c>{{/unless}}</c> akzeptiert.
    /// Unbekannte Bausteine (z. B. ein Tippfehler in der Bedingung) bleiben als Text stehen.
    /// </remarks>
    internal static partial class MailTemplateParser
    {
        /// <summary>Erkennt einen Baustein der Vorlage; der Inhalt darf keine geschweiften Klammern enthalten.</summary>
        [GeneratedRegex(@"\{\{(?<body>[^{}]*)\}\}")]
        private static partial Regex TokenRegex();

        /// <summary>Erkennt einen Platzhalter mit optionalem Ersatzwert (z. B. <c>OrderNumber|keine Angabe</c>).</summary>
        [GeneratedRegex(@"^(?<token>[A-Za-z0-9_]+)\s*(\|(?<fallback>.*))?$", RegexOptions.Singleline)]
        private static partial Regex PlaceholderRegex();

        /// <summary>Erkennt den Beginn eines Bedingungsblockes (z. B. <c>#if OrderNumber</c> bzw. <c>#unless OrderNumber</c>).</summary>
        [GeneratedRegex(@"^#(?<keyword>if|unless)\s+(?<token>[A-Za-z0-9_]+)$", RegexOptions.IgnoreCase)]
        private static partial Regex ConditionStartRegex();

        /// <summary>Erkennt das Ende eines Bedingungsblockes (z. B. <c>/if</c>).</summary>
        [GeneratedRegex(@"^/(if|endif|unless|endunless)$", RegexOptions.IgnoreCase)]
        private static partial Regex ConditionEndRegex();

        /// <summary>
        /// Zerlegt die Vorlage in ihre Bausteine. Eine fehlerhafte Blockstruktur (z. B.
        /// <c>{{#if}}</c> ohne <c>{{/if}}</c>) führt dazu, dass die Vorlage wie Text behandelt
        /// wird: Platzhalter funktionieren weiterhin, die Blockplatzhalter bleiben sichtbar.
        /// </summary>
        public static IReadOnlyList<MailTemplateNode> Parse(string template)
        {
            var tokens = Tokenize(template);

            if (!IsStructureValid(tokens))
                return Flatten(tokens);

            var index = 0;
            return ReadNodes(tokens, ref index);
        }

        /// <summary>Zerlegt die Vorlage in Text- und Baustein-Token.</summary>
        private static List<MailTemplateToken> Tokenize(string template)
        {
            var tokens = new List<MailTemplateToken>();
            var position = 0;

            foreach (Match match in TokenRegex().Matches(template))
            {
                if (match.Index > position)
                    tokens.Add(new MailTemplateToken(MailTemplateTokenKind.Text, template[position..match.Index]));

                tokens.Add(CreateToken(match));
                position = match.Index + match.Length;
            }

            if (position < template.Length)
                tokens.Add(new MailTemplateToken(MailTemplateTokenKind.Text, template[position..]));

            return tokens;
        }

        /// <summary>Bestimmt die Art eines gefundenen Bausteins.</summary>
        private static MailTemplateToken CreateToken(Match match)
        {
            var raw = match.Value;
            var body = match.Groups["body"].Value.Trim();

            if (body.StartsWith('!'))
                return new MailTemplateToken(MailTemplateTokenKind.Comment, raw);

            if (body.Equals("else", StringComparison.OrdinalIgnoreCase))
                return new MailTemplateToken(MailTemplateTokenKind.Else, raw);

            var condition = ConditionStartRegex().Match(body);
            if (condition.Success)
            {
                var isNegated = condition.Groups["keyword"].Value.Equals("unless", StringComparison.OrdinalIgnoreCase);

                return new MailTemplateToken(MailTemplateTokenKind.OpenCondition, raw,
                    condition.Groups["token"].Value, IsNegated: isNegated);
            }

            if (ConditionEndRegex().IsMatch(body))
                return new MailTemplateToken(MailTemplateTokenKind.CloseCondition, raw);

            var placeholder = PlaceholderRegex().Match(body);
            if (placeholder.Success)
            {
                var fallback = placeholder.Groups["fallback"].Success
                    ? placeholder.Groups["fallback"].Value.Trim()
                    : null;

                return new MailTemplateToken(MailTemplateTokenKind.Placeholder, raw,
                    placeholder.Groups["token"].Value, fallback);
            }

            // Unbekannte Bausteine bleiben unverändert stehen, damit Tippfehler auffallen.
            return new MailTemplateToken(MailTemplateTokenKind.Text, raw);
        }

        /// <summary>
        /// Prüft, ob die Bedingungsblöcke vollständig und korrekt verschachtelt sind
        /// (jedes <c>{{#if}}</c> hat ein passendes Ende, <c>{{else}}</c> nur innerhalb eines Blockes).
        /// </summary>
        private static bool IsStructureValid(IReadOnlyList<MailTemplateToken> tokens)
        {
            var depth = 0;
            var elseSeenAtDepth = new HashSet<int>();

            foreach (var token in tokens)
            {
                switch (token.Kind)
                {
                    case MailTemplateTokenKind.OpenCondition:
                        depth++;
                        elseSeenAtDepth.Remove(depth);
                        break;

                    case MailTemplateTokenKind.Else:
                        // {{else}} außerhalb eines Blockes oder mehrfach im selben Block.
                        if (depth == 0 || !elseSeenAtDepth.Add(depth))
                            return false;
                        break;

                    case MailTemplateTokenKind.CloseCondition:
                        if (depth == 0)
                            return false;

                        elseSeenAtDepth.Remove(depth);
                        depth--;
                        break;
                }
            }

            return depth == 0;
        }

        /// <summary>
        /// Wandelt die Bausteine einer strukturell fehlerhaften Vorlage in Text um
        /// (Kommentare werden entfernt, Platzhalter bleiben nutzbar).
        /// </summary>
        private static IReadOnlyList<MailTemplateNode> Flatten(IReadOnlyList<MailTemplateToken> tokens)
        {
            var nodes = new List<MailTemplateNode>(tokens.Count);

            foreach (var token in tokens)
            {
                if (token.Kind == MailTemplateTokenKind.Comment)
                    continue;

                nodes.Add(token.Kind == MailTemplateTokenKind.Placeholder
                    ? new MailTemplatePlaceholderNode(token.Token!, token.Fallback, token.Raw)
                    : new MailTemplateTextNode(token.Raw));
            }

            return nodes;
        }

        /// <summary>
        /// Liest die Bausteine eines Blockes; bei <c>{{else}}</c> bzw. am Blockende endet
        /// die Liste, der Marker bleibt für den Aufrufer stehen.
        /// </summary>
        private static List<MailTemplateNode> ReadNodes(IReadOnlyList<MailTemplateToken> tokens, ref int index)
        {
            var nodes = new List<MailTemplateNode>();

            while (index < tokens.Count)
            {
                var token = tokens[index];

                switch (token.Kind)
                {
                    case MailTemplateTokenKind.Placeholder:
                        nodes.Add(new MailTemplatePlaceholderNode(token.Token!, token.Fallback, token.Raw));
                        index++;
                        break;

                    case MailTemplateTokenKind.Comment:
                        index++;
                        break;

                    case MailTemplateTokenKind.OpenCondition:
                        index++;
                        nodes.Add(ReadCondition(token, tokens, ref index));
                        break;

                    case MailTemplateTokenKind.Else:
                    case MailTemplateTokenKind.CloseCondition:
                        return nodes;

                    default:
                        nodes.Add(new MailTemplateTextNode(token.Raw));
                        index++;
                        break;
                }
            }

            return nodes;
        }

        /// <summary>Liest einen Bedingungsblock samt optionalem <c>{{else}}</c>-Zweig.</summary>
        private static MailTemplateConditionNode ReadCondition(
            MailTemplateToken openToken, IReadOnlyList<MailTemplateToken> tokens, ref int index)
        {
            var whenTrue = ReadNodes(tokens, ref index);
            var whenFalse = new List<MailTemplateNode>();

            if (index < tokens.Count && tokens[index].Kind == MailTemplateTokenKind.Else)
            {
                index++;
                whenFalse = ReadNodes(tokens, ref index);
            }

            // Das Blockende ({{/if}} bzw. {{/unless}}) überspringen – die Struktur ist geprüft.
            if (index < tokens.Count)
                index++;

            return new MailTemplateConditionNode(openToken.Token!, openToken.IsNegated, whenTrue, whenFalse);
        }

        /// <summary>Art eines Bausteins der Vorlage.</summary>
        private enum MailTemplateTokenKind
        {
            /// <summary>Unveränderter Text (auch unbekannte Bausteine).</summary>
            Text,

            /// <summary>Platzhalter (<c>{{Token}}</c> bzw. <c>{{Token|Ersatzwert}}</c>).</summary>
            Placeholder,

            /// <summary>Kommentar (<c>{{! …}}</c>).</summary>
            Comment,

            /// <summary>Beginn eines Bedingungsblockes (<c>{{#if Token}}</c> bzw. <c>{{#unless Token}}</c>).</summary>
            OpenCondition,

            /// <summary><c>{{else}}</c> innerhalb eines Bedingungsblockes.</summary>
            Else,

            /// <summary>Ende eines Bedingungsblockes (<c>{{/if}}</c> bzw. <c>{{/unless}}</c>).</summary>
            CloseCondition
        }

        /// <summary>Ein Baustein der Vorlage vor dem Aufbau der Blockstruktur.</summary>
        private sealed record MailTemplateToken(
            MailTemplateTokenKind Kind,
            string Raw,
            string? Token = null,
            string? Fallback = null,
            bool IsNegated = false);
    }

    /// <summary>Ein Baustein einer E-Mail-Vorlage.</summary>
    internal abstract record MailTemplateNode;

    /// <summary>Unveränderter Text der Vorlage.</summary>
    internal sealed record MailTemplateTextNode(string Text) : MailTemplateNode;

    /// <summary>Ein Platzhalter mit optionalem Ersatzwert.</summary>
    internal sealed record MailTemplatePlaceholderNode(string Token, string? Fallback, string Raw) : MailTemplateNode;

    /// <summary>
    /// Ein Bedingungsblock: <see cref="WhenTrue"/> wird ausgegeben, wenn der Platzhalterwert
    /// gefüllt ist (bei <see cref="IsNegated"/> umgekehrt), ansonsten <see cref="WhenFalse"/>.
    /// </summary>
    internal sealed record MailTemplateConditionNode(
        string Token,
        bool IsNegated,
        IReadOnlyList<MailTemplateNode> WhenTrue,
        IReadOnlyList<MailTemplateNode> WhenFalse) : MailTemplateNode;
}
