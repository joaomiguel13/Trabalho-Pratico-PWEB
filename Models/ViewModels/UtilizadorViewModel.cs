using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class UtilizadorViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Username", Prompt = "Introduza o username",
            Description = "Username do utilizador")]
        public string UserName { get; set; }

        [Display(Name = "Primeiro Nome", Prompt = "Introduza o primeiro nome",
            Description = "Primeiro nome do utilizador")]
        public string PrimeiroNome { get; set; }

        [Display(Name = "Último Nome", Prompt = "Introduza o último nome",
            Description = "Último nome do utilizador")]
        public string UltimoNome { get; set; }

        [Display(Name = "Conta de utilizador ativa", Description = "Se a conta de utilizador está ativa")]
        public bool Ativo { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
