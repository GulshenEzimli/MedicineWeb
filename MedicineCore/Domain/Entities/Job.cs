using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Job : IEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
