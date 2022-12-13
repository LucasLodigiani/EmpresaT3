// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
$('#open').on('click', function (e) {
    simple_showpopup("popup", e);
});

function simple_showpopup(id, evt) {
    var _pnl = $("#" + id);
    _pnl.show();
    _pnl.css({
        "left": evt.pageX - ($("#" + id).width() / 2),
        "top": (evt.pageY + 10)
    });

    $(document).on("mouseup", function (e) {
        var popup = $("#" + id);
        if (!popup.is(e.target) && popup.has(e.target).length == 0) {
            popup.hide();
            $(this).off(e);
        }
    });
}

$("#popup").hide();