using HospitalCore.Domain.Enums;
using System.Reflection;

namespace MedicineWeb.Models
{
    public class DoctorModel
    {
        public int Id { get; set; }
        public int No { get; set; }
        public PositionModel? Position { get; set; }
        public string? DepartmentName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PIN { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal? Salary { get; set; }
        public bool IsChiefDoctor { get; set; }

        public string? IsChiefDoctorValue
        {
            get
            {
                return IsChiefDoctor ? "Baş həkim" : "Həkim";
            }
        }

    }
}
