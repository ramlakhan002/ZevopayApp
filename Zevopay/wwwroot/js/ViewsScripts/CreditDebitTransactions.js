$(document).ready(function () {
    fillTransactionData();
});
function fillTransactionData() {
    $.ajax({
        url: "/Admin/AdminCreditDebitTransactionsPartial",
        type: "get",
        success: function (d) {
            $('#Transaction-Div').html('');
            $('#Transaction-Div').html(d);
        },
        error: function (error) {
            toastr.error('Failed to get walletTransaction List!');
        }
    });
}