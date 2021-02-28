using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Domain.Identity;
using ProjAgil.WebAPI.Dtos;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ProjAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        public UserController(IConfiguration config, UserManager<User> userManager,
                              SignInManager<User> signInManager, IMapper mapper){
            _mapper = mapper;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(){
            return Ok(new UserDto());
        }

        [HttpPost("Register")]
        [AllowAnonymous] //dizer que não há necessidade de verificar a authenticação para passar neste metodo ou seja qualquer pessoa pode acessá-lo
        public async Task<IActionResult> Register(UserDto userDto){
            try
            {
                var user = _mapper.Map<User>(userDto);
                var result = await _userManager.CreateAsync(user, userDto.Password);

                var userToReturn = _mapper.Map<UserDto>(user);
                if(result.Succeeded){
                    return Created("GetUser", userToReturn);
                }

                return BadRequest(result.Errors);

            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto){
            try
            {
                var user = await _userManager.FindByNameAsync(userLoginDto.UserName);
                var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);
                if(result.Succeeded){
                    var appUser = await _userManager.Users
                                            .FirstOrDefaultAsync(u => u.NormalizedUserName == userLoginDto.UserName.ToUpper());
                    var userToReturn = _mapper.Map<UserLoginDto>(appUser);
                    return Ok(value: new {
                        token = GenerateJWToken(appUser).Result,
                        user = userToReturn 
                    });
                }
                
                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
        }


        private async Task<string> GenerateJWToken(User user){
            
            //criar uma lista com id, nome e as permissões do usuario
            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            //pegar permissões do banco
            var roles = await _userManager.GetRolesAsync(user);

            foreach(var role in roles){
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            //chave de criptografia
            var key = new SymmetricSecurityKey(Encoding.ASCII
                                .GetBytes(_config.GetSection("AppSettings:Tokken").Value));
            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //criptografar
            
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}