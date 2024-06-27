using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class PesquisaArrendamentosGestorFuncionarioViewModel
    {
        public List<Arrendamento> ListaDeArrendamentos { get; set; }
        [Display(Name = "Pesquisa de habitação/cliente:", Prompt = "introduza o texto a pesquisar")]
        public string TextoAPesquisar { get; set; }

        [Display(Name = "Data de entrada", Prompt = "Introduza data de entrada",
            Description = "Data de entrada na habitação")]
        [DataType(DataType.Date)]
        public DateTime DataEntrada { get; set; }

        [Display(Name = "Data de saida", Prompt = "Introduza data de saida",
            Description = "Data de saida da habitação")]
        [DataType(DataType.Date)]
        public DateTime DataSaida { get; set; }

        public int CategoriaId { get; set; }
        public string Ordem { get; set; }
    }
}
