<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<Models+FirstTestModel[]>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    FirstTestModelArray
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <%: Html.FSharp().GetAllValidators() %>

    <h2>FirstTestModelArray</h2>

    <% Html.EnableClientValidation(); Html.EnableUnobtrusiveJavaScript(false); %>

    <%: Html.ValidationSummary() %>

    <script type="text/javascript">
    </script>

    <%using (Html.BeginForm("FirstTestModelArraySubmit", "test")) { %>

        <% for (int i = 0; i < Model.Length; i++ ) { %>
            <label>IsSelected:</label>
            <%= Html.CheckBoxFor(a => a[i].IsSelected)%>
            <%--<input type="text" value="true" name="IsSelected" />--%>
            <%= Html.FSharp().ValidationMessageFor(a => a[i].IsSelected)%>
            <br />
            <br />
            <label>Name:</label>
            <%= Html.TextBoxFor(a => a[i].Name)%>
            <%= Html.FSharp().ValidationMessageFor(a => a[i].Name)%>
            <br />
            <br />

            <label>Date Of Birth:</label>
            <%= Html.EditorFor(a => a[i].DateOfBirth, "OptionDateTime")%>
            <%= Html.FSharp().ValidationMessageFor(a => a[i].DateOfBirth)%>
            <br />
            <br />

            <label>PickANumber:</label>
            <%= Html.TextBoxFor(a => a[i].PickANumber)%>
            <%= Html.FSharp().ValidationMessageFor(a => a[i].PickANumber)%>
            <br />
            <br />

            <label>Id:</label>
            <%= Html.TextBoxFor(a => a[i].Id)%>
            <%= Html.FSharp().ValidationMessageFor(a => a[i].Id)%>
            <br />
            <br />

            <label>Email:</label>
            <%= Html.TextBoxFor(a => a[i].Email)%>
            <%= Html.FSharp().ValidationMessageFor(a => a[i].Email)%>
            <br />
            <br />
        
            

            <br />
            <%= Html.FSharp().Validate(a => a[i])%>

            <div>------------------------------------------------------------------------</div>
        <%} %>
        <input type="submit" value="Submit"/>
    <%} %>

</asp:Content>
