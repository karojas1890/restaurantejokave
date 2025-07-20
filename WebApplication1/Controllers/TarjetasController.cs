using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TarjetasController : Controller
    {
     
        private readonly ApplicationDbContext _context;
        public TarjetasController( ApplicationDbContext context)
        {
            
            _context = context;
        }
      
        [HttpGet]
        public IActionResult TraerTarjetas()
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Login", "Auth");
            }

            var tarjetas = _context.Tarjeta
                .Where(t => t.IdCliente == idUsuario)
                .ToList();

            return View("~/Views/pages/CargarSaldo.cshtml", tarjetas);
        }
        [HttpPost]
        public IActionResult CargarSaldo(string numeroTarjeta, decimal monto)
        {
         
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("TraerTarjetas");
            }

          
            if (monto <= 0)
            {
                TempData["ErrorMessage"] = "El monto debe ser mayor que cero.";
                return RedirectToAction("TraerTarjetas");
            }

          
            var tarjeta = _context.Tarjeta.FirstOrDefault(t => t.NumeroTarjeta == numeroTarjeta && t.IdCliente == idUsuario);
            if (tarjeta == null)
            {
                TempData["ErrorMessage"] = "La tarjeta no fue encontrada o no pertenece al usuario.";
                return RedirectToAction("TraerTarjetas");
            }

        
            tarjeta.Saldo = (tarjeta.Saldo ?? 0) + monto;

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Saldo cargado correctamente.";
            return RedirectToAction("TraerTarjetas");
        }
       

        [HttpPost]
        public IActionResult AgregarTarjeta(Tarjeta tarjetas)
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return View("~/Views/pages/AgregarTarjeta.cshtml");
            }

            // se crea el objeto
            var tarjeta = new Tarjeta
            {
                NumeroTarjeta = tarjetas.NumeroTarjeta,
                IdCliente = idUsuario,
                MesVencimiento = tarjetas.MesVencimiento,
                AnioVencimiento = tarjetas.AnioVencimiento,
                CodigoSeguridad = tarjetas.CodigoSeguridad,
                Saldo=0
            };
            

            try
            {
                  
                    _context.Tarjeta.Add(tarjeta);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Tarjeta registrada correctamente.";
             
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al contactar la API: " + ex.Message;
            }

            return View("~/Views/pages/AgregarTarjeta.cshtml");
        }
        [HttpGet]
        public IActionResult MisTarjetas()
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Login", "Cuenta");
            }

           
            var tarjetas = _context.Tarjeta
                .Where(t => t.IdCliente == idUsuario)
                .ToList();

            return View("~/Views/pages/MisTarjetas.cshtml", tarjetas);
        }


    }
}
