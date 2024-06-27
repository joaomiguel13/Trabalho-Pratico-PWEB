using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trabalho_Pratico.Data;
using Trabalho_Pratico.Models;
using Trabalho_Pratico.Models.ViewModels;


namespace Trabalho_Pratico.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class LocalizacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocalizacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Localizacoes
        public async Task<IActionResult> Index([Bind("TextoAPesquisar,Ordem")] PesquisaLocalizacaoViewModel pesquisaLocalizacao)
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }

            IQueryable<Localizacoes> task;
            if (string.IsNullOrWhiteSpace(pesquisaLocalizacao.TextoAPesquisar))
            {
                task = _context.Localizacoes.Where(e => e.Nome.Contains("")).OrderByDescending(e => e.Nome);
            }
            else
            {
                task = _context.Localizacoes.Where(e => e.Nome.Contains(pesquisaLocalizacao.TextoAPesquisar));
            }
            if (pesquisaLocalizacao.Ordem != null)
            {
                if (pesquisaLocalizacao.Ordem.Equals("nomeDesc"))
                {
                    task = task.OrderByDescending(e => e.Nome);
                }
                else if (pesquisaLocalizacao.Ordem.Equals("nomeAsc"))
                {
                    task = task.OrderBy(e => e.Nome);
                }
            }

            pesquisaLocalizacao.ListaDeLocalizacoes = await task.ToListAsync();

            return View(pesquisaLocalizacao);
        }

        // GET: Localizacoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Localizacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Localizacoes localizacoes)
        {
            ModelState.Remove("Habitacoes");
            if (ModelState.IsValid)
            {
                _context.Add(localizacoes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(localizacoes);
        }

        // GET: Localizacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Localizacoes == null)
            {
                return NotFound();
            }

            var localizacao = await _context.Localizacoes.FindAsync(id);
            if (localizacao == null)
            {
                return NotFound();
            }
            return View(localizacao);
        }

        // POST: Localizacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Localizacoes localizacao)
        {
            if (id != localizacao.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Habitacoes");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(localizacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocalizacaoExists(localizacao.Id))
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
            return View(localizacao);
        }

        // GET: Localizacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Localizacoes == null)
            {
                return NotFound();
            }

            var localizacao = await _context.Localizacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (localizacao == null)
            {
                return NotFound();
            }

            if (_context.Habitacao.Where(v => v.LocalizacaoId == id).Any())
            {
                TempData["error"] = "A localização " + localizacao.Nome + " ainda tem habitações para arrendar.";
                return RedirectToAction(nameof(Index));
            }

            return View(localizacao);
        }

        // POST: Localizacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Localizacoes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Localizacao'  is null.");
            }
            var localizacao = await _context.Localizacoes.FindAsync(id);
            if (localizacao != null)
            {
                _context.Localizacoes.Remove(localizacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocalizacaoExists(int id)
        {
            return _context.Localizacoes.Any(e => e.Id == id);
        }
    }
}