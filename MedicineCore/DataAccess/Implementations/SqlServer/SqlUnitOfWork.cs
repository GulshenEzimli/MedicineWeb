using HospitalCore.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.DataAccess.Implementations.SqlServer
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly string _connectionString;
        public SqlUnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDoctorRepository DoctorRepository => new SqlDoctorRepository(_connectionString);
        public IPositionRepoitory PositionRepository => new SqlPositionRepository(_connectionString);
        public IOperationRepository OperationRepository => new SqlOperationRepository(_connectionString);
        public IOperationDoctorRepository OperationDoctorRepository => new SqlOperationDoctorRepository(_connectionString);
        public IOperationNurseRepository OperationNurseRepository => new SqlOperationNurseRepository(_connectionString);

        public INurseRepository NurseRepository => new SqlNurseRepository(_connectionString);
        public IOtherEmployeeRepository OtherEmployeeRepository => new SqlOtherEmployeeRepository(_connectionString);
        public IPatientProcedureRepository PatientProcedureRepository => new SqlPatientProcedureRepository(_connectionString);
        public IJobRepository JobRepository => new SqlJobRepository(_connectionString);

        public IReceptionistRepository ReceptionistRepository => new SqlReceptionistRepository(_connectionString);
        public IPatientRepository PatientRepository => new SqlPatientRepository(_connectionString);
        public IProcedureRepository ProcedureRepository => new SqlProcedureRepository(_connectionString);

        public IRoomRepository RoomRepository => new SqlRoomRepository(_connectionString);

        public IAdminRepository AdminRepository => new SqlAdminRepository(_connectionString);
        public IQueueRepository QueueRepository => new SqlQueueRepository(_connectionString);
    }
}
