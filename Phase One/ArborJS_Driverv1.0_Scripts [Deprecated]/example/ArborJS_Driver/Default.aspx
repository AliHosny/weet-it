<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="ArborJS_Driver._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">

    <!-- JS libraries-->

    <script src="Scripts/jquery-1.6.1.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.4.1-vsdoc.js" type="text/javascript"></script>
    <script src="Scripts/arborjs/arbor.js" type="text/javascript"></script>
    <script src="Scripts/arborjs/arbor-tween.js" type="text/javascript"></script>
    <script src="Scripts/arborjs/graphics.js" type="text/javascript"></script>
  
    <!-- helper JS files -->
    <script src="Scripts/HelperScripts/Interaction_Driver.js" type="text/javascript"></script>
    <script src="Scripts/HelperScripts/renderer.js" type="text/javascript"></script>
    <script src="Scripts/HelperScripts/ArborJSDriver.js" type="text/javascript"></script>

</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <canvas id="viewport" width="1100" height="400" ></canvas>
    
    <script language="javascript" type="text/javascript">

//        var sys = arbor.ParticleSystem(2600, 512, 0.5);
//        sys.parameters({ gravity: true });
//        myInteractionDriver = InteractionDriver(sys);
//        sys.renderer = Renderer("#viewport", myInteractionDriver);

        var driver = ArborDriver();

        driver.initialize("#viewport"); 

        var data = {
            nodes: {
                inception: { 'color': 'green', 'shape': 'rectangle', 'label': 'inception' },
                actors: { 'color': 'blue', 'shape': 'dot', 'label': 'actors' } ,
                tomcruise: { 'color': 'red', 'shape': 'dot', 'label': 'tom cruise' } ,
                robertDeNiro: { 'color': 'red', 'shape': 'dot', 'label': 'Robert de Niro' } ,
                angelinaJoulie: { 'color': 'red', 'shape': 'dot', 'label': 'Angelina Joulie' } ,


            },
            edges: {
                inception: {actors:{}} ,
                actors : {tomcruise:{},robertDeNiro:{},angelinaJoulie:{}}
            }
        };

        driver.addNodes(data);

        


    </script>





</asp:Content>
