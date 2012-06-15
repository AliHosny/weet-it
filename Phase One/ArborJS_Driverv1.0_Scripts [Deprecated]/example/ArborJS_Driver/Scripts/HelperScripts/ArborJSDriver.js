/// <reference path="../jquery-1.4.1-vsdoc.js" />
/// <reference path="../arborjs/arbor.js" />
/// <reference path="Interaction_Driver.js" />
/// <reference path="renderer.js" />



var ArborDriver = function () {

    var Variable = {

        sys: {}
,
        InteractionHandler: {}
,
        initialize: function (canvasView) {

            this.sys = arbor.ParticleSystem({'repulsion':100,'stuffness':28000,'friction':0.5});
            this.sys.parameters({ gravity: true });
            this.InteractionHandler = InteractionDriver(this.sys);
            this.sys.renderer = Renderer("#viewport", this.InteractionHandler);

        }
,
        addNodes: function (data) {

            this.sys.graft(data);

        }
,
        getSelected: function () {
            return (this.InteractionHandler.getSelectedNodes());
        }
    }


    return Variable;

}