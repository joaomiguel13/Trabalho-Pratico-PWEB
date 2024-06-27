using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class PesquisaCategoriaViewModel
    {
        public List<CategoriaHabitacao> ListaDeCategorias { get; set; }
        [Display(Name = "Pesquisa de categorias:", Prompt = "introduza o nome a pesquisar")]
        public string TextoAPesquisar { get; set; }
        public string Ordem { get; set; }
    }
}
