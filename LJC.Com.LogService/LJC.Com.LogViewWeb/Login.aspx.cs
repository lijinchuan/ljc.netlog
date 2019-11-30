using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LJC.Com.LogViewWeb
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Login1.Authenticate += Login1_Authenticate;
        }

        void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            if (System.Web.Security.FormsAuthentication.Authenticate(Login1.UserName, Login1.Password))
            {
                e.Authenticated = true;
                //var cookie = System.Web.Security.FormsAuthentication.GetAuthCookie(Login1.UserName, true);
                //cookie.Expires = DateTime.Now.AddMinutes(10);
                //Response.Cookies.Add(cookie);
            }
        }
    }
}