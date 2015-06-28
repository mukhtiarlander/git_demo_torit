var Tournament = new function () {
    var thisViewModel = this;
    var teamCountAdding = 0;
    var tournamentId;
    thisViewModel.teams = ko.observableArray();
    this.GetTeamsOfGame = function (dropDown, pairingId) {
        var id = $(dropDown).find("option:selected").val();
        var team1 = $("#TeamId-0-" + pairingId);
        var team2 = $("#TeamId-1-" + pairingId);
        $.getJSON("/game/GetTeamsOfGame", { gameId: id }, function (result) {
            if (result.isSuccess === true) {
                team1.empty();
                team2.empty();
                $(result.teams).each(function () {
                    $("<option />", {
                        val: this.TeamId,
                        text: this.Name
                    }).appendTo(team1);

                    $("<option />", {
                        val: this.TeamId,
                        text: this.Name
                    }).appendTo(team2);
                });
                var score1 = $("#ScoreId-0-" + pairingId);
                var score2 = $("#ScoreId-1-" + pairingId);
                score1.val(result.teams[0].Score);
                score2.val(result.teams[0].Score);
            }
        }).error(function () {
        });
    }
    this.GetScoreOfTeamForGame = function (dropDown, pairingId, teamNumber) {
        var score = $("#ScoreId-" + teamNumber + "-" + pairingId);
        var gameId = $("#GameId-" + pairingId).find("option:selected").val();
        var id = $(dropDown).find("option:selected").val();
        $.getJSON("/game/GetTeamScoreOfGame", { gameId: gameId, teamId: id }, function (result) {
            if (result.isSuccess === true) {
                score.val(result.score);

            }
        }).error(function () {
        });
    }
    this.SavePairingOfTournament = function (link, pId) {
        $("#savePairingCheck-" + pId).toggleClass("displayNone", true);
        $("#savePairing-" + pId).toggleClass("displayNone", false);
        var tn = $("#trackNumber-" + pId);
        var tt = $("#trackTime-" + pId);
        var s1 = $("#ScoreId-0-" + pId);
        var s2 = $("#ScoreId-1-" + pId);
        var gId = $("#GameId-" + pId).find("option:selected");
        var t1 = $("#TeamId-0-" + pId).find("option:selected");
        var t2 = $("#TeamId-1-" + pId).find("option:selected");
        $.getJSON("/tournament/SavePairingOfTournament", { tournId: tournamentId, pairingId: pId, gameId: gId.val(), team1Id: t1.val(), team2Id: t2.val(), team1Score: s1.val(), team2Score: s2.val(), trackNumber: tn.val(), trackTime: tt.val() }, function (result) {
            if (result.isSuccess === true) {
                $("#savePairingCheck-" + pId).toggleClass("displayNone", false);
                $("#savePairing-" + pId).toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#savePairing-" + pId).toggleClass("displayNone", true);
        });

    }
    this.PublishBrackets = function (button) {
        $.getJSON("/tournament/PublishTournamentBrackets", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                $("#publishTournament").toggleClass("displayNone", false);
                $(button).remove();
            }
        }).error(function () {
            $("#publishTournament").toggleClass("displayNone", true);
        });
    }
    this.PublishTournament = function (button) {
        $.getJSON("/tournament/PublishTournament", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                $("#publishTournament").toggleClass("displayNone", false);
                $(button).remove();
            }
        }).error(function () {
            $("#publishTournament").toggleClass("displayNone", true);
        });
    }
    this.StartNextRound = function (button) {
        $("#loadingRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/StartNextRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingRounds").toggleClass("displayNone", true);
        });
    }
    this.RollBackRound = function (button) {
        $("#loadingRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/RollBackRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingRounds").toggleClass("displayNone", true);
        });
    }
    this.StartNextPerformanceRound = function (button) {
        $("#loadingPerformanceRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/StartNextPerformanceRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingPerformanceRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingPerformanceRounds").toggleClass("displayNone", true);
        });
    }
    this.RollBackPerformanceRound = function (button) {
        $("#loadingPerformanceRounds").toggleClass("displayNone", false);
        $.getJSON("/tournament/RollBackPerformanceRound", { tournId: tournamentId }, function (result) {
            if (result.isSuccess === true) {
                window.location = window.location;
                $("#loadingPerformanceRounds").toggleClass("displayNone", true);
            }
        }).error(function () {
            $("#loadingPerformanceRounds").toggleClass("displayNone", true);
        });
    }

    this.SetTournamentId = function (id) {
        tournamentId = id;
    }

    this.AddTeamName = function (span) {
        var teamName = $("#teamName0");
        var seedRating = $("#seedRating0");
        var poolNumber = $("#pool0");
        if (teamName.val().length === 0) {
            teamName.toggleClass("error", true);
            return;
        }
        else
            teamName.toggleClass("error", false);
        if (seedRating.val().length === 0) {
            seedRating.toggleClass("error", true);
            return;
        }
        else
            seedRating.toggleClass("error", false);
        if (parseInt(seedRating.val()) > 100) {
            seedRating.val("100");
            $("#seedWarning").text("Must be <= 100");
            return;
        }
        else { $("#seedWarning").text(""); }
        $("#loadingTeams").toggleClass("displayNone", false);
        $("#successTeams").toggleClass("displayNone", true);
        $.getJSON("/tournament/SaveTeamToTournament", { tournId: tournamentId, team: teamName.val(), rating: seedRating.val(), pool: poolNumber.val() }, function (result) {
            if (result.isSuccess === true) {
                $("#TeamsTable tbody").append("<tr><td class='bracketTeamTd center'><span id='teamName" + teamCountAdding + "' name='teamName" + teamCountAdding + "'  >" + teamName.val() + "</span></td><td class='center'>" + seedRating.val() + "</td><td class='center'>" + poolNumber.val() + "</td><td> <span onclick=\"Tournament.RemoveTeamName(this, '" + result.id + "')\" class='spanLink'>Remove</span></td></tr>");
                $("#loadingTeams").toggleClass("displayNone", true);
                $("#successTeams").toggleClass("displayNone", false);
                teamName.val("");
                seedRating.val("100");
                teamName.focus();
            }
        }).error(function () {
        });
    }
    this.RemoveTeamName = function (span, teamId) {
        $(span).parent().parent().remove();
        $.getJSON("/tournament/RemoveTeamFromTournament", { tournId: tournamentId, teamId: teamId }, function (result) {
            if (result.isSuccess === true) {

            }
        }).error(function () {
        });
    }
    this.SaveBracketTeams = function () {
        $("#loadingTeams").toggleClass("displayNone", false);
        $("#successTeams").toggleClass("displayNone", true);
        thisViewModel.teams.push($("#teamName0").val());
        $.getJSON("/tournament/SaveTeamsToTournament", { tournId: tournamentId, teams: ko.toJSON(thisViewModel.teams) }, function (result) {
            if (result.isSuccess === true) {
                $("#loadingTeams").toggleClass("displayNone", true);
                $("#successTeams").toggleClass("displayNone", false);
                window.location = window.location;
            }
        }).error(function () {
        });

    }
}