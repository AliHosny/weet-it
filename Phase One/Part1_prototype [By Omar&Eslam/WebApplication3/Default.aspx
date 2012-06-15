<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebApplication3._Default" validateRequest="false" EnableEventValidation="false"   %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
  
    <asp:TextBox ID="TextBox1" runat="server" Height="57px" Width="392px" CausesValidation="false"></asp:TextBox>
<p>
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Button" 
        Width="261px" CausesValidation="false" />
</p>
<div id="resultTable" runat="server" > </div>
  
</asp:Content>
