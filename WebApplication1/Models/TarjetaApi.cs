using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class TarjetaApi
    {
        public string NumeroTarjeta { get; set; }

        public int IdCliente { get; set; }

        public int MesVencimiento { get; set; }

        public int AnioVencimiento { get; set; }

        public string CodigoSeguridad { get; set; }

        
        public decimal? Monto { get; set; }
    }
}
