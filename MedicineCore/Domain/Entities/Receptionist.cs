using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Receptionist : IEntity
    {

        public int Id { get; set; }

        // Receptionist main data

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string PIN { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Salary { get; set; }


        public DateTime CreationDate { get; set; }
        public bool IsDelete { get; set; }
        public DateTime ModifierDate { get; set; }


        public int CreatorId => Creator?.Id ?? 0;
        public int ModifierId => Modifier?.Id ?? 0;


        public Admin Creator { get; set; }
        public Admin Modifier { get; set; }


        public int JobId => Job?.Id ?? 0;

        public Job Job { get; set; }



    }
}