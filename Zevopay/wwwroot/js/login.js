$(document).ready(function () {

    var isUser2FAuthenticated = false;

    $('#sign_in').on('click', function (e) {
        var current = $(this);
        e.preventDefault();
        showLoader();
        var email = $('#EmailInput').val();
        var password = $('#PasswordInput').val();
        var formData = {
            Email: email,
            Password: password
        };
        if (email == '' || email == ' ') {
            toastr.error('please enter email!');
            hideLoader();
        } else if (password == '' || password == ' ') {
            toastr.error('please enter password!');
            hideLoader();
        } else {

            $.ajax({
                url: "/Account/CheckCredentials",
                type: "post",
                data: formData,
                success: function (d) {
                    if (d.resultFlag == 0) {
                        hideLoader();
                        toastr.error(d.message);
                    } else if (d.resultFlag == 1) {
                        hideLoader();
                        $('#login_element').hide();
                        $('#code_div').show();
                        $('#sign_in').hide();
                        $('#sign_in').next().show();
                        isUser2FAuthenticated = true;
                    } else {
                        hideLoader();
                        $('#login_div').hide();
                        $('#authenticater_div_with_qr').show();
                        isUser2FAuthenticated = false;
                        $('#qr_code').attr('src', d.data.barcodeImageUrl);
                    }
                },
                error: function (error) {
                    hideLoader();
                    toastr.error('internal server error please try again later!');
                }
            });
        }
    })

    $(document).on('click', '.varify_code', function (e) {
        e.preventDefault();
        showLoader();
        var email = $('#EmailInput').val();
        var password = $('#PasswordInput').val();
        var twoFCode;
        if (isUser2FAuthenticated) {
            twoFCode = $(this).parents('.main-form').find('.authenticator_code').val();
        } else {
            twoFCode = $(this).parent().find('.authenticator_code').val();
        }

        var formData = {
            Email: email,
            Password: password,
            AuthenticatorCode: twoFCode
        };

        if (twoFCode == '') {
            toastr.error('please enter code!');
            hideLoader();
        } else {
            $.ajax({
                url: "/Account/Login",
                type: "post",
                data: formData,
                success: function (d) {
                    if (d.resultFlag == 0) {
                        hideLoader();
                        toastr.error(d.message);
                    } else if (d.resultFlag == 1){
                        hideLoader();
                        toastr.success(d.message);
                        window.location.href="/Home/Index"
                    }
                },
                error: function (error) {
                    hideLoader();
                    toastr.error('internal server error please try again later!');
                }
            });
        }
    })
});