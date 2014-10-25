$(document).ready(function () {
    $.ajaxSetup({ cache: false });
});

// Numeric only control handler
jQuery.fn.ForceTimeOnly =
function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.charCode || e.keyCode || 0;
            // allow backspace, delete, enter, arrows, numbers and keypad numbers ONLY
            // home, end, period, and numpad decimal
            return (
                key == 8 ||
                key == 9 ||
                key == 16 ||
                key == 46 ||
                key == 185 ||
                (key >= 35 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
        });
    });
};
jQuery.fn.ForceNumberOnly =
function () {
    return this.each(function () {
        $(this).keydown(function (e) {
            var key = e.charCode || e.keyCode || 0;
            // allow backspace, delete, enter, arrows, numbers and keypad numbers ONLY
            // home, end, period, and numpad decimal
            return (
                key == 8 ||
                key == 9 ||
                key == 16 ||
                key == 46 ||
                (key >= 35 && key <= 40) ||
                (key >= 48 && key <= 57) ||
                (key >= 96 && key <= 105));
        });
    });
};

function convertMillisecondsToHuman(milliseconds) {
    if (milliseconds != null) {
        if (milliseconds <= 0)
            return "0:00";
        var returned = "";
        var x = milliseconds / 1000;
        var seconds = Math.floor(x % 60);
        x /= 60;
        var minutes = Math.floor(x % 60);
        returned = minutes;
        returned += ":";
        if (seconds < 10)
            returned += "0" + seconds;
        else
            returned += seconds;

        return returned;
    }
    return "";
}

function LoadMainScreen() {
    $.getJSON("loadMainScreen").success(function (result) {
        $("#fourbythree").attr("href", result.fourbyThree);
        $("#sixteenbynine").attr("href", result.sixteenByNine);
    }).error(function () {
    });
}

function StartJam() {
    $.getJSON("startStopJam").success(function (result) {
        if (result.result === "started")
            $("#startStopJam").val("Stop Jam");
        else if (result.result === "stopped")
            $("#startStopJam").val("Start Jam");
    }).error(function () {
    });

}
function StartOfficialTimeOut() {
    $.getJSON("startOfficialTimeOut").success(function (result) {
    }).error(function () {
    });
}
function AddToScore(teamNumber, points) {
    $.getJSON("addToScore?t=" + teamNumber + "&points=" + points).success(function (result) {
    }).error(function () {
    });
    if (teamNumber === '1')
        $("#team1Score").html(parseFloat($("#team1Score").html()) + points);
    else
        $("#team2Score").html(parseFloat($("#team2Score").html()) + points);
}
function SubtractFromScore(teamNumber) {
    $.getJSON("subtractFromScore?t=" + teamNumber).success(function (result) {

    }).error(function () {
    });
    if (teamNumber === '1')
        $("#team1Score").html(parseFloat($("#team1Score").html()) - 1);
    else
        $("#team2Score").html(parseFloat($("#team2Score").html()) - 1);
}
function TakeTimeOut(teamNumber) {
    $.getJSON("takeTimeOut?t=" + teamNumber).success(function (result) {
    }).error(function () {
    });
}

function GetTeamNames() {
    $.getJSON("GetTeamNames").success(function (result) {
        if (result.team1Name != null) {
            $("#team1Name").html(result.team1Name);
        }
        if (result.team2Name != null) {
            $("#team2Name").html(result.team2Name);
        }
    }).error(function () {
    });
}

var timerMobile = $.timer(function () {
    mobileViewModel.PullMobileUpdate();
});

timerMobile.set({ time: 500, autostart: false });

var mobileViewModel = new function () {
    var currentViewModel = this;
    //call back initiallizes a datatable so we can have pretty jquery datatables.
    this.callback = null;
    //precall helps clear a table from datatables so we can stuff it again.
    this.preCall = null;
    this.PeriodNumber = ko.observable(0);
    this.PeriodTime = ko.observable();
    this.PeriodTimeInEditMode = ko.observable();
    this.JamTime = ko.observable();
    this.JamTimeInEditMode = ko.observable();
    this.JamNumber = ko.observable();
    this.JamNumberInEditMode = ko.observable();
    this.TimeOutTime = ko.observable();
    this.LineUpTime = ko.observable();
    this.TeamName1 = ko.observable();
    this.TeamName2 = ko.observable();
    this.Team1Score = ko.observable();
    this.Team2Score = ko.observable();
    this.Team1JamScore = ko.observable();
    this.Team2JamScore = ko.observable();
    this.IsJamRunning = ko.observable();
    this.IsTimeOutRunning = ko.observable();
    this.IsPeriodRunning = ko.observable();
    this.IsEditingJamNumber = ko.observable(false);
    this.IsEditingJamTime = ko.observable(false);
    this.IsEditingPeriodTime = ko.observable(false);

    this.initialize = function (preCall, callback) {
        currentViewModel.preCall = preCall;
        currentViewModel.callback = callback;
        timerMobile.play();
    };
    this.EditJamTime = function () {
        currentViewModel.IsEditingJamTime(true);
        currentViewModel.JamTimeInEditMode(currentViewModel.JamTime());
    }
    this.EditPeriodTime = function () {
        currentViewModel.IsEditingPeriodTime(true);
        currentViewModel.PeriodTimeInEditMode(currentViewModel.PeriodTime());
    }
    this.SavePeriodTime = function () {
        $.getJSON("savePeriodTime?t=" + currentViewModel.PeriodTimeInEditMode()).success(function (result) {
        }).error(function () {
        });
        currentViewModel.PeriodTime(currentViewModel.PeriodTimeInEditMode());
        currentViewModel.IsEditingPeriodTime(false);
    }
    this.CancelEditPeriodTime = function () {
        currentViewModel.IsEditingPeriodTime(false);
    }
    this.SaveJamTime = function () {
        $.getJSON("saveJamTime?t=" + currentViewModel.JamTimeInEditMode()).success(function (result) {
        }).error(function () {
        });
        currentViewModel.JamTime(currentViewModel.JamTimeInEditMode());
        currentViewModel.IsEditingJamTime(false);
    }
    this.CancelEditJamTime = function () {
        currentViewModel.IsEditingJamTime(false);
    }
    this.EditJamNumber = function () {
        currentViewModel.IsEditingJamNumber(true);
        currentViewModel.JamNumberInEditMode(currentViewModel.JamNumber());
    }
    this.SaveJamNumber= function () {
        $.getJSON("saveJamNumber?n=" + currentViewModel.JamNumberInEditMode()).success(function (result) {
        }).error(function () {
        });
        currentViewModel.JamNumber(currentViewModel.JamNumberInEditMode());
        currentViewModel.IsEditingJamNumber(false);
    }
    this.CancelEditJamNumber= function () {
        currentViewModel.IsEditingJamNumber(false);
    }

    this.PullMobileUpdate = function () {
        $.getJSON("GrabMobileUpdate", function (result) {
            currentViewModel.PopulateMobilePage(result);
        });
    }

    this.PopulateMobilePage = function (result) {
        currentViewModel.PeriodNumber(result.PeriodNumber);
        currentViewModel.TeamName1(result.team1Name);
        currentViewModel.TeamName2(result.team2Name);
        currentViewModel.JamTime(convertMillisecondsToHuman(result.JamTime));
        currentViewModel.JamNumber(result.JamNumber);
        currentViewModel.TimeOutTime(convertMillisecondsToHuman(result.TimeOutTime));
        currentViewModel.PeriodTime(convertMillisecondsToHuman(result.PeriodTime));
        currentViewModel.LineUpTime(convertMillisecondsToHuman(result.LineUpTime));
        currentViewModel.Team1Score(result.Team1Score);
        currentViewModel.Team2Score(result.Team2Score);
        currentViewModel.Team1JamScore(result.Team1JamScore);
        currentViewModel.Team2JamScore(result.Team2JamScore);
        currentViewModel.IsJamRunning(result.IsJamRunning);
        currentViewModel.IsTimeOutRunning(result.IsTimeOutRunning);
        currentViewModel.IsPeriodRunning(result.IsPeriodRunning);
    }

    this.StartJam = function () {
        $.getJSON("startStopJam").success(function (result) {
        }).error(function () {
        });

    }

    this.StartOfficialTimeOut = function () {
        $.getJSON("startOfficialTimeOut").success(function (result) {
        }).error(function () {
        });
    }
    this.AddT1P1 = function () {
        AddToScore(1, 1);
    }
    this.AddT1P4 = function () {
        AddToScore(1, 4);
    }
    this.AddT1P5 = function () {
        AddToScore(1, 5);
    }
    this.AddT2P1 = function () {
        AddToScore(2, 1);
    }
    this.AddT2P4 = function () {
        AddToScore(2, 4);
    }
    this.AddT2P5 = function () {
        AddToScore(2, 5);
    }
    this.SubtractT1 = function () {
        SubtractFromScore(1);
    }
    this.SubtractT2 = function () {
        SubtractFromScore(2);
    }
    this.AddToScore = function (teamNumber, points) {
        $.getJSON("addToScore?t=" + teamNumber + "&points=" + points).success(function (result) {
        }).error(function () {
        });
    }
    this.SubtractFromScore = function (teamNumber) {
        $.getJSON("subtractFromScore?t=" + teamNumber).success(function (result) {
        }).error(function () {
        });
    }
    this.TakeTeam1Timeout = function () {
        TakeTimeOut(1);
    }
    this.TakeTeam2Timeout = function () {
        TakeTimeOut(2);
    }
    this.TakeTimeOut = function (teamNumber) {
        $.getJSON("takeTimeOut?t=" + teamNumber).success(function (result) {
        }).error(function () {
        });
    }

    this.GetTeamNames = function () {
        $.getJSON("GetTeamNames").success(function (result) {
            if (result.team1Name != null) {
                $("#team1Name").html(result.team1Name);
            }
            if (result.team2Name != null) {
                $("#team2Name").html(result.team2Name);
            }
        }).error(function () {
        });
    }

}