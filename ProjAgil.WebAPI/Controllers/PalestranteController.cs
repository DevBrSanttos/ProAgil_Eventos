using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProjAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PalestranteController : ControllerBase
    {

        public IProAgilRepository _repo;
        public PalestranteController(IProAgilRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{palestranteNome}")]
        public async Task<IActionResult> Get(string palestranteNome)
        {   
            try
            {
                var results = await _repo.GetAllPalestrantesAsyncByName(palestranteNome, false);
                return Ok(results);
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de dados falhou");
            }
            
        }

        [HttpGet("{palestranteId}")]
        public async Task<IActionResult> Get(int palestranteId)
        {   
            try
            {
                var results = await _repo.GetPalestranteAsync(palestranteId, false);
                return Ok(results);
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(Palestrante palestrante)
        {   
            try
            {
                _repo.Add(palestrante);

                if(await _repo.SaveChangesAsync())
                {
                    return Created($"api/evento/{palestrante.Id}", palestrante);
                }
                
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }

            return BadRequest();
            
        }


        [HttpPut]
        public async Task<IActionResult> Put(int palestranteId, Palestrante palestrante)
        {   
            try
            {
                //verificar se o palestrante existe
                var evento = await _repo.GetPalestranteAsync(palestranteId, false);
                if(evento == null) return NotFound();

                _repo.Update(palestrante);

                if(await _repo.SaveChangesAsync())
                {
                    return Created($"api/palestrante/{palestrante.Id}", palestrante);
                }
                
            }
            catch (System.Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Bando de dados falhou");
            }

            return BadRequest();
            
        }

        [HttpDelete("{palestranteId}")]
        public async Task<IActionResult> Delete(int palestranteId)
        {   
            try
            {                
                //verificar se o palestrante existe
                var evento = await _repo.GetEventoAsyncById(palestranteId, false);
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