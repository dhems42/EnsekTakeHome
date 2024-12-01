using CsvHelper.Configuration.Attributes;

namespace EnsekTakeHome.Repositories.Accounts
{
    public class AccountsCsvData
    {
        public int AccountId { get; set; }
        [Format("dd/MM/yyyy HH:mm")]
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; }
    }
}
