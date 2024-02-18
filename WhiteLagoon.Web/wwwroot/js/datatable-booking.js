var dataTable;

//let table = new DataTable('#booking-data-table', {
//     resposive: true,
//    // config options...
//    ajax: {
//        url: "/booking/GetAllBookings"
//    }
//});

$('document').ready(() =>
{
    debugger;
    datatable = new DataTable('#booking-data-table', {
       // resposive: true,
        // config options...
        "ajax": {
            url: "/booking/GetAllBookings"
        },
        "columns": [
            {data:'id' , "width":"5%"},
            {data:'name' , "width":"5%"},
            { data:'phoneNumber' , "width":"5%"},
            { data: 'email', "width": "5%" },
            { data: 'status', "width": "5%" },
            { data: 'checkInDate', "width": "5%" },
            { data: 'checkOutDate', "width": "5%" }
            //{ data: 'totalCost', "width": "5%" }
        ]
    });
 });