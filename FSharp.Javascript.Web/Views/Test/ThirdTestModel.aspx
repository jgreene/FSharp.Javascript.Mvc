<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<Models+ThirdTestModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    FirstTestModel
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--<%: Html.GetCompiledModule(typeof(FSharp.Javascript.Web.TestModule)) %>--%>

    <h2>FirstTestModel</h2>

    <%--<% Html.EnableClientValidation(); Html.EnableUnobtrusiveJavaScript(false); %>--%>

    <%: Html.ValidationSummary() %>

    <%using (Html.BeginForm("ThirdTestModelSubmit", "test")){ %>
        <label>FirstName:</label>
        <%: Html.TextBoxFor(a => a.FirstName)%>
        <%: Html.FSharp().ValidationMessageFor(a => a.FirstName)%>
        <br />
        <br />
        <label>LastName:</label>
        <%: Html.TextBoxFor(a => a.LastName)%>
        <%: Html.FSharp().ValidationMessageFor(a => a.LastName)%>
        <br />
        <br />

       
        <label>Address1:</label>
        <%= Html.TextBoxFor(a => a.Address.Address1)%>
        <%= Html.FSharp().ValidationMessageFor(a => a.Address.Address1)%>
        <br />
        <br />

        <label>Address2:</label>
        <%= Html.TextBoxFor(a => a.Address.Address2)%>
        <%= Html.FSharp().ValidationMessageFor(a => a.Address.Address2)%>
        <br />
        <br />

            <label>Zip:</label>
        <%= Html.TextBoxFor(a => a.Address.Zip)%>
        <%= Html.FSharp().ValidationMessageFor(a => a.Address.Zip)%>
        <br />
        <br />

        <%: Html.FSharp().Validate(a => a.Address)%>

        <%: Html.FSharp().Validate()%>
        <input type="submit" value="Submit" />
    <%} %>

</asp:Content>
