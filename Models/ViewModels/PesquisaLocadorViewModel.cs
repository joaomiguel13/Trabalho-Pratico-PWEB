using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class PesquisaLocadorViewModel
    {
        public List<Locador> ListaDeLocadores { get; set; }
        [Display(Name = "Pesquisa de locadores:", Prompt = "introduza o nome a pesquisar")]
        public string TextoAPesquisar { get; set; }
        public string SubscricaoAtiva { get; set; }
        public string Ordem { get; set; }
    }
}
