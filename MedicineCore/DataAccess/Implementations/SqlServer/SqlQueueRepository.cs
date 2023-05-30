using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlQueueRepository : IQueueRepository
    {
        private readonly string _connectionString;
        public SqlQueueRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete  from Queues where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<Queue> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Queues.Id as Id,QueueNumber,Date,Patients.Id as PatientId,
                                    Patients.FirstName as PatientName,Patients.LastName as PatientSurname,
                                    Patients.PIN as PatientPIN,Doctors.Id as DoctorId, Doctors.FirstName as DoctorName, 
                                    Doctors.LastName as DoctorSurname,Doctors.PIN as DoctorPIN, Doctors.PositionId as DoctorPositionId, 
                                    (select DepartmentId from DoctorPositions where DoctorPositions.Id = Doctors.PositionId) as DoctorDepartmentId,
                                    Procedures.Id as ProcedureId,Procedures.Name as ProcedureName
                                    from Queues inner join Doctors on Queues.DoctorId = Doctors.Id
                                    inner join Patients on Queues.PatientId = Patients.Id
                                    inner join Procedures on Queues.ProcedureId = Procedures.Id ";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<Queue> queues = new List<Queue>();
                    while (reader.Read())
                    {
                        Queue queue = GetQueue(reader);
                        queues.Add(queue);
                    }
                    return queues;
                }
            }
        }

        public Queue GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select Queues.Id as Id,QueueNumber,Date,Patients.Id as PatientId,
                                    Patients.FirstName as PatientName,Patients.LastName as PatientSurname,
                                    Patients.PIN as PatientPIN,Doctors.Id as DoctorId, Doctors.FirstName as DoctorName, 
                                    Doctors.LastName as DoctorSurname,Doctors.PIN as DoctorPIN, Doctors.PositionId as DoctorPositionId, 
                                    (select DepartmentId from DoctorPositions where DoctorPositions.Id = Doctors.PositionId) as DoctorDepartmentId,
                                    Procedures.Id as ProcedureId,Procedures.Name as ProcedureName
                                    from Queues inner join Doctors on Queues.DoctorId = Doctors.Id
                                    inner join Patients on Queues.PatientId = Patients.Id
                                    inner join Procedures on Queues.ProcedureId = Procedures.Id
                                    where Queues.Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    Queue queue = GetQueue(reader);
                    return queue;
                }
            }
        }

        public int Insert(Queue queue)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into Queues output inserted.Id 
                                    values (@doctorId,@patientId,@procedureId,@queueNumber,@date)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, queue);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public bool Update(Queue queue)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update Queues set DoctorId = @doctorId, PatientId=@patientId, 
                                   QueueNumber= @queueNumber, Date=@Date where Id=@id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("@id", queue.Id);
                    AddParameters(command, queue);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        #region GetQueue
        private Queue GetQueue(SqlDataReader reader)
        {
            Queue queue = new Queue();

            queue.Id = reader.GetInt32("Id");

            queue.Doctor = new Doctor() {
                Id = reader.GetInt32("DoctorId"),
                FirstName = reader.GetString("DoctorName"),
                LastName = reader.GetString("DoctorSurname"),
                PIN = reader.GetString("DoctorPIN"),
                Position=new Position()
                {
                    Id = reader.GetInt32("DoctorPositionId"),
                    Department = new Department()
                    {
                        Id = reader.GetInt32("DoctorDepartmentId"),
                    },
                },
            };
            queue.Patient = new Patient()
            {
                Id = reader.GetInt32("PatientId"),
                Name = reader.GetString("PatientName"),
                Surname = reader.GetString("PatientSurname"),
                PIN = reader.GetString("PatientPIN"),
            };
            queue.Procedure = new Procedure()
            {
                Id = reader.GetInt32("ProcedureId"),
                Name = reader.GetString("ProcedureName"),
            };
            queue.UseDate = reader.GetDateTime("Date");
            queue.QueueNumber = reader.GetInt32("QueueNumber");

            return queue;

        }
        #endregion

        #region AddParameters
        private void AddParameters(SqlCommand command,Queue queue)
        {
            command.Parameters.AddWithValue("@doctorId",queue.DoctorId);
            command.Parameters.AddWithValue("@patientId", queue.PatientId);
            command.Parameters.AddWithValue("@procedureId", queue.ProcedureId);
            command.Parameters.AddWithValue("@queueNumber", queue.QueueNumber);
            command.Parameters.AddWithValue("@date", queue.UseDate);

        }
        #endregion
    }
}
