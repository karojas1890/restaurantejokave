// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

public class AuthController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    public AuthController(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    [HttpGet("/Login")]
    public IActionResult Login()
    {
        if (TempData["ErrorMessage"] == null)
        {
            TempData["ErrorMessage"] = "Primero inicia sesion para continuar.";
        }
        return View("~/Views/pages/LoginCliente.cshtml");
    }
    [HttpGet("/AccesoDenegado")]
    public IActionResult Denegado()
    {
       
        return View("~/Views/pages/AccesoDenegado.cshtml");
    }
    public IActionResult LoginCliente()
    {
        return View();
    }
   
  
    [HttpPost]
    public IActionResult LoginCliente(string username, string password)
    {
        var code = new Random().Next(0, 1000000).ToString("D6");

        // Buscar usuario en la base de datos

        var usuario = _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefault(u => u.Usuario1 == username);
       
        if (usuario == null)
        {
            TempData["ErrorMessage"] = "Credenciales no son validas.";
            return View("~/Views/pages/LoginCliente.cshtml");
        }

        
        bool esValida = BCrypt.Net.BCrypt.Verify(password, usuario.Password);

        if (!esValida)
        {
            TempData["ErrorMessage"] = "Credenciales no son validas.";
            return View("~/Views/pages/LoginCliente.cshtml");
        }
        usuario.Codigo = code;
        _context.Usuarios.Update(usuario);
        _context.SaveChanges();

        _emailService.SendCode(usuario.Email,code);

        HttpContext.Session.SetInt32("UserId", usuario.IdUsuario);

        
        return View("~/Views/pages/ValidarPorCorreo.cshtml", usuario);

    }
    [HttpPost]
    public async Task<IActionResult> ValidarCodigo(int id, string Codigo)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);

        if (usuario == null)
        {
            TempData["ErrorMessage"] = "Usuario no encontrado.";
            return RedirectToAction("LoginCliente");
        }

        if (usuario.Codigo != Codigo)
        {
            TempData["ErrorMessage"] = "El código ingresado no es válido.";
            return View("~/Views/pages/ValidarPorCorreo.cshtml", usuario);
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.IdUsuario.ToString()),
        new Claim(ClaimTypes.Role, usuario.IdRol.ToString())
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var nuevaVisita = new Visita
        {
            IdUsuario = usuario.IdUsuario,
            Estado = 1, // Esperando mesa
           FechaHoraIngreso = DateTime.Now // FechaHoraIngreso se llena automáticamente
        };

        _context.Visita.Add(nuevaVisita);
        _context.SaveChanges();

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);



        return RedirectToAction("LoginC", new { id = usuario.IdUsuario });
    }
    [Authorize(Roles = "1")]
    public IActionResult LoginC(int id)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
        return View("~/Views/pages/MenuClientes.cshtml", usuario);
    }

    public async Task<IActionResult> Logout()
    {

        var idUsuarioClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        if (idUsuarioClaim != null && int.TryParse(idUsuarioClaim.Value, out int idUsuario))
        {
            var visitasActivas = _context.Visita
                .Where(v => v.IdUsuario == idUsuario && v.FechaHoraSalida == null)
                .ToList();

            foreach (var visita in visitasActivas)
            {
                visita.FechaHoraSalida = DateTime.Now;
                visita.Estado = 8;

                if (visita.IdSilla != null)
                {
                    var silla = _context.Sillas.FirstOrDefault(s => s.IdSilla == visita.IdSilla);
                    if (silla != null)
                    {
                        silla.Estado = 1; 
                    }
                }
            }

            _context.SaveChanges();
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();

        // Redirige al login
        return View("~/Views/Home/Index.cshtml");
    }


}
