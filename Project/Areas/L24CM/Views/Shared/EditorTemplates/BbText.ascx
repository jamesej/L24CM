<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<L24CM.Models.BbText>" %>
<%@ Import Namespace="L24CM.Models" %>
<%= Html.TextBox("", (Model ?? BbText.Empty).Text,
    new { @class = "text-box single-line bb-text " + ViewData["classes"] })  %>

