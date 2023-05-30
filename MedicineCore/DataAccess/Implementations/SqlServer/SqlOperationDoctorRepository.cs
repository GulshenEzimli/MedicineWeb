using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlOperationDoctorRepository : IOperationDoctorRepository
    {
        private readonly string _connectionString;
        public SqlOperationDoctorRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<OperationDoctor> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select OperationDoctors.Id as OperationDoctorId, Doctors.Id as DoctorId, Operations.Id as OperationId,
                                   DoctorPositions.Id as PositionId, Departments.Id as DepartmentId, CreatorId, ModifierId, FirstName, LastName, Gender,
                                   BirthDate, PIN, Email, Phonenumber, Salary, IsChiefDoctor, CreationDate, ModifiedDate, Doctors.IsDelete as DoctorIsDelete, PositionName, DepartmentName
                                   from OperationDoctors 
                                   inner join Doctors on OperationDoctors.DoctorId = Doctors.Id
                                   inner join Operations on OperationDoctors.OperationId = Operations.Id
                                   inner join DoctorPositions on Doctors.PositionId = DoctorPositions.Id
                                   inner join Departments on DoctorPositions.DepartmentId = Departments.Id where Operations.IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<OperationDoctor> operationDoctors = new List<OperationDoctor>();

                    while (reader.Read())
                    {
                        OperationDoctor operationDoctor = GetOperationDoctor(reader);
                        operationDoctors.Add(operationDoctor);
                    }
                    return operationDoctors;
                }
            }
        }

        public OperationDoctor GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int Insert(OperationDoctor entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(OperationDoctor entity)
        {
            throw new NotImplementedException();
        }


        private OperationDoctor GetOperationDoctor(SqlDataReader reader)
        {
            OperationDoctor operationDoctor = new OperationDoctor();
            operationDoctor.Id = reader.GetInt32("OperationDoctorId");
            operationDoctor.Doctor = new Doctor()
            {
                Id = reader.GetInt32("DoctorId"),
                Position = new Position()
                {
                    Id = reader.GetInt32("PositionId"),
                    Name = reader.GetString("PositionName"),
                    Department = new Department()
                    {
                        Id = reader.GetInt32("DepartmentId"),
                        Name = reader.GetString("DepartmentName")
                    }
                },
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Gender = reader.GetBoolean("Gender") ? Gender.Kişi : Gender.Qadın,
                PIN = reader.GetString("PIN"),
                Email = reader.GetString("Email"),
                PhoneNumber = reader.GetString("Phonenumber"),
                BirthDate = reader.GetDateTime("BirthDate"),
                Salary = reader.GetDecimal("Salary"),
                CreationDate = reader.GetDateTime("CreationDate"),
                ModifiedDate = reader.GetDateTime("ModifiedDate"),
                IsDelete = reader.GetBoolean("DoctorIsDelete"),
                IsChiefDoctor = reader.GetBoolean("IsChiefDoctor"),
                Creator = new Admin()
                {
                    Id = reader.GetInt32("CreatorId")
                },
                Modifier = new Admin()
                {
                    Id = reader.GetInt32("ModifierId")
                }
            };
            operationDoctor.Operation = new Operation()
            {
                Id = reader.GetInt32("OperationId")
            };
            return operationDoctor;
        }
    }
}
