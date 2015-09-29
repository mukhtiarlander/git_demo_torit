var Member = new function () {
    var thisViewModel = this;
    this.RetireSelf = function (btn) {
        if (confirm("Are You Sure?")) {
            $.ajax({
                url: '/member/retireself',
                type: 'POST',
                dataType: 'json',
                data: {},
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.success) {
                        $('.bottom-right').notify({
                            message: { text: 'Your Account has been Retired.' },
                            fadeOut: { enabled: true, delay: 3000 }
                        }).show();
                        $(btn).remove();
                        return false;
                    }
                    else {
                        alert("Something happened.  Try again later.");
                    }
                }
            });
            return false;
        }
    };

    this.RetireYourProfile = function (btn) {
            $(btn).remove();
            $.ajax({
                url: '/member/retireself',
                type: 'POST',
                dataType: 'json',
                data: {},
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.success) {
                        $('.bottom-right').notify({
                            message: { text: 'Your Account has been Retired.' },
                            fadeOut: { enabled: true, delay: 3000 }
                        }).show();                       
                        return false;
                    }
                    else {
                        alert("Something happened.  Try again later.");
                    }
                }
            });
            return false;        
    };

    this.UnRetireSelf = function (btn) {
        if (confirm("Are You Sure?")) {
            $.ajax({
                url: '/member/unretireself',
                type: 'POST',
                dataType: 'json',
                data: {},
                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    if (data.success) {
                        $('.bottom-right').notify({
                            message: { text: 'Your Account has been UnRetired.' },
                            fadeOut: { enabled: true, delay: 3000 }
                        }).show();
                        $(btn).remove();
                        return false;
                    }
                    else {
                        alert("Something happened.  Try again later.");
                    }
                }
            });
            return false;
        }
    };
    this.RemoveSelfFromLeague = function (span, memberId, leagueId) {
        if (confirm("Remove Your Self Entirely From the League?")) {
            $.getJSON("/member/removememberfromleague", { memberId: memberId, leagueId: leagueId }, function (result) {
                if (result.isSuccess === true) {
                    $(span).parent().html("Removed From League");
                }
            }).error(function () {
            });
        }
    };
    this.BuildLeagueReport = function (link, leagueId) {

        $.getJSON("/member/removememberfromleague", { memberId: memberId, leagueId: leagueId }, function (result) {
            if (result.isSuccess === true) {
                $(span).parent().html("Removed From League");
            }
        }).error(function () {
        });
    };
    this.SetPrivacySetting = function (checkbox, privacySetting) {

        $.getJSON("/member/setprivacysetting", { setting: privacySetting }, function (result) {
            if (result.isSuccess === true) {

            }
        }).error(function () {
        });
    };
    this.changeForumMessageOrderSetting = function (cb) {
        var isChecked = $(cb).is(":checked");
        $.getJSON("/member/ChangeForumMessageOrderSetting", { checkedUnCheck: isChecked }, function (result) {
            if (result.isSuccess === true) {
                $('.bottom-right').notify({
                    message: { text: 'Saved! ' },
                    fadeOut: { enabled: true, delay: 3000 }
                }).show();
            }
            else {
            }
        });
    }
};