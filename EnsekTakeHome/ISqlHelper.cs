﻿using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SQLite;

namespace EnsekTakeHome
{
    public interface ISqlHelper
    {
        // SQL helper methods
        bool Insert(SQLiteConnection connection, string table, CsvData values);

        bool Update(SQLiteConnection connection, string table, CsvData values, string whereClause);

        bool DeleteAllAccounts(SQLiteConnection connection, string table, string whereClause);

        List<int> GetAllAccounts(SQLiteConnection connection, string table);

        DateTime GetLatestDateOnAccount(SQLiteConnection connection, string table, CsvData csvData);
        
        
    }
}
