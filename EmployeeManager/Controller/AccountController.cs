using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using EmployeeManager.Models;
using EmployeeManager.Services;
using EmployeeManager.Models.DTO;

namespace EmployeeManager.Controller
{
    [Route("[controller]/[action]")]
    public class AccountController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IEmailSender emailService)
        {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Index(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.User, Email = model.User };
                var existingUser = await userManager.FindByEmailAsync(model.User);
                if (existingUser != null)
                {
                    await userManager.DeleteAsync(existingUser);
                }

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var identityUser = await userManager.FindByEmailAsync(model.User);
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                    var confirmUrl = Url.Action("ConfirmEmail", "Account", new { Id = identityUser.Id, Token = token }, Request.Scheme);
                    var body = new BodyBuilder();
                    body.HtmlBody = $"<h5>Hi {identityUser.UserName.Split("@").First()},</h5>" +
                        $"<p> Please click on the following link to verify " +
                        $"your email at the Employee Manager app.</p></br> <a href='{confirmUrl}'>Click here</a> </br> " +
                        $"<p style='color: orange;'>If you did not make this request, please simply ignore</p>";

                    await emailService.SendMailAsync(identityUser.Email, body,
                        "Email varification");

                    ViewBag.ErrorMessage = "A confirmation email is sent to your email address. Please click on the link we sent to your email to varify.";
                    return View("~/Views/Error/HandleError.cshtml");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }


            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Content("~/"),
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var signInAttempt = await signInManager.PasswordSignInAsync(model.User, model.Password
                    , model.RememberMe, false);
                if (signInAttempt.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }

                }
                ModelState.AddModelError(string.Empty, "User or password do not match");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [AcceptVerbs("Get, Post")]
        public async Task<IActionResult> IsEmailExists(string email)
        {
            var user = await userManager.FindByEmailAsync((email));
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"A user with email {email} already exists");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var model = new LoginViewModel
            {
                ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("~/"),
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loading external login info");
                return View("Login", model);
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            EmailConfirmed = true
                        };
                        var identityResult = await userManager.CreateAsync(user);
                        if (!identityResult.Succeeded)
                        {
                            ViewBag.ErrorMessage = "Sorry! The system failed to create your account from external authentication provider.";
                            return View("~/Views/Error/HandleError.cshtml");
                        }
                    }

                    var identityResult2 = await userManager.AddLoginAsync(user, info);
                    if (identityResult2.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                ViewBag.ErrorMessage = "Sorry! The system failed to create your account from external authentication provider.";
                return View("~/Views/Error/HandleError.cshtml");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string id, string token)
        {
            if (id == null || token == null)
            {
                ViewBag.ErrorMessage = "The url is either expired or incorrect";
                return View("~/Views/Error/HandleError.cshtml");
            }

            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.ErrorMessage = "Your email is varified successfully. You can now log in.";
                return View("~/Views/Error/HandleError.cshtml");
            }

            ViewBag.ErrorMessage = "Can not verfiy email. Please try again later";
            return View("~/Views/Error/HandleError.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (model.Email != null && await userManager.FindByEmailAsync(model.Email) != null)
            {
                var identityUser = await userManager.FindByEmailAsync(model.Email);
                var token = await userManager.GeneratePasswordResetTokenAsync(identityUser);
                var confirmUrl = Url.Action("ResetPassword", "Account", new { Email = identityUser.Email, Token = token }, Request.Scheme);
                var body = new BodyBuilder();
                body.HtmlBody = $"<h5>Hi {identityUser.UserName.Split("@").First()},</h5>" +
                    $"<p> please click on this button to reset " +
                    $"your password at EmployeeManager.</p></br> <a href='{confirmUrl}'>Click here </a> </br>. " +
                    $"<p style='color: orange;'>If you did not make this request, please simply ignore</p>";

                await emailService.SendMailAsync(identityUser.Email, body,
                    "Password reset");

                ViewBag.ErrorMessage = "A password reset link is sent to your email address. " +
                    "Please click on the link and fill the form to reset.";
                return View("~/Views/Error/HandleError.cshtml");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (ModelState.IsValid && email != null && token != null && (await userManager.FindByEmailAsync(email) != null))
            {
                var model = new ResetPassword
                {
                    Email = email,
                    Token = token
                };
                return View(model);
            }
            ViewBag.ErrorMessage = "The url to reset password is not valid";
            return View("~/Views/Error/HandleError.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid && (await userManager.FindByEmailAsync(model.Email) != null))
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.ErrorMessage = "Password reset successful !";
                    return View("~/Views/Error/HandleError.cshtml");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user != null)
                {
                    var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        await signInManager.RefreshSignInAsync(user);
                        ViewBag.ErrorMessage = "Password change successful !";
                        return View("~/Views/Error/HandleError.cshtml");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}