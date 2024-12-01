using CsvHelper;
using System.Globalization;

namespace EnsekTakeHome
{
    public static class CsvValidator
    {
        static CsvValidator() { }

        public static List<CsvData> ReadCsv(string csv)
        {
            using (var reader = new StringReader(csv))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csvReader.GetRecords<CsvData>().ToList();
            }
        }

        public static bool ValidateRow (CsvData row)
        {
            if (row.AccountID.Equals(0))
            {
                return false;
            }

            // Meter Reading must not exceed NNNNN
            if (row.MeterReadValue > 99999)
            {
                return false;
            }

            return true;
        }
    }
}
