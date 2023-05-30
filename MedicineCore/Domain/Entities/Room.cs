using HospitalCore.Domain.Enums;
using HospitalCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalCore.Domain.Entities
{
    public class Room : IEntity
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailable { get; set; }
        public RoomTypes Type { get; set; }
        public int BlockFloor { get; set; }
        public bool IsDelete { get; set; }
    }
}
