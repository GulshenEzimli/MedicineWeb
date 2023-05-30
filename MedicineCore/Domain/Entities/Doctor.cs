using HospitalCore.Domain.Enums;
using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Doctor : IEntity
    {
        public int Id { get; set; }
        public int? PositionId => Position?.Id ?? 0;
        public Position? Position { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PIN { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal? Salary { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsChiefDoctor { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Admin? Creator { get; set; }
        public Admin? Modifier { get; set; }
        public int? CreatorId => Creator?.Id ?? 0;
        public int? ModifierId => Modifier?.Id ?? 0;
    }
}
