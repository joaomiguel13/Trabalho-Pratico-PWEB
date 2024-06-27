using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models
{
    public class Avaliacao
    {
        public int Id { get; set; }

        public int LocadorId { get; set; }
        public Locador Locador { get; set; }

        [ForeignKey(nameof(Arrendamento))]
        public int? ArrendamentoId { get; set; }
        public Arrendamento? Arrendamento { get; set; }

        [Display(Name = "Classificação", Prompt = "Introduza a classificação",
            Description = "Classificação do arrendamento que realizou")]
        [RegularExpression("^(10|[0-9])$", ErrorMessage = "O valor inserido deve estar entre 0 e 10!")]
        public int ClassificacaoReserva { get; set; }
    }
}
