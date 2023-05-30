using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlProcedureRepository : IProcedureRepository
    {
        private readonly string _connectionString;
        public SqlProcedureRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            using(SqlConnection connection=new SqlConnection())
            {
                connection.Open();
                string cmdText = @"Delete from Procedures where Id=@id";
                using(SqlCommand command=new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("Id",id);
                    return command.ExecuteNonQuery()==1;
                }
            }
        }

        public List<Procedure> Get()
        {
            using(SqlConnection connection=new SqlConnection(_connectionString))
            {
                connection.Open ();
                string cmdText = @"Select * from Procedures where IsDelete = 0";
                using(SqlCommand command=new SqlCommand(cmdText, connection))
                {
                    List<Procedure> procedures= new List<Procedure>();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Procedure procedure = GetProcedure(reader);
                        procedures.Add(procedure);
                    }
                    return procedures;
                }
            }
        }

        public Procedure GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select * from Procedures where Id = @id and IsDelete = 0 ";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read() == false)
                        return null;

                    return GetProcedure(reader);
                }
            }
        }

        public int Insert(Procedure procedure)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"Insert into Procedures output inserted.id values(@name,@cost,@isDelete)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {

                    AddParameters(command, procedure);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Procedure procedure)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"Update Procedures set Name=@name,Cost=@cost,IsDelete=@isDelete where Id=@id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("@id", procedure.Id);
                    AddParameters(command, procedure);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }
        #region GetProcedure
        private Procedure GetProcedure(SqlDataReader reader)
        {
            Procedure procedure = new Procedure();
            procedure.Id=reader.GetInt32("Id");
            procedure.Name = reader.GetString("Name");
            procedure.Cost = reader.GetDecimal("Cost");
            procedure.IsDelete = reader.GetBoolean("IsDelete");
            return procedure;
        }
        #endregion
        #region AddParameters
        private void AddParameters(SqlCommand command,Procedure procedure)
        {
            command.Parameters.AddWithValue("@name", procedure.Name);
            command.Parameters.AddWithValue("@cost", procedure.Cost);
            command.Parameters.AddWithValue("@isDelete", procedure.IsDelete);
        }
        #endregion
    }
}
