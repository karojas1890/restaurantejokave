using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel;

namespace WebApplication1.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult MenuC()
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
            {
                TempData["ErrorMessage"] = "Usuario no autenticado.";
                return RedirectToAction("Login", "Auth");
            }
            return RedirectToAction("IrMenu", new { id = idUsuario });
        }
        [Authorize(Roles = "1")]
        public IActionResult IrMenu(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
            return View("~/Views/pages/MenuClientes.cshtml", usuario);
        }
        // GET: Menu/Pedidos
        [HttpGet]
        public IActionResult Pedidos()
        {
            // Si _context es null, hay un problema con la inyección de dependencias
            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Menus' is null.");
            }

            var menuItems = _context.Menus
                .Where(m => m.EsActivo == true)
                .ToList();

            if (menuItems == null || !menuItems.Any())
            {
                // Si no hay items, pasa una lista vacía en lugar de null
                return View(new List<Menu>());
            }

            //esta seccion apra el numero de mesa de forma automatica 
            var idUsuarioClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            if (!string.IsNullOrEmpty(idUsuarioClaim) && int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                var visita = _context.Visita
                    .FirstOrDefault(v => v.IdUsuario == idUsuario && v.FechaHoraSalida == null);

                if (visita != null && visita.IdSilla.HasValue)
                {
                    var silla = _context.Sillas
                         .Include(s => s.IdMesaNavigation)
                         .FirstOrDefault(s => s.IdSilla == visita.IdSilla);

                    if (silla?.IdMesaNavigation != null)
                    {
                        ViewBag.NumeroMesa = silla.IdMesaNavigation.IdMesa; 
                    }
                }
            }
            return View("~/Views/pages/MenuPedidos.cshtml", menuItems); 
        }

        // POST: Menu/RealizarPedido
        [HttpPost]
        public async Task<IActionResult> RealizarPedido([FromBody] PedidoViewModel pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Obtener usuario actual
                var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim, out int idUsuario))
                {
                    return Unauthorized(new { message = "Usuario no autenticado." });
                }

                // Obtener visita activa
                var visitaActiva = await _context.Visita
                    .Where(v => v.IdUsuario == idUsuario && v.FechaHoraSalida == null)
                    .OrderByDescending(v => v.FechaHoraIngreso)
                    .FirstOrDefaultAsync();

                if (visitaActiva == null)
                {
                    return BadRequest(new { message = "No se encontró visita activa para el usuario." });
                }

                // Crear nueva orden vinculada a la visita
                var nuevaOrden = new Orden
                {
                    IdVisita =visitaActiva.IdVisita,
                    IdUsuario = visitaActiva.IdUsuario,
                    HoraRecibida = DateTime.Now,
                    IdMesa = pedido.IdMesa,
                    MeseroAsignado=9,
                    EstadoOrden = 1,
                    Total = pedido.Items.Sum(i => i.PrecioTotal)
                    
                };

                _context.Ordens.Add(nuevaOrden);
                await _context.SaveChangesAsync();

                // Agregar detalles de la orden
                foreach (var item in pedido.Items)
                {
                    var detalle = new DetalleOrden
                    {
                        IdOrden = nuevaOrden.IdOrden,
                        IdProducto = item.IdProducto,
                        CantidadProducto = item.Cantidad,
                        ValorIndividual = item.PrecioUnitario,
                        SubTotal = item.PrecioTotal
                    };

                    _context.DetalleOrdens.Add(detalle);
                }

                await _context.SaveChangesAsync();
                visitaActiva.Estado = 3; 
                _context.Visita.Update(visitaActiva);
                await _context.SaveChangesAsync();
                return Json(new { success = true, ordenId = nuevaOrden.IdOrden });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarOpinion(Visita model)
        {
            
            var visita = await _context.Visita.FindAsync(model.IdVisita);

            if (visita == null)
            {
                TempData["ErrorMessage"] = "No se encontró la visita especificada.";
                return View("NombreVista", model); 
            }

            // Actualizar solo los campos necesarios
            visita.EstadoAnimo = model.EstadoAnimo;
            visita.Satisfaccion = model.Satisfaccion;
            visita.Comentarios = model.Comentarios;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡Gracias por su opinión!";
                return RedirectToAction("NombreVistaExitosa"); // Cambia según tu flujo
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al registrar la opinión: " + ex.Message;
            }

            return View("NombreVista", model); // Reemplaza con tu vista actual
        }


        // GET: Menu/Buscar?term=query
        public IActionResult Buscar(string term)
        {
            var resultados = _context.Menus
                .Where(m => m.Nombre.Contains(term) && m.EsActivo == true)
                .Select(m => new { m.IdProducto, m.Nombre })
                .Take(10)
                .ToList();

            return Json(resultados);
        }
        // Parámetro: idMesa y si es pagoIndividual o grupal (bool)

        [HttpPost]
        public async Task<IActionResult> EnviarFeedback(Visita visitas)
        {
            var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized(new { message = "Usuario no autenticado." });
            }
            if (visitas.EstadoAnimo==null || visitas.Satisfaccion==null|| visitas.Comentarios == null)
            {
                TempData["ErrorMessage"] = "Debe llenar todo los campos";
                return RedirectToAction("Menu", new { id = idUsuario });

            }
            var visita = await _context.Visita
                .Where(v => v.IdUsuario == idUsuario && v.FechaHoraSalida == null)
                .OrderByDescending(v => v.FechaHoraIngreso)
                .FirstOrDefaultAsync();

            if (visita != null)
            {
                visita.EstadoAnimo = visitas.EstadoAnimo;
                visita.Satisfaccion = visitas.Satisfaccion;
                visita.Comentarios = visitas.Comentarios;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡Gracias por su opinión!";
                return RedirectToAction("Menu", new { id = idUsuario });
            }
            TempData["ErrorMessage"] = "No se encontró una visita activa.";
            return RedirectToAction("Menu", new { id = idUsuario });
        }
        [Authorize(Roles = "1")]
        public IActionResult Menu(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
            return View("~/Views/pages/MenuClientes.cshtml", usuario);
        }

    }


}