
function getRadialChartColorsArr(sectionChartDivId) {
    var colorsArr = "";
    if (document.getElementById(sectionChartDivId) != null) {
        var radialColorsStr = document.getElementById(sectionChartDivId).getAttribute("data-radialColors");
        if (radialColorsStr) {
            if (radialColorsStr.includes(","))
                colorsArr = radialColorsStr.split(",");
            else
                colorsArr = [radialColorsStr];
            return colorsArr;
        }
    }

}


function loadRadialChart(data, sectionChartDivId, sectionCountPTag) {
    var radialChartColors = getRadialChartColorsArr(sectionChartDivId);
    const sectionBookingCountPtag = document.getElementById(sectionCountPTag);
    var options = {
        fill: {
            colors: radialChartColors
        },
        series: data.series,
        
        chart: {
            height: 300,
            type: 'radialBar',
            
        },
        plotOptions: {
            radialBar: {
                hollow: {
                    size: '70%',
                }
            },
        },
        labels: data.labels,
    };

    var chart = new ApexCharts(document.getElementById(sectionChartDivId), options);
    chart.render();

    $('#bar-chart-label').html(data.labels[0]);

    //function increase or decrease
    const valuChangeLabelSpan = document.createElement('span');
    if (data.valueChangeLabel < 0) {
        sectionBookingCountPtag.innerHTML = `RevenueDecreased <i class='bi bi-arrow-down-circle' style="color:red;"></i>${data.valueChangeLabel}`;
    }
    if (data.valueChangeLabel > 0) {
        sectionBookingCountPtag.innerHTML = `Revenue Increased <i class='bi bi-arrow-up-circle' style="color:green;"></i>${data.valueChangeLabel}`;
        //green icon
    }
    // sectionBookingCountPtag.appendChild(valuChangeLabelSpan);

    //end-function
    //$('#TotalBookingCount').html(data.TotalCountLabel);

}