function initializeBookingPieChart() {
    debugger
    $('.chart-spinner').show();
    $.ajax({
        type: 'GET',
        url: '/Dashboard/GetCustomerBookingPieChartData',
        dataType: 'json',
        success: (data) => {
            $('.chart-spinner').hide();
            loadPieChart(data, "bookings-pie-chart");

        },
        error: (data) => {
            $('.spinner').hide();
            //TODO: find 
            //let data = xhr.responseJSON;
            console.log(data)
        }
    })

}


function loadPieChart(data, sectionChartDivId) {
    debugger
    var radialChartColors = getRadialChartColorsArr(sectionChartDivId);
    var options = {
        fill: {
            colors: radialChartColors
        },
        series: data.series,

        chart: {
            height: 300,
            type: 'pie',

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


    
   
}