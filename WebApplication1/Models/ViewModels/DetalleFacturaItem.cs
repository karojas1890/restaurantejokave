namespace WebApplication1.Models.ViewModels
{
    public class DetalleFacturaItem
    {
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorIndividual { get; set; }
        public decimal Subtotal => Cantidad * ValorIndividual;
    }
}
