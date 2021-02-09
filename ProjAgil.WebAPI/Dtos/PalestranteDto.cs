using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace ProjAgil.WebAPI.Dtos
{
    public class PalestranteDto
    {
        public int Id { get; set; }
        [Required (ErrorMessage="{0} é obrigatório")]
        public string Nome { get; set; }
        public string MiniCurriculo { get; set; }
        public string ImagemURL { get; set; }
        [Phone (ErrorMessage="{0} inválido")]
        [Required (ErrorMessage="{0} é obrigatório")]
        public string Telefone { get; set; }
        [EmailAddress (ErrorMessage="{0} inválido")]
        [Required (ErrorMessage="{0} é obrigatório")]
        public string Email { get; set; }
        public List<RedeSocialDto> RedesSociais { get; set; }
        public List<EventoDto> Eventos { get; set; }
    }
}