using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;

namespace EnsekTakeHome.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CsvIngestionController : ControllerBase
    {
        private readonly IDbConnection _connection;

        private const string _tableName = "MeterReadings";

        public CsvIngestionController(IDbConnection connection)
        {
            _connection = connection;
        }

        [HttpPost(Name = "CsvIngestion")]
        public IActionResult UploadCsvFile([FromBody] string csvData)
        {
            var sqlDB = new SqlHelper();
            int validReadings = 0;
            int invalidReadings = 0;
            List<CsvData> validData = new List<CsvData>();

            // This will read the CSV file
            List<CsvData> csvRowsRead = CsvValidator.ReadCsv(csvData);

            // Remove duplicates
            List<CsvData> csvRows = csvRowsRead.Distinct().ToList();

            invalidReadings += csvRowsRead.Count - csvRows.Count;

            foreach (var csvRow in csvRows )
            {
                // Validate each row, keep a count of valid rows
                if (CsvValidator.ValidateRow(csvRow))
                {
                    validReadings++;
                    validData.Add(csvRow);
                }
                else 
                    invalidReadings++;
            }

            // Get all known accounts
            List<int> accounts = sqlDB.GetAllAccounts((SQLiteConnection) _connection, _tableName);

            foreach (var validDataRow in validData)
            {
                // Upload to DB
                // Check if the account exists
                if (accounts.Any(x => x.Equals(validDataRow.AccountID)))
                {
                    // This account exists and needs updating

                    // Get the current Date of this accounts last reading
                    DateTime latest = sqlDB.GetLatestDateOnAccount((SQLiteConnection)_connection, _tableName, validDataRow);

                    // Uploaded reading is out of date
                    if (latest > validDataRow.MeterReadingDateTime)
                    {
                        validReadings--;
                        invalidReadings++;
                        continue;
                    }

                }
                
                // This is a new account and is valid, so can be inserted
                sqlDB.Insert((SQLiteConnection)_connection, _tableName, validDataRow);
                
            }

            return Ok($"Csv file processed. {validReadings} Valid meter reading(s). {invalidReadings} Invalid meter reading(s).");
        }
    }
}
