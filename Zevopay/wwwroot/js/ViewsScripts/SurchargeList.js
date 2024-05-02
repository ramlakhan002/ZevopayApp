$(document).ready(function () {
    fillSurchargeData();
});

function fillSurchargeData() {
    $.ajax({
        url: "/Admin/SurchargePartial",
        type: "get",
        success: function (html) {
            $('#Surcharge-Div').html('');
            $('#Surcharge-Div').html(html);
        },
        error: function (error) {
            console.log(error);
        }
    });
}