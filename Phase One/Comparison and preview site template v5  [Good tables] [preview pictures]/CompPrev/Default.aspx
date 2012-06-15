<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CompPrev._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/Default8.css" rel="stylesheet" type="text/css" />
    <script runat="server" id="ViewVariable"></script>
    <!-- arborJS libraries --> 
    <script src="../Scripts/arborjs/arbor.js" type="text/javascript"></script>
    <script src="../Scripts/arborjs/arbor-tween.js" type="text/javascript"></script>
    <script src="../Scripts/arborjs/graphics.js" type="text/javascript"></script>
    <!--Other Libraries -->
    <script src="Scripts/BlockUI.js" type="text/javascript"></script>
    <!--helperfiles -->
      <script src="../Scripts/HelperScripts/Interaction_Driver5.js" type="text/javascript"></script>
      <script src="../Scripts/HelperScripts/renderer.js" type="text/javascript"></script>
      <script src="../Scripts/HelperScripts/ArborJSDriver4.js" type="text/javascript"></script>  
     <script src="../Scripts/HelperScripts/main12.js" type="text/javascript"></script> 
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="uppperBox">
        <textarea id="mainInputTextBox" class="mainInputTextBox" cols="20" rows="2"></textarea>
        <div  class="upperButtonsContainer"> 
        
        <button id="upperPreviewButton" class="upperButtons"onclick="return upperPreviewButtonClicked();">Preview</button>
        <button id="upperCompareButton" class="upperButtons"onclick="return upperCompareButtonClicked();">compare</button>
    </div>
    </div>
    
    <div class="viewBox">
        <canvas id="viewport" width="1160" height="550" ></canvas>
        <div class="Buttons">
        <button id="moreButton" onclick="return moreButtonClicked();">more</button>
        <button id="View_previewButton" onclick="return moreButtonClicked();" >preview</button>
        <button id="lowercompareButton" onclick="return lowerCompareButtonClicked()" >compare</button>
       
        </div>
        <div style="clear:both;"></div>
    </div>

    
    <div  class="comparisonContainer">
   



    <div class="comparisonElement">


    <%--<span class="comparisonElementTitle">testing Title</span>
    <table class="comparisonTable">
    <th>tesitng1</th><th>tesitng1</th>
    <tr><td><p>tesitng1t1tesitng1t1tesitng1t1<br />tesitng1t1tesitng1t1tesitng1t1tesitng1t1tesitng1t1tesitng1t1tesitng1t1tesitng1t1tesitng1t1</p></td><td>tesitng1</td></tr>
    <tr><td>tesitng1</td><td>tesitng1</td></tr>
    <tr><td>tesitng1</td><td>tesitng1</td></tr>
    <tr><td>tesitng1</td><td>tesitng1</td></tr>
    </table>
    </div>
    --%>

    </div>

</asp:Content>
