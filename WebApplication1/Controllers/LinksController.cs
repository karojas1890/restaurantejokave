using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class LinksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public LinksController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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

        [Authorize(Roles = "1")]
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
            string usuario = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("LoginCliente", "Links");
            }

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("usuariojokave.SP_ObtenerIndicadoresMesero", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    HttpContext.Session.SetInt32("Eficiencia", Convert.ToInt32(reader["Eficiencia"]));
                    HttpContext.Session.SetInt32("Energia", Convert.ToInt32(reader["Energia"]));
                    HttpContext.Session.SetInt32("Estres", Convert.ToInt32(reader["Estres"]));
                    HttpContext.Session.SetInt32("Carga", Convert.ToInt32(reader["Carga"]));
                }


                conn.Close();
            }

            return View("~/Views/pages/PanelMesero.cshtml");
        }

        [HttpGet]
        public JsonResult ObtenerMesasAsignadas()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return Json(new { error = "Usuario no autenticado" });
            }

            var mesas = new List<object>();
            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Obtener sección asignada al mesero
                int? idSeccion = null;
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 IdSeccion FROM usuariojokave.Mesas WHERE MeseroAsignado = @IdUsuario", conn))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario.Value);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        idSeccion = Convert.ToInt32(result);
                }

                if (idSeccion == null)
                {
                    return Json(mesas); // Sin sección asignada → lista vacía
                }

                // 2. Llamar al procedimiento almacenado para traer mesas y sillas
                using (SqlCommand cmd = new SqlCommand("ObtenerMesasYSillasParaMesero", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdRol", idUsuario);


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var resultado = new List<object>();

                        while (reader.Read())
                        {
                            resultado.Add(new
                            {
                                IdMesa = reader.GetInt32(reader.GetOrdinal("IdMesa")),
                                IdSeccion = reader.GetInt32(reader.GetOrdinal("IdSeccion")),
                                Capacidad = reader.GetInt32(reader.GetOrdinal("Capacidad")),
                                EstadoMesa = reader.GetInt32(reader.GetOrdinal("Estado")),
                                IdSilla = reader.IsDBNull(reader.GetOrdinal("IdSilla"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("IdSilla")),
                                EstadoSilla = reader.IsDBNull(reader.GetOrdinal("EstadoSilla"))
                                            ? (int?)null
                                            : reader.GetInt32(reader.GetOrdinal("EstadoSilla"))
                            });
                        }
                        
                        return Json(resultado);

                    }
                }
            }
        }



        public IActionResult GeneradorCaos()
        {
            return View("~/Views/pages/PanelGeneradordeCaos.cshtml");
        }


    }
}
