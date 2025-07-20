using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class PagosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApplicationDbContext _context;
        public PagosController(IHttpClientFactory httpClientFactory, ApplicationDbContext context)
        {
            _context = context; 
            _httpClientFactory = httpClientFactory;

        }
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> TraerTarjetas()
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Login", "Auth");
            }

            // Obtener tarjetas del usuario
            var tarjetas = await _context.Tarjeta
                .Where(t => t.IdCliente == idUsuario)
                .ToListAsync();

            var ultimaCuenta = await _context.Cuenta
                 .Include(c => c.IdOrdenNavigation) 
                 .Where(c => c.IdOrdenNavigation.IdUsuario == idUsuario && c.Estado == 2)
                .OrderByDescending(c => c.FechaCreacion)
                 .FirstOrDefaultAsync();


            // Obtener la cuenta relacionada (si existe)
            

            var viewModel = new PagoViewModel
            {
                Tarjetas = tarjetas,
                TotalAPagar = ultimaCuenta?.Total
            };

            return View("~/Views/pages/Pagos.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GestionPagos(TarjetaApi tarjetas)
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("TraerTarjetas");
            }

            var cuenta = await _context.Cuenta
                .Include(c => c.IdOrdenNavigation)
                .Where(c => c.IdOrdenNavigation.IdUsuario == idUsuario && c.Estado == 2) 
                .OrderByDescending(c => c.FechaCreacion) 
                .FirstOrDefaultAsync();

            if (cuenta == null)
            {
                TempData["ErrorMessage"] = "No hay cuenta pendiente de pago para este usuario.";
                return RedirectToAction("TraerTarjetas");
            }

            var orden = cuenta.IdOrdenNavigation;

            var tarjetaApi = new TarjetaApi
            {
                NumeroTarjeta = tarjetas.NumeroTarjeta,
                IdCliente = idUsuario,
                MesVencimiento = tarjetas.MesVencimiento,
                AnioVencimiento = tarjetas.AnioVencimiento,
                CodigoSeguridad = tarjetas.CodigoSeguridad,
                Monto = tarjetas.Monto
            };

            var client = _httpClientFactory.CreateClient();
            var json = JsonSerializer.Serialize(tarjetaApi);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://tiusr24pl.cuc-carrera-ti.ac.cr/APITransaccional/validar_tarjeta.php", content);

            if (response.IsSuccessStatusCode)
            {
                var ultimaVisita = await _context.Visita
                 .Where(v => v.IdVisita == orden.IdVisita)
                 .OrderByDescending(v => v.FechaHoraIngreso)
                 .FirstOrDefaultAsync();

                if (ultimaVisita != null)
                {
                    ultimaVisita.Estado = 7;
                }
                cuenta.Estado = 1; //cuenta pagada
                if (orden != null)
                {
                    orden.EstadoOrden = 6; // Orden cerrada
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pago realizado exitosamente.";
                return RedirectToAction("TraerTarjetas");
            }
            else
            {
                TempData["ErrorMessage"] = "Error al procesar el pago.";
                return RedirectToAction("TraerTarjetas");
            }
        }


    }
}
