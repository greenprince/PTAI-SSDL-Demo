using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using TestWebSite.Models;

namespace TestWebSite.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            // Enable this once you have account confirmation enabled for password reset functionality
            //ForgotPasswordHyperLink.NavigateUrl = "Forgot";
            OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();

                // This doen't count login failures towards account lockout
                // To enable password failures to trigger lockout, change to shouldLockout: true
                var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                        break;
                    case SignInStatus.LockedOut:
                        Response.Redirect("/Account/Lockout");
                        break;
                    case SignInStatus.RequiresVerification:
                        // TODO: AI issue #12, Medium, OR, http://ptssdal.ptsecurity.it.prv/#/taskResults/24
                        // POST /Account/Login.aspx HTTP/1.1
                        // Host: localhost
                        // Content-Length: 103
                        // Content-Type: application/x-www-form-urlencoded
                        //
                        // __AI_Button_mxyknblx=True&Email=ARBITRARY_DATA&Password=ARBITRARY_DATA&ReturnUrl=http%3a%2f%2flocalhost
                        // (Microsoft.AspNet.Identity.Owin.SignInManagerExtensions.PasswordSignIn(Microsoft.AspNet.Identity.Owin.OwinContextExtensions.GetUserManager(System.Web.HttpContextExtensions.GetOwinContext(ASP.Page_Account_Login_aspx.Context)), System.Web.UI.WebControls.TextBox.Text, System.Web.UI.WebControls.TextBox.Text, false, false) == 2)
                        Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}", 
                                                        Request.QueryString["ReturnUrl"],
                                                        RememberMe.Checked),
                                          true);
                        break;
                    case SignInStatus.Failure:
                    default:
                        FailureText.Text = "Invalid login attempt";
                        ErrorMessage.Visible = true;
                        break;
                }
            }
        }
    }
}