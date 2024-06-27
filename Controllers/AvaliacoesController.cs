using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trabalho_Pratico.Data;
using Trabalho_Pratico.Models;

namespace Trabalho_Pratico.Controllers
{
    public class AvaliacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvaliacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Avaliacaoos/Create
        public IActionResult Create(int? idArrendamento)
        {

            Avaliacao classi = new Avaliacao()
            {
                ArrendamentoId = idArrendamento,
                Arrendamento = _context.Arrendamento
                    .Include(r => r.Habitacao)
                    .Include(r => r.Habitacao.Locador)
                    .Include(r => r.Cliente)
                    .Include(r => r.ArrendamentoEstadoHabitacaoEntrada)
                    .Include(r => r.ArrendamentoEstadoHabitacaoSaida)
                    .Where(r => r.Id == idArrendamento).First()
            };

            classi.Locador = _context.Habitacao.Include(v => v.Locador).Where(v => v.LocadorId == classi.Arrendamento.Habitacao.LocadorId).First().Locador;
            classi.LocadorId = classi.Locador.Id;

            return View(classi);
        }

        // POST: Avaliacaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LocadorId,ArrendamentoId,ClassificacaoReserva")] Avaliacao avaliacao)
        {
            ModelState.Remove("Arrendamento");
            ModelState.Remove("Locador");
            if (ModelState.IsValid)
            {
                _context.Add(avaliacao);
                await _context.SaveChangesAsync();
                return RedirectToAction("Historico", "Arrendamentos", null);
            }

            return View(avaliacao);
        }

        // GET: Avaliacaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Avaliacao == null)
            {
                return NotFound();
            }

            var classificacao = await _context.Avaliacao.FindAsync(id);
            if (classificacao == null)
            {
                return NotFound();
            }
            ViewData["LocadorId"] = new SelectList(_context.Locador, "Id", "Id", classificacao.LocadorId);
            ViewData["ArrendamentoId"] = new SelectList(_context.Arrendamento, "Id", "Id", classificacao.ArrendamentoId);
            return View(classificacao);
        }
    }
}