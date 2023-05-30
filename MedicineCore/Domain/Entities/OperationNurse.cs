using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class OperationNurse : IEntity
    {
        public int Id { get; set; }
        public int OperationId => Operation?.Id ?? 0;
        public int NurseId => Nurse?.Id ?? 0;
        public Operation Operation { get; set; }
        public Nurse Nurse { get; set; }
    }
}
