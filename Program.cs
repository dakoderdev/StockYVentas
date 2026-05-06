using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            DB.Conectar();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
            Console.ResetColor();
            Console.WriteLine("Verificá que MySQL esté corriendo y que la contraseña en DB.cs sea correcta.");
            Console.ReadKey();
            return;
        }

        List<Sucursal> sucursales = CargarSucursales();

        while (true)
        {
            Console.Clear();
            EscribirTitulo("SISTEMA DE GESTIÓN DE STOCK Y VENTAS");
            Console.WriteLine("  Seleccione una sucursal:\n");
            ListarMenu(sucursales.Select(s => s.Nombre).ToArray(), "Salir");
            Console.Write("\n  Opción: ");

            int sucOpt = ValidarInput(0, sucursales.Count);
            if (sucOpt == 0) break;

            MenuSucursal(sucursales[sucOpt - 1]);
        }

        Console.WriteLine("\n  Gracias por usar el sistema. ¡Hasta luego!\n");
    }

    static List<Sucursal> CargarSucursales()
    {
        var lista = new List<Sucursal>();
        string sql = "SELECT IdSucursal, Nombre FROM Sucursal";
        using var cmd = new MySqlCommand(sql, DB.Conexion);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Sucursal
            {
                Id     = Convert.ToInt32(reader["IdSucursal"]),
                Nombre = reader["Nombre"].ToString() ?? ""
            });
        }
        return lista;
    }

    static void MenuSucursal(Sucursal sucursal)
    {
        while (true)
        {
            Console.Clear();
            EscribirTitulo($"SUCURSAL {sucursal.Nombre.ToUpper()}");
            Console.WriteLine("  Seleccione una acción:\n");
            ListarMenu(new[]
            {
                "Agregar producto",
                "Modificar producto",
                "Eliminar producto",
                "Listar productos",
                "Registrar venta"
            }, "Volver atrás");
            Console.Write("\n  Opción: ");

            int accion = ValidarInput(0, 5);
            switch (accion)
            {
                case 1: AgregarProducto(sucursal);   break;
                case 2: ModificarProducto(sucursal); break;
                case 3: EliminarProducto(sucursal);  break;
                case 4: ListarProductos(sucursal);   break;
                case 5: VenderProducto(sucursal);    break;
                case 0: return;
            }
        }
    }

    static void AgregarProducto(Sucursal sucursal)
    {
        Console.Clear();
        EscribirTitulo("AGREGAR PRODUCTO");

        Console.WriteLine("  Seleccione tipo de producto:\n");
        ListarMenu(new[] { "Televisor", "Heladera", "Lavarropas" }, "Cancelar");
        Console.Write("\n  Opción: ");
        int tipo = ValidarInput(0, 3);
        if (tipo == 0) return;

        Console.Write("\n  Nombre del producto: ");
        string nombre = Console.ReadLine() ?? "";

        Console.Write("  Código (número único): ");
        if (!int.TryParse(Console.ReadLine(), out int codigo) || codigo <= 0)
        { MostrarError("Código inválido."); return; }

        Console.Write("  Precio base: $");
        if (!decimal.TryParse(Console.ReadLine(), out decimal precio) || precio <= 0)
        { MostrarError("Precio inválido."); return; }

        Console.Write("  Stock inicial: ");
        if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
        { MostrarError("Stock inválido."); return; }

        Producto? p = null;

        switch (tipo)
        {
            case 1:
                Console.Write("  Pulgadas: ");
                if (!int.TryParse(Console.ReadLine(), out int pulg) || pulg <= 0)
                { MostrarError("Valor inválido."); return; }
                Console.Write("  Tipo de pantalla (LED / OLED / QLED): ");
                string pant = Console.ReadLine() ?? "";
                p = new Televisor { Nombre = nombre, Precio = precio, Stock = stock,
                                    Pulgadas = pulg, PantallaTipo = pant };
                break;

            case 2:
                Console.Write("  Capacidad en litros: ");
                if (!int.TryParse(Console.ReadLine(), out int cap) || cap <= 0)
                { MostrarError("Valor inválido."); return; }
                Console.Write("  Tipo (No Frost / Freezer): ");
                string tipHel = Console.ReadLine() ?? "";
                p = new Heladera { Nombre = nombre, Precio = precio, Stock = stock,
                                   CapacidadLitros = cap, Tipo = tipHel };
                break;

            case 3:
                Console.Write("  Carga en kg: ");
                if (!int.TryParse(Console.ReadLine(), out int carga) || carga <= 0)
                { MostrarError("Valor inválido."); return; }
                Console.Write("  Tipo (Automático / Semi): ");
                string tipLav = Console.ReadLine() ?? "";
                p = new Lavarropas { Nombre = nombre, Precio = precio, Stock = stock,
                                     CargaKg = carga, Tipo = tipLav };
                break;
        }

        if (p == null) return;

        try
        {
            sucursal.InsertarProducto(p, codigo);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  ✓ Producto agregado exitosamente.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            MostrarError("No se pudo guardar: " + ex.Message);
        }

        Pausa();
    }

    static void ModificarProducto(Sucursal sucursal)
    {
        Console.Clear();
        EscribirTitulo("MODIFICAR PRODUCTO");
        sucursal.MostrarProductos();

        Console.Write("  Código del producto a modificar: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        { MostrarError("Código inválido."); return; }

        Console.Write("  Nuevo nombre (Enter para no cambiar): ");
        string nuevoNombre = Console.ReadLine() ?? "";

        Console.Write("  Nuevo precio base (0 para no cambiar): $");
        decimal.TryParse(Console.ReadLine(), out decimal nuevoPrecio);

        Console.Write("  Nuevo stock (-1 para no cambiar): ");
        int.TryParse(Console.ReadLine(), out int nuevoStock);

        try
        {
            bool ok = sucursal.ModificarProducto(codigo, nuevoNombre, nuevoPrecio, nuevoStock);
            if (ok)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✓ Producto modificado correctamente.");
                Console.ResetColor();
            }
            else
                MostrarError("No se encontró el producto con ese código en esta sucursal.");
        }
        catch (Exception ex) { MostrarError("Error al modificar: " + ex.Message); }

        Pausa();
    }

    static void EliminarProducto(Sucursal sucursal)
    {
        Console.Clear();
        EscribirTitulo("ELIMINAR PRODUCTO");
        sucursal.MostrarProductos();

        Console.Write("  Código del producto a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        { MostrarError("Código inválido."); return; }

        Console.Write("  ¿Confirmar eliminación? (s/n): ");
        string conf = Console.ReadLine()?.ToLower() ?? "";
        if (conf != "s") { Console.WriteLine("  Operación cancelada."); Pausa(); return; }

        try
        {
            bool ok = sucursal.EliminarProducto(codigo);
            if (ok)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n  ✓ Producto eliminado correctamente.");
                Console.ResetColor();
            }
            else
                MostrarError("No se encontró el producto con ese código en esta sucursal.");
        }
        catch (Exception ex) { MostrarError("Error al eliminar: " + ex.Message); }

        Pausa();
    }

    static void ListarProductos(Sucursal sucursal)
    {
        Console.Clear();
        EscribirTitulo($"PRODUCTOS — SUCURSAL {sucursal.Nombre.ToUpper()}");
        sucursal.MostrarProductos();
        Pausa();
    }

    static void VenderProducto(Sucursal sucursal)
    {
        Console.Clear();
        EscribirTitulo("REGISTRAR VENTA");
        sucursal.MostrarProductos();

        Console.Write("  Código del producto a vender: ");
        if (!int.TryParse(Console.ReadLine(), out int codigo))
        { MostrarError("Código inválido."); return; }

        Console.Write("  Cantidad: ");
        if (!int.TryParse(Console.ReadLine(), out int cant) || cant <= 0)
        { MostrarError("Cantidad inválida."); return; }

        try
        {
            decimal total = sucursal.RegistrarVenta(codigo, cant);

            Console.Clear();
            EscribirTitulo("VENTA REGISTRADA CON ÉXITO");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  Cantidad vendida : {cant} unidad/es");
            Console.WriteLine($"  Total de la venta: ${total:N2}");
            Console.ResetColor();
            Console.WriteLine();

            EscribirSubtitulo("STOCK ACTUALIZADO");
            sucursal.MostrarProductos();
        }
        catch (Exception ex) { MostrarError("No se pudo registrar la venta: " + ex.Message); }

        Pausa();
    }

    static void EscribirTitulo(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"========== {titulo} ==========");
        Console.ResetColor();
    }

    static void EscribirSubtitulo(string subtitulo)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ── {subtitulo} ──");
        Console.ResetColor();
    }

    static void ListarMenu(string[] opciones, string opcionVolver)
    {
        for (int i = 0; i < opciones.Length; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  [{i + 1}]");
            Console.ResetColor();
            Console.WriteLine($" {opciones[i]}");
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("  [0]");
        Console.ResetColor();
        Console.WriteLine($" {opcionVolver}");
    }

    static void MostrarError(string mensaje)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  ✗ {mensaje}");
        Console.ResetColor();
    }

    static void Pausa()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  Presione cualquier tecla para continuar...");
        Console.ResetColor();
        Console.ReadKey();
    }

    static int ValidarInput(int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int num) && num >= min && num <= max)
                return num;

            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("  Opción inválida. Intente de nuevo: ");
        }
    }
}
