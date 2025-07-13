// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult LoginCliente()
    {
        return View();
    }

    
    [HttpPost]
    public IActionResult LoginCliente(string username, string password)
    {
        // Buscar usuario en la base de datos
        var usuario = _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefault(u => u.Usuario1 == username && u.Password == password);

        if (usuario == null)
        {
            TempData["ErrorMessage"] = "Credenciales no son validas.";
            return View("~/Views/pages/LoginCliente.cshtml");
        }


        // Guardar el ID de usuario en la sesión
        HttpContext.Session.SetInt32("UserId", usuario.IdUsuario);


        // Redirigir al MenuClientes con el idUsuario
        return View("~/Views/pages/MenuClientes.cshtml", usuario);
        
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("LoginCliente");
    }
}
