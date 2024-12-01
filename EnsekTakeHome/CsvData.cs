using CsvHelper.Configuration.Attributes;

namespace EnsekTakeHome
{
    public class CsvData
    {
        public int AccountId { get; set; }
        [Format("dd/MM/yyyy HH:mm")]
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; }
    }
}
