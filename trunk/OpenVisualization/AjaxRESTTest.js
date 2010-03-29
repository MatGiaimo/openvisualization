$(document).ready(function() {
    var submitButton = $('#submit');

    submitButton.click(function(e) {
        // checked box
        var xmlChoice = $('.xmlChoice:checked').val();
        //var xmlToLoad = "/OpenVis/Configuration/Charts/"; //Amazon Dev
        var xmlToLoad = "/Configuration/Charts/"; // Localhost

        xmlToLoad = xmlToLoad + xmlChoice;

        $.ajax({
            type: "GET",
            url: xmlToLoad,
            dataType: "xml",
            success: function(xml) {
                loadChartImage(xml);
            }
        });
    });
});

function loadChartImage(xml) {
    var resultsDiv = $('#chartResults');

    $.ajax({
        //url: "/OpenVis/Services/GetChartImageMap.aspx", // Amazon Dev
        url: "/Services/GetChartImageMap.aspx", // Localhost
        type: "POST",
        processData: false,
        contentType: "text/xml",
        data: xml,
        success: function(data) {
            resultsDiv.html(data);
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
	        alert(XMLHttpRequest);
	        alert(textStatus);
	        alert(errorThrown);
	    }
    });
}