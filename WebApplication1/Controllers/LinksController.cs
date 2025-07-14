using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Linq;


namespace WebApplication1.Controllers
{
    public class LinksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LinksController (ApplicationDbContext context)
        {

        _context = context; 
        
        }    
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Alertas()
        {
            return View("~/Views/pages/Alertas.cshtml");
        }

        public IActionResult LoginCliente()
        {
            return View("~/Views/pages/LoginCliente.cshtml");
        }

        public IActionResult CrearCuentaCliente()
        {
            return View("~/Views/pages/CrearCuentaCliente.cshtml");
        }

        public IActionResult Clientes()
        {
            return View("~/Views/pages/Clientes.cshtml");
        }
        public IActionResult InicioOperaciones()
        {
            return View("~/Views/pages/InicioOperecion.cshtml");
        }
        public IActionResult Inventario()
        {
            return View("~/Views/pages/Inventario.cshtml");
        }
        public IActionResult Personal()
        {
            return View("~/Views/pages/Personal.cshtml");
        }
        public IActionResult ResumenGeneral()
        {
            return View("~/Views/pages/ResumenGeneral.cshtml");
        }
        public IActionResult Ventas()
        {
            return View("~/Views/pages/Ventas.cshtml");
        }
        public IActionResult MenuPedidos()
        {
            return View("~/Views/pages/MenuPedidos.cshtml");
        }
        public IActionResult Bomberos()
        {
            return View("~/Views/pages/Bomberos.cshtml");
        }
        public IActionResult Pagos()
        {
            return View("~/Views/pages/Pagos.cshtml");
        }
        
        public IActionResult ExitoExtintor()
        {
            return View("~/Views/pages/ExitoExtintor.cshtml");
        }
        public IActionResult CargarSaldo() 
        {
            return View("~/Views/pages/CargarSaldo.cshtml");
        }
        public IActionResult AgregarTarjeta()
        {
            return View("~/Views/pages/AgregarTarjeta.cshtml");
        }
        public IActionResult MisTarjetas()
        {
            return View("~/Views/pages/MisTarjetas.cshtml");
        }
        public IActionResult Facturas()
        {
            return View("~/Views/pages/Facturas.cshtml");
        }
        public IActionResult SeleccionarMesa() 
        {
            return View("~/Views/pages/SeleccionarMesa.cshtml");
        }
        public IActionResult ErrorDeExtintor() 
        {
            return View("~/Views/pages/ErrorDeExtintor.cshtml");
        }
        public IActionResult MenuClientes()
        {
            // Pasar el modelo de usuario a la vista
            return View("~/Views/pages/MenuClientes.cshtml");
        }
        public IActionResult ValidarPorCorreo()
        {
            return View("~/Views/pages/ValidarPorCorreo.cshtml");
        }
        public IActionResult CrearCuenta() 
        {
            return View("~/Views/pages/CrearCuenta.cshtml");
        }
        public IActionResult PanelMesero()
        {
            return View("~/Views/pages/PanelMesero.cshtml");
        }
        public IActionResult GeneradorCaos()
        {
            return View("~/Views/pages/PanelGeneradordeCaos.cshtml");
        }
    }
}
