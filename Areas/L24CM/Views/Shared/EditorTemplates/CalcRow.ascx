<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TreadLighter.Models.CalcRow>" %>
<% if (Model != null) { %>
<table class='l24-calc-row'>
<tr>
    <td class="editor-label"><%= Html.LabelFor(m => m.RowLabel) %></td>
    <td class="editor-label"><%= Html.LabelFor(m => m.Value) %></td>
    <td class="editor-label"><%= Html.LabelFor(m => m.RunningValue) %></td>
    <td class="editor-label"><%= Html.LabelFor(m => m.Highlighted) %></td>
</tr>
<tr>
    <td><%= Html.EditorFor(m => m.RowLabel) %></td>
    <td><%= Html.EditorFor(m => m.Value) %></td>
    <td><%= Html.EditorFor(m => m.RunningValue) %></td>
    <td><%= Html.EditorFor(m => m.Highlighted) %></td>
</tr>
<tr>
    <td class="editor-label"><%= Html.LabelFor(m => m.CrossHeading) %></td>
    <td colspan="3"><%= Html.EditorFor(m => m.CrossHeading) %></td>
</tr>
</table>
<% } %>

