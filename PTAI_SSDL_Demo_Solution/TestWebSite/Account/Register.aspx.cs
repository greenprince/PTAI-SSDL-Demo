using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using TestWebSite.Models;

namespace TestWebSite.Account
{
    public partial class Register : Page
    {
        protected void CreateUser_Click(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
            var user = new ApplicationUser() { UserName = Email.Text, Email = Email.Text };
            IdentityResult result = manager.Create(user, Password.Text);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                //string code = manager.GenerateEmailConfirmationToken(user.Id);
                //string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
                //manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");

                signInManager.SignIn( user, isPersistent: false, rememberBrowser: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else 
            {
                // TODO: AI issue #13, Medium, XSS, http://ptssdal.ptsecurity.it.prv/#/taskResults/25
                // POST /Account/Register.aspx HTTP/1.1
                // Host: localhost
                // Content-Length: 40
                // Content-Type: application/x-www-form-urlencoded
                //
                // __AI_Button_jbptekyi=True&Password=False
                // Microsoft.AspNet.Identity.UserManagerExtensions.Create(Microsoft.AspNet.Identity.Owin.OwinContextExtensions.GetUserManager(System.Web.HttpContextExtensions.GetOwinContext(ASP.Page_Account_Register_aspx.Context)), TestWebSite.Models.ApplicationUser, System.Web.UI.WebControls.TextBox.Text).Errors[0].Contains("<script>alert(1)</script>")
                ErrorMessage.Text = result.Errors.FirstOrDefault();
            }
        }
    }
}