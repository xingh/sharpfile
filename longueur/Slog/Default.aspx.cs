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

public partial class Slog_Default : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// TODO: Randomize this bitch.
		this.Title = "Slog: Est. 1842.";

		rptContent.DataSource = SlogData.GetSlogs();
		rptContent.DataBind();
	}
}
