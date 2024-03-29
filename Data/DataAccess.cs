using Dapper;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace BankSystem.Data
{
    public class DataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataAccess()
        {
            _connectionString = "Server=127.0.0.1; Port=3306;Initial Catalog=bank-service; Username=root;Password=Acquy_21";
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public DataTable ExecuteQuery(string query, List<MySqlParameter> parameters = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        public void ExecuteNonQuery(string query, List<SqlParameter> parameters = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public T ExecuteStoredProcedure<T>(string storedProcedureName, DynamicParameters parameters)
        {
            using (IDbConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var result = connection.QueryFirstOrDefault<T>(storedProcedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

#pragma warning disable CS8603 // Possible null reference return.
                return result;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
    }
}
