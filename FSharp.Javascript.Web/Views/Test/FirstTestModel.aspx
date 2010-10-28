<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<Models+FirstTestModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    FirstTestModel
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--<%: Html.GetCompiledModule(typeof(FSharp.Javascript.Web.TestModule)) %>--%>

    <%: Html.GetAllValidators() %>

    <h2>FirstTestModel</h2>

    <% Html.EnableClientValidation(); Html.EnableUnobtrusiveJavaScript(false); %>

    <%= Html.ValidationSummary() %>

    <%using (Html.BeginForm("FirstTestModelSubmit", "test")){ %>
        <label>IsSelected:</label>
        <%= Html.CheckBoxFor(a => a.IsSelected)%>
        <%--<input type="text" value="true" name="IsSelected" />--%>
        <%= Html.ValidationMessageFor(a=>a.IsSelected) %>
        <br />
        <br />
        <label>Name:</label>
        <%= Html.TextBoxFor(a => a.Name) %>
        <%= Html.ValidationMessageFor(a=>a.Name) %>
        <br />
        <br />

        <label>Date Of Birth:</label>
        <%= Html.EditorFor(a => a.DateOfBirth, "OptionDateTime") %>
        <%= Html.ValidationMessageFor(a => a.DateOfBirth)%>
        <br />
        <br />

        <label>PickANumber:</label>
        <%= Html.TextBoxFor(a => a.PickANumber) %>
        <%= Html.ValidationMessageFor(a => a.PickANumber)%>
        <br />
        <br />

        <label>Id:</label>
        <%= Html.TextBoxFor(a => a.Id) %>
        <%= Html.ValidationMessageFor(a => a.Id)%>
        <br />
        <br />

        <label>Email:</label>
        <%= Html.TextBoxFor(a => a.Email)%>
        <%= Html.ValidationMessageFor(a => a.Email)%>
        <br />
        <br />
        
        <input type="submit" value="Submit"/>

        <br />
        <%= Html.FSharpValidation() %>

    <%} %>

</asp:Content>
