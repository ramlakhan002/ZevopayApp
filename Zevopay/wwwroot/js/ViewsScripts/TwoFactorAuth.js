$(document).ready(function () {
    $('#varify_code').on('click', function () {

        var code = $('#AuthenticatorCode').val();
        var IsUserTwoFactorEnabled = false;
        var isTwoFactorEnabled = $('#IsUserTwoFactorEnabled').val();
        if (isTwoFactorEnabled == 1) {
            isUserTwoFactorEnabled = true;
        }
        if (code == '' || code == ' ') {
            toastr.error('please enter code!');

        } else {
            var formData = {
                AuthenticatorCode: code,
                IsUserTwoFactorEnabled: isUserTwoFactorEnabled
            };
            $.ajax({
                url: "/Account/TwoFactorAuth",
                type: "post",
                data: formData,
                success: function (d) {
                    if (d.resultFlag == 0) {
                        toastr.error(d.message);

                    } else {
                        toastr.success(d.message);
                        window.location.href = "/Home/Index";
                    }
                },
                error: function (error) {

                    toastr.error('Internal server error!');
                }
            });
        }
    });
})