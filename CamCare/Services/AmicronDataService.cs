using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Models.Amicron;
using FirebirdSql.Data.FirebirdClient;
using Radzen;
using System.Runtime.ConstrainedExecution;

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

        public async Task<ServiceResponse<Paginated<Adressen>>> GetAllAddressAsync(LoadDataArgs args, AdressenFilter? filter = null)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;


            var result = new List<Adressen>();

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT FIRST @Top SKIP @Skip LFDNR, NR, ART, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE FROM ADRESSEN WHERE 1=1 ";
            var countQuery = "SELECT COUNT(*) FROM ADRESSEN WHERE 1=1 ";

            using var command = new FbCommand();
            using var command2 = new FbCommand();

            if (filter?.KdNummer is not null)
            {
                var kdNummer = $"%{filter.KdNummer}%";
                command.Parameters.AddWithValue("@KdNummer", kdNummer);
                command2.Parameters.AddWithValue("@KdNummer", kdNummer);
                query += " AND UPPER(KDNUMMER) LIKE UPPER(@KdNummer)";
                countQuery += " AND WHERE UPPER(KDNUMMER) LIKE UPPER(@KdNummer)";
            }
            if (filter?.Name is not null)
            {
                var name = $"%{filter.Name}%";
                command.Parameters.AddWithValue("@Name", name);
                command2.Parameters.AddWithValue("@Name", name);
                query += " AND UPPER(NAME) LIKE UPPER(@Name)";
                countQuery += " AND UPPER(NAME) LIKE UPPER(@Name)";
            }
            if (filter?.Plz is not null)
            {
                var plz = $"{filter.Plz}%";
                command.Parameters.AddWithValue("@Plz", plz);
                command2.Parameters.AddWithValue("@Plz", plz);
                query += " AND PLZ LIKE @Plz";
                countQuery += " AND PLZ LIKE @Plz";
            }
            if (filter?.Ort is not null)
            {
                var ort = $"%{filter.Ort}%";
                command.Parameters.AddWithValue("@Ort", ort);
                command2.Parameters.AddWithValue("@Ort", ort);
                query += " AND UPPER(ORT) LIKE UPPER(@Ort)";
                countQuery += " AND UPPER(ORT) LIKE UPPER(@Ort)";
            }


            query += $" ORDER BY LFDNR";


            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query);
            command.Connection = connection;
            command.CommandText = query;

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
            command2.CommandText = countQuery;
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Adressen>
            {
                Items = result,
                TotalCount = totalCount
            });
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
            var query = "SELECT FIRST @Top SKIP @Skip ser.LFDNR, ser.SERIENNR, a.BEZEICHNUNG, ad.NR, ad.NAME, ad.PLZ" +
                " FROM ARTSERNR ser " +
                " LEFT JOIN ARTIKEL a ON ser.ARTIKELLFDNR = a.LFDNR " +
                " LEFT JOIN ADRESSEN ad ON ser.KUNDENLFDNR = ad.LFDNR " +
                " WHERE 1=1";
            var countQuery = "SELECT COUNT(*)" +
                " FROM ARTSERNR ser" +
                " LEFT JOIN ARTIKEL a ON ser.ARTIKELLFDNR = a.LFDNR " +
                " LEFT JOIN ADRESSEN ad ON ser.KUNDENLFDNR = ad.LFDNR " +
                " WHERE 1=1";

            using var command = new FbCommand();
            using var command2 = new FbCommand();

            if (filter?.SerialNumber is not null)
            {
                var serialNumber = $"%{filter.SerialNumber}%";
                command.Parameters.AddWithValue("@Serialnumber", serialNumber);
                command2.Parameters.AddWithValue("@Serialnumber", serialNumber);
                query += " AND UPPER(ser.SERIENNR) LIKE UPPER(@Serialnumber)";
                countQuery += " AND UPPER(ser.SERIENNR) LIKE UPPER(@Serialnumber)";
            }
            if (filter?.ArticleName is not null)
            {
                var articleName = $"%{filter.ArticleName}%";
                command.Parameters.AddWithValue("@Article", articleName);
                command2.Parameters.AddWithValue("@Article", articleName);
                query += " AND UPPER(a.BEZEICHNUNG) LIKE UPPER(@Article)";
                countQuery += " AND UPPER(a.BEZEICHNUNG) LIKE UPPER(@Article)";
            }
            if (filter?.CustomerName is not null)
            {
                var customerName = $"%{filter.CustomerName}%";
                command.Parameters.AddWithValue("@CustomerName", customerName);
                command2.Parameters.AddWithValue("@CustomerName", customerName);
                query += " AND UPPER(ad.NAME) LIKE UPPER(@CustomerName)";
                countQuery += " AND UPPER(ad.NAME) LIKE UPPER(@CustomerName)";
            }
            if (filter?.CustomerNumber is not null)
            {
                var customerNumber = $"%{filter.CustomerNumber}%";
                command.Parameters.AddWithValue("@CustomerNumber", customerNumber);
                command2.Parameters.AddWithValue("@CustomerNumber", customerNumber);
                query += " AND UPPER(ad.NR) LIKE UPPER(@CustomerNumber)";
                countQuery += " AND UPPER(ad.NR) LIKE UPPER(@CustomerNumber)";
            }

            query += $" ORDER BY ser.LFDNR";

            logger.LogDebug(query);

            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query);
            command.Connection = connection;
            command.CommandText = query;
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

            logger.LogDebug(countQuery);
            command2.Connection = connection;
            command2.CommandText = countQuery;
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
            var query = "SELECT FIRST @Top SKIP @Skip LFDNR, ARTIKELNR, BEZEICHNUNG, BESTAND, BESTANDMINDEST, MENGENEINHEIT, BILD FROM ARTIKEL WHERE 1=1 ";
            var countQuery = "SELECT COUNT(*) FROM ARTIKEL WHERE 1=1 ";

            using var command = new FbCommand();
            using var command2 = new FbCommand();

            if (filter?.Artikelnummer is not null)
            {
                var artikelNummer = $"%{filter.Artikelnummer}%";
                command.Parameters.AddWithValue("@Artikelnummer", artikelNummer);
                command2.Parameters.AddWithValue("@Artikelnummer", artikelNummer);
                query += " AND UPPER(ARTIKELNR) LIKE UPPER(@Artikelnummer)";
                countQuery += " AND UPPER(ARTIKELNR) LIKE UPPER(@Artikelnummer)";
            }
            if (filter?.Description is not null)
            {
                var description = $"%{filter.Description}%";
                command.Parameters.AddWithValue("@Bezeichnung", description);
                command2.Parameters.AddWithValue("@Bezeichnung", description);
                query += " AND UPPER(BEZEICHNUNG) LIKE UPPER(@Bezeichnung)";
                countQuery += " AND UPPER(BEZEICHNUNG) LIKE UPPER(@Bezeichnung)";
            }

            query += $" ORDER BY LFDNR";


            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);

            logger.LogDebug(query);
            command.Connection = connection;
            command.CommandText = query;

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
            logger.LogDebug(countQuery);
            command2.Connection = connection;
            command2.CommandText = countQuery;
            var countReader = await command2.ExecuteScalarAsync();
            var totalCount = Convert.ToInt32(countReader);

            return ServiceResponse.Success(new Paginated<Artikel>
            {
                Items = result,
                TotalCount = totalCount
            });
        }
    }
}
