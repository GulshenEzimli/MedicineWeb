using MedicineWeb.Models;

namespace MedicineWeb.ViewModels
{
    public class DoctorViewModel
    {
        public DoctorViewModel()
        {
            Doctors = new List<DoctorModel>();
        }
        public List<DoctorModel> Doctors { get; set; }
    }
}
