/// <reference path="../lib/jquery/dist/jquery.js" />
/// <reference path="../lib/jquery-cookie/jquery.cookie.js" />
$(function () {
    let path = window.location.pathname.split("/")[1];
    if (path == undefined || path == null || path == "") {
        $("div [path=dashboard]").addClass("item_focus");
    }
    else {
        $("div [path=" + path + "]").addClass("item_focus");
    }

    $(".item").click(function () {
        let path = $(this).attr("path");
        window.location.href = path;
    });
})