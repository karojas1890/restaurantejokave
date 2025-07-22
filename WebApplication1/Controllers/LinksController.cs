using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class LinksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public LinksController(ApplicationDbContext context, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _config = config;
            _httpClientFactory = httpClientFactory;
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

        [HttpPost]
        public async Task<IActionResult> ObtenerSugerenciaIA(int eficiencia, int energia, int estres)
        {
            string apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("❌ No se encontró la variable de entorno GROQ_API_KEY.");
                return BadRequest("No se encontró la clave API.");
            }

            Console.WriteLine("✅ API Key recibida: " + apiKey);
            string prompt = $"Dado que la eficiencia del mesero es {eficiencia}%, la energía es {energia}% y el estrés es {estres}%, ¿qué acción debe tomar entre: Hidratarse, Tomar un descanso, Solicitar ayuda al gerente o Renunciar? Explica brevemente tu elección. Respuesta corta y profesional";

            var requestBody = new
            {
                model = "llama3-70b-8192",
                messages = new[]
                {
                new { role = "user", content = prompt }
            },
                temperature = 0.7
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Error al consultar Groq.");
            }

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var jsonDoc = await JsonDocument.ParseAsync(responseStream);

            string respuestaIA = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Json(new { sugerencia = respuestaIA });
        }

        [HttpPost]
        public IActionResult EjecutarModoSupervivencia(int opcion)
        {
            string usuario = HttpContext.Session.GetString("Usuario");

            if (string.IsNullOrEmpty(usuario))
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            try
            {
                string connectionString = _config.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usuariojokave.SP_ModoSupervivenciaMesero", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Usuario", usuario);
                    cmd.Parameters.AddWithValue("@Opcion", opcion);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ObtenerOrdenesMesero()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return Json(new { error = "Usuario no autenticado" });
            }
            var ordenes = new List<object>();

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("usuariojokave.sp_OrdenesMesero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Opcion", 1);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ordenes.Add(new
                            {
                                IdOrden = reader["IdOrden"],
                                IdMesa = reader["IdMesa"],
                                HoraRecibida = ((DateTime)reader["HoraRecibida"]).ToString("HH:mm:ss")
                            });
                        }
                    }
                }
            }

            return Json(ordenes);
        }

        [HttpPost]
        public IActionResult ObtenerOrdenesMeserodesdeCocina()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return Json(new { error = "Usuario no autenticado" });
            }
            var ordenes = new List<object>();

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("usuariojokave.sp_OrdenesMesero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Opcion", 5);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ordenes.Add(new
                            {
                                IdOrden = reader["IdOrden"],
                                IdMesa = reader["IdMesa"],
                                HoraRecibida = ((DateTime)reader["HoraRecibida"]).ToString("HH:mm:ss")
                            });
                        }
                    }
                }
            }

            return Json(ordenes);
        }

        [HttpPost]
        public IActionResult AprobarOrden(int idOrden)
        {
            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("usuariojokave.sp_OrdenesMesero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Opcion", 4); // Opción para aprobar
                    cmd.Parameters.AddWithValue("@IdOrden", idOrden); // Solo IdOrden se requiere en esta opción
                    cmd.Parameters.AddWithValue("@IdUsuario", DBNull.Value); // En caso de que lo pida, pero no sea usado

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return Json(new { success = true, mensaje = "Orden aprobada correctamente" });
        }

        public IActionResult AprobarOrdenparalaMesa(int idOrden)
        {
            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("usuariojokave.sp_OrdenesMesero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Opcion", 6); // Opción para aprobar
                    cmd.Parameters.AddWithValue("@IdOrden", idOrden); // Solo IdOrden se requiere en esta opción
                    cmd.Parameters.AddWithValue("@IdUsuario", DBNull.Value); // En caso de que lo pida, pero no sea usado

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return Json(new { success = true, mensaje = "Orden aprobada correctamente" });
        }

        [HttpPost]
        public IActionResult ObtenerDetalleOrden(int idOrden)
        {
            

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Console.WriteLine($"🚀 Entrando a ObtenerDetalleOrden - IdUsuario: {idUsuario}, IdOrden: {idOrden}");

            var detalles = new List<object>();

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("usuariojokave.sp_OrdenesMesero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Opcion", 2);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@IdOrden", idOrden); // Debés agregar este parámetro opcional al SP

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            detalles.Add(new
                            {
                                idOrden = reader["IdOrden"],
                                idMesa = reader["IdMesa"],
                                horaRecibida = ((DateTime)reader["HoraRecibida"]).ToString("HH:mm:ss"),
                                nombre = reader["Nombre"],
                                cantidadProducto = reader["CantidadProducto"]
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"✅ Finalizando ObtenerDetalleOrden - Total productos encontrados: {detalles.Count}");

            return Json(detalles);
        }

        public IActionResult ObtenerDetalleOrdendesdeCocina(int idOrden)
        {


            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            Console.WriteLine($"🚀 Entrando a ObtenerDetalleOrden - IdUsuario: {idUsuario}, IdOrden: {idOrden}");

            var detalles = new List<object>();

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("usuariojokave.sp_OrdenesMesero", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Opcion", 7);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@IdOrden", idOrden); // Debés agregar este parámetro opcional al SP

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            detalles.Add(new
                            {
                                idOrden = reader["IdOrden"],
                                idMesa = reader["IdMesa"],
                                horaRecibida = ((DateTime)reader["HoraRecibida"]).ToString("HH:mm:ss"),
                                nombre = reader["Nombre"],
                                cantidadProducto = reader["CantidadProducto"]
                            });
                        }
                    }
                }
            }
            Console.WriteLine($"✅ Finalizando ObtenerDetalleOrden - Total productos encontrados: {detalles.Count}");

            return Json(detalles);
        }

        public IActionResult GeneradorCaos()
        {
            return View("~/Views/pages/PanelGeneradordeCaos.cshtml");
        }


    }
}
