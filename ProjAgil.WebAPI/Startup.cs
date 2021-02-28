using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Domain.Identity;
using ProAgil.Repository;

namespace ProjAgil.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //conexao com banco de dados
            services.AddDbContext<ProAgilContext>(
                x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            
            //configurações da authenticação
            IdentityBuilder builder = services.AddIdentityCore<User>(options =>
                {   //Remover validações(obrigações) padrôes quando criar um usuario
                    options.Password.RequireDigit = false; //caracteres especiais                    
                    options.Password.RequireLowercase = false; //letras minusculas
                    options.Password.RequireUppercase = false; //letras maiusculas
                    options.Password.RequireNonAlphanumeric = false; //letras e numeros
                    options.Password.RequiredLength = 4; //definir tamanho mínimo
                }
            );

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            //dizer para o EFS que estou trabalhando com o ProAgilContext
            //e que vai trabalhar com os outros detalhes nas linhas de baixo
            builder.AddEntityFrameworkStores<ProAgilContext>();
            builder.AddRoleValidator<RoleValidator<Role>>(); //injetar a class que será usada como roleValidator
            builder.AddRoleManager<RoleManager<Role>>(); // injetar qual class será o meu gerenciador dos papeis
            builder.AddSignInManager<SignInManager<User>>();

            //configuração JWT(authenticação de usuario)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true, //validar pela chave
                            
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                                .GetBytes(Configuration.GetSection("AppSettings:Tokken").Value)), //chave de criptografia
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    }
                );

            services.AddMvc(options =>
                {
                    //criar uma politica, sempre que chamar uma rota, está terá que respeitar a configuração
                    //nesta politica está verificando se o usuario está authenticado
                    var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
             .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = 
                                    Newtonsoft.Json.ReferenceLoopHandling.Ignore); //ignorar qualquer problema de loop infinito da relação dos meus user e roles
            
            //injetar a class ProAgilRepository. obs:quando chamar algum metodo da interface este será substituido pelo metodo da class
            services.AddScoped<IProAgilRepository, ProAgilRepository>();
            
            //dizer para a aplicação que está trabalhando com autoMapper             
            services.AddAutoMapper();

            //permitir angular de puxar os dados (conexao cruzada)
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
           //app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //configurar pasta onde ficaram salvas as imagens upadas
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions(){
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
                RequestPath = new PathString("/Resources")
            });
            app.UseMvc();
        }
    }
}
