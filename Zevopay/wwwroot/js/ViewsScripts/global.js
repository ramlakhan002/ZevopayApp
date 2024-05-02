const pageNumber = 1;
const pageSize = 50;
showLoader = function () {
    $('body').attr('style', 'opacity: 0.5');
    $("#loader").show();

}
pageReload = function () {
    location.reload();
}
hideLoader = function () {
    $('body').removeAttr('style');
    $("#loader").hide();
}


$(document).ready(function () {
    let path = window.location.href;
    
});