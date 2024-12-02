using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiIntento3.Controllers
{
    public class graficaController : Controller
    {
        private readonly IConfiguration _configuration;
        public graficaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("GraficaPresupuestoCategoria")]
        public async Task<IActionResult> GraficaPresupuestoCategoria([FromQuery] int idUsuario, [FromQuery] int idPresupuesto)
        {
            var resultado = new List<object>();
            string conexion = _configuration.GetConnectionString("ConeSpendEz");

            using (SqlConnection connection = new SqlConnection(conexion))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("GraficaPresupuestoCategoria", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ID_U", (object)idUsuario);
                    command.Parameters.AddWithValue("@ID_P", (object)idPresupuesto);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var gastos = new
                            {
                                Monto_P = reader["Monto_P"],
                                Diferencia = reader["Diferencia"],
                                
                            };

                            resultado.Add(gastos);
                        }
                    }
                }
            }

            return Ok(resultado);
        }







    }
}
