using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Domain.Enums;
using HospitalCore.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlOperationRepository : IOperationRepository
    {
        private readonly string _connectionString;
        public SqlOperationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var transaction = connection.BeginTransaction();
                string cmdText = @"delete from OperationDoctors where OperationId=@id";
                using (SqlCommand command = new SqlCommand(cmdText, connection, transaction))
                {
                    bool isSuccess = false;

                    try
                    {
                        command.Parameters.AddWithValue("id", id);
                        isSuccess = command.ExecuteNonQuery() == 1;
                        command.Parameters.Clear();

                        command.CommandText = @"delete from OperationNurses where OperationId=@id";
                        command.Parameters.AddWithValue("id", id);
                        isSuccess = command.ExecuteNonQuery() == 1;
                        command.Parameters.Clear();

                        command.CommandText = @"delete from Operations where Id=@id";
                        command.Parameters.AddWithValue("id", id);
                        isSuccess = command.ExecuteNonQuery() == 1;
                        command.Parameters.Clear();

                        transaction.Commit();
                        return isSuccess;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public List<Operation> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Operations.Id as OperationId, Patients.Id as PatientId, Rooms.Id as RoomId, OperationDate, OperationCost, OperationReason,
                                   CreatorId, ModifierId, Gender, BirthDate, PhoneNumber, CreationDate, ModifiedDate, Operations.IsDelete as OID, FirstName, LastName, PIN, 
                                   Number, BlockFloor, IsAvailable, Type, Patients.IsDelete as PID
                                   from Operations 
                                   inner join Patients on Operations.PatientId = Patients.Id
                                   inner join Rooms on Operations.RoomId = Rooms.Id where Operations.IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Operation> operations = new List<Operation>();

                    while (reader.Read())
                    {
                        Operation operation = GetOperation(reader);
                        operations.Add(operation);
                    }
                    return operations;
                }
            }
        }
        public Operation GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Operations.Id as OperationId, Patients.Id as PatientId, Rooms.Id as RoomId, OperationDate, OperationCost, OperationReason,
                                   CreatorId, ModifierId, Gender, BirthDate, PhoneNumber, CreationDate, ModifiedDate, Operations.IsDelete as OID, FirstName, LastName, PIN, 
                                   Number, BlockFloor, IsAvailable, Type, Patients.IsDelete as PID
                                   from Operations 
                                   inner join Patients on Operations.PatientId = Patients.Id
                                   inner join Rooms on Operations.RoomId = Rooms.Id where Operations.Id = @id and Operations.IsDelete = 0";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read() == false)
                        return null;
                    Operation operation = GetOperation(reader);
                    return operation;
                }
            }
        }

        public int Insert(Operation operation)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                string cmdText = @"insert into Operations output inserted.id 
                                   values(@patientId, @roomId, @operationDate, @operationCost, @operationReason, @isDelete)";

                using (SqlCommand command = new SqlCommand(cmdText, connection, transaction))
                {
                    int operationId = 0;
                    try
                    {
                        AddParametersOperation(command, operation);
                        operationId = (int)command.ExecuteScalar();

                        foreach (Doctor doctor in operation.Doctors)
                        {
                            command.CommandText = @"insert into OperationDoctors output inserted.id 
                                                    values(@operationId, @doctorId)";
                            AddParametersOperationDoctor(command, operationId, doctor);
                            command.ExecuteScalar();
                            command.Parameters.Clear();
                        }
                        foreach (Nurse nurse in operation.Nurses)
                        {
                            command.CommandText = @"insert into OperationNurses output inserted.id 
                                                    values(@operationId, @nurseId)";
                            AddParametersOperationNurse(command, operationId, nurse);
                            command.ExecuteScalar();
                            command.Parameters.Clear();
                        }
                        transaction.Commit();
                        return operationId;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

        public bool Update(Operation operation)
        {
            if (operation.IsDelete == false)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    string cmdText = @"update Operations set
                                PatientId=@patientId, RoomId=@roomId, OperationDate=@operationDate,
                                OperationCost=@operationCost, OperationReason=@operationReason, IsDelete=@isDelete
                                where Id=@id";

                    using (SqlCommand command = new SqlCommand(cmdText, connection, transaction))
                    {
                        bool isSuccess = false;
                        try
                        {
                            command.Parameters.AddWithValue("id", operation.Id);
                            AddParametersOperation(command, operation);
                            isSuccess = command.ExecuteNonQuery() == 1;
                            command.Parameters.Clear();

                            command.CommandText = @"delete from OperationDoctors where OperationId=@id";
                            command.Parameters.AddWithValue("id", operation.Id);
                            isSuccess = command.ExecuteNonQuery() == 1;
                            command.Parameters.Clear();

                            foreach (Doctor doctor in operation.Doctors)
                            {
                                command.CommandText = @"insert into OperationDoctors output inserted.id 
                                                    values(@operationId, @doctorId)";
                                AddParametersOperationDoctor(command, operation.Id, doctor);
                                command.ExecuteScalar();
                                command.Parameters.Clear();
                            }

                            command.CommandText = @"delete from OperationNurses where OperationId=@id";
                            command.Parameters.AddWithValue("id", operation.Id);
                            isSuccess = command.ExecuteNonQuery() == 1;
                            command.Parameters.Clear();

                            foreach (Nurse nurse in operation.Nurses)
                            {
                                command.CommandText = @"insert into OperationNurses output inserted.id 
                                                    values(@operationId, @nurseId)";
                                AddParametersOperationNurse(command, operation.Id, nurse);
                                command.ExecuteScalar();
                                command.Parameters.Clear();
                            }
                            transaction.Commit();
                            return isSuccess;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                }
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string cmdText = @"update Operations set
                                   PatientId=@patientId, RoomId=@roomId, OperationDate=@operationDate,
                                   OperationCost=@operationCost, OperationReason=@operationReason, IsDelete=@isDelete
                                   where Id=@id";
                    using (SqlCommand command = new SqlCommand(cmdText, connection))
                    {
                        command.Parameters.AddWithValue("id", operation.Id);
                        AddParametersOperation(command, operation);
                        return command.ExecuteNonQuery() == 1;
                    }
                }
            }
        }

        private Operation GetOperation(SqlDataReader reader)
        {
            Operation operation = new Operation();
            operation.Id = reader.GetInt32("OperationId");
            operation.OperationDate = reader.GetDateTime("OperationDate");
            operation.OperationCost = reader.GetDecimal("OperationCost");
            operation.OperationReason = reader.GetString("OperationReason");
            operation.IsDelete = reader.GetBoolean("OID");
            operation.Patient = new Patient()
            {
                Id = reader.GetInt32("PatientId"),
                Creator = new Admin()
                {
                    Id = reader.GetInt32("CreatorId")
                },
                Modifier = new Admin()
                {
                    Id = reader.GetInt32("ModifierId")
                },
                Gender = reader.GetBoolean("Gender") ? Gender.Kişi : Gender.Qadın,
                BirthDate = reader.GetDateTime("BirthDate"),
                Name = reader.GetString("FirstName"),
                Surname = reader.GetString("LastName"),
                PIN = reader.GetString("PIN"),
                PhoneNumber = reader.GetString("PhoneNumber"),
                CreationDate = reader.GetDateTime("CreationDate"),
                ModifiedDate = reader.GetDateTime("ModifiedDate"),
                IsDelete = reader.GetBoolean("PID")
            };
            operation.Room = new Room()
            {
                Id = reader.GetInt32("RoomId"),
                Number = reader.GetInt32("Number"),
                BlockFloor = reader.GetInt32("BlockFloor"),
                Type = (RoomTypes)reader.GetByte("Type"),
                IsAvailable = reader.GetBoolean("IsAvailable")
            };
            return operation;
        }

        private void AddParametersOperation(SqlCommand command, Operation operation)
        {
            command.Parameters.AddWithValue("patientId", operation.Patient.Id);
            command.Parameters.AddWithValue("roomId", operation.Room.Id);
            command.Parameters.AddWithValue("operationDate", operation.OperationDate);
            command.Parameters.AddWithValue("operationCost", operation.OperationCost);
            command.Parameters.AddWithValue("operationReason", operation.OperationReason);
            command.Parameters.AddWithValue("isDelete", operation.IsDelete);
        }

        private void AddParametersOperationDoctor(SqlCommand command, int operationId, Doctor doctor)
        {
            command.Parameters.AddWithValue("operationId", operationId);
            command.Parameters.AddWithValue("doctorId", doctor.Id);
        }

        private void AddParametersOperationNurse(SqlCommand command, int operationId, Nurse nurse)
        {
            command.Parameters.AddWithValue("operationId", operationId);
            command.Parameters.AddWithValue("nurseId", nurse.Id);
        }

        public bool UpdateForDelete(Operation operation)
        {
            throw new NotImplementedException();
        }
    }
}
