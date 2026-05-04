
using MySql.Data.MySqlClient;

public static class DB
{
    private static string connectionString = "server=localhost;user=root;password=brat;database=ElectrodomesticosDB";
    public static MySqlConnection? Conexion { get; private set; }

    public static void Conectar()
    {
        Conexion = new MySqlConnection(connectionString);
        Conexion.Open();
    }
}