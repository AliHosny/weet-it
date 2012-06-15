//include all files : 

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


//example : 

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
