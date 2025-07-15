using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrdenesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdenesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> EstadoOrden(int id)
        {
            var orden = await _context.Ordens.FindAsync(id);
            if (orden == null)
                return Json(new { error = "Orden no encontrada" });

            return Json(new { progreso = orden.EstadoOrden });
        }
    }
}
