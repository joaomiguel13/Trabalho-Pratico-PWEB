using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trabalho_Pratico.Models;
using System.Reflection.Metadata;

namespace Trabalho_Pratico.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Locador> Locador { get; set; }
        public DbSet<Habitacao> Habitacao { get; set; }
        public DbSet<Localizacoes> Localizacoes { get; set; }
        public DbSet <CategoriaHabitacao> CategoriasHabitacao { get; set; }
        public DbSet<Arrendamento> Arrendamento { get; set; }
        public DbSet<ArrendamentoEstadoHabitacaoEntrada> EstadoHabitacaoEntradas { get; set; }
        public DbSet<ArrendamentoEstadoHabitacaoSaida> EstadoHabitacaoSaidas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Avaliacao>? Avaliacao { get; set; }
    }
}