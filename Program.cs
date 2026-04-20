using System;
using System.Collections.Generic;
using System.Linq;

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
        if (ListaProductos.Count == 0)
        {
            Console.WriteLine("No hay productos disponibles.");
            return;
        }
        Console.WriteLine("\n=== Productos Disponibles ===");
        foreach (var p in ListaProductos)
        {
            Console.WriteLine(p.MostrarDetalles());
        }
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

public abstract class Producto
{
    private static int nextId = 1;
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public Producto(string nombre, decimal precio, int stock)
    {
        Id = nextId++;
        Nombre = nombre;
        Precio = precio;
        Stock = stock;
    }

    public abstract decimal CalcularPrecioFinal();

    public abstract string MostrarDetalles();

    public int AgregarStock(int cantidad)
    {
        Stock += cantidad;
        return Stock;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Sucursal> sucursales = new List<Sucursal>
        {
            new Sucursal("Centro", "Centro"),
            new Sucursal("Norte", "Norte")
        };

        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== SISTEMA DE GESTIÓN DE STOCK Y VENTAS ==========");
            Console.WriteLine("Seleccione sucursal:");
            Console.WriteLine("1 - Sucursal Centro");
            Console.WriteLine("2 - Sucursal Norte");
            Console.WriteLine("0 - Salir");
            Console.Write("\nOpción: ");

            if (!int.TryParse(Console.ReadLine(), out int sucOpt) || sucOpt < 0 || sucOpt > 2)
            {
                Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                Console.ReadKey();
                continue;
            }

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
            Console.WriteLine($"========== {sucursal.Nombre.ToUpper()} ==========");
            Console.WriteLine("Seleccione acción:");
            Console.WriteLine("1 - Agregar producto");
            Console.WriteLine("2 - Listar productos");
            Console.WriteLine("3 - Vender producto");
            Console.WriteLine("0 - Volver atrás");
            Console.Write("\nOpción: ");

            if (!int.TryParse(Console.ReadLine(), out int accion) || accion < 0 || accion > 3)
            {
                Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                Console.ReadKey();
                continue;
            }

            switch (accion)
            {
                case 1:
                    AgregarProducto(sucursal);
                    break;
                case 2:
                    ListarProductos(sucursal);
                    break;
                case 3:
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
        Console.WriteLine("========== AGREGAR PRODUCTO ==========");
        Console.WriteLine("Seleccione tipo de producto:");
        Console.WriteLine("1 - Televisor");
        Console.WriteLine("2 - Heladera");
        Console.WriteLine("3 - Lavarropa");
        Console.Write("\nOpción: ");

        if (!int.TryParse(Console.ReadLine(), out int tipo) || tipo < 1 || tipo > 3)
        {
            Console.WriteLine("Tipo inválido. Presione cualquier tecla...");
            Console.ReadKey();
            return;
        }

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
}

Console.WriteLine("\n✓ Producto agregado exitosamente. Presione cualquier tecla...");
        Console.ReadKey();
    }

    static void ListarProductos(Sucursal sucursal) // 2
    {
        Console.Clear();
        Console.WriteLine($"========== PRODUCTOS DE {sucursal.Nombre.ToUpper()} ==========");
        sucursal.ListarProductos();
        Console.WriteLine("Presione cualquier tecla para volver...");
        Console.ReadKey();
    }

    static void VenderProducto(Sucursal sucursal) // 3
    {
        Console.Clear();
        Console.WriteLine("========== REALIZAR VENTA ==========");

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
            Console.WriteLine("========== VENTA REALIZADA CON ÉXITO ==========");
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
}