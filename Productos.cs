public abstract class Producto
{
    public int IdProducto { get; set; }
    public int Codigo     { get; set; }
    public string Nombre  { get; set; } = "";
    public decimal Precio { get; set; }
    public int Stock      { get; set; }
    public int IdSucursal { get; set; }

    public abstract decimal CalcularPrecioFinal();
    public abstract string MostrarDetalles();

    public void AgregarStock(int cantidad) => Stock += cantidad;
}

public class Televisor : Producto
{
    public int Pulgadas        { get; set; }
    public string PantallaTipo { get; set; } = "";

    public override decimal CalcularPrecioFinal() => Precio * 1.21m;

    public override string MostrarDetalles() =>
        $"[TV ] {Nombre,-30} | ${CalcularPrecioFinal(),10:F2} | Stock: {Stock,3} | {Pulgadas}\" {PantallaTipo}";
}

public class Heladera : Producto
{
    public int CapacidadLitros { get; set; }
    public string Tipo         { get; set; } = "";

    public override decimal CalcularPrecioFinal() => Precio * 1.18m;

    public override string MostrarDetalles() =>
        $"[HEL] {Nombre,-30} | ${CalcularPrecioFinal(),10:F2} | Stock: {Stock,3} | {CapacidadLitros}L {Tipo}";
}

public class Lavarropas : Producto
{
    public int CargaKg { get; set; }
    public string Tipo { get; set; } = "";

    public override decimal CalcularPrecioFinal() => Precio * 1.25m;

    public override string MostrarDetalles() =>
        $"[LAV] {Nombre,-30} | ${CalcularPrecioFinal(),10:F2} | Stock: {Stock,3} | {CargaKg}kg {Tipo}";
}
