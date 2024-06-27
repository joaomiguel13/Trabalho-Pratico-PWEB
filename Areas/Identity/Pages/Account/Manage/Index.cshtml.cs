// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Trabalho_Pratico.Models;

namespace Trabalho_Pratico.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Número Telefone")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Primeiro Nome", Prompt = "Introduza o primeiro nome",
            Description = "Primeiro nome do utilizador")]
            public string PrimeiroNome { get; set; }

            [Display(Name = "Último Nome", Prompt = "Introduza o último nome",
                Description = "Último nome do utilizador")]
            public string UltimoNome { get; set; }

            [Display(Name = "Data de Nascimento", Prompt = "Introduza a data se nascimento",
                Description = "Data se nascimento do utilizador")]
            [PersonalData, DataType(DataType.Date)]
            public DateTime DataNascimento { get; set; }

            [Display(Name = "NIF", Prompt = "Introduza o NIF",
                Description = "Número de Indentificação Fiscal do utilizador")]
            [PersonalData]
            [RegularExpression("^\\d{9}$", ErrorMessage = "O NIF tem de ter 9 digitos!")]
            public int NIF { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                PrimeiroNome = user.PrimeiroNome,
                UltimoNome = user.UltimoNome,
                NIF = user.NIF,
                DataNascimento = user.DataNascimento
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (user.PrimeiroNome != Input.PrimeiroNome)
            {
                user.PrimeiroNome = Input.PrimeiroNome;
                await _userManager.UpdateAsync(user);
            }


            if (user.UltimoNome != Input.UltimoNome)
            {
                user.UltimoNome = Input.UltimoNome;
                await _userManager.UpdateAsync(user);
            }

            if (user.DataNascimento != Input.DataNascimento)
            {
                user.DataNascimento = Input.DataNascimento;
                await _userManager.UpdateAsync(user);
            }

            if (user.NIF != Input.NIF)
            {
                user.NIF = Input.NIF;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
