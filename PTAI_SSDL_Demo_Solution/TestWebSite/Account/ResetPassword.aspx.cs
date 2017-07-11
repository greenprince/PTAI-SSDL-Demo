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
    public partial class ResetPassword : Page
    {
        protected string StatusMessage
        {
            get;
            private set;
        }

        protected void Reset_Click(object sender, EventArgs e)
        {
            string code = IdentityHelper.GetCodeFromRequest(Request);
            if (code != null)
            {
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

                var user = manager.FindByName(Email.Text);
                if (user == null)
                {
                    ErrorMessage.Text = "No user found";
                    return;
                }
                var result = manager.ResetPassword(user.Id, code, Password.Text);
                if (result.Succeeded)
                {
                    Response.Redirect("~/Account/ResetPasswordConfirmation");
                    return;
                }
                // TODO: AI issue #14, Medium, XSS, http://ptssdal.ptsecurity.it.prv/#/taskResults/26
                // POST /Account/ResetPassword.aspx HTTP/1.1
                // Host: localhost
                // Content-Length: 81
                // Content-Type: application/x-www-form-urlencoded
                //
                // __AI_Button_jbptekyi=True&Email=1&Password=1&TestWebSite.IdentityHelper.CodeKey=+
                // ((Microsoft.AspNet.Identity.UserManagerExtensions.FindByName(Microsoft.AspNet.Identity.Owin.OwinContextExtensions.GetUserManager(System.Web.HttpContextExtensions.GetOwinContext(ASP.Page_Account_ResetPassword_aspx.Context)), System.Web.UI.WebControls.TextBox.Text) != null) && Microsoft.AspNet.Identity.UserManagerExtensions.ResetPassword(Microsoft.AspNet.Identity.Owin.OwinContextExtensions.GetUserManager(System.Web.HttpContextExtensions.GetOwinContext(ASP.Page_Account_ResetPassword_aspx.Context)), Microsoft.AspNet.Identity.UserManagerExtensions.FindByName(Microsoft.AspNet.Identity.Owin.OwinContextExtensions.GetUserManager(System.Web.HttpContextExtensions.GetOwinContext(ASP.Page_Account_ResetPassword_aspx.Context)), System.Web.UI.WebControls.TextBox.Text).Id, " ", System.Web.UI.WebControls.TextBox.Text).Errors[0].Contains("<script>alert(1)</script>"))
                ErrorMessage.Text = result.Errors.FirstOrDefault();
                return;
            }

            ErrorMessage.Text = "An error has occurred";
        }
    }
}