public class Televisor : Producto
{
    public int Pulgadas { get; set; }

    public string PantallaTipo { get; set; }

    public Televisor(string nombre, decimal precio, int stock, int pulgadas, string pantallaTipo) : base(nombre, precio, stock)
    {
        Pulgadas = pulgadas;
        PantallaTipo = pantallaTipo;
    }

    public override decimal CalcularPrecioFinal()
    {
        return Precio * 1.21m;
    }

    public override string MostrarDetalles()
    {
        return $"[ID: {Nombre}] - ${Precio:F2} (Precio final: ${CalcularPrecioFinal():F2}) - Stock: {Stock}\n" +
               $"   └─ Pulgadas: {Pulgadas}\" | Tipo de pantalla: {PantallaTipo}";
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

    public override decimal CalcularPrecioFinal()
    {
        return Precio * 1.18m;
    }

    public override string MostrarDetalles()
    {
        return $"[ID: {Id}] {Nombre} - ${Precio:F2} (Precio final: ${CalcularPrecioFinal():F2}) - Stock: {Stock}\n" +
               $"   └─ Capacidad: {Capacidad} litros | Tipo: {Tipo}";
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

    public override decimal CalcularPrecioFinal()
    {
        return Precio * 1.25m;
    }

    public override string MostrarDetalles()
    {
        return $"[ID: {Id}] {Nombre} - ${Precio:F2} (Precio final: ${CalcularPrecioFinal():F2}) - Stock: {Stock}\n" +
               $"   └─ Carga: {Carga} kg | Tipo: {Tipo}";
    }
}