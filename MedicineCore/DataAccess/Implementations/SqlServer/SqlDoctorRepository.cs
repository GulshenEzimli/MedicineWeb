using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlDoctorRepository : IDoctorRepository
    {
        private readonly string _connectionString;
        public SqlDoctorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete from Doctors where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Doctor> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Doctors.Id as DoctorId, DoctorPositions.Id as PositionId, Departments.Id as DepartmentId, CreatorId, ModifierId, FirstName, LastName, Gender,
                                   BirthDate, PIN, Email, Phonenumber, Salary, IsChiefDoctor, CreationDate, ModifiedDate, IsDelete, PositionName, DepartmentName
                                   from Doctors 
                                   inner join DoctorPositions on Doctors.PositionId = DoctorPositions.Id
                                   inner join Departments on DoctorPositions.DepartmentId = Departments.Id where IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Doctor> doctors = new List<Doctor>();

                    while (reader.Read())
                    {
                        Doctor doctor = GetDoctor(reader);
                        doctors.Add(doctor);
                    }
                    return doctors;
                }
            }
        }

        public Doctor GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Doctors.Id as DoctorId, DoctorPositions.Id as PositionId, Departments.Id as DepartmentId, CreatorId, ModifierId, FirstName, LastName, Gender,
                                   BirthDate, PIN, Email, Phonenumber, Salary, IsChiefDoctor, CreationDate, ModifiedDate, IsDelete, PositionName, DepartmentName
                                   from Doctors 
                                   inner join DoctorPositions on Doctors.PositionId = DoctorPositions.Id
                                   inner join Departments on DoctorPositions.DepartmentId = Departments.Id where Doctors.Id = @id and IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() == false)
                        return null;
                    Doctor doctor = GetDoctor(reader);
                    return doctor;
                }
            }
        }

        public int Insert(Doctor doctor)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into Doctors output inserted.id 
                                   values(@creatorId, @modifierId, @positionId,
                                   @firstName, @lastName, @gender, @birthDate, @pin, @email, @phoneNumber, @salary, @isChiefDoctor, 
                                   @creationDate, @modifiedDate, @isDelete)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, doctor);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Doctor doctor)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update Doctors set PositionId=@positionId, FirstName=@firstname, LastName=@lastname,
                                 Gender=@gender, BirthDate=@birthdate, PIN=@pin, Email=@email, PhoneNumber=@phonenumber,
                                 Salary=@salary, IsChiefDoctor= @ischiefdoctor, IsDelete=@isdelete, CreationDate=@creationdate, ModifiedDate=@modifieddate,
                                 CreatorId=@creatorid, ModifierId=@modifierid where Id=@id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", doctor.Id);
                    AddParameters(command, doctor);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }
        private Doctor GetDoctor(SqlDataReader reader)
        {
            Doctor doctor = new Doctor();
            doctor.Id = reader.GetInt32("DoctorId");
            doctor.Position = new Position()
            {
                Id = reader.GetInt32("PositionId"),
                Name = reader.GetString("PositionName"),
                Department = new Department()
                {
                    Id = reader.GetInt32("DepartmentId"),
                    Name = reader.GetString("DepartmentName")
                }
            };
            doctor.FirstName = reader.GetString("FirstName");
            doctor.LastName = reader.GetString("LastName");
            doctor.Gender = reader.GetBoolean("Gender") ? Gender.Kişi : Gender.Qadın;
            doctor.PIN = reader.GetString("PIN");
            doctor.Email = reader.GetString("Email");
            doctor.PhoneNumber = reader.GetString("Phonenumber");
            doctor.BirthDate = reader.GetDateTime("BirthDate");
            doctor.Salary = reader.GetDecimal("Salary");
            doctor.CreationDate = reader.GetDateTime("CreationDate");
            doctor.ModifiedDate = reader.GetDateTime("ModifiedDate");
            doctor.IsDelete = reader.GetBoolean("IsDelete");
            doctor.IsChiefDoctor = reader.GetBoolean("IsChiefDoctor");
            doctor.Creator = new Admin()
            {
                Id = reader.GetInt32("CreatorId")
            };
            doctor.Modifier = new Admin()
            {
                Id = reader.GetInt32("ModifierId")
            };

            return doctor;
        }
        private void AddParameters(SqlCommand command, Doctor doctor)
        {
            command.Parameters.AddWithValue("positionId", doctor.Position.Id);
            command.Parameters.AddWithValue("firstName", doctor.FirstName);
            command.Parameters.AddWithValue("lastName", doctor.LastName);
            command.Parameters.AddWithValue("gender", doctor.Gender == Gender.Kişi ? true : false);
            command.Parameters.AddWithValue("birthDate", doctor.BirthDate);
            command.Parameters.AddWithValue("pin", doctor.PIN);
            command.Parameters.AddWithValue("email", doctor.Email);
            command.Parameters.AddWithValue("phoneNumber", doctor.PhoneNumber);
            command.Parameters.AddWithValue("salary", doctor.Salary);
            command.Parameters.AddWithValue("isDelete", doctor.IsDelete);
            command.Parameters.AddWithValue("isChiefDoctor", doctor.IsChiefDoctor);
            command.Parameters.AddWithValue("creationDate", doctor.CreationDate);
            command.Parameters.AddWithValue("modifiedDate", doctor.ModifiedDate);
            command.Parameters.AddWithValue("creatorId", doctor.CreatorId);
            command.Parameters.AddWithValue("modifierId", doctor.ModifierId);
        }
    }
}
