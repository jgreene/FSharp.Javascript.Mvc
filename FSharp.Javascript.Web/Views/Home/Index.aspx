<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <fieldset>
        <legend>Forms</legend>

        <ul>
            <li>
                <a href="/test/FirstTestModel">First Test Model</a>
            </li>
            <li>
                <a href="/test/FirstTestModelArray">First Test Model Array</a>
            </li>
        </ul>
    </fieldset>

</asp:Content>
