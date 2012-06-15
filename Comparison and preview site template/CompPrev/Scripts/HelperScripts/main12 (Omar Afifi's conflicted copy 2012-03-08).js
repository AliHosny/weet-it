/// <reference path="../jquery-1.4.1-vsdoc.js" />
/// <reference path="../BlockUI.js" />
/// <reference path="ArborJSDriver.js" />

//#region GlobalVariableDefinitions
var relationVariable;
var comparisonVariable;
var comparisonElements;
//#endRegion

//// #region FunctionDefinitions

function getComparisonTableData(URIs) {
    /// <summary>A great function
    /// <Para>calls a  getComparison WEBMETHOD to get comparison table between many URIs</Para>
    /// </summary>
    /// <Para name="URIs" type="String">comma separated URIs</Para>
  
    URIs = $("#mainInputTextBox").val();
    comparisonElements = URIs.split(",");
    
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

function getJSONComparisonTable(URIs,todoFunction) {
    /// <summary>A great function
    /// <Para>calls a  getComparison WEBMETHOD to get comparison table between many URIs</Para>
    /// </summary>
    /// <Para name="URIs" type="String">Array of URIs</Para>

    console.log(URIs);
    comparisonElements = URIs; 

    $.ajax({
        type: "POST",
        url: "Default.aspx/getJSONComparisonTable",
        data: "{'URIs': '" + URIs + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            console.log(msg);
            eval("comparisonVariable =" + msg.d);
            todoFunction();
       
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
        dataType: "json",
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

function getComparisonElements() {
    /// <summary>
    /// <param>function the main Elements that the Comparison is oriented about comparison must be initialized after  </param>
    /// </summary>

    $.ajax({ type: "POST",
        url: "Default.aspx/getNextRelation",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg == "false") {
                alert("no comparison initialized");
            }
            else {
                if (msg.hasOwnProperty("d")) {
                    if (msg.d != "") {
                        eval("comparisonElements = " + msg.d);
                    }
                }
                else {
                    console.log("node returned doesn't have d property ");
                }
            }

            return comparisonElements;

        }
    });



}

function GetPreviewData(URIs) {

    /// <summary> takes 
    /// <param>calls a JSON OBJECT of the relation ship WEBMETHOD to get comparison table between many URIs</param>
    /// </summary>
    /// <param name="URIs" type="String">comma separated URIs</param>
    console.log(URIs);
    $.ajax({ type: "POST",
        url: "Default.aspx/previewObject",
        data: "{'URIs': '" + URIs + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            console.log(msg);
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

function DrawTable() {

    console.log("drawingtable");
    console.log(comparisonVariable);
    var newHTML = "";

    $(".comparisonElement").slideUp();

    for (i in comparisonVariable) {
        if (i == "__comparisonElements") {
            continue;
        }

        newHTML += "<div class=\"comparisonElement\">";
        newHTML += "<span class=\"comparisonElementTitle\">" + i + "</span>";
        newHTML += "<table class=\"comparisonTable\">";
        for (x in comparisonVariable[i]) {
            newHTML += "<th>" + x + "</th>";
            }
            newHTML += "<tr>";
            for (x in comparisonVariable[i]) {
                newHTML += "<td>";
                for (x2 in comparisonVariable[i][x]) {


                    var str = comparisonVariable[i][x][x2];
                    var pattern = /.*(\.jpg|\.jpeg|\.png|\.gif|\.bmp|\.tif|\.svg)/i;
                    if (str.match(pattern) != null ) {

                        newHTML += "<a href=\"" + str + "\" target=\"_blank\"><img class=\"imgInComparison\" src=\"" + str + "\" /> </a>"; 

                    }
                    else {
                        newHTML += str + "<br/>";

                    }

                }
                newHTML += "</td>";
            }

            newHTML += "</tr>";
            newHTML += "</table>"
            newHTML += "</div>";
        }

        $(".comparisonContainer").html(newHTML);

        $(".comparisonElement").slideDown();
}


/// #endRegion


//#region EventHandlers

function upperCompareButtonClicked(){
    var URIs = $("#mainInputTextBox").val();
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

//    var URIsArray = URIs.split(",");
//    $(".comparisonElement").slideUp();
//    $(".comparisonElement").html("<img src='../Images/loader2.gif'/>");
//    $(".comparisonElement").slideDown();
//    getJSONComparisonTable(URIs, DrawTable);
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

    var URIs = driver.getSelectedURIs();

    $(".comparisonElement").slideUp();
    $(".comparisonElement").html("<img src='../Images/loader2.gif'/>");
    $(".comparisonElement").slideDown();

    getJSONComparisonTable(URIs, DrawTable);

    return false; 
}
function upperPreviewButtonClicked() {

    var URIs = $(".mainInputTextBox").val();

    GetPreviewData(URIs);

    return false;

}

//#endRegion



var driver;
var data;
$(document).ready(function () {

    driver = new ArborDriver();
    driver.initialize("#viewport");

});