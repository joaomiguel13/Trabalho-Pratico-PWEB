using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Trabalho_Pratico.Data;
using Trabalho_Pratico.Models;

namespace Trabalho_Pratico.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public async Task<IActionResult> IndexAsync()
        {
            List<Habitacao> habitacoes = await _context.Habitacao
                .Include(h => h.Categoria)
                .Include(h => h.Locador)
                .Include(h => h.Locador.Avaliacao)
                .Include(h => h.Localizacao)
                .Where(h => h.Disponivel && h.Locador.EstadoSubscricao).OrderBy(h => h.PrecoPorNoite).ToListAsync();

            return View(habitacoes);
        } 
    }

    /*public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }*/
}
