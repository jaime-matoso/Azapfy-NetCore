using System.Collections;

namespace WebAPI_Azapfy.Models
{
    public class Nota
    {
        public string chave { get; set; } 
        public string Numero { get; set; } 
        public string cnpj_remete { get; set; } 
        public string nome_remete { get; set; } 
        public string cnpj_transp { get; set; } 
        public string nome_transp { get; set; } 
        public Object dest { get; set; } 
        public string status { get; set; } 
        public string volumes { get; set; } 
        public decimal Valor { get; set; } 
        public string dt_emis { get; set; } 
        public string dt_entrega { get; set; }
    }
}
