<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<L24CM.Models.MediaFileLink>" %>
<%@ Import Namespace="L24CM.Models" %>
<div class='l24-image'>
    <div class='l24-image-url'>
        <button class='l24-media-load'>Find Media</button>
        <span>Url</span>
        <input type="text" class="l24-file-url" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Url" value="<%= Model.Url %>" />
    </div>
    <div class='l24-media-content'>
        <span>Content</span>
        <input type="text" class="bb-text" name="<%= ViewData.TemplateInfo.HtmlFieldPrefix %>.Content" value="<%= (Model.Content ?? BbText.Empty).Text %>" />
    </div>
</div>

