using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Trabalho_Pratico.Data;
using Trabalho_Pratico.Models;
using Trabalho_Pratico.Models.ViewModels;

namespace Trabalho_Pratico.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class LocadoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocadoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Locadores
        public async Task<IActionResult> Index(
            [Bind("TextoAPesquisar,SubscricaoAtiva,Ordem")] PesquisaLocadorViewModel pesquisaLocador)
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }

            IQueryable<Locador> task = _context.Locador.Include(e => e.Avaliacao);
            if (!string.IsNullOrWhiteSpace(pesquisaLocador.TextoAPesquisar))
            {
                task = task.Where(e => e.Nome.Contains(pesquisaLocador.TextoAPesquisar));
            }

            if (pesquisaLocador.SubscricaoAtiva != null)
            {
                if (pesquisaLocador.SubscricaoAtiva.Equals("subscricaoTrue"))
                {
                    task = task.Where(e => e.EstadoSubscricao);
                }
                else if (pesquisaLocador.SubscricaoAtiva.Equals("subscricaoFalse"))
                {
                    task = task.Where(e => !e.EstadoSubscricao);
                }
            }

            if (pesquisaLocador.Ordem != null)
            {
                if (pesquisaLocador.Ordem.Equals("nomeDesc"))
                {
                    task = task.OrderByDescending(e => e.Nome);
                }
                else if (pesquisaLocador.Ordem.Equals("nomeAsc"))
                {
                    task = task.OrderBy(e => e.Nome);
                }
                else if (pesquisaLocador.Ordem.Equals("classDesc"))
                {
                    task = task.OrderByDescending(e => e.Avaliacao.Sum(c => c.ClassificacaoReserva) / (e.Avaliacao.Count == 0 ? 1 : e.Avaliacao.Count));
                }
                else if (pesquisaLocador.Ordem.Equals("classAsc"))
                {
                    task = task.OrderBy(e => e.Avaliacao.Sum(c => c.ClassificacaoReserva) / (e.Avaliacao.Count == 0 ? 1 : e.Avaliacao.Count));
                }
            }
            else
            {
                task = task.OrderByDescending(e => e.Nome);
            }

            pesquisaLocador.ListaDeLocadores = await task.ToListAsync();

            return View(pesquisaLocador);
        }

        // GET: Locadores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,EstadoSubscricao")] Locador locador)
        {
            ModelState.Remove("Avaliacao");
            ModelState.Remove("Habitacoes");
            ModelState.Remove("Trabalhadores");
            if (ModelState.IsValid)
            {
                _context.Add(locador);
                await _context.SaveChangesAsync();

                var defaultUser = new ApplicationUser
                {
                    UserName = "gestor@" + String.Concat(locador.Nome.ToLower().Where(c => !Char.IsWhiteSpace(c))) + ".com",
                    Email = "gestor@" + String.Concat(locador.Nome.ToLower().Where(c => !Char.IsWhiteSpace(c))) + ".com",
                    PrimeiroNome = "Gestor",
                    UltimoNome = locador.Nome,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    LocadorId = locador.Id,
                    ContaAtiva = true
                };

                var user = await _userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await _userManager.CreateAsync(defaultUser, "1qazZAQ!");
                    await _userManager.AddToRoleAsync(defaultUser,
                    Roles.Gestor.ToString());
                }

                return RedirectToAction(nameof(Index));
            }
            return View(locador);
        }

        // GET: Ativar
        public async Task<IActionResult> Ativar(int? id)
        {
            if (id == null || _context.Locador == null)
            {
                return NotFound();
            }

            var empresa = await _context.Locador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return NotFound();
            }

            empresa.EstadoSubscricao = true;
            _context.Update(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Desativar
        public async Task<IActionResult> Desativar(int? id)
        {
            if (id == null || _context.Locador == null)
            {
                return NotFound();
            }

            var locador = await _context.Locador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (locador == null)
            {
                return NotFound();
            }

            locador.EstadoSubscricao = false;
            _context.Update(locador);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Locadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Locador == null)
            {
                return NotFound();
            }

            var locador = await _context.Locador.FindAsync(id);
            if (locador == null)
            {
                return NotFound();
            }
            return View(locador);
        }

        // POST: Locadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,EstadoSubscricao")] Locador locador)
        {
            if (id != locador.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Avaliacao");
            ModelState.Remove("Habitacoes");
            ModelState.Remove("Trabalhadores");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocadorExists(locador.Id))
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
            return View(locador);
        }

        // GET: Locadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Locador == null)
            {
                return NotFound();
            }

            var locador = await _context.Locador
                .Include(e => e.Avaliacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (locador == null)
            {
                return NotFound();
            }

            if (_context.Habitacao.Where(v => v.LocadorId == id).Any())
            {
                TempData["error"] = "O locador " + locador.Nome + " ainda tem habitações para arrendar.";
                return RedirectToAction(nameof(Index));
            }

            return View(locador);
        }

        // POST: Locadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Locador == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Locador'  is null.");
            }

            var locador = await _context.Locador.FindAsync(id);
            if (locador != null)
            {
                var users = await _userManager.Users.Where(u => u.LocadorId == id).ToListAsync();

                foreach (var user in users)
                {
                    await _userManager.DeleteAsync(user);
                }
                _context.Locador.Remove(locador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocadorExists(int id)
        {
            return _context.Locador.Any(e => e.Id == id);
        }
    }
}