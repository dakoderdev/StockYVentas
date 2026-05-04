using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Tls.Crypto.Impl;
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
            Console.WriteLine("Error al conectar a la base de datos: " + ex.Message);
            return;
        }

        List<Sucursal> sucursales = new List<Sucursal>
        {
            new Sucursal("Centro", "Centro"),
            new Sucursal("Norte", "Norte")
        };

        while (true)
        {
            Console.Clear();
            EscribirTitulo("SISTEMA DE GESTIÓN DE STOCK Y VENTAS");
            Console.WriteLine("Seleccione sucursal:");
            ListarMenu(new string[]
            {"Sucursal Centro",
            "Sucursal Norte"},
            "Salir");
            Console.Write("\nOpción: ");

            int sucOpt = ValidarInput(0, 2);
            if (sucOpt == 0) break;

            Sucursal sucursal = sucursales[sucOpt - 1];
            MenuSucursal(sucursal);
        }
        Console.WriteLine("Gracias por usar el sistema. ¡Hasta luego!");
    }

    static void MenuSucursal(Sucursal sucursal)
    {
        while (true)
        {
            Console.Clear();
            EscribirTitulo("SUCURSAL " + sucursal.Nombre);
            Console.WriteLine("Seleccione acción:");
            ListarMenu(new string[]
            {"Agregar producto",
            "Modificar producto",
            "Eliminar producto",
            "Listar productos",
            "Vender producto"},
            "Volver atrás");
            Console.Write("\nOpción: ");

            int accion = ValidarInput(0, 5);

            switch (accion)
            {
                case 1:
                    AgregarProducto(sucursal);
                    break;
                case 2:
                    ModificarProducto(sucursal);
                    break;
                case 3:
                    EliminarProducto(sucursal);
                    break;
                case 4:
                    ListarProductos(sucursal);
                    break;
                case 5:
                    VenderProducto(sucursal);
                    break;
                case 0:
                    return;
            }
        }
    }

    static void AgregarProducto(Sucursal sucursal) // 1
    {
        Console.Clear();
        EscribirTitulo("AGREGAR PRODUCTO");
        Console.WriteLine("Seleccione tipo de producto:");
        ListarMenu(new string[] { "Televisor", "Heladera", "Lavarropa" }, "Cancelar");
        Console.Write("\nOpción: ");

        if (!int.TryParse(Console.ReadLine(), out int tipo) || tipo < 0 || tipo > 3)
        {
            Console.WriteLine("Tipo inválido. Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }
        if (tipo == 0) return;

        Console.Write("Nombre del producto: ");
        string nombre = Console.ReadLine() ?? "";

        Console.Write("Precio: $ ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal precio) || precio <= 0)
        {
            Console.WriteLine("Precio inválido. Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }

        Console.Write("Stock inicial: ");
        if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
        {
            Console.WriteLine("Stock inválido. Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }

        Producto? p = null;

        switch (tipo)
        {
            case 1:
                Console.Write("Pulgadas (ej: 55): ");
                if (!int.TryParse(Console.ReadLine(), out int pulg) || pulg <= 0)
                {
                    Console.WriteLine("Valor inválido. Presione cualquier tecla...");
                    Console.ReadKey();
                    return;
                }
                Console.Write("Tipo de pantalla (ej: LED, OLED, QLED): ");
                string pant = Console.ReadLine() ?? "";
                p = new Televisor(nombre, precio, stock, pulg, pant);
                break;

            case 2:
                Console.Write("Capacidad en litros (ej: 500): ");
                if (!int.TryParse(Console.ReadLine(), out int cap) || cap <= 0)
                {
                    Console.WriteLine("Valor inválido. Presione cualquier tecla...");
                    Console.ReadKey();
                    return;
                }
                Console.Write("Tipo (ej: Freezer, No Frost): ");
                string tipHeladera = Console.ReadLine() ?? "";
                p = new Heladera(nombre, precio, stock, cap, tipHeladera);
                break;

            case 3:
                Console.Write("Carga en kg (ej: 8): ");
                if (!int.TryParse(Console.ReadLine(), out int carga) || carga <= 0)
                {
                    Console.WriteLine("Valor inválido. Presione cualquier tecla...");
                    Console.ReadKey();
                    return;
                }
                Console.Write("Tipo (ej: Automático, Semi): ");
                string tipLavarropa = Console.ReadLine() ?? "";
                p = new Lavarropa(nombre, precio, stock, carga, tipLavarropa);
                break;
        }
        if (p != null)
        {
            sucursal.AgregarNuevoProducto(p);
            Console.WriteLine("\n✓ Producto agregado exitosamente. Presione cualquier tecla...");
        }
        else
        {
            Console.WriteLine("\n✗ No se pudo agregar el producto. Presione cualquier tecla...");
        }
        Console.ReadKey();
    }

    static void ModificarProducto(Sucursal sucursal) { }

    static void EliminarProducto(Sucursal sucursal) { }

    static void ListarProductos(Sucursal sucursal) // 2
    {
        Console.Clear();
        EscribirTitulo($"PRODUCTOS DE {sucursal.Nombre}");
        sucursal.ListarProductos();
        Console.WriteLine("Presione cualquier tecla para volver...");
        Console.ReadKey();
    }

    static void VenderProducto(Sucursal sucursal) // 3
    {
        Console.Clear();
        EscribirTitulo("REALIZAR VENTA");

        if (sucursal.ListaProductos.Count == 0)
        {
            Console.WriteLine("No hay productos disponibles para vender.");
            Console.WriteLine("Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }

        sucursal.ListarProductos();

        Console.Write("Ingrese código de producto a vender: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Código inválido. Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }

        Console.Write("Cantidad a vender: ");
        if (!int.TryParse(Console.ReadLine(), out int cant) || cant <= 0)
        {
            Console.WriteLine("Cantidad inválida. Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }

        try
        {
            decimal total = sucursal.VenderProducto(id, cant);
            var prod = sucursal.ListaProductos.First(p => p.Id == id);

            Console.Clear();
            EscribirTitulo("VENTA REALIZADA CON ÉXITO");
            Console.WriteLine($"Producto: {prod.Nombre}");
            Console.WriteLine($"Cantidad: {cant}");
            Console.WriteLine($"Precio total: ${total:F2}");
            Console.WriteLine($"Stock restante: {prod.Stock}");
            Console.WriteLine("\nPresione cualquier tecla para volver...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
            Console.WriteLine("Presione cualquier tecla...");
            Console.ReadKey();
        }
    }

    static void EscribirTitulo(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"========== {titulo.ToUpper()} ==========");
        Console.ResetColor();
    }

    static void EscribirInput(int index, string input)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write(index);
        Console.ResetColor();
        Console.Write($" - {input}\n");
    }

    static void ListarMenu(string[] inputs, string returnInput)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            EscribirInput(i + 1, inputs[i]);
        }
        EscribirInput(0, returnInput);
    }

    static int ValidarInput(int min, int max)
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int num) && num >= min && num <= max)
                return num;

            ReemplazarLineaAnterior("Opción inválida. Intente de nuevo: ");
        }
    }

    static void ReemplazarLineaAnterior(string linea)
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(linea);
    }
}