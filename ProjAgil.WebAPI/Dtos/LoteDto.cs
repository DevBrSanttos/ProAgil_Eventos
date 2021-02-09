using System.ComponentModel.DataAnnotations;

namespace ProjAgil.WebAPI.Dtos
{
    public class LoteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [Required (ErrorMessage="{0} é obrigatório")]
        public decimal Preco { get; set; }        
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        [Required  (ErrorMessage="{0} é obrigatório")]
        [Range(2, 120000)]
        public int Quantidade { get; set; }               

    }
}