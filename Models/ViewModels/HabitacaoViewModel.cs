using System.ComponentModel.DataAnnotations;

namespace Trabalho_Pratico.Models.ViewModels
{
    public class HabitacaoViewModel
    {
            public int Id { get; set; }

            [Display(Name = "Foto da Habitação")]
            public byte[]? Foto { get; set; }
            public IFormFile FotoFile { get; set; }

            [Display(Name = "Tipo", Prompt = "Introduza o tipo da habitação",
                Description = "Tipo da nova habitação a inserir")]
            public string Tipo { get; set; } //Quarto, Apartamento, estudio etc

            [Display(Name = "Descrição", Prompt = "Introduza a descrição da habitação",
                Description = "Descrição da nova habitação a inserir")]
            public string Descricao { get; set; }

            [Display(Name = "Ano de Construção", Prompt = "Introduza o ano de construção da habitação",
                Description = "Ano de construção da nova habitação a inserir")]
            public int AnoConstrucao { get; set; }

            [Display(Name = "Morada", Prompt = "Introduza a morada da habitação",
                Description = "Morada da nova habitação a inserir")]
            public string Morada { get; set; }

            [Display(Name = "Área (m²)", Prompt = "Introduza a área total da habitação",
                Description = "Área total da nova habitação a inserir")]
            public int AreaTotal { get; set; }

            [Display(Name = "Disponível", Prompt = "Disponível para arrendar",
                Description = "Nova habitação está disponível para alugar")]
            public bool Disponivel { get; set; }

            [Display(Name = "Preço por Noite", Prompt = "Preço por noite de aluguer da habitação",
                Description = "Preço por noite de aluguer da nova habitação a inserir")]
            public decimal PrecoPorNoite { get; set; }

            [Display(Name = "Categoria", Prompt = "Insira a categoria da habitação",
                Description = "Categoria à qual a habitação pertence")]
            public int CategoriaId { get; set; }
            public CategoriaHabitacao Categoria { get; set; } //Residencial, luxo, ferias 

            [Display(Name = "Localização", Prompt = "Insira a localização da habitação",
                Description = "Localização onde a habitação se encontra")]
            public int LocalizacaoId { get; set; }
            public Localizacoes Localizacao { get; set; }

            [Display(Name = "Locador", Prompt = "Insira o locador da habitação",
                Description = "locador da habitação")]
            public int LocadorId { get; set; }
            public Locador Locador { get; set; }
    }
}
