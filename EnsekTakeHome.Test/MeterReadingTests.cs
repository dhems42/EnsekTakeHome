using Moq;
using System.Data;
using EnsekTakeHome.Controllers;
using Microsoft.AspNetCore.Http;
using System.Data.SQLite;
using EnsekTakeHome.Repositories.Accounts;

namespace EnsekTakeHome.Test
{
    public class MeterReadingTests
    {
        private SqlHelper _sqlHelper = new SqlHelper();
        private SQLiteConnection _connection;

        [SetUp]
        public void Setup()
        {
            // Create an in-memory SQLite connection
            _connection = new SQLiteConnection("Data Source=:memory:");
            _connection.Open();

            // Create the table
            using (var cmd = new SQLiteCommand("CREATE TABLE MeterReadings (Id INTEGER PRIMARY KEY, AccountId INTEGER, MeterReadingDateTime DATE, MeterReadValue  INTEGER);", _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void OneValidMeterReading_AddsOneValidEntry()
        {
            // Arrange
            var accountsRepo = new AccountsRepository(_connection);
            var csvData = new List<AccountsCsvData>{
                new AccountsCsvData
                {
                    AccountId = 2344,
                    MeterReadingDateTime = new DateTime(2019, 04, 22, 09, 24, 00),
                    MeterReadValue = 1002
                },
            };

            // Act
            var results = accountsRepo.HandleCsv(csvData);

            //Assert.IsNotNull(results);
            Assert.That(results.valid.Equals(1));
            Assert.That(results.invalid.Equals(0));
            Assert.That(_sqlHelper.GetAllAccounts(_connection, "MeterReadings").Count == 1);
        }

        [Test]
        public void OneInvalidMeterReading_AddsOneInvalidEntry_NoDBEntry()
        {
            // Arrange
            var accountsRepo = new AccountsRepository(_connection);
            var csvData = new List<AccountsCsvData>{
                new AccountsCsvData
                {
                    AccountId = 2344,
                    MeterReadingDateTime = new DateTime(2019, 04, 22, 09, 24, 00),
                    MeterReadValue = 1002222
                },
            };

            // Act
            var results = accountsRepo.HandleCsv(csvData);

            //Assert.IsNotNull(results);
            Assert.That(results.valid.Equals(0));
            Assert.That(results.invalid.Equals(1));
            Assert.That(_sqlHelper.GetAllAccounts(_connection, "MeterReadings").Count == 0);
        }

        [Test]
        public void TwoEntriesForOneAccount_AddsOneValidDBEntry()
        {
            // Arrange
            var accountsRepo = new AccountsRepository(_connection);
            var csvData = new List<AccountsCsvData>{
                new AccountsCsvData
                {
                    AccountId = 2344,
                    MeterReadingDateTime = new DateTime(2019, 04, 22, 09, 24, 00),
                    MeterReadValue = 1002
                },
                new AccountsCsvData
                {
                    AccountId = 2344,
                    MeterReadingDateTime = new DateTime(2019, 03, 22, 09, 24, 00),
                    MeterReadValue = 1002222
                },
            };

            // Act
            var results = accountsRepo.HandleCsv(csvData);

            //Assert.IsNotNull(results);
            Assert.That(results.valid.Equals(1));
            Assert.That(results.invalid.Equals(1));
            Assert.That(_sqlHelper.GetAllAccounts(_connection, "MeterReadings").Count == 1);
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }
    }
}