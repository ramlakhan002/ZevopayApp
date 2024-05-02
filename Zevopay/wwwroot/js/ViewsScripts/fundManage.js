
$(document).ready(function () {

    $("#fundManage_btn").on("click", function (e) {

        var memberId = $("#memberId").val().split(",");
        var factor = $("#factor").val();
        var amount = $("#amount").val().trim();
        var description = $("#description").val();
        var formData = {
            MemberId: memberId[0],
            Factor: factor,
            Amount: amount,
            Description: description
        }

        if (memberId == '') {
            toastr.error('please select user!');
        } else if (factor == '') {
            toastr.error('please select factor!');
        } else if (amount == '' || amount == 0 || amount == ' ') {
            if (amount == '' || amount == ' ') {
                toastr.error('please enter amount!');
            } else {
                toastr.error('Amount should be grater then 0!');
            }
        } else if (description == '' || description == ' ') {
            toastr.error('please enter description!');
        } else {
            $.ajax({
                url: "/Admin/FundManage",
                type: "post",
                data: formData,
                success: function (result) {
                    if (result.resultFlag == 1) {
                        toastr.success(result.message);
                        setTimeout(pagrChange,1000)
                      
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

    pagrChange = function () {
        window.location.href = "/Admin/AdminCreditDebitTransactions";
    };


    $(".selectMember").change(function () {
        // Your code to execute when the selection changes
        var Id = $("#memberId").val().split(",");
        $.ajax({
            url: "/Admin/GetBalanceByUser",
            type: "post",
            data: { Id: Id[1] },
            success: function (result) {
                    $("#balance").text("Wallet Balance: " + result.balance);                     
            },
            error: function (error) {
                toastr.error("error");
            }
        });
    });
})