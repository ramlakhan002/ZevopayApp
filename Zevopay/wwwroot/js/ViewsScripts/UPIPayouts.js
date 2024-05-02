$(document).ready(function () {
    $("#UPIPayouts_Submit_btn").on("click", function (e) {

        var UpiId = $("#UPIId").val();
        var fullName = $("#FullName").val();
        var amount = $("#Amount").val();

        var formData = {
            UPIId: UpiId,
            FullName: fullName,
            Amount: amount

        }
        var upiRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z]+$/;
        if (UpiId == '') {
            toastr.error('please enter Upi id!');

        } else if (!validateUPI(UpiId)) {
            toastr.error('please enter a valid Upi id!');

        } else if (fullName == '') {
            toastr.error('please enter full name!');

        } else if (amount == '' || amount == 0) {
            toastr.error('please enter amount!');

        } else {
            $.ajax({
                url: "/Member/UPIPayoutsSave",
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
                    toastr.error("error");
                }
            });
        }
    })
    function validateUPI(upi) {
        // Regular expression to validate UPI ID
        var upiRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z]+$/;
        return upiRegex.test(upi);
    }
})