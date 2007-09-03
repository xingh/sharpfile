<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Photos_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
	"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<title>Photos</title>
	<link rel="stylesheet" type="text/css" href="styles.css" />

	<script src="http://www.google-analytics.com/urchin.js" type="text/javascript">
	</script>
	<script type="text/javascript">
	_uacct = "UA-247760-1";
	//urchinTracker();
</script>
</head>
<body>
<form id="frm" runat="server">
	<div id="header">
		<a href="../default.aspx">longueur.org</a> >> Photos
	</div>
	
	<div id="thumbnails">
		<asp:Repeater ID="rptPhotos" runat="server">
			<ItemTemplate>
				<input type="hidden" id="inpId" runat="server" value='<%# Eval("Id") %>' />
				<asp:ImageButton ID="imgThumbnail" runat="server" 
					AlternateText='<%# Eval("Title") %>' ToolTip='<%# Eval("Title") %>' 
					ImageUrl='<%# Eval("ThumbnailUrl") %>' CommandArgument='<%# Eval("Id") %>' 
					OnClick="imgThumbnail_OnClick" CssClass="photo" />
			</ItemTemplate>
		</asp:Repeater>
	</div>
	<div id="container">
		<div id="titlebar">
			<div id="title">
				<asp:Literal ID="ltlTitle" runat="server" />
			</div>
		</div>
		<div id="content">
			<asp:HyperLink ID="hypCurrent" runat="server" Target="_blank">
				<asp:Image ID="imgCurrent" runat="server" style="border: solid 1px #000" />
			</asp:HyperLink>
		</div>
		<div id="divDescription" class="description" runat="server">
			<asp:Literal ID="ltlDescription" runat="server" />
		</div>
		<div id="navigation">
			<div id="previous">
				<asp:LinkButton ID="lnkPrevious" Text="<< Previous" runat="server" />
			</div>
			<div id="next">
				<asp:LinkButton ID="lnkNext" Text="Next >>" runat="server" />
			</div>		
		</div>
	</div>
</body>
</form>
</html>