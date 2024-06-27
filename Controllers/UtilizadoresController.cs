using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Trabalho_Pratico.Data;
using Trabalho_Pratico.Models.ViewModels;
using Trabalho_Pratico.Models;

namespace Trabalho_Pratico.Controllers
{
    public class UtilizadoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UtilizadoresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Utilizadores
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index([Bind("TextoAPesquisar,Ordem")] PesquisaUtilizadorViewModel pesquisaUtilizador)
        {

            IQueryable<ApplicationUser> task;
            if (string.IsNullOrWhiteSpace(pesquisaUtilizador.TextoAPesquisar))
            {
                task = _userManager.Users.Where(e => e.PrimeiroNome.Contains(""))
                    .OrderByDescending(e => e.PrimeiroNome)
                    .ThenByDescending(e => e.UltimoNome)
                    .ThenByDescending(e => e.UserName);
            }
            else
            {
                task = _userManager.Users.Where(e =>
                e.PrimeiroNome.Contains(pesquisaUtilizador.TextoAPesquisar)
                || e.UltimoNome.Contains(pesquisaUtilizador.TextoAPesquisar)
                || e.UserName.Contains(pesquisaUtilizador.TextoAPesquisar)
                );
            }
            if (pesquisaUtilizador.Ordem != null)
            {
                if (pesquisaUtilizador.Ordem.Equals("nomeDesc"))
                {
                    task = task.OrderByDescending(e => e.PrimeiroNome)
                        .ThenByDescending(e => e.UltimoNome)
                        .ThenByDescending(e => e.UserName);
                }
                else if (pesquisaUtilizador.Ordem.Equals("nomeAsc"))
                {
                    task = task.OrderBy(e => e.PrimeiroNome)
                        .ThenBy(e => e.UltimoNome)
                        .ThenBy(e => e.UserName);
                }
            }

            var listaUsers = await task.ToListAsync();

            var rolesPesquisa = await _context.Roles.Where(r => r.Name.Contains(pesquisaUtilizador.TextoAPesquisar)).ToListAsync();

            foreach (var role in rolesPesquisa)
            {
                var usersInRoles = await _userManager.GetUsersInRoleAsync(role.Name);

                foreach (var user in usersInRoles)
                {
                    if (!listaUsers.Contains(user))
                    {
                        listaUsers.Add(user);
                    }
                }
            }

            List<UtilizadorViewModel> usersViewModel = new List<UtilizadorViewModel>();

            foreach (var user in listaUsers)
            {
                UtilizadorViewModel userRolesViewModel = new UtilizadorViewModel();

                userRolesViewModel.Id = user.Id;
                userRolesViewModel.UserName = user.UserName;
                userRolesViewModel.PrimeiroNome = user.PrimeiroNome;
                userRolesViewModel.UltimoNome = user.UltimoNome;
                userRolesViewModel.Ativo = user.ContaAtiva;

                userRolesViewModel.Roles = await _userManager.GetRolesAsync(user);

                usersViewModel.Add(userRolesViewModel);
            }

            pesquisaUtilizador.ListaDeUtilizadores = usersViewModel;

            return View(pesquisaUtilizador);
        }

        // GET: Ativar
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> Ativar(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            utilizador.ContaAtiva = true;
            await _userManager.UpdateAsync(utilizador);

            var admins = await _userManager.GetUsersInRoleAsync("Administrador");
            if (admins.Contains(await _userManager.GetUserAsync(User)))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Gestor));
        }

        // GET: Desativar
        [Authorize(Roles = "Administrador,Gestor")]
        public async Task<IActionResult> Desativar(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            utilizador.ContaAtiva = false;
            await _userManager.UpdateAsync(utilizador);

            var admins = await _userManager.GetUsersInRoleAsync("Administrador");
            if (admins.Contains(await _userManager.GetUserAsync(User)))
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Gestor));
        }

        // GET: Utilizadores/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Users.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }

            EditarUtilizadorViewModel editarUtilizador = new EditarUtilizadorViewModel()
            {
                utilizador = utilizador,
                roles = new List<RolesViewModel>()
            };

            var userRoles = await _userManager.GetRolesAsync(await _userManager.Users.Where(u => u.Id == utilizador.Id).FirstAsync());

            var listRoles = await _context.Roles.ToListAsync();

            foreach (var role in listRoles)
            {
                RolesViewModel roleViewModel = new RolesViewModel();
                roleViewModel.RoleId = role.Id;
                roleViewModel.RoleName = role.Name;
                roleViewModel.Selected = userRoles.Contains(role.Name);

                editarUtilizador.roles.Add(roleViewModel);
            }

            SelectList sl = new SelectList(_context.Locador, "Id", "Nome");
            if (utilizador.Locador != null)
            {
                var selected = sl.Where(x => x.Value == utilizador.Locador.Id.ToString()).First();
                selected.Selected = true;
            }
            ViewBag.LocadorId = sl;
            return View(editarUtilizador);
        }

        // POST: Utilizadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(string id, List<RolesViewModel> roles, ApplicationUser utilizador)
        {

            var user = await _userManager.Users.Where(u => u.Id == utilizador.Id).FirstAsync();

            if (id != utilizador.Id || user == null)
            {
                return NotFound();
            }

            var utilizadorB4Changes = await _context.Users.FindAsync(id);
            EditarUtilizadorViewModel editarUtilizador = new EditarUtilizadorViewModel()
            {
                utilizador = utilizadorB4Changes,
                roles = new List<RolesViewModel>()
            };

            var listRoles = await _context.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(await _userManager.Users.Where(u => u.Id == utilizador.Id).FirstAsync());

            foreach (var role in listRoles)
            {
                RolesViewModel roleViewModel = new RolesViewModel();
                roleViewModel.RoleId = role.Id;
                roleViewModel.RoleName = role.Name;
                roleViewModel.Selected = userRoles.Contains(role.Name);

                editarUtilizador.roles.Add(roleViewModel);
            }

            ModelState.Remove("utilizador.Arrendamentos");
            ModelState.Remove("utilizador.Avaliacoes");
            ModelState.Remove("utilizador.HabitacaoEntregueAClientes");
            if (ModelState.IsValid)
            {
                if (user.PrimeiroNome != utilizador.PrimeiroNome)
                {
                    user.PrimeiroNome = utilizador.PrimeiroNome;
                    await _userManager.UpdateAsync(user);
                }
                if (user.UltimoNome != utilizador.UltimoNome)
                {
                    user.UltimoNome = utilizador.UltimoNome;
                    await _userManager.UpdateAsync(user);
                }
                if (user.NIF != utilizador.NIF)
                {
                    user.NIF = utilizador.NIF;
                    await _userManager.UpdateAsync(user);
                }
                if (user.DataNascimento != utilizador.DataNascimento)
                {
                    user.DataNascimento = utilizador.DataNascimento;
                    await _userManager.UpdateAsync(user);
                }
                bool gestorOuFuncionario = false;
                foreach (var role in roles)
                {
                    if ((role.RoleName.Equals("Gestor") || role.RoleName.Equals("Funcionario")) && role.Selected)
                    {
                        gestorOuFuncionario = true;
                        if (user.LocadorId != utilizador.LocadorId)
                        {
                            user.LocadorId = utilizador.LocadorId;
                            await _userManager.UpdateAsync(user);
                            break;
                        }
                    }
                }
                if (!gestorOuFuncionario)
                {
                    user.LocadorId = null;
                    await _userManager.UpdateAsync(user);
                }

                editarUtilizador.utilizador = user;

                var rolesUser = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, rolesUser);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove user existing roles");
                    return View(editarUtilizador);
                }

                result = await _userManager.AddToRolesAsync(user,
                    roles.Where(x => x.Selected).Select(y => y.RoleName));

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected roles to user");
                    return View(editarUtilizador);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(editarUtilizador);
        }

        // GET: Utilizadores
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Gestor([Bind("TextoAPesquisar,Ordem")] PesquisaUtilizadorViewModel pesquisaUtilizador)
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
                TempData.Remove("error");
            }

            var userLogado = await _userManager.GetUserAsync(User);

            IQueryable<ApplicationUser> task;
            if (string.IsNullOrWhiteSpace(pesquisaUtilizador.TextoAPesquisar))
            {
                task = _userManager.Users.Where(e => e.PrimeiroNome.Contains("") && e.LocadorId == userLogado.LocadorId)
                    .OrderByDescending(e => e.PrimeiroNome)
                    .ThenByDescending(e => e.UltimoNome)
                    .ThenByDescending(e => e.UserName);
            }
            else
            {
                task = _userManager.Users.Where(e =>
                e.LocadorId == userLogado.LocadorId

                && (e.PrimeiroNome.Contains(pesquisaUtilizador.TextoAPesquisar)
                || e.UltimoNome.Contains(pesquisaUtilizador.TextoAPesquisar)
                || e.UserName.Contains(pesquisaUtilizador.TextoAPesquisar))
                );
            }
            if (pesquisaUtilizador.Ordem != null)
            {
                if (pesquisaUtilizador.Ordem.Equals("nomeDesc"))
                {
                    task = task.OrderByDescending(e => e.PrimeiroNome)
                        .ThenByDescending(e => e.UltimoNome)
                        .ThenByDescending(e => e.UserName);
                }
                else if (pesquisaUtilizador.Ordem.Equals("nomeAsc"))
                {
                    task = task.OrderBy(e => e.PrimeiroNome)
                        .ThenBy(e => e.UltimoNome)
                        .ThenBy(e => e.UserName);
                }
            }

            var listaUsers = await task.ToListAsync();

            var rolesPesquisa = await _context.Roles.Where(r => r.Name.Contains(pesquisaUtilizador.TextoAPesquisar)).ToListAsync();

            foreach (var role in rolesPesquisa)
            {
                var usersInRoles = await _userManager.GetUsersInRoleAsync(role.Name);

                foreach (var userInRole in usersInRoles)
                {
                    if (!listaUsers.Contains(userInRole) && userInRole.LocadorId == userLogado.LocadorId)
                    {
                        listaUsers.Add(userInRole);
                    }
                }
            }

            listaUsers.Remove(userLogado);

            List<UtilizadorViewModel> usersViewModel = new List<UtilizadorViewModel>();

            foreach (var user in listaUsers)
            {
                UtilizadorViewModel userRolesViewModel = new UtilizadorViewModel();

                userRolesViewModel.Id = user.Id;
                userRolesViewModel.UserName = user.UserName;
                userRolesViewModel.PrimeiroNome = user.PrimeiroNome;
                userRolesViewModel.UltimoNome = user.UltimoNome;
                userRolesViewModel.Ativo = user.ContaAtiva;

                userRolesViewModel.Roles = await _userManager.GetRolesAsync(user);

                usersViewModel.Add(userRolesViewModel);
            }

            pesquisaUtilizador.ListaDeUtilizadores = usersViewModel;

            return View(pesquisaUtilizador);
        }

        // GET: Create
        [Authorize(Roles = "Gestor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Create([Bind("Email,PrimeiroNome,UltimoNome,DataNascimento,NIF,TipoUser,Password,ConfirmPassword")] AdicionarFuncionarioGestorViewModel utilizador)
        {

            if (ModelState.IsValid)
            {

                var userLogado = await _userManager.GetUserAsync(User);

                var newUser = new ApplicationUser
                {
                    UserName = utilizador.Email,
                    Email = utilizador.Email,
                    PrimeiroNome = utilizador.PrimeiroNome,
                    UltimoNome = utilizador.UltimoNome,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    ContaAtiva = true,
                    NIF = utilizador.NIF,
                    DataNascimento = utilizador.DataNascimento
                };

                newUser.LocadorId = (await _context.Users.Include(u => u.Locador).Where(u => u.Id == userLogado.Id).FirstOrDefaultAsync()).Locador.Id;

                var user = await _userManager.FindByEmailAsync(newUser.Email);
                if (user == null)
                {
                    await _userManager.CreateAsync(newUser, utilizador.Password);
                    if (utilizador.TipoUser.Equals("func"))
                    {
                        await _userManager.AddToRoleAsync(newUser, Roles.Funcionario.ToString());
                    }
                    else if (utilizador.TipoUser.Equals("gestor"))
                    {
                        await _userManager.AddToRoleAsync(newUser, Roles.Gestor.ToString());
                    }
                }

                return RedirectToAction(nameof(Gestor));
            }

            return View(utilizador);
        }
        // GET: Utilizadores/Delete/5
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (_context.EstadoHabitacaoEntradas.Where(r => r.FuncionarioId == id).Any()
            || _context.EstadoHabitacaoSaidas.Where(r => r.FuncionarioId == id).Any())
            {
                TempData["error"] = "O funcionário '" + user.UserName + "' tem reservas associadas a si.";
                return RedirectToAction(nameof(Gestor));
            }

            return View(user);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Locador == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AspNetUsers'  is null.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Gestor));
        }

    }
}
