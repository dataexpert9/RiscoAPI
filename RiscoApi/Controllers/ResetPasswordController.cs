using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BasketApi.Components.Helpers;

namespace BasketApi.Controllers
{
    public class ResetPasswordController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string code)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel();

            using (RiscoContext ctx = new RiscoContext())
            {
                var token = ctx.ForgotPasswordTokens.FirstOrDefault(x => x.Code == code && x.IsDeleted == false && (DateTime.UtcNow.Hour - x.CreatedAt.Hour) < 4);

                if (token != null)
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Id == token.User_ID);
                    model.UserId = user.Id;
                    model.Email = user.Email;
                    return View(model);
                }
                else
                    return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (RiscoContext ctx = new RiscoContext())
            {
                var tokens = ctx.ForgotPasswordTokens.Where(x => x.User_ID == model.UserId && x.IsDeleted == false);

                if (tokens.Count() > 0)
                {
                    foreach (var token in tokens)
                        token.IsDeleted = true;
                    var hashedPassword = CryptoHelper.Hash(model.Password);
                    ctx.Users.FirstOrDefault(x => x.Id == model.UserId).Password = hashedPassword;

                    ctx.SaveChanges();

                }
                else
                    return View("Error");
            }

            return View("PasswordResetSuccess");
        }
    }

    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

    }
}