using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;
using ProjAgil.WebAPI.Dtos;

namespace ProjAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase {
        public IProAgilRepository _repo;
        public IMapper _mapper;

        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<IActionResult> Get()
        {   
            try
            {
                var eventos = await _repo.GetAllEventosAsync(true);
                var results = _mapper.Map<EventoDto[]>(eventos); // retornar os dados de acordo com a class Dto

                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }
            
        }

        [HttpGet("{EventoId}")]
        public async Task<IActionResult> Get(int EventoId)
        {   
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, true);
                var results = _mapper.Map<EventoDto>(evento); // filtrar dados usando o Dto
                return Ok(results);
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }
            
        }

        [HttpGet("getByTema/{tema}")]
        public async Task<IActionResult> Get(string tema)
        {   
            try
            {
                var eventos = await _repo.GetAllEventosAsyncByTema(tema, true);
                var results = _mapper.Map<EventoDto[]>(eventos);
                return Ok(results);
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {   
            try
            {
                var evento = _mapper.Map<Evento>(model);
                _repo.Add(evento);

                if(await _repo.SaveChangesAsync())
                {
                    return Created($"api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
                }
                
            }
            catch (System.Exception ex)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }

            return BadRequest();
            
        }

        [HttpPut("{EventoId}")]
        public async Task<IActionResult> Put(int EventoId, EventoDto model)
        {   
            try
            {
                //verificar se o evento existe
                var evento = await _repo.GetEventoAsyncById(EventoId, false);
                if(evento == null) return NotFound();

                _mapper.Map(model, evento);
                _repo.Update(evento);

                if(await _repo.SaveChangesAsync())
                {
                    return Created($"api/evento/{model.Id}", _mapper.Map<EventoDto>(evento));
                }
                
            }
            catch (System.Exception ex)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Bando de dados falhou {ex.Message}");
            }

            return BadRequest();
            
        }

        [HttpDelete("{EventoId}")]
        public async Task<IActionResult> Delete(int EventoId)
        {   
            try
            {
                //verificar se o evento existe
                var evento = await _repo.GetEventoAsyncById(EventoId, false);
                if(evento == null) return NotFound();

                _repo.Delete(evento);

                if(await _repo.SaveChangesAsync())
                {
                    return Ok();
                }
                
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }

            return BadRequest();
            
        }
    }
}