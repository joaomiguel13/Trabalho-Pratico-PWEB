using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models
{
    public class Arrendamento
    {
        public int Id { get; set; }

        [Display(Name = "Data de Entrada", Prompt = "Introduza data de entrada",
            Description = "Data de entrada na habitação")]
        [DataType(DataType.Date)]
        public DateTime DataEntrada { get; set; }

        [Display(Name = "Data de Saida", Prompt = "Introduza data de saida",
            Description = "Data de saida da habitação")]
        [DataType(DataType.Date)]
        public DateTime DataSaida { get; set; }

        [Display(Name = "Arrendamento Confirmada", Description = "Se o arrendamento já foi confirmado por um trabalhador")]
        public bool Confirmado { get; set; }

        public int HabitacaoId { get; set; }
        public Habitacao Habitacao { get; set; }

        public string ClienteId { get; set; }
        public ApplicationUser Cliente { get; set; }

        public int? ReservaEstadoHabitacaoEntradaId { get; set; }
        public ArrendamentoEstadoHabitacaoEntrada? ArrendamentoEstadoHabitacaoEntrada { get; set; }

        public int? ArrendamentoEstadoHabitacaoSaidaId { get; set; }
        public ArrendamentoEstadoHabitacaoSaida? ArrendamentoEstadoHabitacaoSaida { get; set; }
    }
}
