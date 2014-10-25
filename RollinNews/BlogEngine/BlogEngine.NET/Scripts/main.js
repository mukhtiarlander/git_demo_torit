function openCommentForumDialog(textarea) {
    $("#dialog").dialog("open");
}

function moveScroller(anchorId, scrollerId) {
    $("#" + scrollerId).css({ display: "none", position: "relative", top: "" });
    var move = function () {
        var position = $('.sharingDiv').position();
        var s = $("#" + scrollerId);
        if ($(window).width() > 1128) {
            var st = $(window).scrollTop();
            var ot = $("#" + anchorId).offset().top;

            if (st > ot) {
                s.css({ display: "block", position: "fixed", top: "10px", left: (position.left - 63) + "px" });
            } else {
                if (st <= ot) {
                    s.css({ display: "none", position: "relative", top: "" });
                }
            }
        } else { s.css({ display: "none", position: "relative", top: "" }); }
    };
    $(window).scroll(move);
    move();
}