<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<L24CM.Models.MediaFileLink>" %>
<table class='l24-image'>
<tr class='l24-image-url'>
    <td>Url</td>
    <td><button class='l24-media-load'>Find Media</button><input type="text" class="l24-file-url" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Url" value="<%= Model.Url %>" /></td>
</tr>
</table>

