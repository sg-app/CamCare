using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Models.Amicron;
using FirebirdSql.Data.FirebirdClient;
using Radzen;

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
    }
}
