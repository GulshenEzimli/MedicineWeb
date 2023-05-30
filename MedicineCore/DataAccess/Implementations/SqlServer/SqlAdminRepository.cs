using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlAdminRepository : IAdminRepository
    {
        private string _connectionString;
        public SqlAdminRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete from Admins where Id=@id";

                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Admin> Get()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.Open();
                string cmdText = @"select * from Admins";

                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Admin> admins = new List<Admin>();

                    while (reader.Read())
                    {
                        Admin admin = GetAdmin(reader);
                        admins.Add(admin);
                    }
                    return admins;
                }
            }
        }

        public Admin GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.Open();

                string cmdText = @"select * from Admins where Id = @id";

                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() == false)
                        return null;
                    Admin admin = GetAdmin(reader);
                    return admin;
                }
            }
        }

        public int Insert(Admin admin)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.Open();
                string cmdText = @"insert into Admins values(@username,@password)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, admin);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Admin admin)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update Admins set Username=@username,Password=@password where Id=@id";

                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", admin.Id);
                    AddParameters(command, admin);
                    return command.ExecuteNonQuery() == 1;

                }
            }
        }

        public Admin Get(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = $"select * from Admins where Username = '{username}'";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() == false)
                        return null;
                    Admin admin = GetAdmin(reader);
                    return admin;
                }
            }
        }
        private Admin GetAdmin(SqlDataReader reader)
        {
            Admin admin = new Admin();

            admin.Id = reader.GetInt32("Id");
            admin.UserName = reader.GetString("Username");
            admin.Password = reader.GetString("Password");

            return admin;
        }

        private void AddParameters(SqlCommand command, Admin admin)
        {
            command.Parameters.AddWithValue("username", admin.UserName);
            command.Parameters.AddWithValue("password", admin.Password);
        }
    }
}
