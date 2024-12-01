using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SQLite;

namespace EnsekTakeHome
{
    public class SqlHelper : ISqlHelper
    {
        // SQL helper methods
        public bool Insert(SQLiteConnection connection, string table, CsvData values)
        {
            using (var cmd = new SQLiteCommand(
                $"INSERT INTO {table} HEADERS (AccountId,MeterReadingDateTime,MeterReadValue) VALUES ({values.AccountId},{values.MeterReadingDateTime},{values.MeterReadValue})",
                connection))
            {
                var rows = cmd.ExecuteNonQuery();

                return rows > 0;
            }
        }

        public bool Update(SQLiteConnection connection, string table, CsvData values, string whereClause)
        {
            using (var cmd = new SQLiteCommand(
                $"UPDATE {table} VALUES ({values.AccountId},{values.MeterReadingDateTime},{values.MeterReadValue}) WHERE {whereClause}",
                connection))
            {
                var rows = cmd.ExecuteNonQuery();

                return rows > 0;
            }
        }

        public bool DeleteAllAccounts(SQLiteConnection connection, string table, string whereClause)
        {
            using (var cmd = new SQLiteCommand(
                $"DELETE FROM {table} WHERE {whereClause}",
                connection))
            {
                var rows = cmd.ExecuteNonQuery();

                return rows > 0;
            }
        }

        public List<int> GetAllAccounts(SQLiteConnection connection, string table)
        {
            List<int> accounts = new List<int>();

            using (var cmd = new SQLiteCommand($"SELECT * FROM {table}", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    accounts.Add(reader.GetInt32(0));
                }
            }

            return accounts;
        }

        public DateTime GetLatestDateOnAccount(SQLiteConnection connection, string table, CsvData csvData)
        {
            using (var cmd = new SQLiteCommand(
                $"SELECT * WHERE AccountId == {csvData.AccountId}",
                connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetDateTime(1);
                    }
                }
            }

            // If a date time is not retrieved - do we want to always update or never update?
            return DateTime.MinValue;
        }
    }
}
