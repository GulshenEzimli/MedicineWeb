using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class OperationDoctor : IEntity
    {
        public int Id { get; set; }
        public int OperationId => Operation?.Id ?? 0;
        public int DoctorId => Doctor?.Id ?? 0;
        public Operation Operation { get; set; }
        public Doctor Doctor { get; set; }
    }
}
