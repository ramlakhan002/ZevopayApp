
$(document).ready(function () {
    $("#Package_Submit_btn").on("click", function (e) {

        var packageName = $("#PackageName").val();
        var packageId = $("#PackageId").val();

        var formData = {
            PackageName: packageName,
            PackageId:packageId
        }

        if (packageName == '') {
            toastr.error('please enter packageName!');

        } else {
            $.ajax({
                url: "/Admin/SavePackage",
                type: "post",
                data: formData,
                success: function (result) {
                    if (result.resultFlag == 1) {
                        toastr.success(result.message);

                        setTimeout(pageChange, 1000)
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



    pageChange = function () {
        window.location.href = "/Admin/PackagesList";
    };
})