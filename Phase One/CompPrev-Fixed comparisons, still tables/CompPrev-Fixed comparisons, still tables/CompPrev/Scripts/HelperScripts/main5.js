/// <reference path="../jquery-1.4.1-vsdoc.js" />
/// <reference path="../BlockUI.js" />
/// <reference path="ArborJSDriver.js" />

//#region GlobalVariableDefinitions
var relationVariable;
var comparisonVariable;

//#endRegion

//// #region FunctionDefinitions

function getComparisonTable(URIs) {
    /// <summary>A great function
    /// <Para>calls a  getComparison WEBMETHOD to get comparison table between many URIs</Para>
    /// </summary>
    /// <Para name="URIs" type="String">comma separated URIs</Para>
    
    
    $.ajax({
        type: "POST",
        url: "Default.aspx/getComparisonTable",
        data: "{'URIs': '" + URIs + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            console.log(msg);
            $(".resultbox").html(msg.d);
        }
    });


}

function getRelations(URIs) {
    /// <summary>A great function
    /// <para>calls a JSON OBJECT of the relation ship WEBMETHOD to get comparison table between many URIs</para>
    /// </summary>
    /// <param name="URIs" type="String">comma separated URIs</param>
    /// <param name="URIs" type="callbackFunction">call back function to be excuted after the ajax request</param>
    console.log(URIs);
    $.ajax({ type: "POST",
        url: "Default.aspx/getRelations",
        data: "{'URIs':'" + URIs + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "JSON",
        success: function (msg) {
            console.log(msg);
            if (msg.hasOwnProperty("d")) {
                eval("relationVariable =" + msg.d);
                driver.addNodes(relationVariable);
                

            }

            $(".viewBox").unblock();
            
        }
    });

}

function getNextRelation() {
    /// <summary>A great function
    /// <param>calls a JSON OBJECT of the relation ship WEBMETHOD to get comparison table between many URIs</param>
    /// </summary>
    /// <param name="URIs" type="String">comma separated URIs</param>

    $.ajax({ type: "POST",
        url: "Default.aspx/getNextRelation",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg == "false") {
                alert("no more relations");
                // to be handled
            }
            else {
                if (msg.hasOwnProperty("d")) {
                    eval("relationVariable = " + msg.d);
                    driver.addNodes(relationVariable);
                }
                else {

                    console.log("node returned doesn't have d property ");
                }
            }

           $(".viewBox").unblock();

        }
    });



}

/// #endRegion



//#region EventHandlers

function upperCompareButtonClicked(){
    var URIs = $("#mainInputTextBox").val();

    $.ajax({
        type: "POST",
        url: "Default.aspx/TrimString",
        data: "{'URIs': '" + URIs + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            console.log(msg);
            URIs = msg;
        }
    });
    console.log(URIs);
    $(".viewBox").block({
        message: "<img src='../Images/loader5.gif' /><br/><b>LOADING...</b>",
        overlayCSS: {
            backgroundColor: '#2a5d7c',
            opacity: 0.6
        },
        css: {
            padding: 0,
            margin: 0,
            width: '30%',
            top: '40%',
            left: '35%',
            textAlign: 'center',
            color: '#ffffff',
            border: '0px solid',
            backgroundColor: '',
            cursor: 'wait'
        }
    })
    getRelations(URIs);
    return false;
}
function moreButtonClicked() {
    $(".viewBox").block({
        message: "<img src='../Images/loader5.gif' /><br/><b>LOADING...</b>",
        overlayCSS: {
            backgroundColor: '#2a5d7c',
            opacity: 0.6
        },
        css: {
            padding: 0,
            margin: 0,
            width: '30%',
            top: '40%',
            left: '35%',
            textAlign: 'center',
            color: '#ffffff',
            border: '0px solid',
            backgroundColor: '',
            cursor: 'wait'
        }
    })
    getNextRelation();
    return false;
}
function lowerCompareButtonClicked() {

    var URIsobject = driver.getSelectedURIs();
    var URIs = "";
    var x = 1; 
    for (objname in URIsobject) {
        URIs += URIsobject[i] + ",";
        x++;
    }
    URIs = URIs.slice(0, URIs.length-1);

    console.log(URIs);

    getComparisonTable(URIs);

    return false; 
}

//#endRegion



var driver;
var data;
$(document).ready(function () {

    //data = [{ danodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "22-rdf-syntax-ns#type": { "uri": "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "shape": "rectangle", "color": "blue", "label": "22-rdf-syntax-ns#type" }, "owl#DatatypeProperty": { "uri": "http://www.w3.org/2002/07/owl#DatatypeProperty", "shape": "dot", "color": "blue", "label": "owl#DatatypeProperty" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "22-rdf-syntax-ns#type": {} }, "22-rdf-syntax-ns#type": { "owl#DatatypeProperty": {} }, "owl#DatatypeProperty": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "22-rdf-syntax-ns#type": { "uri": "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", "shape": "rectangle", "color": "blue", "label": "22-rdf-syntax-ns#type" }, "owl#DatatypeProperty": { "uri": "http://www.w3.org/2002/07/owl#DatatypeProperty", "shape": "dot", "color": "blue", "label": "owl#DatatypeProperty" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "22-rdf-syntax-ns#type": {} }, "22-rdf-syntax-ns#type": { "owl#DatatypeProperty": {} }, "owl#DatatypeProperty": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#label": { "uri": "http://www.w3.org/2000/01/rdf-schema#label", "shape": "rectangle", "color": "blue", "label": "rdf-schema#label" }, "objectif@fr": { "uri": "objectif@fr", "shape": "dot", "color": "blue", "label": "objectif@fr" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#label": {} }, "rdf-schema#label": { "objectif@fr": {} }, "objectif@fr": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#label": { "uri": "http://www.w3.org/2000/01/rdf-schema#label", "shape": "rectangle", "color": "blue", "label": "rdf-schema#label" }, "objectif@fr": { "uri": "objectif@fr", "shape": "dot", "color": "blue", "label": "objectif@fr" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#label": {} }, "rdf-schema#label": { "objectif@fr": {} }, "objectif@fr": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#label": { "uri": "http://www.w3.org/2000/01/rdf-schema#label", "shape": "rectangle", "color": "blue", "label": "rdf-schema#label" }, "purpose@en": { "uri": "purpose@en", "shape": "dot", "color": "blue", "label": "purpose@en" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#label": {} }, "rdf-schema#label": { "purpose@en": {} }, "purpose@en": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#label": { "uri": "http://www.w3.org/2000/01/rdf-schema#label", "shape": "rectangle", "color": "blue", "label": "rdf-schema#label" }, "purpose@en": { "uri": "purpose@en", "shape": "dot", "color": "blue", "label": "purpose@en" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#label": {} }, "rdf-schema#label": { "purpose@en": {} }, "purpose@en": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#domain": { "uri": "http://www.w3.org/2000/01/rdf-schema#domain", "shape": "rectangle", "color": "blue", "label": "rdf-schema#domain" }, "Organisation": { "uri": "http://dbpedia.org/ontology/Organisation", "shape": "dot", "color": "blue", "label": "Organisation" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#domain": {} }, "rdf-schema#domain": { "Organisation": {} }, "Organisation": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#domain": { "uri": "http://www.w3.org/2000/01/rdf-schema#domain", "shape": "rectangle", "color": "blue", "label": "rdf-schema#domain" }, "Organisation": { "uri": "http://dbpedia.org/ontology/Organisation", "shape": "dot", "color": "blue", "label": "Organisation" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#domain": {} }, "rdf-schema#domain": { "Organisation": {} }, "Organisation": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#range": { "uri": "http://www.w3.org/2000/01/rdf-schema#range", "shape": "rectangle", "color": "blue", "label": "rdf-schema#range" }, "XMLSchema#string": { "uri": "http://www.w3.org/2001/XMLSchema#string", "shape": "dot", "color": "blue", "label": "XMLSchema#string" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#range": {} }, "rdf-schema#range": { "XMLSchema#string": {} }, "XMLSchema#string": { "supplementalDraftRound": {}}} }, { nodes: { "purpose": { "uri": "http://dbpedia.org/ontology/purpose", "shape": "dot", "color": "blue", "label": "purpose" }, "rdf-schema#range": { "uri": "http://www.w3.org/2000/01/rdf-schema#range", "shape": "rectangle", "color": "blue", "label": "rdf-schema#range" }, "XMLSchema#string": { "uri": "http://www.w3.org/2001/XMLSchema#string", "shape": "dot", "color": "blue", "label": "XMLSchema#string" }, "supplementalDraftRound": { "uri": "http://dbpedia.org/ontology/supplementalDraftRound", "shape": "rectangle", "color": "blue", "label": "supplementalDraftRound"} }, edges: { "purpose": { "rdf-schema#range": {} }, "rdf-schema#range": { "XMLSchema#string": {} }, "XMLSchema#string": { "supplementalDraftRound": {}}}}]

    driver = new ArborDriver();
    driver.initialize("#viewport");
    
})