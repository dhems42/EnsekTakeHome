using EnsekTakeHome.Repositories.Accounts;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SQLite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EnsekTakeHome
{
    public class SqlHelper : ISqlHelper
    {
        // SQL helper methods
        public bool Insert(SQLiteConnection connection, string table, AccountsCsvData values)
        {
            var rows = 0;

            var cmd = new SQLiteCommand(
                $"INSERT INTO {table} (AccountId,MeterReadingDateTime,MeterReadValue) VALUES (@param1,@param2,@param3)",
                connection);

            cmd.Parameters.AddWithValue("@param1", values.AccountId);
            cmd.Parameters.AddWithValue("@param2", values.MeterReadingDateTime);
            cmd.Parameters.AddWithValue("@param3", values.MeterReadValue);

            try
            {
                rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to Insert Row data: " + e);
            }
        }

        public bool Update(SQLiteConnection connection, string table, AccountsCsvData values, string whereClause)
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
                    accounts.Add(reader.GetInt32(1));
                }
            }

            return accounts;
        }

        public DateTime GetLatestDateOnAccount(SQLiteConnection connection, string table, AccountsCsvData csvData)
        {
            DateTime date = DateTime.MinValue;

            using (var cmd = new SQLiteCommand(
                $"SELECT MeterReadingDateTime FROM {table} WHERE AccountId == {csvData.AccountId}",
                connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime rowDateTime = reader.GetDateTime(0);
                        
                        date = rowDateTime > date ? rowDateTime : date;
                    }
                }
            }

            // If a date time is not retrieved - do we want to always update or never update?
            return DateTime.MinValue;
        }

        public bool MatchedEntry(SQLiteConnection connection, string table, AccountsCsvData csvData)
        {
            using (var cmd = new SQLiteCommand(
                $"SELECT * FROM {table} WHERE AccountId == {csvData.AccountId} AND " +
                    $"MeterReadingDateTime == '{csvData.MeterReadingDateTime}' AND " +
                    $"MeterReadValue == {csvData.MeterReadValue}",
                connection))
            {
                var reader = cmd.ExecuteReader();
                
                return reader.HasRows;
            }
        }
    }
}
