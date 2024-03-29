using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace BankSystem.Data
{
    public interface IDataAccess
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        DataTable ExecuteQuery(string query, List<MySqlParameter> parameters = null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        void ExecuteNonQuery(string query, List<SqlParameter> parameters = null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        T ExecuteStoredProcedure<T>(string storedProcedureName, DynamicParameters parameters);
    }
}
