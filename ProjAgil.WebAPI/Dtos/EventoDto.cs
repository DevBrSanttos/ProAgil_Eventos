using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjAgil.WebAPI.Dtos
{
    public class EventoDto
    {
        public int Id { get; set; }
        public string ImagemURL {get; set;}
        [Required (ErrorMessage="{0} é obrigatório")]
        public string Local {get; set; }
        public string DataEvento {get; set; }        
        public int QtdPessoas { get; set; }
        [Phone (ErrorMessage="{0} inválido")]
        [Required (ErrorMessage="{0} é obrigatório")]
        public string Telefone { get; set; }
        [EmailAddress (ErrorMessage="{0} inválido")]
        [Required (ErrorMessage="{0} é obrigatório")]
        public string Email { get; set; }
        public string Tema { get; set; }
        public List<LoteDto> Lotes { get; set; }
        public List<RedeSocialDto> RedesSociais { get; set; }
        public List<PalestranteDto> Palestrantes { get; set; }
    }
}