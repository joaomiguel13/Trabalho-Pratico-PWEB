using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models
{
    public class Locador
    {
        public int Id { get; set; }

        [Display(Name = "Nome", Prompt = "Introduza o nome do locador",
            Description = "Nome do novo locador a inserir")]
        public string Nome { get; set; }

        [Display(Name = "Estado Subscrição", Prompt = "Introduza o estado da subscrição",
            Description = "Estado da subscrição do novo locador a inserir")]
        public bool EstadoSubscricao { get; set; }

        public ICollection<Avaliacao> Avaliacao { get; set; }
        public ICollection<Habitacao> Habitacoes { get; set; }
        public ICollection<ApplicationUser> Trabalhadores { get; set; }
    }
}
