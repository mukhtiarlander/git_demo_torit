var Dues = new function () {
    var thisViewModel = this;
    this.RemoveWaivedFee = function (span, memId) {
        memId = $.trim(memId);
        var collected = $("#" + memId + "-Collected");
        var duesManagementId = $("#DuesId").val();
        var DuesItemId = $("#DuesItemId").val();
        var forumId = $("#ForumId").val();
        $.getJSON("/Dues/RemoveDuesWaived", { duesId: DuesItemId, duesManagementId: duesManagementId, memberId: memId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
            $.getJSON("/Dues/RemoveDuesWaived", { duesId: DuesItemId, duesManagementId: duesManagementId, memberId: memId }, function (result) {
                if (result.isSuccess === true) {
                } else {
                }
            })
        });
        collected.text("");
        $(span).remove();
    }
}