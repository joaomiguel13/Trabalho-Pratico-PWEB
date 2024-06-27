using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class PesquisaLocalizacaoViewModel
    {
        public List<Localizacoes> ListaDeLocalizacoes { get; set; }
        [Display(Name = "Pesquisa de Localizações:", Prompt = "introduza o nome a pesquisar")]
        public string TextoAPesquisar { get; set; }
        public string Ordem { get; set; }
    }
}
