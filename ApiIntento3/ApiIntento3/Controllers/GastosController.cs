using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiIntento3.Controllers
{
    public class GastosController : Controller
    {
        private readonly IConfiguration _configuration;
        public GastosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Gastos por categoria")]
        public async Task<IActionResult> ObtenerGastosPorCategoria([FromQuery] int idUsuario,[FromQuery] int idCategoria)
        {
            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("ObtenerGastosPorCategoria", connection))
                {
                    command.CommandType=CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_U", (object)idUsuario);
                    command.Parameters.AddWithValue("@ID_Cat", (object)idCategoria);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var gastos = new
                            {
                                idGasto = reader["ID_Gas"],
                                DescripcionGas = reader["Descripcion_Gas"] != DBNull.Value ? reader["Descripcion_Gas"].ToString() : null,
                                MontoGas = reader["Monto_Gas"],
                                FechaGas = reader["Fecha_Gas"],
                                Categoria = reader["Categoria"],
                                PresupuestoAsignado = reader["PresupuestoAsignado"],
                                TipoPresupuesto = reader["TipoPresupuesto"]
                            };

                            resultado.Add(gastos);
                        }
                    }
                }
            }

            return Ok(resultado);
        }

        [HttpGet("GastosUsuario")]
        public async Task<IActionResult> ObtenerGastosPorUsuario([FromQuery] int idUsuario)
        {
            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("ObtenerGastosPorUsuario", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_U", (object)idUsuario);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var gastos = new
                            {
                                idGasto = reader["ID_Gas"],
                                DescripcionGas = reader["Descripcion_Gas"] != DBNull.Value ? reader["Descripcion_Gas"].ToString() : null,
                                MontoGas = reader["Monto_Gas"],
                                FechaGas = reader["Fecha_Gas"]
                            };

                            resultado.Add(gastos);
                        }
                    }
                }
            }
            return Ok(resultado);
        }

        [HttpDelete("Eliminar Gasto")]
        public async Task<IActionResult> EliminarGasto([FromQuery] int id)
        {

            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("EliminarGasto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID_Gas", id);

                        // Ejecuta el procedimiento almacenado y captura el número de filas afectadas
                        int filasAfectadas = await command.ExecuteNonQueryAsync();

                        if (filasAfectadas > 0)
                        {
                            return Ok(new { Mensaje = "El gasto fue eliminado exitosamente." });
                        }
                        else
                        {
                            return NotFound(new { Mensaje = "No se encontró ningún gasto con el ID proporcionado." });
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






        [HttpPost("AgregarGasto")]
        public async Task<IActionResult> AgregarGasto([FromForm] int? idCat, [FromForm] string descripcion, [FromForm] decimal monto, [FromForm] DateTime fecha, [FromForm] int idUsuario)
        {
            if (string.IsNullOrEmpty(descripcion) || monto <= 0 || fecha == DateTime.MinValue || idUsuario <= 0)
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            try
            {
                string conexion = _configuration.GetConnectionString("ConeSpendEz");
                using (var connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("dbo.AgregarGasto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID_Cat", (object)idCat ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Descripcion_Gas", descripcion);
                        command.Parameters.AddWithValue("@Monto_Gas", monto);
                        command.Parameters.AddWithValue("@Fecha_Gas", fecha);
                        command.Parameters.AddWithValue("@ID_U", idUsuario);

                        var result = await command.ExecuteScalarAsync();

                        // Si el resultado es un error, devolverlo
                        if (result is string message && message.Contains("error"))
                        {
                            return BadRequest(message);
                        }

                        return Ok(new { ID_Gasto = result, Mensaje = "Gasto agregado exitosamente" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Error de servidor", Detalle = ex.Message });
            }
        }






        [HttpPut("Actualizar Gasto")]
        public async Task<IActionResult> ActualizarGasto([FromForm] int idGasto, [FromForm] int idPreCat, [FromForm] string descripcion, [FromForm] double monto, [FromForm] string fecha)
        {
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("ActualizarGasto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros del procedimiento almacenado
                        command.Parameters.AddWithValue("@ID_Gas", idGasto);
                        command.Parameters.AddWithValue("@ID_PreCat", idPreCat);
                        command.Parameters.AddWithValue("@Descripcion_Gas", descripcion ?? (object)DBNull.Value); // Permite nulo
                        command.Parameters.AddWithValue("@Monto_Gas", monto);
                        command.Parameters.AddWithValue("@Fecha_Gas", fecha);

                        // Ejecuta el procedimiento almacenado
                        int filasAfectadas = await command.ExecuteNonQueryAsync();

                        if (filasAfectadas > 0)
                        {
                            return Ok(new { Mensaje = "El gasto fue actualizado exitosamente." });
                        }
                        else
                        {
                            return BadRequest(new { Mensaje = "No se pudo actualizar el gasto. Por favor, verifica los datos." });
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
    }
}

