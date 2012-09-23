<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>" class="collection <%= ViewData["formState"] == null || (ViewData["formState"] as string).Contains(ViewData.TemplateInfo.HtmlFieldPrefix) ? "" : "closed" %>">
<%
    if (Model != null) {
        string oldPrefix = ViewData.TemplateInfo.HtmlFieldPrefix;
        int index = 0;

        ViewData.TemplateInfo.HtmlFieldPrefix = String.Empty;
        
        foreach (object item in (IEnumerable)Model) {
            string fieldName = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}[{1}]", oldPrefix, index++);
            %>
            <div id="del-<%= fieldName %>" class="action-button" style="float: left">x</div>
            <%
                MvcHtmlString editor = Html.EditorFor(m => item, null, fieldName);
                Response.Write(editor);
            
        }
        
        ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;
    }
%>
</div>
<div id="add-<%= ViewData.TemplateInfo.HtmlFieldPrefix %>" class="add-button depth-<%= ViewData.TemplateInfo.TemplateDepth %>">+</div>