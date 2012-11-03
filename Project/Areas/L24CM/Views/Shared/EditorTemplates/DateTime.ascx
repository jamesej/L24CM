<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.DateTime?>" %>
<%@ Import Namespace="L24CM.Models" %>
<%@ Import Namespace="L24CM.Utility" %>
<%: Html.TextBox("", string.Format("{0:yyyy-MM-dd}", Model.HasValue ? Model : DateTime.Today), 
    new { @class = "l24-datetime" }) %>

