using System;

namespace Inventario.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string NombreProd { get; set; }
        public int Cantidad { get; set; }
        public bool Activo { get; set; }
    }

    public class MovimientoProducto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public TipoMovimiento Tipo { get; set; }
        public DateTime FechaHora { get; set; }
        public string UsuarioId { get; set; }
        public string RealizadoPor { get; set; }
    }

    public enum TipoMovimiento
    {
        Entrada,
        Salida
    }
}
