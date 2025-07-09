using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context;
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

             return View("~/Views/pages/MenuPedidos.cshtml", menuItems); 
        }

        // POST: Menu/RealizarPedido
        [HttpPost]
        public IActionResult RealizarPedido([FromBody] PedidoViewModel pedido)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Crear nueva orden
                var nuevaOrden = new Orden
                {
                    HoraRecibida = DateTime.Now,
                    IdMesa = pedido.IdMesa,
                    EstadoOrden = 1,
                    Total = pedido.Items.Sum(i => i.PrecioTotal)
                };

                _context.Ordens.Add(nuevaOrden);
                _context.SaveChanges();

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

                _context.SaveChanges();

                return Json(new { success = true, ordenId = nuevaOrden.IdOrden });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
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
    }
}