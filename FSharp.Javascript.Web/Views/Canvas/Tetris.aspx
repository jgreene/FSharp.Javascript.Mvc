<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Main.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    CanvasTest
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%: Html.FSharp().GetCompiledModule(typeof(FSharp.Javascript.Web.Tetris)) %>

<h2>Tetris</h2>

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


<div id="score">
    
</div>

<div id="images" style="display: none;">
    <img class="blue" src="/content/tetris/blue.png" alt="blue" />
    <img class="cyan" src="/content/tetris/cyan.png" alt="cyan" />
    <img class="green" src="/content/tetris/green.png" alt="green" />
    <img class="orange" src="/content/tetris/orange.png" alt="orange" />
    <img class="purple" src="/content/tetris/purple.png" alt="purple" />
    <img class="red" src="/content/tetris/red.png" alt="red" />
    <img class="yellow" src="/content/tetris/yellow.png" alt="yellow" />
</div>

<canvas id="canvas">
    <p>Your browser does not support the canvas element</p>
</canvas>
<br />
<div id="log">
    
</div>

</asp:Content>
