using System.Linq;
using AutoMapper;
using ProAgil.Domain;
using ProjAgil.WebAPI.Dtos;

namespace ProjAgil.WebAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // filtrar o relacionamento de muitos para muitos adicionando somente uma parte do relacionamento.
            //neste caso estamos adicionanto somente a lista de palestrantes no atributo do relacionamento, assim retornando somente os palestrates
            CreateMap<Evento, EventoDto>()
                .ForMember(dest => dest.Palestrantes, opt =>{
                    opt.MapFrom(src => src.PalestrantesEventos.Select(x => x.Palestrante).ToList());
                }).ReverseMap();
            
            CreateMap<Palestrante, PalestranteDto>()
                .ForMember(dest => dest.Eventos, opt =>{
                    opt.MapFrom(src => src.PalestrantesEventos.Select(x => x.Evento).ToList());
                }).ReverseMap();
            CreateMap<Lote, LoteDto>().ReverseMap();
            CreateMap<RedeSocial, RedeSocialDto>().ReverseMap();
        }
    }
}