using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        IDoctorRepository DoctorRepository { get; }
        IPositionRepoitory PositionRepository { get; }
        INurseRepository NurseRepository { get; }
        IOtherEmployeeRepository OtherEmployeeRepository { get; }
        IPatientProcedureRepository PatientProcedureRepository { get; }
        IJobRepository JobRepository { get; }
        IRoomRepository RoomRepository { get; }
        IAdminRepository AdminRepository { get; }
        IReceptionistRepository ReceptionistRepository { get; }
        IPatientRepository PatientRepository { get; }
        IProcedureRepository ProcedureRepository { get; }
        IOperationRepository OperationRepository { get; }
        IOperationDoctorRepository OperationDoctorRepository { get; }
        IOperationNurseRepository OperationNurseRepository { get; }
        IQueueRepository QueueRepository { get; }
    }
}
