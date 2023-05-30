using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Queue : IEntity
    {
        public int Id { get; set; }
        public int QueueNumber { get; set; }
        public DateTime UseDate { get; set; }

        public int PatientId => Patient?.Id ?? 0;
        public int DoctorId => Doctor?.Id ?? 0;
        public int ProcedureId => Procedure?.Id ?? 0;

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Procedure Procedure { get; set; }
    }
}
