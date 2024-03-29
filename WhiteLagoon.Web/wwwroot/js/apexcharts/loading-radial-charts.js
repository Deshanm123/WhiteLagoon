

function initializeLoadBookingRadialCharts() {
    $('.chart-spinner').show();
    $.ajax({
        type: 'GET',
        url: '/Dashboard/GetTotalBookingRadialChartData',
        dataType :'json',
        success: (data) => {
            $('.chart-spinner').hide();
            loadRadialChart(data, "bookings-radial-chart", 'sectionBookingCount')
            
        },
        error: (data) => {
            $('.spinner').hide();
            //TODO: find 
            //let data = xhr.responseJSON;
            console.log(data)
        }
    })

}
function initializeLoadUsersRadialCharts() {
    $('.chart-spinner').show();
    $.ajax({
        type: 'GET',
        url: '/Dashboard/GetRegisteredUsersRadialChartData',
        dataType :'json',
        success: (data) => {
            $('.chart-spinner').hide();
            loadRadialChart(data, "users-radial-chart", 'sectionUsersCount')
            
        },
        error: (data) => {
            $('.spinner').hide();
            //TODO: find 
            //let data = xhr.responseJSON;
            console.log(data)
        }
    })

}
function initializeLoadRevenueCharts() {
    $('.chart-spinner').show();
    $.ajax({
        type: 'GET',
        url: '/Dashboard/GetRevenueRadialChartData',
        dataType :'json',
        success: (data) => {
            $('.chart-spinner').hide();
            loadRadialChart(data, "revenue-radial-chart", 'sectionRevenueCount')
            
        },
        error: (data) => {
            $('.spinner').hide();
            //TODO: find 
            //let data = xhr.responseJSON;
            console.log(data)
        }
    })

}