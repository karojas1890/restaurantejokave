using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        public IActionResult ConfirmarSilla([FromBody] Silla datos)
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(idUsuarioClaim))
            {
                return BadRequest(new { message = "Usuario no autenticado." });
            }

            int idUsuario = int.Parse(idUsuarioClaim);

            var visitaExistente = _context.Visita
                .FirstOrDefault(v => v.IdUsuario == idUsuario && v.FechaHoraSalida == null);


           
            if (visitaExistente.IdSilla != null)
            {
                return BadRequest(new { message = "Ya seleccionaste una silla. No puedes seleccionar otra." });
            }

            
            var sillaDisponible = _context.Sillas.FirstOrDefault(s => s.IdSilla == datos.IdSilla);

            if (sillaDisponible == null || sillaDisponible.Estado != 1)
            {
                return BadRequest(new { message = "La silla ya está ocupada o no existe." });
            }

            
            bool sillaEnUso = _context.Visita.Any(v => v.IdSilla == datos.IdSilla && v.FechaHoraSalida == null);
            if (sillaEnUso)
            {
                return BadRequest(new { message = "La silla ya está asignada a otra persona." });
            }

           
            sillaDisponible.Estado = 0; 
            visitaExistente.IdSilla = sillaDisponible.IdSilla;
            visitaExistente.Estado = 2;

            _context.SaveChanges();

            return Ok(new { message = "Silla ocupada con éxito." });
        }
        
    }

}
