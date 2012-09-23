<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<L24CM.Models.BbText>" %>
<%= Html.TextBox("", (Model ?? BbText.Empty).Text,
    new { @class = "text-box single-line " + ViewData["classes"] })  %>

