/// <reference path="../arborjs/arbor.js" />

//**************
//interaction driver is a javascript file responsible for .
//it takes the particles System and handles the nodes interaction of / selection / unselection / changecolor / get selected / 
//and also contains the handlers for different events NodeClicked , NodeHovered , Nodedoublecliked , 
//**************

(function () {

    InteractionDriver = function (system) {
        var SelectedNodes = {};
        var sys = system
        var driver = {

            SelectNode: function (node) {

                if ((node.data.label in SelectedNodes) == false) {

                    SelectedNodes[node.data.label] = { "node": node, "Originalcolor": node.data.color }
                    sys.tweenNode(SelectedNodes[node.data.label].node, 0.8, { 'color': '#FFAE00' });

                }

            }
,
            unSelectNode: function (node) {

                if (node.data.label in SelectedNodes) {

                    sys.tweenNode(SelectedNodes[node.data.label].node, 0.4, { 'color': SelectedNodes[node.data.label].Originalcolor });
                    delete SelectedNodes[node.data.label];
                }

            }
,
            SelectAllNodes: function () {

                sys.eachNode(this.SelectNode);
                return true;

            }
,
            unSelectAllNodes: function () {

                sys.eachNode(this.unSelectNode);
                return true;
            }
,
            nodeClickedHandler: function (node) {

                if (node.data.label in SelectedNodes) {
                    this.unSelectNode(node);
                }
                else {
                    this.SelectNode(node);
                }
            }
,
            nodeHoveredHandler: function (node) {


            }
,
            getSelectedNodes: function () {
                var retuned = SelectedNodes;
                for (i in retuned) {
                    delete retuned[i].Originalcolor;
                }
                return retuned;
            }
,

            getSelectedNodesURIs: function () {
                var returned = new Array();

                for (i in SelectedNodes) {

                    returned.push(SelectedNodes[i].node.data.uri);

                }
                return returned;

            }
        }
        return driver
    }


})()