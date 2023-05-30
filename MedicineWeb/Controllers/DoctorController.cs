using Microsoft.AspNetCore.Mvc;

namespace MedicineWeb.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
