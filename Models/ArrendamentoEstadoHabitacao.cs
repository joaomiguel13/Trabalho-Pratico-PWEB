using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;
using Trabalho_Pratico.Models;

namespace Trabalho_Pratico.Models
{
    public abstract class ArrendamentoEstadoHabitacao
    {
        public int Id { get; set; }

        [Display(Name = "Área (m²)", Prompt = "Introduza a área total da habitação",
            Description = "Área total da habitação em metros quadrados")]
        public int AreaTotal { get; set; }

        [Display(Name = "Danos na Habitação", Prompt = "A habitação tem danos?",
            Description = "Indique se a habitação possui danos")]
        public bool DanosHabitacao { get; set; }

        [Display(Name = "Observações", Prompt = "Introduza observações sobre a habitação",
            Description = "Observações sobre a condição e características da habitação")]
        [DataType(DataType.MultilineText)]
        public string Observacoes { get; set; }


        public string? FuncionarioId { get; set; }
        public ApplicationUser? Funcionario { get; set; }

        [ForeignKey(nameof(Arrendamento))]
        public int? ArrendamentoId { get; set; }
        public Arrendamento? Arrendamento { get; set; }
    }

    [Table("ArrendamentoEstadoHabitacaoEntrada")]
    public class ArrendamentoEstadoHabitacaoEntrada : ArrendamentoEstadoHabitacao
    {
    }

    [Table("ArrendamentoEstadoHabitacaoSaida")]
    public class ArrendamentoEstadoHabitacaoSaida : ArrendamentoEstadoHabitacao
    {
      
    }

}
