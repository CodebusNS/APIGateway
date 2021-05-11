/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/jquery-cookie/jquery.cookie.js" />
$(function () {
    let path = $.cookie("path");
    if (path == undefined || path == null) {
        $("div [path=home]").addClass("item_focus");
    }
    else {
        $("div [path=" + path + "]").addClass("item_focus");
    }

    $(".item").click(function () {
        let path = $(this).attr("path");
        $.cookie("path", path);
        window.location.href = path;
    });
})