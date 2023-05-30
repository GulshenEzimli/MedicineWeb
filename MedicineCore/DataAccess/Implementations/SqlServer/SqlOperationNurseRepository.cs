using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlOperationNurseRepository : IOperationNurseRepository
    {
        private readonly string _connectionString;
        public SqlOperationNurseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<OperationNurse> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select OperationNurses.Id as OperationNurseId, Nurses.Id as NurseId, Operations.Id as OperationId,
                                   DoctorPositions.Id as PositionId, Departments.Id as DepartmentId, 
                                   CreatorId, ModifierId, FirstName,LastName,Gender, BirthDate, PIN,
                                   Email,PhoneNumber, Salary,CreationDate, ModifiedDate, Nurses.IsDelete as NurseIsDelete, PositionName, DepartmentName
                                   from OperationNurses 
                                   inner join Nurses on OperationNurses.NurseId = Nurses.Id
                                   inner join Operations on OperationNurses.OperationId = Operations.Id
                                   inner join DoctorPositions on Nurses.PositionId = DoctorPositions.Id
                                   inner join Departments on DoctorPositions.DepartmentId = Departments.Id where Operations.IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<OperationNurse> operationNurses = new List<OperationNurse>();

                    while (reader.Read())
                    {
                        OperationNurse operationNurse = GetOperationNurse(reader);
                        operationNurses.Add(operationNurse);
                    }
                    return operationNurses;
                }
            }
        }


        public OperationNurse GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int Insert(OperationNurse entity)
        {
            throw new NotImplementedException();
        }

        public bool Update(OperationNurse entity)
        {
            throw new NotImplementedException();
        }
        private OperationNurse GetOperationNurse(SqlDataReader reader)
        {
            OperationNurse operationNurse = new OperationNurse();
            operationNurse.Id = reader.GetInt32("OperationNurseId");
            operationNurse.Nurse = new Nurse()
            {
                Id = reader.GetInt32("NurseId"),
                Position = new Position()
                {
                    Id = reader.GetInt32("PositionId"),
                    Name = reader.GetString("PositionName"),
                    Department = new Department()
                    {
                        Id = reader.GetInt32("DepartmentId"),
                        Name = reader.GetString("DepartmentName"),
                    },
                },
                Creator = new Admin()
                {
                    Id = reader.GetInt32("CreatorId")
                },

                Modifier = new Admin
                {
                    Id = reader.GetInt32("ModifierId")
                },
                FirstName = reader.GetString("FirstName"),
                LastName = reader.GetString("LastName"),
                Gender = reader.GetBoolean("Gender") ? Gender.Kişi : Gender.Qadın,
                PIN = reader.GetString("PIN"),
                Email = reader.GetString("Email"),
                PhoneNumber = reader.GetString("PhoneNumber"),
                BirthDate = reader.GetDateTime("BirthDate"),
                Salary = reader.GetDecimal("Salary"),
                CreationDate = reader.GetDateTime("CreationDate"),
                ModifiedDate = reader.GetDateTime("ModifiedDate"),
                IsDelete = reader.GetBoolean("NurseIsDelete")
            };
            operationNurse.Operation = new Operation()
            {
                Id = reader.GetInt32("OperationId")
            };
            return operationNurse;
        }
    }
}
