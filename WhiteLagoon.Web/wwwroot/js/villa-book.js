var formBtn = document.querySelector('#form-villa-book');
let CheckInDate = document.querySelector("input[name='CheckInDate']");
let CheckOutDate = document.querySelector("input[name='CheckOutDate']");
let Occupancy = document.querySelector("input[name='Occupancy']");


formBtn.addEventListener('submit', (e) => {
    e.preventDefault();
    $('.spinner').show();

    if (new Date(CheckOutDate.value) <= new Date(CheckInDate.value)) {
        alert("Invalid Dates Selection");
        $('.spinner').hide();
    } else {

        $.ajax({
            type: 'GET',
            url: '/Home/CheckAvialabilityByDate',
            data: {
                CheckInDate: CheckInDate.value,
                CheckOutDate: CheckOutDate.value,
                Occupancy: Occupancy.value
            },
            success: (data) => {
                $('.spinner').hide();
                $('#villa-showcase').empty();
                $('#villa-showcase').html(data);
                console.log(data);
            },
            error: (data) => {
                $('.spinner').hide();
                //TODO: find 
                //let data = xhr.responseJSON;
                console.log(data)
            }
        })

    }
    
});