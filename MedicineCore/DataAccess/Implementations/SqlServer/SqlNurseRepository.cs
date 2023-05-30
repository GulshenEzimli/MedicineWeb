using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlNurseRepository : INurseRepository
    {
        private readonly string _connectionString;
        public SqlNurseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete from Nurses where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Nurse> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Nurses.Id as NurseId,DoctorPositions.Id as PositionId,Departments.Id as  
                                DepartmentId, CreatorId, ModifierId, FirstName,LastName,Gender, BirthDate, PIN,
                                Email,PhoneNumber, Salary,CreationDate, ModifiedDate,IsDelete,PositionName, DepartmentName 
                                from Nurses inner join DoctorPositions on Nurses.PositionId = DoctorPositions.Id
                                inner join Departments on DoctorPositions.DepartmentId= Departments.Id where IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Nurse> nurses = new List<Nurse>();

                    while (reader.Read())
                    {
                        Nurse nurse = GetNurse(reader);
                        nurses.Add(nurse);
                    }
                    return nurses;
                }
            }
        }

        public Nurse GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Nurses.Id as NurseId,CreatorId, ModifierId, FirstName,LastName,Gender, BirthDate, PIN,
                                Email,PhoneNumber, Salary,CreationDate, ModifiedDate,IsDelete,DoctorPositions.Id as 
                                PositionId,PositionName,Departments.Id as  DepartmentId,  DepartmentName 
                                from Nurses inner join DoctorPositions on Nurses.PositionId = DoctorPositions.Id
                                inner join Departments on DoctorPositions.DepartmentId= Departments.Id 
                                where Nurses.Id=@id and IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() == false)
                        return null;
                    Nurse nurse = GetNurse(reader);
                    return nurse;
                }
            }
        }

        public int Insert(Nurse nurse)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into Nurses output inserted.id values(@creatorId,@modifierId,
                                 @positionId, @firstName,@lastName,@gender,
                                 @birthDate,@pin,@email,@phoneNumber,@salary,@creationDate,@modifiedDate,@isDelete)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, nurse);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Nurse nurse)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update Nurses set PositionId=@positionId, 
                                 FirstName=@firstName, LastName=@lastName,
                                 Gender=@gender, BirthDate=@birthDate, PIN=@pin, Email=@email, PhoneNumber=@phoneNumber,
                                 Salary=@salary, IsDelete=@isDelete, CreationDate=@creationDate, ModifiedDate=@modifiedDate,
                                 CreatorId=@creatorId, ModifierId=@modifierId where Id=@id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", nurse.Id);
                    AddParameters(command, nurse);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        private Nurse GetNurse(SqlDataReader reader)
        {
            Nurse nurse = new Nurse();
            nurse.Id = reader.GetInt32("NurseId");
            nurse.Position = new Position()
            {
                Id = reader.GetInt32("PositionId"),
                Name = reader.GetString("PositionName"),
                Department = new Department()
                {
                    Id = reader.GetInt32("DepartmentId"),
                    Name = reader.GetString("DepartmentName"),
                },
            };
            nurse.Creator = new Admin()
            {
                Id = reader.GetInt32("CreatorId")
            };

            nurse.Modifier = new Admin
            {
                Id = reader.GetInt32("ModifierId")
            };
            nurse.FirstName = reader.GetString("FirstName");
            nurse.LastName = reader.GetString("LastName");
            nurse.Gender = reader.GetBoolean("Gender") ? Gender.Kişi : Gender.Qadın;
            nurse.PIN = reader.GetString("PIN");
            nurse.Email = reader.GetString("Email");
            nurse.PhoneNumber = reader.GetString("PhoneNumber");
            nurse.BirthDate = reader.GetDateTime("BirthDate");
            nurse.Salary = reader.GetDecimal("Salary");
            nurse.CreationDate = reader.GetDateTime("CreationDate");
            nurse.ModifiedDate = reader.GetDateTime("ModifiedDate");
            nurse.IsDelete = reader.GetBoolean("IsDelete");

            return nurse;
        }

        private void AddParameters(SqlCommand command, Nurse nurse)
        {
            command.Parameters.AddWithValue("creatorId", nurse.Creator.Id);
            command.Parameters.AddWithValue("modifierId", nurse.Modifier.Id);
            command.Parameters.AddWithValue("positionId", nurse.Position.Id);
            command.Parameters.AddWithValue("firstName", nurse.FirstName);
            command.Parameters.AddWithValue("lastName", nurse.LastName);
            command.Parameters.AddWithValue("gender", nurse.Gender == Gender.Qadın ? false : true);
            command.Parameters.AddWithValue("birthDate", nurse.BirthDate);
            command.Parameters.AddWithValue("pin", nurse.PIN);
            command.Parameters.AddWithValue("email", nurse.Email);
            command.Parameters.AddWithValue("phoneNumber", nurse.PhoneNumber);
            command.Parameters.AddWithValue("salary", nurse.Salary);
            command.Parameters.AddWithValue("isDelete", nurse.IsDelete);
            command.Parameters.AddWithValue("creationDate", nurse.CreationDate);
            command.Parameters.AddWithValue("modifiedDate", nurse.ModifiedDate);
        }
    }
}
