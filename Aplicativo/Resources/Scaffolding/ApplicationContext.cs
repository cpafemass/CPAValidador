using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicativo.Resources.Scaffolding
{
    /// <summary>
    ///  Essa classe é uma interface do banco, para que o aplicativo consiga fazer chamadas diretas sem precisar de uma API,
    ///  ou seja, o aplicativo tem acesso direto ao banco de dados, e pode fazer consultas e alterações diretamente nele,
    ///  sem precisar de uma API intermediária.
    /// </summary>
    public partial class ApplicationContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationContext(DbContextOptions<ApplicationContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration["Configs:DatabaseConfig:ConnectionString"]);  
            base.OnConfiguring(optionsBuilder);
        }
        //Criar os dbsets para as respostas do formulario, verificar com joshua a estrutura dos formularios para criar as classes and therefore os dbsets
    }
}
