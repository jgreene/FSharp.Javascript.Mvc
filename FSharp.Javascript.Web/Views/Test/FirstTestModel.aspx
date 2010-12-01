<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<Models+FirstTestModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    FirstTestModel
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--<%: Html.GetCompiledModule(typeof(FSharp.Javascript.Web.TestModule)) %>--%>

    <h2>FirstTestModel</h2>

    <%--<% Html.EnableClientValidation(); Html.EnableUnobtrusiveJavaScript(false); %>--%>

    <%: Html.ValidationSummary() %>

    <%using (Html.BeginForm("FirstTestModelSubmit", "test")){ %>
        <div class="formItem">
            <div class="formElement">
                <label>IsSelected:</label>
            
                <%= Html.CheckBoxFor(a => a.IsSelected)%>
            </div>
            <%= Html.FSharp().ValidationMessageFor(a => a.IsSelected)%>
        </div>
        <br />

        <label>Name:</label>
        <%= Html.TextBoxFor(a => a.Name) %>
        <%= Html.FSharp().ValidationMessageFor(a => a.Name)%>
        <br />
        <br />

        <label>Date Of Birth:</label>
        <%= Html.EditorFor(a => a.DateOfBirth, "OptionDateTime") %>
        <%= Html.FSharp().ValidationMessageFor(a => a.DateOfBirth)%>
        <br />
        <br />

        <label>PickANumber:</label>
        <%= Html.TextBoxFor(a => a.PickANumber) %>
        <%= Html.FSharp().ValidationMessageFor(a => a.PickANumber)%>
        <br />
        <br />

        <label>Id:</label>
        <%= Html.TextBoxFor(a => a.Id) %>
        <%= Html.FSharp().ValidationMessageFor(a => a.Id)%>
        <br />
        <br />

        <label>Email:</label>
        <%= Html.TextBoxFor(a => a.Email)%>
        <%= Html.FSharp().ValidationMessageFor(a => a.Email)%>
        <br />
        <br />
        
        <input type="submit" value="Submit"/>

        <br />
        <%= Html.FSharp().Validate() %>

    <%} %>

</asp:Content>
