using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Position : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int DepartmentId => Department?.Id ?? 0;
        public Department? Department { get; set; }
    }
}
