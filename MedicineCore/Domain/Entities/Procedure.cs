using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Procedure : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public bool IsDelete { get; set; }
    }
}
