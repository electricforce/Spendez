using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using ApiIntento3.seguridad; 


namespace ApiIntento3.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly hash _hash;
        private readonly IConfiguration _configuration;
        public UsuarioController(IConfiguration configuration)
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






        
        [HttpPost("Registrar Usuario")]
        public async Task<IActionResult> RegistrarUsuario(
            [FromForm] string emailU, [FromForm] string passwordU, [FromForm] string nombreU, [FromForm] string apellidoU, [FromForm] string nomIniU)
        {
            
            
            string conexion = _configuration.GetConnectionString("ConeSpendEz");
            string hashedPassword = HashPassword(passwordU);

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("RegistrarUsuario", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros del procedimiento almacenado
                        command.Parameters.AddWithValue("@EmailU", emailU);
                        command.Parameters.AddWithValue("@PasswordU", hashedPassword); 
                        command.Parameters.AddWithValue("@NombreU", nombreU ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ApellidoU", apellidoU ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NomIniU", nomIniU);

                        // Ejecuta el procedimiento almacenado
                        int filasAfectadas = await command.ExecuteNonQueryAsync();

                        if (filasAfectadas > 0)
                        {
                            return Ok(new { Mensaje = "El usuario fue registrado exitosamente." });
                        }
                        else
                        {
                            return BadRequest(new { Mensaje = "No se pudo registrar el usuario. Por favor, verifica los datos." });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { Mensaje = "Error en la base de datos.", Detalle = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = "Ocurrió un error inesperado.", Detalle = ex.Message });
            }
        }
































        [HttpGet("loguearUsuario")]
        public async Task<IActionResult> LoguearUsuario([FromQuery] string emailU, [FromQuery] string password, [FromQuery] string nomIniU)
        {
            
            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");
            string storedHash = ObtenerHashPorEmail(emailU);

            if (storedHash == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            // Verificar la contraseña
            bool isPasswordValid = AutenticarUsuario(password, storedHash);

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("LoguearUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmailU", (object)emailU ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NomIniU", (object)nomIniU ?? (object)DBNull.Value);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var Usuario = new
                            {
                                idUsuario = reader["ID_U"],
                                //emailU = reader["Email_U"],
                                //passwordU = reader["Password_U"],
                                //nombreU = reader["Nombre_U"] != DBNull.Value ? reader["Nombre_U"].ToString() : null,
                                //apellidoU = reader["Apellido_U"] != DBNull.Value ? reader["Apellido_U"].ToString() : null,
                                //nomIniU = reader["NomIni_U"]
                            };

                            resultado.Add(Usuario);
                        }
                    }
                }
            }

            // Verifica si se encontraron resultados
            if (resultado.Count == 0)
            {
                return NotFound(new { Mensaje = "No se encontró una coincidencia entre el email o el nombre de inicio y la contraseña proporcionados." });
            }

            return Ok(resultado);
        }



















        [HttpGet("Mostrar ID del Usuario")]
        public async Task<IActionResult> MostrarIDUsuario([FromQuery] string emailU, [FromQuery] string password, [FromQuery] string nomIniU)
        {
            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("MostrarID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmailU", (object)emailU ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@PasswordU", (object)password ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NomIniU", (object)nomIniU ?? (object)DBNull.Value);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows) // Verificar si hay filas antes de leer
                        {
                            return NotFound(new { Mensaje = "No se encontró una coincidencia entre el email o el nombre de inicio y la contraseña proporcionados." });
                        }

                        while (await reader.ReadAsync()) // Leer las filas existentes
                        {
                            var IdUsuario = reader["ID_U"];
                            resultado.Add(IdUsuario);
                        }
                    }
                }
            }

            return Ok(resultado);
        }

    }
}
