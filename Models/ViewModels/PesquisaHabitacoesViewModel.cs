using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class PesquisaHabitacoesViewModel
    {
        public List<Habitacao> ListaDeHabitacoes { get; set; }

        [Display(Name = "Pesquisa de habitações:", Prompt = "introduza a habitação a pesquisar")]
        public string TextoAPesquisar { get; set; }

        [Display(Name = "Data de Entrada", Prompt = "Introduza data de entrada",
            Description = "Data de entrada na habitação")]
        [DataType(DataType.Date)]
        public DateTime DataEntrada { get; set; }

        [Display(Name = "Data de Saida", Prompt = "Introduza data de saida",
            Description = "Data de saida da habitação")]
        [DataType(DataType.Date)]
        public DateTime DataSaida { get; set; }

        public int CategoriaId { get; set; }
        public int LocadorId { get; set; }
        public string Ordem { get; set; }
    }
}
