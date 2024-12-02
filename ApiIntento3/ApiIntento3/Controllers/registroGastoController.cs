
using ApiIntento3.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ApiIntento3.Controllers
{
    public class registroGastoController
    {
        private readonly IConfiguration _configuration;
        public registroGastoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class Categoria
        {
            public int? IdCategoria { get; set; } // ID_Categoria puede ser null
            public int IdPresupuesto { get; set; }
            public string Nombre_Cat { get; set; }
            public string Descripcion { get; set; }
            public decimal MontoCategoria { get; set; }
        }

        
    }
}

