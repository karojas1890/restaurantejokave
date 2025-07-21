using Microsoft.AspNetCore.Http; // para sesión
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace WebApplication1.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public JsonResult Validar(string usuario, string password)
        {
            string mensaje = "";
            int idRol = 0;
            string redirectUrl = "";

            string connectionString = _config.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_ValidarUsuario", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    mensaje = reader["Mensaje"].ToString();

                    if (mensaje == "Login exitoso")
                    {
                        idRol = Convert.ToInt32(reader["IdRol"]);
                        int idJornadaLaboral = Convert.ToInt32(reader["IdJornadaLaboral"]);
                        HttpContext.Session.SetString("Usuario", reader["Usuario"].ToString());
                        HttpContext.Session.SetInt32("IdRol", idRol);
                        HttpContext.Session.SetInt32("IdUsuario", Convert.ToInt32(reader["IdUsuario"]));
                        HttpContext.Session.SetInt32("IdJornadaLaboral", idJornadaLaboral);

                        if (idRol == 3)
                        {
                            if (reader["NombreCompleto"] != DBNull.Value)
                                HttpContext.Session.SetString("Nombre", reader["NombreCompleto"].ToString());
                        }

                        redirectUrl = idRol switch
                        {
                            3 => "/Links/PanelMesero",
                            5 => "/Links/PanelGeneradordeCaos",
                            _ => "/Links/Index"
                        };
                    }
                }
                else
                {
                    mensaje = "Error inesperado.";
                }

                conn.Close();
            }

            return Json(new { mensaje = mensaje, redirectUrl = redirectUrl });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Obtener los valores de sesión
            var idJornadaLaboral = HttpContext.Session.GetInt32("IdJornadaLaboral");
            var usuario = HttpContext.Session.GetString("Usuario");

            if (idJornadaLaboral.HasValue && !string.IsNullOrEmpty(usuario))
            {
                // Ejecutar el procedimiento almacenado
                string connectionString = _config.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SP_CerrarSesion", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdJornadaLaboral", idJornadaLaboral.Value);
                    cmd.Parameters.AddWithValue("@Usuario", usuario);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            // Limpiar la sesión
            HttpContext.Session.Clear();

            // Redirigir al Index (por ejemplo, página de login)
            return RedirectToAction("Index", "Home"); // Asegúrate de que el controlador y vista existan
        }

    }
}


