window.downloadFileFromByteArray = function (fileName, byteArray) {
  var blob = new Blob([byteArray], { type: "application/octet-stream" });
  var url = URL.createObjectURL(blob);
  var a = document.createElement("a");
  a.style.display = "none";
  a.href = url;
  a.download = fileName;
  document.body.appendChild(a);
  a.click();
  URL.revokeObjectURL(url);
  document.body.removeChild(a);
};

// Rendert anhand der übergebenen URL einen QR-Code (qrcode.min.js, MIT, davidshimjs)
// in das Element mit der angegebenen ID. Vorheriger Inhalt wird ersetzt.
window.renderQrCode = function (elementId, text) {
  var container = document.getElementById(elementId);
  if (!container) return;
  container.innerHTML = "";
  new QRCode(container, { text: text, width: 100, height: 100, correctLevel: QRCode.CorrectLevel.M });
};

// Kopiert Text in die Zwischenablage (Fallback mit execCommand, falls Clipboard-API blockiert ist).
window.copyTextToClipboard = function (text) {
  if (navigator.clipboard && navigator.clipboard.writeText) {
    navigator.clipboard.writeText(text);
    return;
  }
  var ta = document.createElement("textarea");
  ta.value = text;
  ta.style.position = "fixed";
  ta.style.opacity = "0";
  document.body.appendChild(ta);
  ta.select();
  document.execCommand("copy");
  document.body.removeChild(ta);
};

// Verhindert das implizite Absenden eines <form>-Elements durch die Enter-Taste in Eingabefeldern.
// Ohne diesen Schutz löst ein Enter in einem <input> innerhalb eines RadzenTemplateForm das Submit
// aus (z. B. OnSaveClicked auf der EditRepairOrder-Seite), wodurch die Seite ungewollt geschlossen
// würde – etwa während der Bearbeitung einer neu eingefügten Grid-Zeile ("Beschreibung" eingeben
// und Enter drücken).
// Nur die Standard-Aktion (implizites Submit) wird unterdrückt; alle anderen Listener (z. B. Radzen
// AutoComplete/DataGrid) laufen weiter. Buttons (Speichern) bleiben per Enter/Klick funktionsfähig,
// Textareas und Selects werden nicht angetastet (Enter dort = neue Zeile bzw. Dropdown).
// Idempotent: Der Guard wird pro Formular nur einmal aktiviert.
window.camcarePreventImplicitFormSubmit = function (formElement) {
  if (!formElement || formElement.__camcareEnterGuard) return;
  formElement.__camcareEnterGuard = true;
  formElement.addEventListener("keydown", function (event) {
    var target = event.target;
    if (!target || target.tagName !== "INPUT") return;
    if (target.type === "submit" || target.type === "button") return;
    if (event.key === "Enter") {
      event.preventDefault();
    }
  });
};

// Liefert den aktuellen Wert des fokussierten Eingabefelds zurück. Wird genutzt, um die Eingabe
// eines RadzenAutoComplete bereits beim Enter-Druck zu übernehmen: Das change-Event (und damit
// die ValueChanged-Modellbindung) feuert sonst erst beim Verlassen des Felds (Blur).
window.camcareGetActiveElementValue = function () {
  var el = document.activeElement;
  if (el && (el.tagName === "INPUT" || el.tagName === "TEXTAREA")) {
    return el.value;
  }
  return null;
};

// Leert das Eingabefeld eines RadzenAutoComplete innerhalb des übergebenen Containers.
// Wird nach dem Übernehmen einer Eingabe benötigt: Der gebundene Wert wird auf null gesetzt,
// Radzen aktualisiert das Eingabefeld dabei aber nicht (es bleibt mit dem zuletzt erfassten
// Text gefüllt). Das input-Event informiert Radzen anschließend über den geleerten Wert.
// Der Container wird geprüft: Ist die Komponente (noch) nicht gerendert, wird ein leeres
// ElementReference übergeben, das kein DOM-Element ist.
window.camcareClearAutoComplete = function (container) {
  if (!isElement(container)) return;
  var input = container.querySelector("input.rz-autocomplete-input");
  if (!input) return;
  input.value = "";
  input.dispatchEvent(new Event("input", { bubbles: true }));
};

// Prüft, ob im übergebenen Container ein AutoComplete-Eintrag markiert ist (Navigation im
// Dropdown mit den Pfeiltasten). Nur dann übernimmt Radzen den Eintrag beim Enter-Druck selbst
// (SelectedItemChanged). Die Markierung wird über `aria-selected` vom Server gerendert und beim
// Schließen des Dropdowns wieder zurückgesetzt – sie ist damit immer aktuell.
window.camcareHasHighlightedAutoCompleteItem = function (container) {
  if (!isElement(container)) return false;
  return !!container.querySelector('.rz-autocomplete-list-item[aria-selected="true"]');
};

// Hilfsfunktion: `container` kann ein ElementReference sein, das (noch) nicht gerendert wurde
// und von Blazor als Objekt statt als DOM-Element übergeben wird.
function isElement(container) {
  return !!container && typeof container.querySelector === "function";
}
