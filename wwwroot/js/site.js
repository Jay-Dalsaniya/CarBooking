$(document).ready(function () {
  

    $('#btnLogout').click(function (e) {
        e.preventDefault();

        $.ajax({
            type: "POST",
            url: '/User/Logout',
            success: function (response) {
                if (response.success) {
                    window.location.href = '/User/Login';
                } else {
                    alert("Logout failed.");
                }
            },
            error: function () {
                alert("Error during logout.");
            }
        });
    });


    $(document).on('click', '.btnDeleteCar', function () {
        debugger
        var carid = $(this).data('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "This car will be deleted permanently!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/AdminCar/DeleteCar/" + carid,
                    method: 'POST',
                    success: function (response) {
                        Swal.fire('Deleted!', 'Car deleted successfully.', 'success').then(() => {
                            location.reload();
                        });
                    },
                    error: function (xhr) {
                        var errorMessage = xhr.responseText || 'Something went wrong!';
                        Swal.fire('Error!', errorMessage, 'error');
                    }
                });
            }
        });
    });

    $(document).on('click', '.btnDeleteCarBooking', function () {
        debugger
        var carBookingId = $(this).data('id');

        Swal.fire({
            title: 'Are you sure?',
            text: "This Car Booking will be deleted permanently!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/CarBooking/DeleteCarBooking/" + carBookingId,
                    method: 'POST',
                    success: function (response) {
                        Swal.fire('Deleted!', 'Car Booking deleted successfully.', 'success').then(() => {
                            location.reload();
                        });
                    },
                    error: function (xhr) {
                        var errorMessage = xhr.responseText || 'Something went wrong!';
                        Swal.fire('Error!', errorMessage, 'error');
                    }
                });
            }
        });
    });
    

    if ($("#CarCreatedMsg").val()) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: $("#CarCreatedMsg").val(),
            confirmButtonColor: '#3085d6',
        });
    }

    if ($("#CarUpdatedMsg").val()) {
        Swal.fire({
            icon: 'success',
            title: 'Updated',
            text: $("#CarUpdatedMsg").val(),
            confirmButtonColor: '#3085d6',
        });
    }

    if ($("#ErrorMsg").val()) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: $("#ErrorMsg").val(),
            confirmButtonColor: '#d33'
        });
    }

    if ($("#SuccessMsg").val()) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: $("#SuccessMsg").val(),
            confirmButtonColor: '#3085d6',
        });
    }

    if ($("#SuccessImgDet").val()) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: $("#SuccessMsg").val(),
            confirmButtonColor: '#3085d6',
        });
    }

    if ($("#RegisterSuccessfull").val()) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: $("#RegisterSuccessfull").val(),
            confirmButtonColor: '#3085d6',
        });
    }

    if ($("#Login").val()) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: $("#Login").val(),
            confirmButtonColor: '#3085d6',
        });
    }

});
