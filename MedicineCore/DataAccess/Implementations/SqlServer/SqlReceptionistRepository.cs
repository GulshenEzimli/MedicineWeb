using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlReceptionistRepository : IReceptionistRepository
    {
        private string _connectionString;
        public SqlReceptionistRepository( string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            using (SqlConnection connection= new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"Delete from Receptionist where Id=@id";
                using (SqlCommand command= new SqlCommand(cmdText,connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }
      

        public List<Receptionist> Get()
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Receptionist.Id as ReceptionistId, FirstName, LastName, Gender, BirthDate, PIN, Email, PhoneNumber,
                                   Salary, CreationDate, ModifiedDate, JobId, OtherJobs.JobName as JobName,  CreatorId, ModifierId
			                      from Receptionist inner join OtherJobs on Receptionist.JobId = OtherJobs.Id where IsDelete = 0 ";
                using(SqlCommand command= new SqlCommand( cmdText,connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Receptionist> receptionists = new List<Receptionist>();

                    while (reader.Read())
                    {
                        Receptionist receptionist = GetReceptionist(reader);
                        receptionists.Add(receptionist);
                    }
                    return receptionists;
                    
                    // yadindan cixir bax 
                }
            }
        }

        public Receptionist GetById(int id)
        {
            using (SqlConnection connection= new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Receptionist.Id as ReceptionistId, FirstName, LastName, Gender, BirthDate, PIN, Email, PhoneNumber,
                                   Salary, CreationDate, ModifiedDate, JobId, OtherJobs.JobName as JobName,  CreatorId, ModifierId
			                      from Receptionist inner join OtherJobs on Receptionist.JobId = OtherJobs.Id where IsDelete = 0 and Id=@id";

                using (SqlCommand command = new SqlCommand(cmdText,connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader= command.ExecuteReader();
                    Receptionist receptionist=GetReceptionist(reader);
                    return receptionist;    
                }
            }
        }

        public int Insert(Receptionist receptionist)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into Receptionist output inserted.id values(@creatorId,@modifierId,
                                 (select Id from OtherJobs where JobName = @jobName),
                                 @firstName,@lastName,@gender,
                                 @birthDate,@pin,@email,@phoneNumber,@salary,@creationDate,@modifiedDate,@isDelete)";
                using (SqlCommand command= new SqlCommand(cmdText,connection))
                {
                    AddParameters(command, receptionist);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Receptionist receptionist)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update Receptionist set JobId=(select Id from OtherJobs where JobName = @jobName), 
                                 FirstName=@firstName, LastName=@lastName,
                                 Gender=@gender, BirthDate=@birthDate, PIN=@pin, Email=@email, PhoneNumber=@phoneNumber,
                                 Salary=@salary, IsDelete=@isDelete, CreationDate=@creationDate, ModifiedD";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", receptionist.Id);
                    AddParameters(command, receptionist);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }
            

        private Receptionist GetReceptionist(SqlDataReader reader)
        {
            Receptionist receptionist = new Receptionist();

            receptionist.Id = reader.GetInt32("ReceptionistId");
            receptionist.FirstName= reader.GetString("FirstName");
            receptionist.LastName = reader.GetString("LastName");
            receptionist.Gender = reader.GetBoolean("Gender");
            receptionist.PIN = reader.GetString("PIN");
            receptionist.Email=reader.GetString("Email");
            receptionist.PhoneNumber = reader.GetString("PhoneNumber");
            receptionist.BirthDate = reader.GetDateTime("BirthDate");
            receptionist.Salary = reader.GetDecimal("Salary");
            receptionist.CreationDate = reader.GetDateTime("CreationDate");
            receptionist.ModifierDate = reader.GetDateTime("ModifierDate");
            receptionist.IsDelete = reader.GetBoolean("IsDeleted");


            receptionist.Job = new Job()
            {
                Id = reader.GetInt32("JobId"),
                Name = reader.GetString("JobName")
            };

            receptionist.Creator = new Admin()
            {
                Id = reader.GetInt32("CreatorId")
            };
            receptionist.Modifier = new Admin()
            {
                Id = reader.GetInt32("ModifierId")
            };
            
            return receptionist;
        }

        private void AddParameters(SqlCommand command, Receptionist receptionist)
        {
            command.Parameters.AddWithValue("creatorId", receptionist.Creator.Id);
            command.Parameters.AddWithValue("modifierId", receptionist.Modifier.Id);
            command.Parameters.AddWithValue("jobName", receptionist.Job.Name);
            command.Parameters.AddWithValue("firstName", receptionist.FirstName);
            command.Parameters.AddWithValue("lastName", receptionist.LastName);
            command.Parameters.AddWithValue("gender", receptionist.Gender);
            command.Parameters.AddWithValue("birthDate", receptionist.BirthDate);
            command.Parameters.AddWithValue("pin", receptionist.PIN);
            command.Parameters.AddWithValue("email", receptionist.Email);
            command.Parameters.AddWithValue("phoneNumber", receptionist.PhoneNumber);
            command.Parameters.AddWithValue("salary", receptionist.Salary);
            command.Parameters.AddWithValue("isDelete", receptionist.IsDelete);
            command.Parameters.AddWithValue("creationDate", receptionist.CreationDate);
            command.Parameters.AddWithValue("modifiedDate", receptionist.ModifierDate);
        }
    }
}
