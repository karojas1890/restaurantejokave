namespace WebApplication1.Models.ViewModels
{
    public class FacturaViewModel
    {
        public int IdMesa { get; set; }
        public int NumeroPedido { get; set; }
        public DateTime Fecha { get; set; }
        public string EstadoPago { get; set; } // Ej: "Sin cancelar"

        public List<DetalleFacturaItem> Items { get; set; } = new List<DetalleFacturaItem>();

        public decimal Subtotal => Items.Sum(i => i.Subtotal);
        public decimal IVA => Subtotal * 0.13m;  // Ejemplo 13% IVA
        public decimal Total => Subtotal + IVA;
    }
}
