using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        public async Task<JsonResult> EstadoOrden()
        {

            var idUsuarioClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            if (idUsuarioClaim == null)
                return Json(new { error = "Usuario no autenticado" });

            int idUsuario = int.Parse(idUsuarioClaim);

            
            var orden = await _context.Ordens
                .Where(o => o.IdUsuario == idUsuario)
                .OrderByDescending(o => o.HoraRecibida) 
                .FirstOrDefaultAsync();

            if (orden == null)
                return Json(new { error = "Orden no encontrada para el usuario" });

            return Json(new
            { 
                progreso = orden.EstadoOrden 
            });
        }
        [HttpGet]
        public async Task<JsonResult> EstadoVisita()
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            if (idUsuarioClaim == null)
                return Json(new { error = "Usuario no autenticado" });

            int idUsuario = int.Parse(idUsuarioClaim);

            var visita = await _context.Visita
                .Where(o => o.IdUsuario == idUsuario)
                .OrderByDescending(o => o.FechaHoraIngreso)
                .FirstOrDefaultAsync();

            if (visita == null)
                return Json(new { error = "Orden no encontrada para el usuario" });

            return Json(new
            {
                estadoActual = visita.Estado,
                fechaIngreso = visita.FechaHoraIngreso
            });
        }

    }
}
