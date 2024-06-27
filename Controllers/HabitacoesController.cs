using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trabalho_Pratico.Data;
using Trabalho_Pratico.Models;
using Trabalho_Pratico.Models.ViewModels;

namespace Trabalho_Pratico.Controllers
{
    public class HabitacoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HabitacoesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Habitacaos
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Index(
            [Bind("TextoAPesquisar,Ordem,CategoriaId,Disponível")] PesquisaPortfolioHabitacoesViewModel pesquisaHabitacoes, 
            string Categoria)
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }

            var user = await _userManager.GetUserAsync(User);

            IQueryable<Habitacao> task = _context.Habitacao.Include(h => h.Categoria).Include(h => h.Locador)
                .Include(h => h.Localizacao).Where(h => h.LocadorId == user.LocadorId);
            if (string.IsNullOrWhiteSpace(pesquisaHabitacoes.TextoAPesquisar))
            {
                task = task.OrderByDescending(h => h.Tipo).ThenByDescending(h => h.PrecoPorNoite).ThenByDescending(h => h.Descricao);
            }
            else
            {
                task = task
                    .Where(e => e.Tipo.Contains(pesquisaHabitacoes.TextoAPesquisar)
                    || e.PrecoPorNoite.ToString().Contains(pesquisaHabitacoes.TextoAPesquisar)
                    || e.Descricao.Contains(pesquisaHabitacoes.TextoAPesquisar));                                                            
            }
            if(Categoria != null)
            {
                pesquisaHabitacoes.CategoriaId = int.Parse(Categoria);
            }
            if(pesquisaHabitacoes.CategoriaId != 0)
            {
                task = task.Where(h => h.CategoriaId == pesquisaHabitacoes.CategoriaId);
            }
            if(pesquisaHabitacoes.Disponivel != null)
            {
                if(pesquisaHabitacoes.Disponivel.Equals("disponívelSim"))
                {
                    task = task.Where(e => e.Disponivel);
                }
                else if(pesquisaHabitacoes.Disponivel.Equals("disponívelNao"))
                {
                    task = task.Where(e => !e.Disponivel);
                }
            }
            if (pesquisaHabitacoes.Ordem != null)
            {
                if (pesquisaHabitacoes.Ordem.Equals("desc"))
                {
                    task = task.OrderByDescending(h => h.Tipo).ThenByDescending(h => h.PrecoPorNoite).ThenByDescending(h => h.Descricao);
                }
                else if (pesquisaHabitacoes.Ordem.Equals("asc"))
                {
                    task = task.OrderBy(h => h.Tipo).ThenBy(h => h.PrecoPorNoite).ThenBy(h => h.Descricao);
                }
            }

            ViewBag.CategoriaId = _context.CategoriasHabitacao.ToList();
            pesquisaHabitacoes.ListaDeHabitacoes = await task.ToListAsync();
            return View(pesquisaHabitacoes);

        }

        // GET: Habitacaos/Details/5
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao
                .Include(h => h.Categoria)
                .Include(h => h.Locador)
                .Include(h => h.Localizacao)
                .Include(h => h.Locador.Avaliacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            return View(habitacao);
        }

        //GET: Ativar
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Ativar(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            habitacao.Disponivel = true;
            _context.Update(habitacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET: Desativar
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Desativar(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }

            habitacao.Disponivel = false;
            _context.Update(habitacao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Habitacaos/Create
        [Authorize(Roles = "Funcionario, Gestor")]
        public IActionResult Create()
        {
            if(TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }

            ViewData["CategoriaId"] = new SelectList(_context.CategoriasHabitacao, "Id", "Nome");
            ViewBag.LocalizacaoId = new SelectList(_context.Localizacoes, "Id", "Nome");
            return View(new HabitacaoViewModel());
        }

        // POST: Habitacaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Funcionario, Gestor")]
        public async Task<IActionResult> Create([Bind("Id,Foto,FotoFile,Tipo,Descricao,AnoConstrucao,Morada,AreaTotal,Disponivel,PrecoPorNoite,CategoriaId,LocalizacaoId,LocadorId")] HabitacaoViewModel habitacao)
        {
            ViewBag.CategoriaId = new SelectList(_context.CategoriasHabitacao, "Id", "Nome", habitacao.CategoriaId);
            ViewBag.LocalizacaoId = new SelectList(_context.Localizacoes, "Id", "Nome", habitacao.LocalizacaoId);

            habitacao.LocadorId = (int)(await _userManager.GetUserAsync(User)).LocadorId;

            ModelState.Remove("Categoria");
            ModelState.Remove("Localizacao");
            ModelState.Remove("Locador");
            if (ModelState.IsValid)
            {
                if(habitacao.FotoFile != null)
                {
                    if(habitacao.FotoFile.Length > (1024*1024))
                    {
                        TempData["error"] = "Ficheiro demasiado grande";
                        ViewBag.error = "Error: Ficheiro demasiado grande";
                        return View(habitacao);
                    }
                    //verificar a extensão do ficheiro
                    if(!isValidFileType(habitacao.FotoFile.FileName))
                    {
                        TempData["error"] = "Error: Ficheiro não suportado";
                        ViewBag.error = "Error: Ficheiro não suportado";
                        return View(habitacao);
                    }
                    using (var dataStream = new MemoryStream())
                    {
                        await habitacao.FotoFile.CopyToAsync(dataStream);
                        habitacao.Foto = dataStream.ToArray();
                    }
                }
                
                Habitacao h = new Habitacao()
                {
                    Tipo = habitacao.Tipo,
                    Descricao = habitacao.Descricao,
                    AnoConstrucao = habitacao.AnoConstrucao,
                    Morada = habitacao.Morada,
                    AreaTotal = habitacao.AreaTotal,
                    Disponivel = habitacao.Disponivel,
                    PrecoPorNoite = habitacao.PrecoPorNoite,
                    CategoriaId = habitacao.CategoriaId,
                    LocalizacaoId = habitacao.LocalizacaoId,
                    LocadorId = habitacao.LocadorId,
                    Foto = habitacao.Foto
                };

                _context.Add(h);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(habitacao);
        }

        //Verificar a extensão do ficheiro
        public bool isValidFileType(string fileName)
        {
            List<string> fileExtensions = new List<string>() { "jpg", "png", "jpeg" };
            List<string> filenameSeparated = fileName.Split(".").Reverse().ToList<string>();

            foreach(var extension in fileExtensions)
            {
                if (extension.Equals(filenameSeparated[0]))
                {
                    return true;
                }
            }
            return false;
        }

        // GET: Habitacaos/Edit/5
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao.FindAsync(id);
            if (habitacao == null)
            {
                return NotFound();
            }

            HabitacaoViewModel h = new HabitacaoViewModel()
            {
                Tipo = habitacao.Tipo,
                Descricao = habitacao.Descricao,
                AnoConstrucao = habitacao.AnoConstrucao,
                Morada = habitacao.Morada,
                AreaTotal = habitacao.AreaTotal,
                Disponivel = habitacao.Disponivel,
                PrecoPorNoite = habitacao.PrecoPorNoite,
                CategoriaId = habitacao.CategoriaId,
                LocalizacaoId = habitacao.LocalizacaoId,
                LocadorId = habitacao.LocadorId,
                Foto = habitacao.Foto
            };

            ViewData["CategoriaId"] = new SelectList(_context.CategoriasHabitacao, "Id", "Nome", habitacao.CategoriaId);
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes, "Id", "Nome", habitacao.LocalizacaoId);
            return View(h);
        }

        // POST: Habitacaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Funcionario, Gestor")]
        public async Task<IActionResult> Edit(
            int id, 
            [Bind("Id,Foto,Tipo,Descricao,AnoConstrucao,Morada,AreaTotal,Disponivel,PrecoPorNoite,CategoriaId,LocalizacaoId,LocadorId")] HabitacaoViewModel habitacao)
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriasHabitacao, "Id", "Nome", habitacao.CategoriaId);
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes, "Id", "Nome", habitacao.LocalizacaoId);

            var habitacaoDB = _context.Habitacao.Where(h => h.Id == id).First();
            habitacao.Foto = habitacaoDB.Foto;
            _context.Entry(habitacaoDB).State = EntityState.Detached;

            if (id != habitacao.Id)
            {
                return NotFound();
            }
            habitacao.LocadorId = (int)(await _userManager.GetUserAsync(User)).LocadorId;

            ModelState.Remove("FotoFile");
            ModelState.Remove("Categoria");
            ModelState.Remove("Localizacao");
            ModelState.Remove("Locador");
            if (ModelState.IsValid)
            {
                try
                {
                    if(habitacao.FotoFile != null)
                    {
                        if(habitacao.FotoFile.Length > (200*1024))
                        {
                            ViewData["error"] = "Error: Ficheiro demasiado grande";
                            return View(habitacao);
                    }
                    if(!isValidFileType(habitacao.FotoFile.FileName))
                        {
                            ViewData["error"] = "Error: Ficheiro não suportado";
                        return View(habitacao);
                    }
                    using (var dataStream = new MemoryStream())
                     {
                        await habitacao.FotoFile.CopyToAsync(dataStream);
                        habitacao.Foto = dataStream.ToArray();
                    }
                 }

                    Habitacao h = new Habitacao()
                    {
                        Id = habitacao.Id,
                        Tipo = habitacao.Tipo,
                        Descricao = habitacao.Descricao,
                        AnoConstrucao = habitacao.AnoConstrucao,
                        Morada = habitacao.Morada,
                        AreaTotal = habitacao.AreaTotal,
                        Disponivel = habitacao.Disponivel,
                        PrecoPorNoite = habitacao.PrecoPorNoite,
                        CategoriaId = habitacao.CategoriaId,
                        LocalizacaoId = habitacao.LocalizacaoId,
                        LocadorId = habitacao.LocadorId,
                        Foto = habitacao.Foto
                     };

                    _context.Update(h);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Habitacao.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(habitacao);
        }

        // GET: Habitacaos/Delete/5
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            var habitacao = await _context.Habitacao
                .Include(h => h.Categoria)
                .Include(h => h.Locador)
                .Include(h => h.Localizacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (habitacao == null)
            {
                return NotFound();
            }
            if(_context.Arrendamento.Where(a => a.HabitacaoId == id).Any())
            {
                TempData["error"] = "Não é possível eliminar a habitação porque existem arrendamentos associados";
                return RedirectToAction(nameof(Index));
            }

            return View(habitacao);
        }

        // POST: Habitacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario, Gestor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Habitacao == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Habitacao'  is null.");
            }
            var habitacao = await _context.Habitacao.FindAsync(id);
            if (habitacao != null)
            {
                _context.Habitacao.Remove(habitacao);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(
            [Bind("TextoAPesquisar,Ordem,CategoriaId,LocadorId,DataEntrada,DataSaida")] PesquisaHabitacoesViewModel pesquisaHabitacoes,
            string Categoria, string Locador)
        {

            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }
            if(pesquisaHabitacoes.DataEntrada == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaHabitacoes.DataEntrada = DateTime.Now;
            }
            if (pesquisaHabitacoes.DataSaida == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaHabitacoes.DataSaida = DateTime.Now;
            }

            IQueryable<Habitacao> task = _context.Habitacao
                .Include(h => h.Categoria)
                .Include(h => h.Locador)
                .Include(h => h.Locador.Avaliacao)
                .Include(h => h.Localizacao)
                .Where(h => h.Disponivel && h.Locador.EstadoSubscricao);

            if (string.IsNullOrEmpty(pesquisaHabitacoes.TextoAPesquisar))
            {
                task = task.OrderBy(h => h.PrecoPorNoite);
            }
            else
            {
                task = task
                    .Where(e => (
                        e.Tipo.Contains(pesquisaHabitacoes.TextoAPesquisar)
                        || e.Descricao.Contains(pesquisaHabitacoes.TextoAPesquisar)
                        || e.Localizacao.Nome.Contains(pesquisaHabitacoes.TextoAPesquisar)
                    ));

                if(pesquisaHabitacoes.CategoriaId != 0)
                {
                    task = task.Where(e => e.CategoriaId == pesquisaHabitacoes.CategoriaId);
                }
            }

            if(Categoria != null)
            {
                pesquisaHabitacoes.CategoriaId = int.Parse(Categoria);
            }
            if(pesquisaHabitacoes.CategoriaId != 0)
            {
                task = task.Where(r => r.CategoriaId == pesquisaHabitacoes.CategoriaId);
            }

            if(Locador != null)
            {
                pesquisaHabitacoes.LocadorId = int.Parse(Locador);
            }
            if(pesquisaHabitacoes.LocadorId != 0)
            {
                task = task.Where(r => r.LocadorId == pesquisaHabitacoes.LocadorId);
            }

            if(pesquisaHabitacoes.Ordem != null)
            {
                if(pesquisaHabitacoes.Ordem.Equals("precoDesc"))
                {
                    task = task.OrderByDescending(h => h.PrecoPorNoite);
                }
                else if(pesquisaHabitacoes.Ordem.Equals("precoAsc"))
                {
                    task = task.OrderBy(h => h.PrecoPorNoite);
                }
                else if(pesquisaHabitacoes.Ordem.Equals("classDesc"))
                {
                    task = task.OrderByDescending(e => e.Locador.Avaliacao.Sum(c => c.ClassificacaoReserva) / (e.Locador.Avaliacao.Count == 0 ? 1 : e.Locador.Avaliacao.Count)); ;
                }
            }

            ViewBag.CategoriaId = _context.CategoriasHabitacao.ToList();
            ViewBag.LocadorId = _context.Locador.ToList();

            pesquisaHabitacoes.ListaDeHabitacoes = await task.ToListAsync();

            foreach(var habitacao in pesquisaHabitacoes.ListaDeHabitacoes.ToList())
            {
                var res = await _context.Arrendamento.Include(r => r.Habitacao).Where(r => r.Habitacao.Id == habitacao.Id
                &&((DateTime.Compare(r.DataEntrada, pesquisaHabitacoes.DataEntrada) >= 0 
                && DateTime.Compare(r.DataEntrada, pesquisaHabitacoes.DataSaida) <= 0
                || DateTime.Compare(r.DataSaida, pesquisaHabitacoes.DataEntrada) >= 0
                && DateTime.Compare(r.DataSaida, pesquisaHabitacoes.DataSaida) <= 0))
                ).FirstOrDefaultAsync();

                if(res != null)
                {
                    pesquisaHabitacoes.ListaDeHabitacoes.Remove(habitacao);
                }
            }

            if(pesquisaHabitacoes.DataSaida < pesquisaHabitacoes.DataEntrada)
            {
                TempData["error"] = "Data de saída não pode ser inferior à data de entrada";
                return RedirectToAction(nameof(Index));
            }
            return View(pesquisaHabitacoes);

        }
    }
}
