using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MesaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MesaController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SeleccionarMesa()
        {
            var mesas = _context.Mesas
                .Include(m => m.Sillas)
                .ToList();

            return View("~/Views/pages/SeleccionarMesa.cshtml", mesas);
        }

        [HttpPost]
        public IActionResult SeleccionarSilla(int idSilla)
        {
            var silla = _context.Sillas.FirstOrDefault(s => s.IdSilla == idSilla && s.Estado == 1);
            if (silla == null)
                return BadRequest("Silla no disponible.");

            silla.Estado = 0; 
            _context.SaveChanges();

            return Ok();
        }
    }
}
