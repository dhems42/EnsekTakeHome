using CsvHelper;
using System.Globalization;

namespace EnsekTakeHome.Repositories.Accounts
{
    public static class AccountsCsvValidator
    {
        static AccountsCsvValidator() { }

        public static List<AccountsCsvData> ReadCsv(Stream csv)
        {
            using (var reader = new StreamReader(csv))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Return 1 row at a time and add error handling
                return csvReader.GetRecords<AccountsCsvData>().ToList();
            }
        }

        public static bool ValidateRow(AccountsCsvData row)
        {
            if (row.AccountId.Equals(0))
            {
                return false;
            }

            // Meter Reading must not exceed NNNNN
            if (row.MeterReadValue > 99999 || row.MeterReadValue < 0)
            {
                return false;
            }


            return true;
        }
    }
}
