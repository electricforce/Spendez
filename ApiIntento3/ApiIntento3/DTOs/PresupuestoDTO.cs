namespace ApiIntento3.DTOs
{
    public class CategoriaDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal MontoEstimado { get; set; }  // Monto estimado a gastar en la categoría
    }

   
    public class CategoriaObtener
    {
        public string ID_Categoria { get; set; }
        public string Nombre_Cat { get; set; }
        public string Descripcion { get; set; }
        public decimal MontoEstimado { get; set; }
        public decimal dif_Gas { get; set; }

    }


}
