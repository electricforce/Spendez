using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using ApiIntento3.DTOs;
using static ApiIntento3.Controllers.FinalController;
using Microsoft.Extensions.Configuration;

namespace ApiIntento3.Controllers
{
    public class presupuestoController : Controller
    {
        private readonly IConfiguration _configuration;
        public presupuestoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        [HttpPost("IngresarPresupuesto")]
        public async Task<IActionResult> IngresarPresupuestoS([FromBody] PresupuestoModel presupuesto)
        {
            if (presupuesto == null)
            {
                return BadRequest("Los datos del presupuesto son inválidos.");
            }

            // Validación de la fecha de fin
            if (presupuesto.FechaFin <= presupuesto.FechaInicio)
            {
                return BadRequest("La fecha de fin debe ser posterior a la fecha de inicio.");
            }

            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                try
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("dbo.IngresarDatosPresupuestoS", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros al comando
                        command.Parameters.AddWithValue("@NombrePresupuesto", presupuesto.NombrePresupuesto);
                        command.Parameters.AddWithValue("@MontoPresupuesto", presupuesto.MontoPresupuesto);
                        command.Parameters.AddWithValue("@FechaInicio", presupuesto.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", presupuesto.FechaFin);
                        command.Parameters.AddWithValue("@Periodo", presupuesto.Periodo);
                        command.Parameters.AddWithValue("@IDCliente", presupuesto.IDCliente);

                        // Parámetro de salida
                        SqlParameter outputParameter = new SqlParameter("@IDPresupuestoGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outputParameter);

                        // Ejecutar el procedimiento almacenado
                        await command.ExecuteNonQueryAsync();

                        // Obtener el ID generado
                        presupuesto.IDPresupuestoGenerado = (int)outputParameter.Value;

                        return Ok(presupuesto); // Retornar el presupuesto con el ID generado
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error interno del servidor: {ex.Message}");
                }
            }
        }



        public class PresupuestoModel
        {
            public string NombrePresupuesto { get; set; }
            public decimal MontoPresupuesto { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public string Periodo { get; set; }
            public int IDCliente { get; set; }
            public int IDPresupuestoGenerado { get; set; }
        }

































        [HttpGet("VisualizarPresupuestos")]
        public async Task<IActionResult> VisualizarPresupuestos([FromQuery] int idCliente)
        {
            string conexion = _configuration.GetConnectionString("ConeSpendEz");
            var resultado = new List<object>();

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("dbo.ObtenerPresupuestoPorCliente", connection))  // Nombre correcto del SP
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Pasar el parámetro del cliente
                        command.Parameters.AddWithValue("@ID_Usuario", idCliente);  // Actualizado al nombre del parámetro

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var presupuesto = new
                                {
                                    IDPresupuesto = reader["ID_P"],
                                    NombrePresupuesto = reader["Nombre_P"],
                                    Diferencia = reader["Diferencia"],
                                    MontoPresupuesto = reader["MontoPresupuesto"],
                                    FechaInicio = reader["FechaIni_P"],
                                    FechaFin = reader["FechaFin_P"],
                                    Periodo = reader["Periodicidad_P"]
                                    
                                };

                                resultado.Add(presupuesto);
                            }
                        }
                    }
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
            }
        }























        [HttpPost("AgregarCategoria")]
        public async Task<IActionResult> AgregarCategoria([FromBody] Categoria categoria)
        {
            // Validación de los datos recibidos
            if (categoria == null)
            {
                return BadRequest("Datos de categoría no válidos.");
            }

            try
            {
                string conexion = _configuration.GetConnectionString("ConeSpendEz");
                using (SqlConnection conn = new SqlConnection(conexion))
                {
                    await conn.OpenAsync();

                    // Crear y configurar el comando para ejecutar el procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand("dbo.AgregarCategoria", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros del procedimiento almacenado
                        cmd.Parameters.AddWithValue("@IDPresupuesto", categoria.IDPresupuesto);
                        cmd.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
                        cmd.Parameters.AddWithValue("@Descripcion", categoria.Descripcion);
                        cmd.Parameters.AddWithValue("@MontoEstimado", categoria.MontoEstimado);

                        // Ejecutar el procedimiento almacenado
                        var result = await cmd.ExecuteScalarAsync();

                        // Si se obtuvo un resultado, se devuelve un mensaje de éxito
                        if (result != null)
                        {
                            return Ok(new { message = "Categoría agregada correctamente." });
                        }
                        else
                        {
                            return BadRequest("No se pudo agregar la categoría.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de que ocurra una excepción
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    

        // Clase que representa los datos de la categoría
        public class Categoria
        {
            public string IDPresupuesto { get; set; }
            public string NombreCategoria { get; set; }
            public string Descripcion { get; set; }
            public decimal MontoEstimado { get; set; }
        }



















    //recisar
    [HttpPut("ActualizarCategoria")]
        public async Task<IActionResult> ActualizarCategoria(
            [FromQuery] int? IdCategoria,
            [FromQuery] int IdPresupuesto,
            [FromQuery] string NombreCategoria,
            [FromQuery] string DescripcionCategoria,
            [FromQuery] decimal MontoCategoria)
        {
            if (IdPresupuesto == 0)
            {
                return BadRequest("El ID del presupuesto no puede ser NULL.");
            }

            try
            {
                string conexion = _configuration.GetConnectionString("ConeSpendEz");
                // Establecer la conexión con la base de datos
                using (var connection = new SqlConnection(conexion))
                {
                    // Ejecutar el procedimiento almacenado
                    using (var command = new SqlCommand("ActualizarCategoria", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar los parámetros al comando
                        command.Parameters.AddWithValue("@IdCategoria", IdCategoria ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@IdPresupuesto", IdPresupuesto);
                        command.Parameters.AddWithValue("@NombreCategoria", NombreCategoria);
                        command.Parameters.AddWithValue("@DescripcionCategoria", DescripcionCategoria);
                        command.Parameters.AddWithValue("@MontoCategoria", MontoCategoria);

                        // Abrir la conexión
                        await connection.OpenAsync();

                        // Ejecutar el procedimiento almacenado
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Categoría actualizada correctamente.");
            }
            catch (SqlException ex)
            {
                // Si ocurre un error con la base de datos
                return StatusCode(500, $"Error al ejecutar el procedimiento: {ex.Message}");
            }
        }





        [HttpGet("ObtenerCategorias")]
        public async Task<IActionResult> ObtenerCategoriasPorPresupuesto([FromQuery] string idPresupuesto)
        {

            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            if (string.IsNullOrWhiteSpace(idPresupuesto))
            {
                return BadRequest("El ID del presupuesto es obligatorio.");
            }

            

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ObtenerCategoriasPorPresupuesto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID_Presupuesto", idPresupuesto);

                        

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var Usuario = new 
                                {
                                    ID_Categoria = reader["ID_Cat"].ToString(),
                                    Nombre_Cat = reader["Nombre_Cat"].ToString(),
                                    Descripcion = reader["Descripcion_Cat"].ToString(),
                                    MontoEstimado = Convert.ToDecimal(reader["MontoEstimado"]),
                                    dif_Gas = Convert.ToDecimal(reader["dif_Gas"])

                                };
                                resultado.Add(Usuario);
                            }
                        }
                        

                        if (resultado.Count == 0)
                        {
                            return NotFound("No se encontraron categorías para el presupuesto especificado.");
                        }

                        return Ok(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
            }
        }


        [HttpGet("ObtenerCategorias_")]
        public async Task<IActionResult> ObtenerCategorias_([FromQuery] string idPresupuesto)
        {
            if (string.IsNullOrWhiteSpace(idPresupuesto))
            {
                return BadRequest("El ID del presupuesto es obligatorio.");
            }

            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("ObtenerCategorias", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID_Presupuesto", idPresupuesto);

                        var categorias = new List<CategoriaObtener>();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                categorias.Add(new CategoriaObtener
                                {
                                    ID_Categoria = reader["ID_Cat"].ToString(),
                                    Nombre_Cat = reader["Nombre_Cat"].ToString(),
                                    Descripcion = reader["Descripcion_Cat"].ToString(),
                                    MontoEstimado = Convert.ToDecimal(reader["MontoEstimado"])
                                });
                            }
                        }

                        if (categorias.Count == 0)
                        {
                            return NotFound("No se encontraron categorías para el presupuesto especificado.");
                        }

                        return Ok(categorias);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
            }
        }











        //APIS DELETE


        //Elimina la categori dependiendo del id de categoria y el id de presupuesto
        [HttpDelete("EliminarCategoria")]
        public async Task<IActionResult> EliminarCategoria([FromQuery] int idCategoria, [FromQuery] int idPresupuesto)
        {
            if (idCategoria <= 0 || idPresupuesto <= 0)
            {
                return BadRequest("El ID de la categoría y el ID del presupuesto son obligatorios y deben ser mayores a cero.");
            }

            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("EliminarCategoria", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@idCategoria", idCategoria);
                        command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                string resultado = reader["Resultado"].ToString();

                                if (resultado == "Categoría eliminada correctamente")
                                {
                                    return Ok(new { message = resultado });
                                }
                                else
                                {
                                    return NotFound(new { message = resultado });
                                }
                            }
                            else
                            {
                                return NotFound(new { message = "No se encontraron coincidencias para la eliminación." });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la solicitud: {ex.Message}");
            }
        }





        //Eliminar el presupouesto de un usuario dependiendo de su id de usuario y su id de presupuesto
        [HttpDelete("EliminarPresupuesto")]
        public async Task<IActionResult> EliminarPresupuesto([FromQuery] int idPresupuesto, [FromQuery] int idUsuario)
        {
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("dbo.EliminarPresupuesto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        command.Parameters.AddWithValue("@ID_Presupuesto", idPresupuesto);
                        command.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        // Ejecutar el procedimiento almacenado
                        await command.ExecuteNonQueryAsync();

                        return Ok(new { mensaje = "Presupuesto eliminado exitosamente." });
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el presupuesto", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error inesperado", error = ex.Message });
            }
        }










        [HttpGet("Insertar Gastos Sin Categoría")]
        public async Task<IActionResult> ObtenerGastosSinCategoria([FromQuery] int idUsuario)
        {
            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("ObtenerGastosPorCategoria", connection))
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
                                FechaGas = reader["Fecha_Gas"],
                            };

                            resultado.Add(gastos);
                        }
                    }
                }
            }

            return Ok(resultado);
        }




        [HttpPost("Agregar Gasto Sin Categoria")]
        public async Task<IActionResult> AgregarGastoSinCategoria([FromForm] string descripcion, [FromForm] double monto, [FromForm] string fecha, [FromForm] int idUsuario)
        {
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("AgregarGasto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Parámetros del procedimiento almacenado
                        command.Parameters.AddWithValue("@Descripcion_Gas", descripcion ?? (object)DBNull.Value); // Permite nulo
                        command.Parameters.AddWithValue("@Monto_Gas", monto);
                        command.Parameters.AddWithValue("@Fecha_Gas", fecha);
                        command.Parameters.AddWithValue("@ID_U", idUsuario);

                        // Ejecuta el procedimiento almacenado
                        int filasAfectadas = await command.ExecuteNonQueryAsync();

                        if (filasAfectadas > 0)
                        {
                            return Ok(new { Mensaje = "El gasto fue agregado exitosamente." });
                        }
                        else
                        {
                            return BadRequest(new { Mensaje = "No se pudo agregar el gasto. Por favor, verifica los datos." });
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

