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
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    datatable = new DataTable('#booking-data-table', {
       // resposive: true,
        // config options...
        "ajax": {
            url: `/booking/GetAllBookings?status=${status}`
        },
        "columns": [
            {data:'id' , "width":"5%"},
            {data:'name' , "width":"10%"},
            { data:'phoneNumber' , "width":"10%"},
            { data: 'email', "width": "10%" },
            { data: 'status', "width": "5%" },
            { data: 'checkInDate', "width": "20%" },
            { data: 'checkOutDate', "width": "20%" },
            { data: 'totalCost', "width": "10%" },
            {
                data: 'id',
                width:"20%",
                "render": (data) => {
                    return `<div class="btn-group">
                        <a href="/booking/bookingDetails?bookingId=${data}" class="btn btn-outline-warning mx-2">
                            <i class="bi bi-info"></i>Details
                        </a>
                    </div>`;
                }
            }
        ]
    });
 });