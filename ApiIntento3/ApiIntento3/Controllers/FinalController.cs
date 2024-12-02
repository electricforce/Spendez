using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiIntento3.Controllers
{
    public class FinalController : Controller
    {
        private readonly IConfiguration _configuration;
        public FinalController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        [Route("api/[controller]")]
        [ApiController]
        public class PresupuestoController : ControllerBase
        {
            private readonly string _connectionString = "your_connection_string_here"; // Configura tu cadena de conexión a la base de datos

            [HttpPost("IngresarPresupuesto")]
            public async Task<IActionResult> IngresarPresupuesto([FromBody] PresupuestoModel presupuesto)
            {
                if (presupuesto == null)
                {
                    return BadRequest("Los datos del presupuesto son inválidos.");
                }

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    try
                    {
                        await connection.OpenAsync();

                        using (SqlCommand command = new SqlCommand("dbo.IngresarDatosPresupuesto", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Agregar parámetros
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

                            return Ok(presupuesto);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error interno del servidor: {ex.Message}");
                    }
                }
            }
        }
    }
}
