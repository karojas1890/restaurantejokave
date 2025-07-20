using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class FacturaController : Controller
    { 
        private readonly ApplicationDbContext _context;

        public FacturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MostrarFactura(bool pagoSeparado)
        {
            var idUsuarioClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            Orden? ordenUsuario = null;
            List<Orden> ordenes = new();
            int idMesa;

            if (pagoSeparado)
            {
                // Obtener orden del usuario para el día de hoy
                ordenUsuario = await _context.Ordens
                    .Include(o => o.DetalleOrdens)
                    .Where(o => o.IdUsuario == idUsuario && o.EstadoOrden == 5 && o.HoraRecibida.HasValue && o.HoraRecibida.Value.Date == DateTime.Today)
                    .OrderByDescending(o => o.HoraRecibida)
                    .FirstOrDefaultAsync();

                if (ordenUsuario == null)
                {
                    TempData["ErrorMessage"] = "No hay órdenes del día de hoy para este usuario.";
                    return View("~/Views/pages/Facturas.cshtml");
                }

                // Validar que no tenga ya cuenta individual
                var cuentaExistente = await _context.Cuenta
                    .FirstOrDefaultAsync(c => c.IdOrden == ordenUsuario.IdOrden);

                if (cuentaExistente != null)
                {
                    TempData["ErrorMessage"] = "Ya has generado tu cuenta individual.";
                    return View("~/Views/pages/Facturas.cshtml");
                }

                ordenes.Add(ordenUsuario);
                idMesa = ordenUsuario.IdMesa;
            }
            else
            {
                // Obtener una orden del usuario para saber su mesa
                ordenUsuario = await _context.Ordens
                    .Where(o => o.IdUsuario == idUsuario && o.EstadoOrden == 5)
                    .OrderByDescending(o => o.HoraRecibida)
                    .FirstOrDefaultAsync();

                if (ordenUsuario == null || !ordenUsuario.HoraRecibida.HasValue || ordenUsuario.HoraRecibida.Value.Date != DateTime.Today)
                {
                    TempData["ErrorMessage"] = "Solo puedes generar facturas de órdenes del día de hoy.";
                    return View("~/Views/pages/Facturas.cshtml");
                }

                idMesa = ordenUsuario.IdMesa;

                // Buscar todas las órdenes del día de hoy en esa mesa
                ordenes = await _context.Ordens
                    .Include(o => o.DetalleOrdens)
                    .Where(o => o.IdMesa == idMesa && o.EstadoOrden == 5 &&
                                o.HoraRecibida.HasValue && o.HoraRecibida.Value.Date == DateTime.Today)
                    .ToListAsync();

                if (!ordenes.Any())
                {
                    TempData["ErrorMessage"] = "No hay órdenes del día de hoy para esta mesa.";
                    return View("~/Views/pages/Facturas.cshtml");
                }

                // Verificar que no exista ya una cuenta generada para alguna orden
                var cuentasMesa = await _context.Cuenta
                    .Where(c => ordenes.Select(o => o.IdOrden).Contains(c.IdOrden) && c.Estado == 2)
                    .ToListAsync();

                if (cuentasMesa.Any())
                {
                    TempData["ErrorMessage"] = "Ya se ha generado una cuenta para esta mesa.";
                    return View("~/Views/pages/Facturas.cshtml");
                }
            }

            // Construir detalles de factura
            var detalles = new List<DetalleFacturaItem>();
            foreach (var orden in ordenes)
            {
                foreach (var d in orden.DetalleOrdens)
                {
                    var producto = await _context.Menus.FirstOrDefaultAsync(m => m.IdProducto == d.IdProducto);
                    detalles.Add(new DetalleFacturaItem
                    {
                        NombreProducto = producto?.Nombre ?? "Producto",
                        Cantidad = d.CantidadProducto,
                        ValorIndividual = d.ValorIndividual
                    });
                }
            }

            if (!detalles.Any())
            {
                TempData["ErrorMessage"] = "No hay detalles disponibles para facturar.";
                return View("~/Views/pages/Facturas.cshtml");
            }

            // Crear factura view model
            var facturaVM = new FacturaViewModel
            {
                IdMesa = idMesa,
                Fecha = DateTime.Now,
                EstadoPago = "Sin cancelar",
                NumeroPedido = ordenes.Max(o => o.IdOrden),
                Items = detalles
            };

            // Verificar que no exista ya una cuenta con ese número de pedido
            var cuentaPrev = await _context.Cuenta
                .FirstOrDefaultAsync(c => c.IdOrden == facturaVM.NumeroPedido);

            if (cuentaPrev != null)
            {
                TempData["ErrorMessage"] = "Ya se ha generado una cuenta para esta orden.";
                return View("~/Views/pages/Facturas.cshtml");
            }

            // Crear nueva cuenta
            var subtotal = detalles.Sum(d => d.ValorIndividual * d.Cantidad);
            var nuevaCuenta = new Cuenta
            {
                IdOrden = pagoSeparado ? ordenUsuario.IdOrden : facturaVM.NumeroPedido,
                Subtotal = subtotal,
                Iva = subtotal * 0.13m,
                Total = subtotal * 1.13m,
                Estado = 2,
                FechaCreacion = DateTime.Now
            };

            _context.Cuenta.Add(nuevaCuenta);

            // Actualizar estado de última visita
            var ultimaVisita = await _context.Visita
                .Where(v => ordenes.Select(o => o.IdVisita).Contains(v.IdVisita))
                .OrderByDescending(v => v.FechaHoraIngreso)
                .FirstOrDefaultAsync();

            if (ultimaVisita != null)
            {
                ultimaVisita.Estado = 6;
            }

            await _context.SaveChangesAsync();

            return View("~/Views/pages/Facturas.cshtml", facturaVM);
        }



    }
}
