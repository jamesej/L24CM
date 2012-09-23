<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<L24CM.Models.Image>" %>
<table class='l24-image'>
<tr class='l24-image-url'>
    <td>Url</td>
    <td><button class='l24-image-load'>Find File</button><input type="text" class="l24-file-url" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Url" value="<%= Model.Url %>" /></td>
</tr>
<tr class='l24-image-alt'>
    <td>Alt</td>
    <td><%= Html.EditorFor(m => m.Alt) %></td>
</tr>
<tr class='l24-image-content'>
    <td>Content</td>
    <td class='l24-image-content-cell'>
        <% if (string.IsNullOrEmpty(Model.Url))
           { %>
        no image
        <% }
           else
           { %>
        <img class='file-image-thumb' src="<%= Model.Url %>" />
        <% } %>
    </td>
</tr>
</table>

