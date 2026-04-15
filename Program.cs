public class Sucursal
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Ubicacion { get; set; }

    public List<Producto> ListaProductos { get; set; }

    public Sucursal(string nombre, string ubicacion)
    {
        Nombre = nombre;
        Ubicacion = ubicacion;
        ListaProductos = new List<Producto>();
    }

    public void AgregarNuevoProducto(Producto producto)
    {
        var productoFiltrado = ListaProductos.Where(p => p.Id == producto.Id);
        if (!productoFiltrado.Any())
        {
            ListaProductos.Add(producto);
        } else
        {
            var productoExistente = ListaProductos.First(p => p.Id == producto.Id);
            productoExistente.AgregarStock(producto.Stock);
        }
    }

    public void 
}

public abstract class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public Producto(string nombre, decimal precio, int stock)
    {
        Nombre = nombre;
        Precio = precio;
        Stock = stock;
    }

    public abstract int CalcularPrecioFinal();

    public int AgregarStock(int cantidad)
    {
        Stock += cantidad;
        return Stock;
    }
}