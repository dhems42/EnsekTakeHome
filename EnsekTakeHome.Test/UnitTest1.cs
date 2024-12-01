using NUnit;
using Moq;
using System.Data;
using EnsekTakeHome.Controllers;

namespace EnsekTakeHome.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void OneValidMeterReading_AddsOneValidEntry()
        {
            // Arrange
            var mockDb = new Mock<IDbConnection>();
            var sqlHelper = new Mock<ISqlHelper>();
            var controller = new CsvIngestionController(mockDb.Object);
            var csvData = "AccountId,MeterReadingDateTime,MeterReadValue,\r\n2344,22/04/2019 09:24,1002";

            // Act
            var results = controller.UploadCsvFile();

            //Assert.IsNotNull(results);
            Assert.That(results.ToString().Contains("1 Valid meter reading(s). 0 Invalid meter reading(s)"), Is.True);
            Assert.That(sqlHelper.GetAllAccounts(mockDb, "TestTable").Count == 1, Is.True);
        }

        [Test]
        public void OneInvalidMeterReading_AddsOneInvalidEntry_NoDBEntry()
        {
            // Arrange
            var mockDb = new Mock<IDbConnection>();
            var sqlHelper = new Mock<ISqlHelper>();
            var controller = new CsvIngestionController(mockDb.Object);
            var csvData = "AccountId,MeterReadingDateTime,MeterReadValue,\r\n2344,22/04/2019 09:24,1002222";

            // Act
            var results = controller.UploadCsvFile();

            
            Assert.That(results.ToString().Contains("0 Valid meter reading(s). 1 Invalid meter reading(s)"), Is.True);
            Assert.That(sqlHelper.GetAllAccounts(mockDb, "TestTable").Count == 0, Is.True);
        }

        [Test]
        public void TwoEntriesForOneAccount_AddsOneValidDBEntry()
        {
            // Arrange
            var mockDb = new Mock<IDbConnection>();
            var sqlHelper = new Mock<ISqlHelper>();
            var controller = new CsvIngestionController(mockDb.Object);
            var csvData = "AccountId,MeterReadingDateTime,MeterReadValue,\r\n2344,22/04/2019 09:24,1002\r\n2344,23/04/2019 10:15,1004";

            // Act
            var results = controller.UploadCsvFile();

            
            Assert.That(results.ToString().Contains("0 Valid meter reading(s). 1 Invalid meter reading(s)"), Is.True);
            Assert.That(sqlHelper.GetAllAccounts(mockDb, "TestTable").Count == 1, Is.True);
        }
    }
}