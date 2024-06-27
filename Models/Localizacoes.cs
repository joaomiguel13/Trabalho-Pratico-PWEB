namespace Trabalho_Pratico.Models
{
    public class Localizacoes
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<Habitacao> Habitacoes { get; set; }
    }
}
