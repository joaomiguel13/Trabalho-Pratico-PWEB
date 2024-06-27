using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
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
    public class ArrendamentosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ArrendamentosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Arrendamentos
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Index([Bind("TextoAPesquisar,Ordem,CategoriaId,DataEntrada,DataSaida")] PesquisaArrendamentosGestorFuncionarioViewModel pesquisaArrendamento,
            string Categoria)
        {
            var user = await _userManager.GetUserAsync(User);

            IQueryable<Arrendamento> task = _context.Arrendamento
                .Include(r => r.Cliente)
                .Include(r => r.Habitacao)
                .Include(r => r.Habitacao.Categoria)
                .Include(r => r.ArrendamentoEstadoHabitacaoEntrada)
                .Include(r => r.ArrendamentoEstadoHabitacaoSaida)
                .Include(r => r.Habitacao.Locador)
                .Include(r => r.Habitacao.Locador.Avaliacao)
                .Where(r => r.Habitacao.LocadorId == user.LocadorId);

            if (pesquisaArrendamento.DataEntrada == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaArrendamento.DataEntrada = DateTime.MinValue;
            }
            if (pesquisaArrendamento.DataSaida == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaArrendamento.DataSaida = DateTime.MaxValue;
            }

            if (string.IsNullOrWhiteSpace(pesquisaArrendamento.TextoAPesquisar))
            {
                task = task.OrderByDescending(r => r.Cliente.UserName).ThenByDescending(r => r.Habitacao.Tipo).ThenByDescending(r => r.Habitacao.Categoria.Nome);
            }
            else
            {
                task = task
                    .Where(r =>
                        r.Habitacao.Tipo.Contains(pesquisaArrendamento.TextoAPesquisar)
                        || r.Habitacao.Descricao.Contains(pesquisaArrendamento.TextoAPesquisar)
                        || r.Habitacao.Morada.Contains(pesquisaArrendamento.TextoAPesquisar)
                        || r.Cliente.UserName.Contains(pesquisaArrendamento.TextoAPesquisar)
                    );
            }

            task = task.Where(r => DateTime.Compare(r.DataEntrada, pesquisaArrendamento.DataEntrada) > 0
                    && DateTime.Compare(r.DataSaida, pesquisaArrendamento.DataSaida) < 0);

            if (Categoria != null)
            {
                pesquisaArrendamento.CategoriaId = int.Parse(Categoria);
            }
            if (pesquisaArrendamento.CategoriaId != 0)
            {
                task = task.Where(r => r.Habitacao.CategoriaId == pesquisaArrendamento.CategoriaId);
            }

            if (pesquisaArrendamento.Ordem != null)
            {
                if (pesquisaArrendamento.Ordem.Equals("desc"))
                {
                    task = task.OrderByDescending(r => r.Cliente.UserName).ThenByDescending(r => r.Habitacao.Tipo).ThenByDescending(r => r.Habitacao.Categoria.Nome);
                }
                else if (pesquisaArrendamento.Ordem.Equals("asc"))
                {
                    task = task.OrderBy(r => r.Cliente.UserName).ThenBy(r => r.Habitacao.Tipo).ThenBy(r => r.Habitacao.Categoria.Nome);
                }
            }

            ViewBag.CategoriaId = _context.CategoriasHabitacao.ToList();

            pesquisaArrendamento.ListaDeArrendamentos = await task.ToListAsync();

            if (pesquisaArrendamento.DataSaida < pesquisaArrendamento.DataEntrada)
            {
                ViewBag.error = "Datas de entrada e saida incorretas";
            }

            return View(pesquisaArrendamento);
        }

        // GET: Confirmar
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Confirmar(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .FirstOrDefaultAsync(r => r.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            arrendamento.Confirmado = true;
            _context.Update(arrendamento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: ArrendamentoEstadoHabitacao/Entrada
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> EntradaAsync(int idArrendamento)
        {
            if (idArrendamento == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento.Where(r => r.Id == idArrendamento).Include(r => r.Habitacao).Include(r => r.Cliente).FirstOrDefaultAsync();
            if (arrendamento == null)
            {
                return NotFound();
            }

            ArrendamentoEstadoHabitacaoEntrada arrendamentoEntrada = new ArrendamentoEstadoHabitacaoEntrada();

            arrendamentoEntrada.ArrendamentoId = idArrendamento;
            arrendamentoEntrada.Arrendamento = arrendamento;

            return View(arrendamentoEntrada);
        }

        // POST: ArrendamentoEstadoHabitacao/Entrada
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Entrada(
            [Bind("Id,DanosHabitacao,Observacoes,ArrendamentoId,FuncionarioId")] ArrendamentoEstadoHabitacaoEntrada arrendamentoEstadoHabitacaoEntrada
            )
        {
            arrendamentoEstadoHabitacaoEntrada.FuncionarioId = (await _userManager.GetUserAsync(User)).Id;
            arrendamentoEstadoHabitacaoEntrada.Arrendamento = await _context.Arrendamento
                .Where(r => r.Id == arrendamentoEstadoHabitacaoEntrada.ArrendamentoId)
                .Include(r => r.Habitacao)
                .Include(r => r.Cliente)
                .Include(r => r.ArrendamentoEstadoHabitacaoEntrada)
                .Include(r => r.ArrendamentoEstadoHabitacaoSaida)
                .Include(r => r.Habitacao.Locador)
                .Include(r => r.Habitacao.Locador.Avaliacao)
                .FirstAsync();

            ModelState.Remove("Funcionario");
            ModelState.Remove("Arrendamento");

            if (ModelState.IsValid)
            {
                arrendamentoEstadoHabitacaoEntrada.Arrendamento.ReservaEstadoHabitacaoEntradaId = arrendamentoEstadoHabitacaoEntrada.Id;
                _context.Add(arrendamentoEstadoHabitacaoEntrada);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(arrendamentoEstadoHabitacaoEntrada);
        }

        // GET: arrendamentoEstadoHabitacao/Saida
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Saida(int idArrendamento)
        {
            if (idArrendamento == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento.Where(r => r.Id == idArrendamento).Include(r => r.Habitacao).Include(r => r.Cliente).FirstOrDefaultAsync();
            if (arrendamento == null)
            {
                return NotFound();
            }

            ArrendamentoEstadoHabitacaoSaida arrendamentoSaida = new ArrendamentoEstadoHabitacaoSaida();

            arrendamentoSaida.ArrendamentoId = idArrendamento;
            arrendamentoSaida.Arrendamento = arrendamento;

            return View(arrendamentoSaida);
        }

        // POST: arrendamentoEstadoHabitacao/Saida
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Saida(
            [Bind("Id,DanosHabitacao,Observacoes,ArrendamentoId,FuncionarioId")] ArrendamentoEstadoHabitacaoSaida arrendamentoSaida,
            [FromForm] List<IFormFile> provas
            )
        {

            arrendamentoSaida.FuncionarioId = (await _userManager.GetUserAsync(User)).Id;
            arrendamentoSaida.Arrendamento = await _context.Arrendamento.Where(r => r.Id == arrendamentoSaida.ArrendamentoId).Include(r => r.Habitacao).Include(r => r.Cliente).FirstAsync();

            if (arrendamentoSaida.DanosHabitacao && provas.Count == 0)
            {
                ViewBag.error = "Insira as provas do dano na habitação";
                return View(arrendamentoSaida);
            }

            ModelState.Remove("Funcionario");
            ModelState.Remove("Arrendamento");

            if (ModelState.IsValid)
            {
                arrendamentoSaida.Arrendamento.ReservaEstadoHabitacaoEntradaId = arrendamentoSaida.Id;
                _context.Add(arrendamentoSaida);

                var habitacao = await _context.Habitacao.Where(v => v.Id == arrendamentoSaida.Arrendamento.Habitacao.Id).FirstAsync();

                _context.Update(habitacao);

                await _context.SaveChangesAsync();

                if (arrendamentoSaida.DanosHabitacao)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/provasDanosHabitacao/");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    // Dir relativo aos ficheiros do curso
                    path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/provasDanosHabitacoes/" + arrendamentoSaida.Id.ToString());
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    foreach (var formFile in provas)
                    {
                        if (formFile.Length > 0)
                        {
                            var filePath = Path.Combine(path, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                            while (System.IO.File.Exists(filePath))
                            {
                                filePath = Path.Combine(path, Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
                            }
                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(arrendamentoSaida);
        }

        // GET: Arrendamentos/Delete/5
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .Include(r => r.Cliente)
                .Include(r => r.Habitacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        // POST: Arrendamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Gestor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Arrendamento == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Arrendamento'  is null.");
            }
            var arrendamento = await _context.Arrendamento.FindAsync(id);
            if (arrendamento != null)
            {
                _context.Arrendamento.Remove(arrendamento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Arrendamento/Create
        [Authorize(Roles = "Cliente")]
        public IActionResult Create(int HabitacaoId)
        {
            if (HabitacaoId == null || _context.Habitacao == null)
            {
                return NotFound();
            }

            Arrendamento arrendamento = new Arrendamento();
            arrendamento.HabitacaoId = HabitacaoId;
            arrendamento.Habitacao = _context.Habitacao.Include(v => v.Localizacao)
                .Include(v => v.Categoria)
                .Include(v => v.Locador)
                .Include(v => v.Locador.Avaliacao)
                .Where(v => v.Id == HabitacaoId).First();
            arrendamento.DataEntrada = DateTime.Now.AddDays(1);
            arrendamento.DataSaida = DateTime.Now.AddDays(2);

            return View(arrendamento);

        }

        [Authorize(Roles = "Cliente")]
        public IActionResult CreateConfirmation([Bind("Id,DataEntrada,DataSaida,HabitacaoId")] Arrendamento arrendamento)
        {
            arrendamento.Habitacao = _context.Habitacao
                .Include(v => v.Localizacao)
                .Include(v => v.Categoria)
                .Include(v => v.Locador)
                .Include(v => v.Locador.Avaliacao)
                .Where(v => v.Id == arrendamento.HabitacaoId).First();

            return View(arrendamento);
        }

        // POST: Arrendamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([Bind("Id,DataEntrada,DataSaida,HabitacaoId")] Arrendamento arrendamento)
        {
            arrendamento.Habitacao = _context.Habitacao.Include(v => v.Localizacao)
                .Include(v => v.Categoria)
                .Include(v => v.Locador)
                .Include(v => v.Locador.Avaliacao)
                .Where(v => v.Id == arrendamento.HabitacaoId).First();
            arrendamento.Confirmado = false;
            arrendamento.ClienteId = (await _userManager.GetUserAsync(User)).Id;

            if (arrendamento.DataEntrada < DateTime.Now)
            {
                ViewBag.error = "Não pode entrar na habitação no passado!";
                return View(arrendamento);
            }

            if (arrendamento.DataEntrada > arrendamento.DataSaida)
            {
                ViewBag.error = "Datas de entrada e saida incorretas";
                return View(arrendamento);
            }

            if (_context.Arrendamento.Include(r => r.Habitacao).Where(r => r.Habitacao.Id == arrendamento.HabitacaoId
                    && ((DateTime.Compare(arrendamento.DataEntrada, r.DataEntrada) >= 0
                    && DateTime.Compare(arrendamento.DataEntrada, r.DataSaida) <= 0)
                    || (DateTime.Compare(arrendamento.DataSaida, r.DataEntrada) >= 0
                    && DateTime.Compare(arrendamento.DataSaida, r.DataSaida) <= 0))
                    ).Any())
            {
                ViewBag.error = "Esta habitação já tem um arrendamento entre essas datas";
                return View(arrendamento);
            }

            ModelState.Remove("ClienteId");
            ModelState.Remove("Cliente");
            ModelState.Remove("Habitacao");
            if (ModelState.IsValid)
            {
                _context.Add(arrendamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Historico));
            }

            return View(arrendamento);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Historico([Bind("TextoAPesquisar,Ordem,CategoriaId,DataEntrada,DataSaida")] PesquisaHistoricoArrendamentosViewModel pesquisaArrendamentos,
            string Categoria)
        {
            var user = await _userManager.GetUserAsync(User);

            IQueryable<Arrendamento> task = _context.Arrendamento
                .Include(r => r.Cliente)
                .Include(r => r.Habitacao)
                .Include(r => r.Habitacao.Categoria)
                .Include(r => r.ArrendamentoEstadoHabitacaoEntrada)
                .Include(r => r.ArrendamentoEstadoHabitacaoSaida)
                .Include(r => r.Habitacao.Locador)
                .Include(r => r.Habitacao.Locador.Avaliacao)
                .Where(r => r.ClienteId == user.Id);


            if (pesquisaArrendamentos.DataEntrada == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaArrendamentos.DataEntrada = DateTime.MinValue;
            }
            if (pesquisaArrendamentos.DataSaida == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaArrendamentos.DataSaida = DateTime.MaxValue;
            }

            if (string.IsNullOrWhiteSpace(pesquisaArrendamentos.TextoAPesquisar))
            {
                task = task.OrderByDescending(r => r.Cliente.UserName).ThenByDescending(r => r.Habitacao.Tipo).ThenByDescending(r => r.Habitacao.Categoria.Nome);
            }
            else
            {
                task = task
                    .Where(r =>
                        r.Habitacao.Tipo.Contains(pesquisaArrendamentos.TextoAPesquisar)
                        || r.Habitacao.Descricao.Contains(pesquisaArrendamentos.TextoAPesquisar)
                        || r.Habitacao.Morada.Contains(pesquisaArrendamentos.TextoAPesquisar)
                        || r.Cliente.UserName.Contains(pesquisaArrendamentos.TextoAPesquisar)
                    );
            }

            task = task.Where(r => DateTime.Compare(r.DataEntrada, pesquisaArrendamentos.DataEntrada) > 0
                    && DateTime.Compare(r.DataSaida, pesquisaArrendamentos.DataSaida) < 0);

            if (Categoria != null)
            {
                pesquisaArrendamentos.CategoriaId = int.Parse(Categoria);
            }
            if (pesquisaArrendamentos.CategoriaId != 0)
            {
                task = task.Where(r => r.Habitacao.CategoriaId == pesquisaArrendamentos.CategoriaId);
            }

            if (pesquisaArrendamentos.Ordem != null)
            {
                if (pesquisaArrendamentos.Ordem.Equals("desc"))
                {
                    task = task.OrderByDescending(r => r.Cliente.UserName).ThenByDescending(r => r.Habitacao.Tipo).ThenByDescending(r => r.Habitacao.Categoria.Nome);
                }
                else if (pesquisaArrendamentos.Ordem.Equals("asc"))
                {
                    task = task.OrderBy(r => r.Cliente.UserName).ThenBy(r => r.Habitacao.Tipo).ThenBy(r => r.Habitacao.Categoria.Nome);
                }
            }

            ViewBag.CategoriaId = _context.CategoriasHabitacao.ToList();

            pesquisaArrendamentos.ListaDeArrendamentos = await task.ToListAsync();

            if (pesquisaArrendamentos.DataSaida < pesquisaArrendamentos.DataEntrada)
            {
                ViewBag.error = "Datas de entrada e saida incorretas";
            }

            return View(pesquisaArrendamentos);
        }

        // Cliente
        // GET: Arrendamentos/Details/5
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Arrendamento == null)
            {
                return NotFound();
            }

            var arrendamento = await _context.Arrendamento
                .Include(r => r.Cliente)
                .Include(r => r.Habitacao)
                .Include(r => r.Habitacao.Categoria)
                .Include(r => r.Habitacao.Localizacao)
                .Include(r => r.Habitacao.Locador)
                .Include(r => r.ArrendamentoEstadoHabitacaoEntrada)
                .Include(r => r.ArrendamentoEstadoHabitacaoSaida)
                .Include(v => v.Habitacao.Locador.Avaliacao)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arrendamento == null)
            {
                return NotFound();
            }

            return View(arrendamento);
        }

        private bool ArrendamentoExists(int id)
        {
            return _context.Locador.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Gestor, Funcionario")]
        public ActionResult EstatisticasArrendamentoPorMes()
        {
            string gestorEmail = User.Identity.Name;
            string nomeDaEmpresa = gestorEmail.Split('@')[1].Split('.')[0];

            var empresaId = _context.Locador
                .Where(locador => locador.Nome == nomeDaEmpresa)
                .Select(locador => locador.Id)
                .FirstOrDefault();

            int numeroArrendamentosLocador = CalcularNumeroArrendamentosLocador(empresaId);

            ViewBag.NumeroArrendamentosLocador = numeroArrendamentosLocador;

            var estatisticas = _context.Arrendamento
                .Where(a => a.Habitacao.LocadorId == empresaId)
                .GroupBy(a => new { a.DataEntrada.Year, a.DataEntrada.Month })
                .Select(g => new EstatisticasViewModel
                {
                    Ano = g.Key.Year,
                    Mes = g.Key.Month,
                    NumeroArrendamentos = g.Count()
                })
                .OrderBy(g => g.Ano)
                .ThenBy(g => g.Mes)
                .ToList();

            return View(estatisticas);
        }

        private int CalcularNumeroArrendamentosLocador(int locadorId)
        {
            return _context.Arrendamento
                .Count(a => a.Habitacao.LocadorId == locadorId);
        }

        [Authorize(Roles = "Gestor, Funcionario")]
        public ActionResult ListaArrendamentos(int ano, int mes)
        {
            string gestorEmail = User.Identity.Name;
            string nomeDaEmpresa = gestorEmail.Split('@')[1].Split('.')[0];

            var empresaId = _context.Locador
                .Where(locador => locador.Nome == nomeDaEmpresa)
                .Select(locador => locador.Id)
                .FirstOrDefault();

            var arrendamentos = _context.Arrendamento
                .Include(a => a.Cliente)
                .Include(a => a.Habitacao.Locador)
                .Where(a => a.Habitacao.LocadorId == empresaId &&
                            a.DataEntrada.Year == ano &&
                            a.DataEntrada.Month == mes)
                .ToList();

            return View(arrendamentos);
        }
    }
}