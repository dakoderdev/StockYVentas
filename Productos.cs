public abstract class Producto
{
    public int IdProducto { get; set; }
    public int Codigo     { get; set; }
    public string Nombre  { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock      { get; set; }
    public int IdSucursal { get; set; }

    public abstract decimal CalcularPrecioFinal();

    public void AgregarStock(int cantidad) => Stock += cantidad;
}

public class Televisor : Producto
{
    public int Pulgadas        { get; set; }
    public string PantallaTipo { get; set; } = "";

    public override decimal CalcularPrecioFinal() => Precio * 1.21m;
}

public class Heladera : Producto
{
    public int CapacidadLitros { get; set; }
    public string Tipo         { get; set; } = "";

    public override decimal CalcularPrecioFinal() => Precio * 1.18m;
}

public class Lavarropas : Producto
{
    public int CargaKg { get; set; }
    public string Tipo { get; set; } = "";

    public override decimal CalcularPrecioFinal() => Precio * 1.25m;
}
