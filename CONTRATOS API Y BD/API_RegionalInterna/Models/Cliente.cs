using System.ComponentModel.DataAnnotations;

namespace API_RegionalInterna.Models
{
    public class Cliente
    {
        public int Id_Cliente { get; set; }
        public string Nombre { get; set; }= string.Empty;
    }

}
