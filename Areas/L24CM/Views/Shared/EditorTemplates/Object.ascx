<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<script runat="server">
    bool ShouldShow(ModelMetadata metadata) {
        return metadata.ShowForEdit
            && metadata.ModelType != typeof(System.Data.EntityState)
            //&& !metadata.IsComplexType
            && !ViewData.TemplateInfo.Visited(metadata);
    }
</script>
<%
    if (ViewData.TemplateInfo.TemplateDepth > 3) { %>
    <% if (Model == null) { %>
        <%= ViewData.ModelMetadata.NullDisplayText %>
    <% } else { %>
        <%= ViewData.ModelMetadata.SimpleDisplayText %>
    <% } %>
<% } else { %>    
    <% foreach (var prop in ViewData.ModelMetadata.Properties.Where(pm => ShouldShow(pm))) {
           int indent = (ViewData.TemplateInfo.TemplateDepth + ((ViewData["addDepth"] as int?) ?? 0) - 1) * 10;
           if (prop.HideSurroundingHtml) { %>
            <div class="editor-field" style="margin-left: <%= indent %>px"><%= Html.Editor(prop.PropertyName) %></div>
        <% } else { %>
            <% if (!String.IsNullOrEmpty(Html.Label(prop.PropertyName).ToHtmlString())) { %>
                <div class="editor-label" style="margin-left: <%= indent %>px"><%= Html.Label(prop.PropertyName) %></div>
            <% } %>
            <div class="editor-field" style="margin-left: <%= indent %>px"><%= Html.Editor(prop.PropertyName) %> <%= Html.ValidationMessage(prop.PropertyName, "*") %></div>
        <% } %>
    <% } %>
<% } %>