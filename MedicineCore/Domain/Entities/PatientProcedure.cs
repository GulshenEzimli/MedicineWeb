using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class PatientProcedure : IEntity
    {
        public int Id { get; set; }
        public int PatientId => Patient?.Id ?? 0;
        public int DoctorId => Doctor?.Id ?? 0;
        public int NurseId => Nurse?.Id ?? 0;
        public int ProcedureId => Procedure?.Id ?? 0;
        public DateTime UseDate { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Nurse Nurse { get; set; }
        public Procedure Procedure { get; set; }
    }
}
