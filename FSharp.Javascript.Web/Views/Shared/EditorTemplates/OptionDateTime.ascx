<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Microsoft.FSharp.Core.FSharpOption<System.DateTime>>" %>

<%= Html.TextBox("", Microsoft.FSharp.Core.FSharpOption<DateTime>.get_IsSome(Model) ? Model.Value.ToShortDateString() : "") %>