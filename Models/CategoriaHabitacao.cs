using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models
{
    public class CategoriaHabitacao
    {
        public int Id { get; set; }

        [Display(Name = "Nome", Prompt = "Introduza o nome da categoria",
            Description = "Nome da nova categoria a inserir")]
        public string Nome { get; set; }

        public ICollection<Habitacao> Habitacoes { get; set; }
    }
}
