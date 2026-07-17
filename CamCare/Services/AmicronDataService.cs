using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Models.Amicron;
using FirebirdSql.Data.FirebirdClient;
using Radzen;
using System.Text;

namespace CamCare.Services
{
    public class AmicronDataService(IConfiguration configuration, ILogger<AmicronDataService> logger) : IAmicronDataService
    {
        private string ConnectionString => configuration.GetConnectionString("AmicronDatabase") ?? throw new InvalidOperationException("Connection string 'AmicronDatabase' not found.");

        public async Task<ServiceResponse<Adressen>> GetAddressByCustomerIdAsync(int customerId)
        {
            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT LFDNR, NR, ART, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE FROM ADRESSEN WHERE LFDNR = @CustomerId";

            using var command = new FbCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);

            using var reader = await command.ExecuteReaderAsync();

            Adressen? result = null;
            if (await reader.ReadAsync())
            {
                result = new Adressen
                {
                    LfdNr = reader.GetInt32(0),
                    KdNummer = reader.GetString(1),
                    Art = reader.GetString(2),
                    Vorname = reader.GetString(3),
                    Name = reader.GetString(4),
                    Strasse = reader.GetString(5),
                    Land = reader.GetString(6),
                    Plz = reader.GetString(7),
                    Ort = reader.GetString(8),
                    Zahlweise = reader.GetString(9)
                };
            }

            return result != null
                ? ServiceResponse.Success(result)
                : ServiceResponse.Failure<Adressen>("Kein Datensatz gefunden.");
        }

        public async Task<ServiceResponse<List<Adressen>>> GetAddressesByCustomerIdsAsync(IEnumerable<int> customerIds)
        {
            var ids = customerIds.Distinct().ToList();
            if (ids.Count == 0)
                return ServiceResponse.Success(new List<Adressen>());

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();

            var parameterNames = ids.Select((_, index) => $"@CustomerId{index}").ToList();
            var query = $"SELECT LFDNR, NR, ART, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE FROM ADRESSEN WHERE LFDNR IN ({string.Join(", ", parameterNames)})";

            using var command = new FbCommand(query, connection);
            for (var i = 0; i < ids.Count; i++)
            {
                command.Parameters.AddWithValue(parameterNames[i], ids[i]);
            }

            using var reader = await command.ExecuteReaderAsync();

            var result = new List<Adressen>();
            while (await reader.ReadAsync())
            {
                result.Add(new Adressen
                {
                    LfdNr = reader.GetInt32(0),
                    KdNummer = reader.GetString(1),
                    Art = reader.GetString(2),
                    Vorname = reader.GetString(3),
                    Name = reader.GetString(4),
                    Strasse = reader.GetString(5),
                    Land = reader.GetString(6),
                    Plz = reader.GetString(7),
                    Ort = reader.GetString(8),
                    Zahlweise = reader.GetString(9)
                });
            }

            return ServiceResponse.Success(result);
        }

        public async Task<ServiceResponse<Paginated<Adressen>>> GetAllAddressAsync(LoadDataArgs args, AdressenFilter? filter = null)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;


            var result = new List<Adressen>();

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = new StringBuilder("SELECT FIRST @Top SKIP @Skip LFDNR, NR, ART, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE FROM ADRESSEN WHERE 1=1");
            var countQuery = new StringBuilder("SELECT COUNT(*) FROM ADRESSEN WHERE 1=1");

            using var command = new FbCommand();
            using var command2 = new FbCommand();

            if (filter?.KdNummer is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(KDNUMMER) LIKE UPPER(@KdNummer)", "@KdNummer", $"%{filter.KdNummer}%");
            }
            if (filter?.Name is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(NAME) LIKE UPPER(@Name)", "@Name", $"%{filter.Name}%");
            }
            if (filter?.Plz is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "PLZ LIKE @Plz", "@Plz", $"{filter.Plz}%");
            }
            if (filter?.Ort is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(ORT) LIKE UPPER(@Ort)", "@Ort", $"%{filter.Ort}%");
            }


            query.Append(" ORDER BY LFDNR");


            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query.ToString());
            command.Connection = connection;
            command.CommandText = query.ToString();

            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                result.Add(new Adressen
                {
                    LfdNr = reader.GetInt32(0),
                    KdNummer = reader.GetString(1),
                    Art = reader.GetString(2),
                    Vorname = reader.GetString(3),
                    Name = reader.GetString(4),
                    Strasse = reader.GetString(5),
                    Land = reader.GetString(6),
                    Plz = reader.GetString(7),
                    Ort = reader.GetString(8),
                    Zahlweise = reader.GetString(9)
                });
            }

            command2.Connection = connection;
            command2.CommandText = countQuery.ToString();
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Adressen>
            {
                Items = result,
                TotalCount = totalCount
            });
        }

        public async Task<ServiceResponse<Paginated<Adressen>>> GetAllAddressAsync(LoadDataArgs args, AdressenFilter filter, string serialnumber)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;

            var result = new List<Adressen>();

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = new StringBuilder("SELECT FIRST @Top SKIP @Skip a.LFDNR, NR, ART, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE" +
                " FROM ADRESSEN a" +
                " LEFT JOIN ARTSERNR ser ON a.LFDNR = ser.KUNDENLFDNR" +
                " WHERE ser.SERIENNR = @Serialnumber");

            var countQuery = new StringBuilder("SELECT COUNT(*)" +
                " FROM ADRESSEN" +
                " LEFT JOIN ARTSERNR ser ON ADRESSEN.LFDNR = ser.KUNDENLFDNR" +
                " WHERE ser.SERIENNR = @Serialnumber");

            using var command = new FbCommand();
            using var command2 = new FbCommand();
            command.Parameters.AddWithValue("@Serialnumber", serialnumber);
            command2.Parameters.AddWithValue("@Serialnumber", serialnumber);

            if (filter?.KdNummer is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(KDNUMMER) LIKE UPPER(@KdNummer)", "@KdNummer", $"%{filter.KdNummer}%");
            }
            if (filter?.Name is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(NAME) LIKE UPPER(@Name)", "@Name", $"%{filter.Name}%");
            }
            if (filter?.Plz is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "PLZ LIKE @Plz", "@Plz", $"{filter.Plz}%");
            }
            if (filter?.Ort is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(ORT) LIKE UPPER(@Ort)", "@Ort", $"%{filter.Ort}%");
            }


            query.Append(" ORDER BY LFDNR");


            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query.ToString());
            command.Connection = connection;
            command.CommandText = query.ToString();

            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                result.Add(new Adressen
                {
                    LfdNr = reader.GetInt32(0),
                    KdNummer = reader.GetString(1),
                    Art = reader.GetString(2),
                    Vorname = reader.GetString(3),
                    Name = reader.GetString(4),
                    Strasse = reader.GetString(5),
                    Land = reader.GetString(6),
                    Plz = reader.GetString(7),
                    Ort = reader.GetString(8),
                    Zahlweise = reader.GetString(9)
                });
            }

            command2.Connection = connection;
            command2.CommandText = countQuery.ToString();
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Adressen>
            {
                Items = result,
                TotalCount = totalCount
            });
        }

        public async Task<ServiceResponse<List<Serials>>> GetSerialsFromCustomerIdAsync(int customerId)
        {
            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT ser.SERIENNR, a.BEZEICHNUNG " +
                "FROM ARTSERNR ser " +
                "LEFT JOIN ARTIKEL a ON ser.ARTIKELLFDNR = a.LFDNR " +
                "WHERE ser.KUNDENLFDNR = @CustomerId";

            using var command = new FbCommand(query, connection);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            logger.LogDebug(query);

            using var reader = await command.ExecuteReaderAsync();

            var result = new List<Serials>();
            while (await reader.ReadAsync())
            {
                result.Add(new Serials
                {
                    Seriennummer = reader.GetString(0),
                    Artikelbezeichnung = reader.GetString(1),
                });
            }

            return result.Any()
                ? ServiceResponse.Success(result)
                : ServiceResponse.Failure<List<Serials>>("Kein Datensatz gefunden.");
        }
        public async Task<ServiceResponse<Paginated<Serials>>> GetSerialsFromCustomerIdAsync(LoadDataArgs args, int customerId)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;
            var filter = string.IsNullOrEmpty(args.Filter) ? null : $"%{args.Filter}%";
            var result = new List<Serials>();

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT FIRST @Top SKIP @Skip ser.LFDNR, ser.ARTIKELLFDNR, ser.SERIENNR, ser.KUNDENLFDNR, a.BEZEICHNUNG " +
                "FROM ARTSERNR ser " +
                "LEFT JOIN ARTIKEL a ON ser.ARTIKELLFDNR = a.LFDNR " +
                "WHERE ser.KUNDENLFDNR = @CustomerId";
            var countQuery = "SELECT COUNT(*) FROM ARTSERNR WHERE KUNDENLFDNR = @CustomerId";

            if (filter is not null)
            {
                query += " AND UPPER(ser.SERIENNR) LIKE UPPER(@Filter)";
                countQuery += " AND UPPER(SERIENNR) LIKE UPPER(@Filter)";
            }
            query += $" ORDER BY ser.LFDNR";

            logger.LogDebug(query);
            logger.LogDebug(countQuery);

            using var command = new FbCommand(query, connection);
            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);
            command.Parameters.AddWithValue("@CustomerId", customerId);
            command.Parameters.AddWithValue("@Filter", filter);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new Serials
                {
                    LfdNr = reader.GetInt32(0),
                    ArtikelLfdNr = reader.GetInt32(1),
                    Seriennummer = reader.GetString(2),
                    KundenLfdNr = reader.GetInt32(3),
                    Artikelbezeichnung = reader.GetString(4),
                });
            }

            using var command2 = new FbCommand(countQuery, connection);
            command2.Parameters.AddWithValue("@CustomerId", customerId);
            command2.Parameters.AddWithValue("@Filter", filter);
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Serials>
            {
                Items = result,
                TotalCount = totalCount
            });
        }

        public async Task<ServiceResponse<Paginated<Serials>>> GetAllSerialsAsync(LoadDataArgs args, SerialsFilter? filter = null)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;
            var result = new List<Serials>();


            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = new StringBuilder("SELECT FIRST @Top SKIP @Skip ser.LFDNR, ser.SERIENNR, a.BEZEICHNUNG, ad.NR, ad.NAME, ad.PLZ" +
                " FROM ARTSERNR ser " +
                " LEFT JOIN ARTIKEL a ON ser.ARTIKELLFDNR = a.LFDNR " +
                " LEFT JOIN ADRESSEN ad ON ser.KUNDENLFDNR = ad.LFDNR " +
                " WHERE 1=1");
            var countQuery = new StringBuilder("SELECT COUNT(*)" +
                " FROM ARTSERNR ser" +
                " LEFT JOIN ARTIKEL a ON ser.ARTIKELLFDNR = a.LFDNR " +
                " LEFT JOIN ADRESSEN ad ON ser.KUNDENLFDNR = ad.LFDNR " +
                " WHERE 1=1");

            using var command = new FbCommand();
            using var command2 = new FbCommand();

            if (!string.IsNullOrEmpty(args.Filter))
            {
                filter ??= new();
                filter.SerialNumber = args.Filter;
            }
            if (filter?.SerialNumber is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(ser.SERIENNR) LIKE UPPER(@Serialnumber)", "@Serialnumber", $"%{filter.SerialNumber}%");
            }
            if (filter?.ArticleName is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(a.BEZEICHNUNG) LIKE UPPER(@Article)", "@Article", $"%{filter.ArticleName}%");
            }
            if (filter?.CustomerName is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(ad.NAME) LIKE UPPER(@CustomerName)", "@CustomerName", $"%{filter.CustomerName}%");
            }
            if (filter?.CustomerNumber is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(ad.NR) LIKE UPPER(@CustomerNumber)", "@CustomerNumber", $"%{filter.CustomerNumber}%");
            }

            query.Append(" ORDER BY ser.LFDNR");

            logger.LogDebug(query.ToString());

            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query.ToString());
            command.Connection = connection;
            command.CommandText = query.ToString();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new Serials
                {
                    LfdNr = reader.GetInt32(0),
                    Seriennummer = reader.GetString(1),
                    Artikelbezeichnung = reader.GetString(2),
                    CustomerNumber = reader.GetString(3),
                    CustomerName = reader.GetString(4),
                    CustomerPlz = reader.GetString(5),
                });
            }

            logger.LogDebug(countQuery.ToString());
            command2.Connection = connection;
            command2.CommandText = countQuery.ToString();
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Serials>
            {
                Items = result,
                TotalCount = totalCount
            });
        }

        public async Task<ServiceResponse<Paginated<Artikel>>> GetArticleAsync(LoadDataArgs args, ArtikelFilter? filter = null)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;


            var result = new List<Artikel>();

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = new StringBuilder("SELECT FIRST @Top SKIP @Skip LFDNR, ARTIKELNR, BEZEICHNUNG, BESTAND, BESTANDMINDEST, MENGENEINHEIT, BILD FROM ARTIKEL WHERE 1=1");
            var countQuery = new StringBuilder("SELECT COUNT(*) FROM ARTIKEL WHERE 1=1");

            using var command = new FbCommand();
            using var command2 = new FbCommand();

            if (filter?.Artikelnummer is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(ARTIKELNR) LIKE UPPER(@Artikelnummer)", "@Artikelnummer", $"%{filter.Artikelnummer}%");
            }
            if (filter?.Description is not null)
            {
                AddFilterCondition(command, command2, query, countQuery, "UPPER(BEZEICHNUNG) LIKE UPPER(@Bezeichnung)", "@Bezeichnung", $"%{filter.Description}%");
            }

            query.Append(" ORDER BY LFDNR");


            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query.ToString());
            command.Connection = connection;
            command.CommandText = query.ToString();

            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                var artikel = new Artikel
                {
                    LfdNr = reader.GetInt32(0),
                    Artikelnummer = reader.GetString(1),
                    Description = reader.GetString(2),
                    InStock = reader.IsDBNull(3) ? null : reader.GetDecimal(3),
                    MinStock = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    Unit = reader.GetString(5),
                };

                //if (!reader.IsDBNull(5))
                //{
                //    using var stream = reader.GetStream(6);
                //    using var ms = new MemoryStream();
                //    await stream.CopyToAsync(ms);
                //    var blobBytes = ms.ToArray();
                //    artikel.Picture = blobBytes;
                //}
                result.Add(artikel);

            }
            logger.LogDebug(countQuery.ToString());
            command2.Connection = connection;
            command2.CommandText = countQuery.ToString();
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Artikel>
            {
                Items = result,
                TotalCount = totalCount
            });
        }

        private static void AddFilterCondition(
            FbCommand dataCommand,
            FbCommand countCommand,
            StringBuilder dataQuery,
            StringBuilder countQuery,
            string sqlCondition,
            string parameterName,
            object parameterValue)
        {
            dataCommand.Parameters.AddWithValue(parameterName, parameterValue);
            countCommand.Parameters.AddWithValue(parameterName, parameterValue);
            dataQuery.Append(" AND ").Append(sqlCondition);
            countQuery.Append(" AND ").Append(sqlCondition);
        }
    }
}
