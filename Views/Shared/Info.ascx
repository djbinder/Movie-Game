<!-- <%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<JSSerializeTestMVC2.Models.Info>" %> -->
@model movieGame.Models.Movie

<fieldset id="infoForm">
    <div style="margin: 1em .5em">
        <%: Html.LabelFor(model => model.Title)%>
        <%: Html.TextBoxFor(model => model.Title)%>
    </div>

    <p>
        <input id="submitButton" type="button" value="Submit Info" />
    </p>
</fieldset>