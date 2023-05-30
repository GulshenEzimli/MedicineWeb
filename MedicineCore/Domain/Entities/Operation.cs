using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Operation : IEntity
    {
        public Operation()
        {
            Doctors = new List<Doctor>();
            Nurses = new List<Nurse>();
        }
        public int Id { get; set; }
        public DateTime? OperationDate { get; set; }
        public decimal? OperationCost { get; set; }
        public string? OperationReason { get; set; }
        public bool? IsDelete { get; set; }
        public int? PatientId => Patient?.Id ?? 0;
        public int? RoomId => Room?.Id ?? 0;
        public Patient? Patient { get; set; }
        public Room? Room { get; set; }
        public List<Doctor>? Doctors { get; set; }
        public List<Nurse>? Nurses { get; set; }
    }
}
