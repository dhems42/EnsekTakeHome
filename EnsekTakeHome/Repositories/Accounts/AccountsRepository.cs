using CsvHelper;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace EnsekTakeHome.Repositories.Accounts
{
    public class AccountsRepository : IAccountsRepository
    {

        private const string _tableName = "MeterReadings";
        private readonly SQLiteConnection _connection;

        public AccountsRepository(IDbConnection connection)
        {
            _connection = (SQLiteConnection)connection;
        }

        public (int valid, int invalid) HandleCsv(Stream stream)
        {
            List<AccountsCsvData> csvRowsRead = AccountsCsvValidator.ReadCsv(stream);

            stream.Dispose();
            return HandleCsv(csvRowsRead);
        }

        public (int valid, int invalid) HandleCsv(List<AccountsCsvData> csvData)
        {
            int validReadings = 0;
            int invalidReadings = 0;
            var sqlDB = new SqlHelper();
            List<AccountsCsvData> validData = new List<AccountsCsvData>();

            // Remove duplicates
            List<AccountsCsvData> csvRows = csvData.Distinct().ToList();

            invalidReadings += csvData.Count - csvRows.Count;
            
            foreach (var csvRow in csvRows)
            {
                // Validate each row, keep a count of valid rows
                if (!AccountsCsvValidator.ValidateRow(csvRow))
                {
                    invalidReadings++;
                    continue;
                }
                // Upload to DB
                // This account exists and needs updating
                // Check for duplicate entry
                bool matchedEntry = sqlDB.MatchedEntry((SQLiteConnection)_connection, _tableName, csvRow);

                if (matchedEntry)
                {
                    invalidReadings++;
                    continue;
                }

                // Get the current Date of this accounts last reading
                DateTime latest = sqlDB.GetLatestDateOnAccount((SQLiteConnection)_connection, _tableName, csvRow);

                // Uploaded reading is out of date
                if (latest > csvRow.MeterReadingDateTime)
                {
                    invalidReadings++;
                    continue;
                }

                // This is a new account and is valid, so can be inserted
                validReadings++;
                sqlDB.Insert((SQLiteConnection)_connection, _tableName, csvRow);
            }

            return (validReadings, invalidReadings);
        }
    }
}
