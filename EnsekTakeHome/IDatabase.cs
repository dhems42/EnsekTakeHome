using Microsoft.Extensions.Primitives;
using System.Data;

namespace EnsekTakeHome
{
    public interface IDatabase
    {
        abstract bool Insert(IDbConnection connection, string table, CsvData values);
        abstract bool Update(IDbConnection connection, string table, CsvData values, string whereClause);
        abstract bool Delete(IDbConnection connection, string table, string whereClause);
    }
}
