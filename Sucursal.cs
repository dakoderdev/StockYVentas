using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

public class Sucursal
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";

    public List<Producto> ObtenerProductos()
    {
        var lista = new List<Producto>();

        string sql = @"
            SELECT p.IdProducto, p.Codigo, p.Nombre, p.Precio, p.Stock, p.TipoProducto,
                   t.Pulgadas,        t.TipoPantalla,
                   h.CapacidadLitros, h.Tipo  AS TipoHeladera,
                   l.CargaKg,         l.Tipo  AS TipoLavarropas
            FROM   Producto p
            LEFT JOIN Televisor  t ON p.IdProducto = t.IdProducto
            LEFT JOIN Heladera   h ON p.IdProducto = h.IdProducto
            LEFT JOIN Lavarropas l ON p.IdProducto = l.IdProducto
            WHERE  p.IdSucursal = @id
            ORDER  BY p.Codigo";

        using var cmd = new MySqlCommand(sql, DB.Conexion);
        cmd.Parameters.AddWithValue("@id", Id);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string tipo = reader["TipoProducto"].ToString() ?? "";
            Producto? p = null;

            switch (tipo)
            {
                case "Televisor":
                    p = new Televisor
                    {
                        Pulgadas = Convert.ToInt32(reader["Pulgadas"]),
                        PantallaTipo = reader["TipoPantalla"].ToString() ?? ""
                    };
                    break;

                case "Heladera":
                    p = new Heladera
                    {
                        CapacidadLitros = Convert.ToInt32(reader["CapacidadLitros"]),
                        Tipo = reader["TipoHeladera"].ToString() ?? ""
                    };
                    break;

                case "Lavarropas":
                    p = new Lavarropas
                    {
                        CargaKg = Convert.ToInt32(reader["CargaKg"]),
                        Tipo = reader["TipoLavarropas"].ToString() ?? ""
                    };
                    break;
            }

            if (p != null)
            {
                p.IdProducto = Convert.ToInt32(reader["IdProducto"]);
                p.Codigo = Convert.ToInt32(reader["Codigo"]);
                p.Nombre = reader["Nombre"].ToString() ?? "";
                p.Precio = Convert.ToDecimal(reader["Precio"]);
                p.Stock = Convert.ToInt32(reader["Stock"]);
                p.IdSucursal = Id;
                lista.Add(p);
            }
        }

        return lista;
    }

    public void MostrarProductos()
    {
        var productos = ObtenerProductos();

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"  {"Cod",4}  {"Tipo",-5}  {"Nombre",-28}  {"Precio base",11}  {"P.Final",11}  {"Stock",5}  Detalles");
        Console.WriteLine("  " + new string('-', 90));
        Console.ResetColor();

        if (productos.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  (Sin productos en esta sucursal)");
            Console.ResetColor();
        }
        else
        {
            foreach (var p in productos)
            {
                string tipoTag = p switch
                {
                    Televisor => "TV ",
                    Heladera => "HEL",
                    Lavarropas => "LAV",
                    _ => "???"
                };

                string detalles = p switch
                {
                    Televisor tv => $"{tv.Pulgadas}\" {tv.PantallaTipo}",
                    Heladera hel => $"{hel.CapacidadLitros}L {hel.Tipo}",
                    Lavarropas lav => $"{lav.CargaKg}kg {lav.Tipo}",
                    _ => ""
                };

                Console.Write($"  {p.Codigo,4}  ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{tipoTag}]");
                Console.ResetColor();
                Console.WriteLine($"  {p.Nombre,-28}  ${p.Precio,10:N2}  ${p.CalcularPrecioFinal(),10:N2}  {p.Stock,5}  {detalles}");
            }
        }

        Console.WriteLine();

    }

    public void InsertarProducto(Producto p, int codigo)
    {
        string tipoStr = p switch
        {
            Televisor => "Televisor",
            Heladera => "Heladera",
            Lavarropas => "Lavarropas",
            _ => throw new Exception("Tipo de producto desconocido")
        };

        string sqlProducto = @"
            INSERT INTO Producto (Codigo, Nombre, Precio, Stock, TipoProducto, IdSucursal)
            VALUES (@codigo, @nombre, @precio, @stock, @tipo, @sucursal);
            SELECT LAST_INSERT_ID();";

        using var cmd = new MySqlCommand(sqlProducto, DB.Conexion);
        cmd.Parameters.AddWithValue("@codigo", codigo);
        cmd.Parameters.AddWithValue("@nombre", p.Nombre);
        cmd.Parameters.AddWithValue("@precio", p.Precio);
        cmd.Parameters.AddWithValue("@stock", p.Stock);
        cmd.Parameters.AddWithValue("@tipo", tipoStr);
        cmd.Parameters.AddWithValue("@sucursal", Id);

        int newId = Convert.ToInt32(cmd.ExecuteScalar());

        switch (p)
        {
            case Televisor tv:
                {
                    using var c1 = new MySqlCommand(
                        "INSERT INTO Televisor (IdProducto, Pulgadas, TipoPantalla) VALUES (@id, @p, @t)",
                        DB.Conexion);
                    c1.Parameters.AddWithValue("@id", newId);
                    c1.Parameters.AddWithValue("@p", tv.Pulgadas);
                    c1.Parameters.AddWithValue("@t", tv.PantallaTipo);
                    c1.ExecuteNonQuery();
                    break;
                }

            case Heladera hel:
                {
                    using var c2 = new MySqlCommand(
                        "INSERT INTO Heladera (IdProducto, CapacidadLitros, Tipo) VALUES (@id, @c, @t)",
                        DB.Conexion);
                    c2.Parameters.AddWithValue("@id", newId);
                    c2.Parameters.AddWithValue("@c", hel.CapacidadLitros);
                    c2.Parameters.AddWithValue("@t", hel.Tipo);
                    c2.ExecuteNonQuery();
                    break;
                }

            case Lavarropas lav:
                {
                    using var c3 = new MySqlCommand(
                        "INSERT INTO Lavarropas (IdProducto, CargaKg, Tipo) VALUES (@id, @c, @t)",
                        DB.Conexion);
                    c3.Parameters.AddWithValue("@id", newId);
                    c3.Parameters.AddWithValue("@c", lav.CargaKg);
                    c3.Parameters.AddWithValue("@t", lav.Tipo);
                    c3.ExecuteNonQuery();
                    break;
                }
        }
    }

    public bool ModificarProducto(int codigo, string nuevoNombre, decimal nuevoPrecio, int nuevoStock)
    {
        string sqlSelect = "SELECT Nombre, Precio, Stock FROM Producto WHERE Codigo = @codigo AND IdSucursal = @sucursal";
        using var cmdSel = new MySqlCommand(sqlSelect, DB.Conexion);
        cmdSel.Parameters.AddWithValue("@codigo", codigo);
        cmdSel.Parameters.AddWithValue("@sucursal", Id);
        using var reader = cmdSel.ExecuteReader();

        if (!reader.Read()) { reader.Close(); return false; }

        string nombreFinal = string.IsNullOrWhiteSpace(nuevoNombre) ? reader["Nombre"].ToString()! : nuevoNombre;
        decimal precioFinal = nuevoPrecio <= 0 ? Convert.ToDecimal(reader["Precio"]) : nuevoPrecio;
        int stockFinal = nuevoStock < 0 ? Convert.ToInt32(reader["Stock"]) : nuevoStock;
        reader.Close();

        string sqlUpdate = @"
            UPDATE Producto
            SET Nombre = @nombre, Precio = @precio, Stock = @stock
            WHERE Codigo = @codigo AND IdSucursal = @sucursal";

        using var cmdUpd = new MySqlCommand(sqlUpdate, DB.Conexion);
        cmdUpd.Parameters.AddWithValue("@nombre", nombreFinal);
        cmdUpd.Parameters.AddWithValue("@precio", precioFinal);
        cmdUpd.Parameters.AddWithValue("@stock", stockFinal);
        cmdUpd.Parameters.AddWithValue("@codigo", codigo);
        cmdUpd.Parameters.AddWithValue("@sucursal", Id);

        return cmdUpd.ExecuteNonQuery() > 0;
    }

    public bool EliminarProducto(int codigo)
    {
        string sql = "DELETE FROM Producto WHERE Codigo = @codigo AND IdSucursal = @sucursal";
        using var cmd = new MySqlCommand(sql, DB.Conexion);
        cmd.Parameters.AddWithValue("@codigo", codigo);
        cmd.Parameters.AddWithValue("@sucursal", Id);
        return cmd.ExecuteNonQuery() > 0;
    }

    public decimal RegistrarVenta(int codigoProducto, int cantidad)
    {
        var productos = ObtenerProductos();
        var producto = productos.FirstOrDefault(p => p.Codigo == codigoProducto)
                        ?? throw new Exception("Producto no encontrado en esta sucursal.");

        if (producto.Stock < cantidad)
            throw new Exception($"Stock insuficiente. Disponible: {producto.Stock} unidad/es.");

        decimal precioUnitario = producto.CalcularPrecioFinal();
        decimal total = precioUnitario * cantidad;

        using var transaccion = DB.Conexion!.BeginTransaction();
        try
        {
            string sqlVenta = @"
                INSERT INTO Venta (Fecha, IdSucursal) VALUES (NOW(), @sucursal);
                SELECT LAST_INSERT_ID();";
            using var cmdV = new MySqlCommand(sqlVenta, DB.Conexion, transaccion);
            cmdV.Parameters.AddWithValue("@sucursal", Id);
            int idVenta = Convert.ToInt32(cmdV.ExecuteScalar());

            string sqlDet = @"
                INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, PrecioUnitario)
                VALUES (@venta, @prod, @cant, @precio)";
            using var cmdD = new MySqlCommand(sqlDet, DB.Conexion, transaccion);
            cmdD.Parameters.AddWithValue("@venta", idVenta);
            cmdD.Parameters.AddWithValue("@prod", producto.IdProducto);
            cmdD.Parameters.AddWithValue("@cant", cantidad);
            cmdD.Parameters.AddWithValue("@precio", precioUnitario);
            cmdD.ExecuteNonQuery();

            string sqlStock = "UPDATE Producto SET Stock = Stock - @cant WHERE IdProducto = @id";
            using var cmdS = new MySqlCommand(sqlStock, DB.Conexion, transaccion);
            cmdS.Parameters.AddWithValue("@cant", cantidad);
            cmdS.Parameters.AddWithValue("@id", producto.IdProducto);
            cmdS.ExecuteNonQuery();

            transaccion.Commit();
        }
        catch
        {
            transaccion.Rollback();
            throw;
        }

        return total;
    }

    public bool ExisteProducto(int codigo)
    {  
        string sql = "SELECT COUNT(*) FROM Producto WHERE Codigo = @cod AND IdSucursal = @suc";
        using var cmd = new MySqlCommand(sql, DB.Conexion);
        cmd.Parameters.AddWithValue("@cod", codigo);
        cmd.Parameters.AddWithValue("@suc", Id);

        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }
}
