using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using BCrypt;
//nuevo en la linea 41 hash
namespace WebApplication1.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CrearCuenta()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearCuenta(Usuario cliente)
        {
            
            var existe = await _context.Usuarios
                .AnyAsync(u => u.DocumentoIdentificacion == cliente.DocumentoIdentificacion);

            if (existe)
            {
                
                TempData["ErrorMessage"] = "Ya existe una cuenta activa para esa cedula.";
                return View("~/Views/pages/CrearCuentaCliente.cshtml", cliente);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    cliente.Password = BCrypt.Net.BCrypt.HashPassword(cliente.Password);
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cuenta creada exitosamente.";
                    return View("~/Views/pages/CrearCuentaCliente.cshtml");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al crear la cuenta: " + ex.Message;
                   
                }
            }

            return View("~/Views/pages/CrearCuentaCliente.cshtml");
        }

    }
}
