using HospitalCore.DataAccess.Interfaces;
using HospitalCore.Domain.Entities;
using HospitalCore.Utils;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlPatientProcedureRepository : IPatientProcedureRepository
    {
        private readonly string _connectionString;
        public SqlPatientProcedureRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public bool Delete(int id)
        {
            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"delete  from PatientProcedures where Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        public List<PatientProcedure> Get()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select PatientProcedures.Id as PatientProceduresId,UseDate,Patients.Id as PatientId,
                                    Patients.FirstName as PatientName,Patients.LastName as PatientSurname,
                                    Patients.PIN as PatientPIN,Doctors.Id as DoctorId, Doctors.FirstName as DoctorName, 
                                    Doctors.LastName as DoctorSurname,Doctors.PIN as DoctorPIN, Doctors.PositionId as DoctorPositionId, 
                                    (select DepartmentId from DoctorPositions where DoctorPositions.Id = Doctors.PositionId) as DoctorDepartmentId,
                                    Nurses.Id as NurseId, Nurses.FirstName as NurseName,
                                    Nurses.LastName as NurseSurname, Nurses.PIN as NursePIN,Nurses.PositionId as NursePositionId,
                                    (select DepartmentId from DoctorPositions where  DoctorPositions.Id = Nurses.PositionId) as NurseDepartmentId,
                                    Procedures.Id as ProcedureId,Procedures.Name as ProcedureName,Procedures.Cost as Cost
                                    from PatientProcedures inner join Doctors on PatientProcedures.DoctorId = Doctors.Id
                                    inner join Nurses on PatientProcedures.NurseId = Nurses.Id
                                    inner join Patients on PatientProcedures.PatientId = Patients.Id
                                    inner join Procedures on PatientProcedures.ProcedureId = Procedures.Id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<PatientProcedure> patientProcedures = new List<PatientProcedure>();
                    while (reader.Read())
                    {
                        PatientProcedure patientProcedure = GetPatientProcedure(reader);
                        patientProcedures.Add(patientProcedure);
                    }
                    return patientProcedures;
                }
            }
        }

        public PatientProcedure GetById(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"select PatientProcedures.Id as PatientProceduresId,UseDate,Patients.Id as PatientId,
                                    Patients.FirstName as PatientName,Patients.LastName as PatientSurname,
                                    Patients.PIN as PatientPIN,Doctors.Id as DoctorId, Doctors.FirstName as DoctorName, 
                                    Doctors.LastName as DoctorSurname,Doctors.PIN as DoctorPIN, Doctors.PositionId as DoctorPositionId, 
                                    (select DepartmentId from DoctorPositions where DoctorPositions.Id = Doctors.PositionId) as DoctorDepartmentId,
                                    Nurses.Id as NurseId, Nurses.FirstName as NurseName,
                                    Nurses.LastName as NurseSurname, Nurses.PIN as NursePIN,Nurses.PositionId as NursePositionId,
                                    (select DepartmentId from DoctorPositions where  DoctorPositions.Id = Nurses.PositionId) as NurseDepartmentId,
                                    Procedures.Id as ProcedureId,Procedures.Name as ProcedureName,Procedures.Cost as Cost
                                    from PatientProcedures inner join Doctors on PatientProcedures.DoctorId = Doctors.Id
                                    inner join Nurses on PatientProcedures.NurseId = Nurses.Id
                                    inner join Patients on PatientProcedures.PatientId = Patients.Id
                                    inner join Procedures on PatientProcedures.ProcedureId = Procedures.Id
                                    where PatientProcedures.Id = @id";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    SqlDataReader reader = command.ExecuteReader();
                    PatientProcedure patientProcedure = GetPatientProcedure(reader);
                    return patientProcedure;
                }
            }
        }

        public int Insert(PatientProcedure patientProcedure)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"insert into PatientProcedures output inserted.Id 
                                    values (@patientId,@doctorId,@nurseId,@procedureId,@useDate)";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, patientProcedure);
                    return (int) command.ExecuteScalar();
                }
            }
        }

        public bool Update(PatientProcedure patientProcedure)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string cmdText = @"update PatientProcedures set PatientId=@patientId, DoctorId = @doctorId,
                                NurseId = @nurseId, ProcedureId= @procedureId, UseDate=@useDate";
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    AddParameters(command, patientProcedure);
                    return command.ExecuteNonQuery() == 1;
                }
            }
        }

        private PatientProcedure GetPatientProcedure(SqlDataReader reader)
        {
            PatientProcedure patientProcedure = new PatientProcedure();

            patientProcedure.Id = reader.GetInt32("PatientProceduresId");

            patientProcedure.Patient = new Patient()
            {
                Id = reader.GetInt32("PatientId"),
                Name = reader.GetString("PatientName"),
                Surname = reader.GetString("PatientSurname"),
                PIN = reader.GetString("PatientPIN"),
            };

            patientProcedure.Doctor = new Doctor()
            {
                Id = reader.GetInt32("DoctorId"),
                FirstName = reader.GetString("DoctorName"),
                LastName = reader.GetString("DoctorSurname"),
                PIN = reader.GetString("DoctorPIN"),
                Position = new Position()
                {
                    Id = reader.GetInt32("DoctorPositionId"),
                    Department = new Department()
                    {
                        Id = reader.GetInt32("DoctorDepartmentId"),
                    },
                },
            };

            patientProcedure.Nurse = new Nurse()
            {
                Id = reader.GetInt32("NurseId"),
                FirstName = reader.GetString("NurseName"),
                LastName = reader.GetString("NurseSurname"),
                PIN = reader.GetString("NursePIN"),
                Position = new Position()
                {
                    Id = reader.GetInt32("NursePositionId"),
                    Department = new Department()
                    {
                        Id = reader.GetInt32("NurseDepartmentId"),
                    },
                },
            };
             
            patientProcedure.Procedure = new Procedure()
            {
                Id = reader.GetInt32("ProcedureId"),
                Name = reader.GetString("ProcedureName"),
                Cost = reader.GetDecimal("Cost")
            };
            patientProcedure.UseDate = reader.GetDateTime("UseDate");

            return patientProcedure;
        }
        private void AddParameters(SqlCommand command, PatientProcedure patientProcedure)
        {
            command.Parameters.AddWithValue("patientId",patientProcedure.Patient.Id);
            command.Parameters.AddWithValue("doctorId",patientProcedure.Doctor.Id);
            command.Parameters.AddWithValue("nurseID",patientProcedure.Nurse.Id); 
            command.Parameters.AddWithValue("procedureId",patientProcedure.Procedure.Id); 
            command.Parameters.AddWithValue("useDate",patientProcedure.UseDate); 
        }
    }
}
