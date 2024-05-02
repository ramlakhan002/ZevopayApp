$(document).ready(function () {
    $("#MoneyTransfer_Submit_btn").on("click", function (e) {

        var paymentMode = $("#PaymentMode").val();
        var accountNumber = $("#AccountNumber").val();
        var iFSCCode = $("#IFSCCode").val();
        var fullName = $("#FullName").val();
        var amount = $("#Amount").val();

        var formData = {
            PaymentMode: paymentMode,
            AccountNumber: accountNumber,
            IFSCCode: iFSCCode,
            FullName: fullName,
            Amount: amount

        }

        if (paymentMode == '') {
            toastr.error('please enter payment mode!');

        } else if (accountNumber == '') {
            toastr.error('please enter account number!');

        } else if (!validateAccountNumber(accountNumber)) {
            toastr.error('please enter a valid account number!');

        } else if (iFSCCode == '') {
            toastr.error('please enter IFSC code!');

        } else if (!validateIFSC(iFSCCode)) {
            toastr.error('please enter a valid IFSC code!');

        }else if (fullName == '') {
            toastr.error('please enter full name!');

        } else if (amount == '' || amount == 0) {
            toastr.error('please enter amount!');

        } else {
            $.ajax({
                url: "/Member/MoneyTransferSave",
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
    function validateIFSC(ifsc) {
        // Regular expression to validate IFSC Code
        var ifscRegex = /^[A-Z]{4}0[A-Z0-9]{6}$/;
        return ifscRegex.test(ifsc);
    }

    function validateAccountNumber(accountNumber) {
        // Regular expression to validate Account Number
        var accountRegex = /^\d{9,18}$/; // Example: 9 to 18 digits
        return accountRegex.test(accountNumber);
    }
})