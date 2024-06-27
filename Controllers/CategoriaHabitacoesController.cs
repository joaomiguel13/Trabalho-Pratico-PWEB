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
    public class CategoriaHabitacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriaHabitacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaHabitacaos
        public async Task<IActionResult> Index([Bind("TextoAPesquisar,Ordem")] PesquisaCategoriaViewModel pesquisaCategoria)
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }

            IQueryable<CategoriaHabitacao> task;
            if (string.IsNullOrWhiteSpace(pesquisaCategoria.TextoAPesquisar))
            {
                task = _context.CategoriasHabitacao.Where(e => e.Nome.Contains("")).OrderByDescending(e => e.Nome);
            }
            else
            {
                task = _context.CategoriasHabitacao.Where(e => e.Nome.Contains(pesquisaCategoria.TextoAPesquisar));
            }
            if (pesquisaCategoria.Ordem != null)
            {
                if (pesquisaCategoria.Ordem.Equals("nomeDesc"))
                {
                    task = task.OrderByDescending(e => e.Nome);
                }
                else if (pesquisaCategoria.Ordem.Equals("nomeAsc"))
                {
                    task = task.OrderBy(e => e.Nome);
                }
            }

            pesquisaCategoria.ListaDeCategorias = await task.ToListAsync();

            return View(pesquisaCategoria);
        }

        // GET: CategoriaHabitacao/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaHabitacao/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] CategoriaHabitacao categoriaHabitacao)
        {

            ModelState.Remove("Habitacoes");
            if (ModelState.IsValid)
            {
                _context.Add(categoriaHabitacao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaHabitacao);
        }

        // GET: categoriaHabitacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CategoriasHabitacao == null)
            {
                return NotFound();
            }

            var categoriaHabitacao = await _context.CategoriasHabitacao.FindAsync(id);
            if (categoriaHabitacao == null)
            {
                return NotFound();
            }
            return View(categoriaHabitacao);
        }

        // POST: categoriaHabitacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] CategoriaHabitacao categoriaHabitacao)
        {
            if (id != categoriaHabitacao.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Habitacoes");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaHabitacao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaHabitacaoExists(categoriaHabitacao.Id))
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
            return View(categoriaHabitacao);
        }

        // GET: categoriaHabitacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CategoriasHabitacao == null)
            {
                return NotFound();
            }

            var categoriaHabitacao = await _context.CategoriasHabitacao
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoriaHabitacao == null)
            {
                return NotFound();
            }

            if (_context.Habitacao.Where(v => v.CategoriaId == id).Any())
            {
                TempData["error"] = "A categoria " + categoriaHabitacao.Nome + " ainda tem veiculos para aluguer.";
                return RedirectToAction(nameof(Index));
            }

            return View(categoriaHabitacao);
        }

        // POST: categoriaHabitacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CategoriasHabitacao == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CategoriaVeiculo'  is null.");
            }
            var categoriaHabitacao = await _context.CategoriasHabitacao.FindAsync(id);
            if (categoriaHabitacao != null)
            {
                _context.CategoriasHabitacao.Remove(categoriaHabitacao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaHabitacaoExists(int id)
        {
            return _context.CategoriasHabitacao.Any(e => e.Id == id);
        }
    }
}