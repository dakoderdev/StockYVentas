public class Televisor : Producto
{
    public int Pulgadas { get; set; }

    public string PantallaTipo { get; set; }

    public Televisor(string nombre, decimal precio, int stock, int pulgadas, string pantallaTipo) : base(nombre, precio, stock)
    {
        Pulgadas = pulgadas;
        PantallaTipo = pantallaTipo;
    }

    public override int CalcularPrecioFinal()
    {
        return (int)(Precio * 1.21m);
    }
}

public class Heladera : Producto
{
    public int Capacidad { get; set; }
    public string Tipo { get; set; }

    public Heladera(string nombre, decimal precio, int stock, int capacidad, string tipo) : base(nombre, precio, stock)
    {
        Capacidad = capacidad;
        Tipo = tipo;
    }

    public override int CalcularPrecioFinal()
    {
        return (int)(Precio * 1.21m);
    }
}

public class Lavarropa : Producto
{
    public int Carga { get; set; }
    public string Tipo { get; set; }

    public Lavarropa(string nombre, decimal precio, int stock, int carga, string tipo) : base(nombre, precio, stock)
    {
        Carga = carga;
        Tipo = tipo;
    }

    public override int CalcularPrecioFinal()
    {
        return (int)(Precio * 1.21m);
    }
}