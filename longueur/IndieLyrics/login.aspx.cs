using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Common;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack) {
			Session["ReferralUrl"] = Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "default.aspx";
			message.Visible = false;
			user.Focus();
		} else {
			if (Request.Form["MultiTaskType"] != null && Request.Form["MultiTaskType"] == "submit") {
				if (Data.ValidateUser(user.Text, Security.Encrypt(password.Text))) {
					Session[Constants.CurrentUser] = Data.GetUser(user.Text);

					Response.Clear();

					if (Session["ReferralUrl"] != null) {
						Response.Redirect(Session["ReferralUrl"].ToString(), true);
					} else {
						Response.Redirect("default.aspx", true);
					}
				} else {
					message.Visible = true;
					message.Text = "The username or password entered is incorrect. Try again.";
				}
			}
		}
    }
}