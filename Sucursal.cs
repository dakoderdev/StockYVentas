using MySql.Data.MySqlClient;

public class Sucursal
{
    private static int nextId = 1;
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Ubicacion { get; set; }

    public List<Producto> ListaProductos { get; set; }

    public Sucursal(string nombre, string ubicacion)
    {
        Id = nextId++;
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
        }
        else
        {
            var productoExistente = ListaProductos.First(p => p.Id == producto.Id);
            productoExistente.AgregarStock(producto.Stock);
        }
    }

    public void ListarProductos()
    {
        string consulta = @"
        SELECT p.IdProducto, p.Nombre, p.Precio, p.Stock, p.TipoProducto,
               t.Pulgadas, t.TipoPantalla,
               h.CapacidadLitros, h.Tipo AS TipoHeladera,
               l.CargaKg, l.Tipo AS TipoLavarropas
        FROM Producto p
        LEFT JOIN Televisor t ON p.IdProducto = t.IdProducto
        LEFT JOIN Heladera h ON p.IdProducto = h.IdProducto
        LEFT JOIN Lavarropas l ON p.IdProducto = l.IdProducto
        WHERE p.IdSucursal = @id";

        MySqlCommand comando = new MySqlCommand(consulta, DB.Conexion);
        comando.Parameters.AddWithValue("@id", Id);
        MySqlDataReader reader = comando.ExecuteReader();

        Console.WriteLine("\n=== Productos Disponibles ===");
        bool hayProductos = false;

        while (reader.Read())
        {
            hayProductos = true;
            string tipo = reader["TipoProducto"].ToString() ?? "";
            Console.Write($"ID: {reader["IdProducto"]} | {reader["Nombre"]} | ${reader["Precio"]} | Stock: {reader["Stock"]} | Tipo: {tipo}");

            switch (tipo)
            {
                case "Televisor":
                    Console.WriteLine($" | {reader["Pulgadas"]}\" {reader["TipoPantalla"]}");
                    break;
                case "Heladera":
                    Console.WriteLine($" | {reader["CapacidadLitros"]}L {reader["TipoHeladera"]}");
                    break;
                case "Lavarropas":
                    Console.WriteLine($" | {reader["CargaKg"]}kg {reader["TipoLavarropas"]}");
                    break;
            }
        }

        reader.Close();

        if (!hayProductos)
            Console.WriteLine("No hay productos disponibles.");

        Console.WriteLine();
    }

    public decimal VenderProducto(int id, int cantidad)
    {
        var producto = ListaProductos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
        {
            throw new Exception("Producto no encontrado");
        }
        if (producto.Stock < cantidad)
        {
            throw new Exception("Stock insuficiente");
        }
        producto.Stock -= cantidad;
        return producto.CalcularPrecioFinal() * cantidad;
    }
}