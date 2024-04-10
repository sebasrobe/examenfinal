using System;
using System.Data.SqlClient;

namespace RegistroTrabajadoresEmpresa
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "tu_cadena_de_conexion";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Conexión establecida con la base de datos.");

                while (true)
                {
                    Console.WriteLine("Ingrese el nombre del trabajador (o escriba 'fin' para terminar):");
                    string nombre = Console.ReadLine();

                    if (nombre.ToLower() == "fin")
                        break;

                    Console.WriteLine("Ingrese los apellidos del trabajador:");
                    string apellidos = Console.ReadLine();

                    Console.WriteLine("Ingrese el sueldo bruto del trabajador:");
                    double sueldoBruto;
                    while (!double.TryParse(Console.ReadLine(), out sueldoBruto) || sueldoBruto <= 0)
                    {
                        Console.WriteLine("Por favor, ingrese un sueldo bruto válido (mayor a cero):");
                    }

                    Console.WriteLine("Ingrese la categoría del trabajador (A/B/C):");
                    string categoria = Console.ReadLine().ToUpper();

                    double porcentajeAumento = 0.0;
                    switch (categoria)
                    {
                        case "A":
                            porcentajeAumento = 0.1;
                            break;
                        case "B":
                            porcentajeAumento = 0.2;
                            break;
                        case "C":
                            porcentajeAumento = 0.3;
                            break;
                        default:
                            Console.WriteLine("Categoría no válida. Se asignará un aumento del 10% por defecto.");
                            porcentajeAumento = 0.1;
                            break;
                    }

                    double montoAumento = porcentajeAumento * sueldoBruto;
                    double sueldoNeto = sueldoBruto + montoAumento;

                    string insertQuery = $"INSERT INTO Trabajadores (Nombre, Apellidos, SueldoBruto, Categoria, MontoAumento, SueldoNeto) " +
                                         $"VALUES ('{nombre}', '{apellidos}', {sueldoBruto}, '{categoria}', {montoAumento}, {sueldoNeto})";

                    SqlCommand command = new SqlCommand(insertQuery, connection);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        Console.WriteLine("Empleado registrado correctamente.");
                    else
                        Console.WriteLine("Error al registrar el empleado.");
                }

                Console.WriteLine("\nLista de empleados registrados:");
                string selectQuery = "SELECT Nombre, Apellidos, SueldoBruto, MontoAumento, SueldoNeto FROM Trabajadores";
                SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                SqlDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Nombre"]} {reader["Apellidos"]} - Sueldo Bruto: {reader["SueldoBruto"]} - Monto Aumento: {reader["MontoAumento"]} - Sueldo Neto: {reader["SueldoNeto"]}");
                }

                reader.Close();

                double totalSueldosNetos = 0.0;
                string totalQuery = "SELECT SUM(SueldoNeto) AS TotalSueldosNetos FROM Trabajadores";
                SqlCommand totalCommand = new SqlCommand(totalQuery, connection);
                object result = totalCommand.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    totalSueldosNetos = Convert.ToDouble(result);
                    Console.WriteLine($"\nMonto total de Sueldos Netos: {totalSueldosNetos}");
                }
            }
        }
    }
}