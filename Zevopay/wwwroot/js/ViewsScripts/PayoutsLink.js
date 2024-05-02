$(document).ready(function () {
    $("#PayoutsLink_Submit_btn").on("click", function (e) {

        var phoneNumber = $("#PhoneNumber").val();
        var email = $("#Email").val();
        var amount = $("#Amount").val();
        var formData = {
            PhoneNumber: phoneNumber,
            Email: email,
            Amount:amount
        }

        if (!phoneNumber && !email) {
            toastr.error('please enter atleast one input!');

        }
        else if (phoneNumber && !validatePhone(phoneNumber)) {
            toastr.error('please enter a valid phone number!');

        } else if (email && !validateEmail(email)) {
            toastr.error('please enter a valid email!');

        } else if (!amount) {
            toastr.error('please enter amount');

        } else if (amount && amount <= 0) {
            toastr.error('amount should be greater then 0!');

        } else {
            $.ajax({
                url: "/Member/PayoutsLinkSave",
                type: "post",
                data: formData,
                success: function (result) {
                    if (result.resultFlag == 1) {
                        toastr.success(result.message);
                    }
                    else {
                        toastr.error(result.message);
                    }

                },
                error: function (error) {
                    toastr.error("internal server error please try again later!");
                }
            });
        }
    })

    // Function to validate email
    function validateEmail(email) {
        // Regular expression to validate email
        var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Function to validate phone number
    function validatePhone(phone) {
        var phoneRegex = /^[0-9]{10}$/; // Example: 10 digit number
        return phoneRegex.test(phone);
    }
})