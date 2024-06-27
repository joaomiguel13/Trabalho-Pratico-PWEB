using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class PesquisaPortfolioHabitacoesViewModel
    {
        public List<Habitacao> ListaDeHabitacoes { get; set; }
        [Display(Name = "Pesquisa de habitações:", Prompt = "introduza a habitação a pesquisar")]
        public string TextoAPesquisar { get; set; }
        public string Disponivel { get; set; }
        public int CategoriaId { get; set; }
        public string Ordem { get; set; }
    }
}
