using BankSystem.Dtos.Request;
using BankSystem.Models;
using MySql.Data.MySqlClient;

namespace BankSystem.Data.Repositories
{
    public class UserRepository
    {
        private readonly IDataAccess _dataAccess;

        public UserRepository(IDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            string query = "SELECT * FROM User, RefreshToken WHERE refreshtoken.refreshToken = @refreshToken and User.id = refreshtoken.userId";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@refreshToken", refreshToken),
            };

            var dataTable = _dataAccess.ExecuteQuery(query, parameters);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                return new User
                {
                    Id = Convert.ToInt64(row["id"]),
                    Email = row["email"].ToString(),
                    Password = row["password"].ToString(),
                    Role = row["role"].ToString()
                };
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8601 // Possible null reference assignment.
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public User GetUserById(long id)
        {
            var query = "SELECT * FROM User WHERE id = @id";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@id", id),
            };

            var dataTable = _dataAccess.ExecuteQuery(query, parameters);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                var user = new User
                {
                    Id = Convert.ToInt64(row["id"]),
                    Email = row["email"].ToString(),
                    Password = row["password"].ToString(),
                    Role = row["role"].ToString()
                };
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8601 // Possible null reference assignment.

                return user;
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public User GetUserByEmail(string email)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string query = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@email", email),
            };

            query = "SELECT * FROM User WHERE email = @Email";

            var dataTable = _dataAccess.ExecuteQuery(query, parameters);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
                return new User
                {
                    Id = Convert.ToInt64(row["id"]),
                    Email = row["email"].ToString(),
                    Password = row["password"].ToString(),
                    Role = row["role"].ToString()
                };
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8601 // Possible null reference assignment.
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public bool AddUser(UserRequest userRequest)
        {
            string query = "INSERT INTO User (email, password, role) VALUES (@email, @password, @role)";

            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("@email", userRequest.Email),
                new MySqlParameter("@password", userRequest.Password),
                new MySqlParameter("@role", "ROLE_ADMIN"),
            };
            try
            {
                _dataAccess.ExecuteQuery(query, parameters);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}