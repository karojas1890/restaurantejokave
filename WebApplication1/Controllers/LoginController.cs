using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http; // para sesión

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
                        HttpContext.Session.SetString("Usuario", reader["Usuario"].ToString());
                        HttpContext.Session.SetInt32("IdRol", idRol);

                        if (idRol == 3)
                        {
                            if (reader["NombreCompleto"] != DBNull.Value)
                                HttpContext.Session.SetString("Nombre", reader["NombreCompleto"].ToString());

                            if (reader["Eficiencia"] != DBNull.Value)
                                HttpContext.Session.SetInt32("Eficiencia", Convert.ToInt32(reader["Eficiencia"]));

                            if (reader["Energia"] != DBNull.Value)
                                HttpContext.Session.SetInt32("Energia", Convert.ToInt32(reader["Energia"]));

                            if (reader["Estres"] != DBNull.Value)
                                HttpContext.Session.SetInt32("Estres", Convert.ToInt32(reader["Estres"]));

                            if (reader["Carga"] != DBNull.Value)
                                HttpContext.Session.SetInt32("Carga", Convert.ToInt32(reader["Carga"]));
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginCliente", "Links");
        }
    }
}


