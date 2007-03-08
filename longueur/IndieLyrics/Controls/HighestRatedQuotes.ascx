<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HighestRatedQuotes.ascx.cs" Inherits="Controls_HighestRatedQuotes" %>
<%@ Import Namespace="Common" %>

<asp:Repeater ID="HighestRepeater" runat="server">
	<HeaderTemplate>
		<div style="text-align: right; font-weight:bold">
			HIGHEST RATED
		</div>
		<br />
	</HeaderTemplate>
	<ItemTemplate>
		<a href="quote.aspx?QID=<%# DataBinder.Eval(Container.DataItem, "QuoteID") %>"><%# General.GetSubString(DataBinder.Eval(Container.DataItem, "QuoteText").ToString(), 15) %></a><br />
		<p style="text-align: right">
			<%# DataBinder.Eval(Container.DataItem, "SongName") %><br />
			<%# DataBinder.Eval(Container.DataItem, "ArtistName") %>
		</p>
		<br /><br />
	</ItemTemplate>
	<FooterTemplate>
	</FooterTemplate>
</asp:Repeater>