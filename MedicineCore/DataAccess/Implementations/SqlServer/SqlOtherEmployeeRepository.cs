using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlOtherEmployeeRepository : IOtherEmployeeRepository
    {
        private readonly string _connectionString;
        public SqlOtherEmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete from OtherEmployees where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<OtherEmployee> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select OtherEmployees.Id as OtherEmployeeId, CreatorId,ModifierId,OtherJobs.Id as 
                                JobId,OtherJobs.JobName as JobName,FirstName,LastName,Gender,BirthDate,
                                PIN,Email,PhoneNumber,Salary,
                                CreationDate,ModifiedDate,IsDelete from OtherEmployees  inner join 
                                OtherJobs on OtherEmployees.JobId = OtherJobs.Id where IsDelete=0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<OtherEmployee> otherEmployees = new List<OtherEmployee>();

                    while (reader.Read())
                    {
                        OtherEmployee otherEmployee = GetOtherEmployee(reader);
                        otherEmployees.Add(otherEmployee);
                    }
                    return otherEmployees;
                }
            }
        }

        public OtherEmployee GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select OtherEmployees.Id as OtherEmployeeId, CreatorId,ModifierId,OtherJobs.Id as 
                                JobId,OtherJobs.JobName as JobName,FirstName,LastName,Gender,BirthDate,
                                PIN,Email,PhoneNumber,Salary,CreationDate,ModifiedDate,IsDelete from 
                                OtherEmployees  inner join OtherJobs on OtherEmployees.JobId = OtherJobs.Id 
                                where OtherEmployees.Id=@id and IsDelete=0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() == false)
                        return null;
                    OtherEmployee otherEmployee = GetOtherEmployee(reader);
                    return otherEmployee;
                }
            }
        }

        public int Insert(OtherEmployee otherEmployee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into OtherEmployees output inserted.id values(@creatorId,
                                 @modifierId,@jobId, @firstName,@lastName,@gender,@birthDate,
                                 @pin,@email,@phoneNumber,@salary,@creationDate,@modifiedDate,@isDelete)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, otherEmployee);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(OtherEmployee otherEmployee)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update OtherEmployees set JobId=(select Id from OtherJobs where 
                                 OtherJobs.JobName = @jobName), FirstName=@firstName, LastName=@lastName,
                                 Gender=@gender, BirthDate=@birthDate, PIN=@pin, Email=@email, PhoneNumber=@phoneNumber,
                                 Salary=@salary, CreationDate=@creationDate, ModifiedDate=@modifiedDate,IsDelete=@isDelete,
                                 CreatorId=@creatorId, ModifierId=@modifierId where Id=@id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", otherEmployee.Id);
                    AddParameters(command, otherEmployee);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        private OtherEmployee GetOtherEmployee(SqlDataReader reader)
        {
            OtherEmployee otherEmployee = new OtherEmployee();

            otherEmployee.Id = reader.GetInt32("OtherEmployeeId");
            otherEmployee.Job = new Job()
            {
                Id = reader.GetInt32("JobId"),
                Name = reader.GetString("JobName"),
            };
            otherEmployee.Modifier = new Admin()
            {
                Id = reader.GetInt32("ModifierId")
            };
            otherEmployee.Creator = new Admin()
            {
                Id = reader.GetInt32("CreatorId")
            };
            otherEmployee.FirstName = reader.GetString("FirstName");
            otherEmployee.LastName = reader.GetString("LastName");
            otherEmployee.Gender = reader.GetBoolean("Gender") ? Gender.Kişi : Gender.Qadın;
            otherEmployee.PIN = reader.GetString("PIN");
            otherEmployee.Email = reader.GetString("Email");
            otherEmployee.PhoneNumber = reader.GetString("PhoneNumber");
            otherEmployee.BirthDate = reader.GetDateTime("BirthDate");
            otherEmployee.Salary = reader.GetDecimal("Salary");
            otherEmployee.CreationDate = reader.GetDateTime("CreationDate");
            otherEmployee.ModifiedDate = reader.GetDateTime("ModifiedDate");
            otherEmployee.IsDelete = reader.GetBoolean("IsDelete");

            return otherEmployee;
        }

        private void AddParameters(SqlCommand command, OtherEmployee otherEmployee)
        {
            command.Parameters.AddWithValue("creatorId", otherEmployee.Creator.Id);
            command.Parameters.AddWithValue("modifierId", otherEmployee.Modifier.Id);
            command.Parameters.AddWithValue("jobId", otherEmployee.Job.Id);
            command.Parameters.AddWithValue("jobName", otherEmployee.Job.Name);
            command.Parameters.AddWithValue("firstName", otherEmployee.FirstName);
            command.Parameters.AddWithValue("lastName", otherEmployee.LastName);
            command.Parameters.AddWithValue("gender", otherEmployee.Gender == Gender.Qadın ? false : true);
            command.Parameters.AddWithValue("birthDate", otherEmployee.BirthDate);
            command.Parameters.AddWithValue("pin", otherEmployee.PIN);
            command.Parameters.AddWithValue("email", otherEmployee.Email);
            command.Parameters.AddWithValue("phoneNumber", otherEmployee.PhoneNumber);
            command.Parameters.AddWithValue("salary", otherEmployee.Salary);
            command.Parameters.AddWithValue("creationDate", otherEmployee.CreationDate);
            command.Parameters.AddWithValue("modifiedDate", otherEmployee.ModifiedDate);
            command.Parameters.AddWithValue("isDelete", otherEmployee.IsDelete);
        }
    }
}
