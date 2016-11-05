function AdjustWindowHeight() {
    var requiredHeight = $(window).height() - 54 - $("footer").height() - $("nav").height();
    $('.container').css('min-height', requiredHeight);
}

$(window).on("resize", AdjustWindowHeight);

$(function () {
    AdjustWindowHeight();
});