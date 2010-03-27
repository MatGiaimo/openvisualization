$(document).ready(function() {
    var submitButton = $('#submit');

    submitButton.click(function(e) {
        // checked box
        var xmlChoice = $('.xmlChoice:checked').val();
        var xmlToLoad = "/Configuration/Charts/";

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
        //url: "/Services/GetStaticChartImage.aspx",
        url: "/Services/GetChartImageMap.aspx",
        type: "POST",
        processData: false,
        contentType: "text/xml",
        data: xml,
        success: function(data) {
            resultsDiv.html(data);
        }
    });
}