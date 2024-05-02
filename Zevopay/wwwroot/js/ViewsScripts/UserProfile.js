$(document).ready(function () {
    $('#Update_Profile').on('click', function (e) {
        e.preventDefault();
        var id = $('#Id').val();
        var fName = $('#FirstName').val();
        var lName = $('#LastName').val();
        var email = $('#Email').val();
        var address = $('#Address').val();
        var phone = $('#PhoneNumber').val();
        var userName = $('#UserName').val();

        if (fName == '') {
            toastr.error(`FirstName is required!`);
        } else if (lName == '') {
            toastr.error(`LastName is required!`);
        } else if (userName == '') {
            toastr.error(`userName is required!`);
        } else if (email == '') {
            toastr.error(`Email is required!`);
        } else if (address == '') {
            toastr.error(`Address is required!`);
        } else if (phone == '') {
            toastr.error(`Phone is required!`);
        }
        else {
            var formData = {
                FirstName: fName,
                UserName: userName,
                LastName: lName,
                Address: address,
                Email: email,
                PhoneNumber: phone,
                Id: id
            };

            $.ajax({
                url: "/Account/SubAdmin",
                type: "post",
                data: formData,
                success: function (d) {
                    if (d.message != '') {

                        if (d.resultFlag == 1) {
                            toastr.success('profile successfully updated!', 'Your fun', {
                                timeOut: 1000,
                                preventDuplicates: true,
                                onHidden: function () {
                                    window.location.href = "/Home/Index";
                                }
                            });
                        } else {

                            toastr.error('error during update profile!');
                        }
                    }
                },
                error: function (error) {
                    toastr.error('Error while adding subAdmin');
                }
            });
        }
    })
});