namespace Piolax_WebApp.DTOs
{
    public class InventarioCategoriasDetalleDTO
    {
        public string nombreCategoria { get; set; }
        public List<InventarioDetalleDTO> refacciones { get; set; }
    }
}
