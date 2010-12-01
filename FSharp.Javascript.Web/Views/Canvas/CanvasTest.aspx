<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CanvasTest
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%: Html.FSharp().GetCompiledModule(typeof(FSharp.Javascript.Web.CanvasTests)) %>

<h2>CanvasTest</h2>

<style>  
body {  
margin:0px;  
padding:0px;  
text-align:center;  
}  
  
canvas{  
outline:0;  
border:1px solid #000;  
margin-left: auto;  
margin-right: auto;  
}  
</style>  

<canvas id="canvas">
    <p>Your browser does not support the canvas element</p>
</canvas>

</asp:Content>
