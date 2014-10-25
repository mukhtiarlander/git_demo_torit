function UpdateEmailAddressVerification(userId) {
    var newEmail = $("#" + userId + "-Email").val();
    $.getJSON("/Account/UpdateNonVerifiedUserEmail", { veriId: userId, email: newEmail }, function (result) {
        $("#" + userId + "-confirm").toggleClass("displayNone", false);
    });
}