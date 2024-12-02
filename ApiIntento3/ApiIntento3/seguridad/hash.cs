using BCrypt.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace ApiIntento3.seguridad
{
    public class hash
    {
        private readonly IConfiguration _configuration;
        // Método para hashear la contraseña antes de almacenarla
        public hash(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string HashPassword(string password)
        {
            // Usa el método estático HashPassword de la clase BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        // Método para verificar la contraseña ingresada con el hash almacenado
        public bool AutenticarUsuario(string email, string password)
        {
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();

                // Crear el comando para ejecutar el procedimiento almacenado
                using (SqlCommand command = new SqlCommand("AutenticarUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros del procedimiento almacenado
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

                    // Parámetro de salida que indicará si la autenticación fue exitosa
                    SqlParameter autenticadoParam = new SqlParameter("@Autenticado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(autenticadoParam);

                    // Ejecutar el procedimiento almacenado
                    command.ExecuteNonQuery();

                    // Obtener el valor del parámetro de salida
                    bool autenticado = (bool)autenticadoParam.Value;

                    return autenticado;
                }
            }
        }



        public string ObtenerHashPorEmail(string email)
        {
            string conexion = _configuration.GetConnectionString("ConeSpendEz");
            using (SqlConnection connection = new SqlConnection(conexion))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT Password_U FROM Usuarios WHERE Email_U = @Email_U", connection))
                {
                    command.Parameters.AddWithValue("@Email_U", email);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader["Password_U"].ToString();
                        }
                    }
                }
            }
            return null; // Devuelve null si el usuario no existe
        }
    }
}
