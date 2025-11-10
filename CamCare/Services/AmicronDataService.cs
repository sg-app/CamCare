using CamCare.Interfaces.Services;
using CamCare.Models;
using CamCare.Models.Amicron;
using FirebirdSql.Data.FirebirdClient;
using Radzen;
using System.Linq.Expressions;

namespace CamCare.Services
{
    public class AmicronDataService(IConfiguration configuration, ILogger<AmicronDataService> logger) : IAmicronDataService
    {
        private string ConnectionString => configuration.GetConnectionString("AmicronDatabase") ?? throw new InvalidOperationException("Connection string 'AmicronDatabase' not found.");

        public async Task<ServiceResponse<Adressen>> GetAddressByCustomerIdAsync(int customerId)
        {
            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT LFDNR, NR, ART, SUCHBEGRIFF, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE FROM ADRESSEN WHERE LFDNR = @CustomerId";

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
                    Suchbegriff = reader.GetString(3),
                    Vorname = reader.GetString(4),
                    Name = reader.GetString(5),
                    Strasse = reader.GetString(6),
                    Land = reader.GetString(7),
                    Plz = reader.GetString(8),
                    Ort = reader.GetString(9),
                    Zahlweise = reader.GetString(10)
                };
            }

            return result != null
                ? ServiceResponse.Success(result)
                : ServiceResponse.Failure<Adressen>("Kein Datensatz gefunden.");
        }

        public async Task<ServiceResponse<Paginated<Adressen>>> GetAllAddressAsync(LoadDataArgs args, Expression<Func<Adressen, bool>>? predicate = null)
        {
            var top = args.Top ?? 100;
            var skip = args.Skip ?? 0;
            var filter = string.IsNullOrEmpty(args.Filter) ? null : $"%{args.Filter}%";

            var result = new List<Adressen>();

            using var connection = new FbConnection(ConnectionString);
            await connection.OpenAsync();
            var query = "SELECT FIRST @Top SKIP @Skip LFDNR, NR, ART, SUCHBEGRIFF, VORNAME, NAME, STRASSE, LAND, PLZ, ORT, ZAHLWEISE FROM ADRESSEN";
            var countQuery = "SELECT COUNT(*) FROM ADRESSEN";

            if (filter is not null)
            {
                query += " WHERE UPPER(SUCHBEGRIFF) LIKE UPPER(@Filter)";
                countQuery += " WHERE UPPER(SUCHBEGRIFF) LIKE UPPER(@Filter)";
            }
            query += $" ORDER BY LFDNR";

            using var command = new FbCommand(query, connection);
            command.Parameters.AddWithValue("@Top", top);
            command.Parameters.AddWithValue("@Skip", skip);
            command.Parameters.AddWithValue("@Filter", filter);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new Adressen
                {
                    LfdNr = reader.GetInt32(0),
                    KdNummer = reader.GetString(1),
                    Art = reader.GetString(2),
                    Suchbegriff = reader.GetString(3),
                    Vorname = reader.GetString(4),
                    Name = reader.GetString(5),
                    Strasse = reader.GetString(6),
                    Land = reader.GetString(7),
                    Plz = reader.GetString(8),
                    Ort = reader.GetString(9),
                    Zahlweise = reader.GetString(10)
                });
            }
            
            using var command2 = new FbCommand(countQuery, connection);
            command2.Parameters.AddWithValue("@Filter", filter);
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
