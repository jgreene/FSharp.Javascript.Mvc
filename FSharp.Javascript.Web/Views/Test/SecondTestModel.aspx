<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<Models+SecondTestModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SecondTestModel
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>SecondTestModel</h2>

    <%--<% Html.EnableClientValidation(); Html.EnableUnobtrusiveJavaScript(false); %>--%>

    <%: Html.ValidationSummary() %>

    <%using (Html.BeginForm("SecondTestModelSubmit", "test")){ %>
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

        <%--<%for(int i = 0; i< Model.Addresses.Length; i++){ %>
            <label>Address1:</label>
            <%= Html.TextBoxFor(a => a.Addresses[i].Address1)%>
            <%= Html.FSharp().ValidationMessageFor(a => a.Addresses[i].Address1)%>
            <br />
            <br />

            <label>Address2:</label>
            <%= Html.TextBoxFor(a => a.Addresses[i].Address2)%>
            <%= Html.FSharp().ValidationMessageFor(a => a.Addresses[i].Address2)%>
            <br />
            <br />

             <label>Zip:</label>
            <%= Html.TextBoxFor(a => a.Addresses[i].Zip)%>
            <%= Html.FSharp().ValidationMessageFor(a => a.Addresses[i].Zip)%>
            <br />
            <br />

            <%: Html.FSharp().Validate(a=>a.Addresses[i]) %>
        <%} %>--%>

        <%: Html.FSharp().Validate()%>
        <input type="submit" value="Submit" />
    <%} %>

</asp:Content>
