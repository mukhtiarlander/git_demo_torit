

//picture for the game.
function Picture(picture) {
    this.url = ko.observable(picture.ImageUrl);
    this.primary = ko.observable(picture.IsPrimaryPhoto);
    this.alt = ko.observable(picture.alt);
}

function Block(b) {
    this.id = ko.observable(b.BlockId);
    this.period = ko.observable(b.Period);
    this.jam = ko.observable(b.JamNumber);
    this.skaterId = ko.observable(b.PlayerWhoBlocked.SkaterId);
}
function Assist(a) {
    this.id = ko.observable(a.AssistId);
    this.period = ko.observable(a.Period);
    this.jam = ko.observable(a.JamNumber);
    this.skaterId = ko.observable(a.PlayerWhoAssisted.SkaterId);
}

function Penalty(p) {
    this.id = ko.observable(p.PenaltyId);
    this.period = ko.observable(p.Period);
    this.jam = ko.observable(p.JamNumber);
    this.skaterId = ko.observable(p.PenaltyAgainstMember.SkaterId);
    this.penaltyType = ko.observable(p.PenaltyTypeAbbre);
}

function Skater(s) {
    this.id = ko.observable(s.SkaterId);
    this.linkId = ko.observable(s.SkaterLinkId);
    this.name = ko.observable(s.SkaterName);
    this.skaterUrl = ko.observable();
    if (s.SkaterLinkId != nullSkater)
        this.skaterUrl = ko.observable(skaterUrl + s.SkaterName.replace(/ /g, "-") + "/" + s.SkaterLinkId.replace(/-/g, ""));
    this.skaterNumber = ko.observable(s.SkaterNumber);
    this.pictureLocation = ko.observable(s.SkaterPictureLocation);
    this.Assists = ko.observableArray([]);
    this.Blocks = ko.observableArray([]);
    this.Penalties = ko.observableArray([]);

}

function Jam(j) {
    this.jamNumber = ko.observable(j.JamNumber);
    this.period = ko.observable(j.CurrentPeriod);
    var time = convertMillisecondsToHuman(j.JamClock.TimeRemaining);
    this.calledTime = ko.observable(time);
    this.t1Points = ko.observable(j.TotalPointsForJamT1);
    this.t2Points = ko.observable(j.TotalPointsForJamT2);

    //team 1
    this.b1t1Url = ko.observable();
    if (j.Blocker1T1 != null) {
        this.b1t1Name = ko.observable(j.Blocker1T1.SkaterName);
        if (j.Blocker1T1.SkaterLinkId != nullSkater)
            this.b1t1Url = ko.observable(skaterUrl + j.Blocker1T1.SkaterName.replace(/ /g, "-") + "/" + j.Blocker1T1.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.b1t1Name = ko.observable("");

    this.b2t1Url = ko.observable();
    if (j.Blocker2T1 != null) {
        this.b2t1Name = ko.observable(j.Blocker2T1.SkaterName);
        if (j.Blocker2T1.SkaterLinkId != nullSkater)
            this.b2t1Url = ko.observable(skaterUrl + j.Blocker2T1.SkaterName.replace(/ /g, "-") + "/" + j.Blocker2T1.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.b2t1Name = ko.observable("");

    this.b3t1Url = ko.observable();
    if (j.Blocker3T1 != null) {
        this.b3t1Name = ko.observable(j.Blocker3T1.SkaterName);
        if (j.Blocker3T1.SkaterLinkId != nullSkater)
            this.b3t1Url = ko.observable(skaterUrl + j.Blocker3T1.SkaterName.replace(/ /g, "-") + "/" + j.Blocker3T1.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.b3t1Name = ko.observable("");

    this.p1t1Url = ko.observable();
    if (j.PivotT1 != null) {
        this.p1t1Name = ko.observable(j.PivotT1.SkaterName);
        if (j.PivotT1.SkaterLinkId != nullSkater)
            this.p1t1Url = ko.observable(skaterUrl + j.PivotT1.SkaterName.replace(/ /g, "-") + "/" + j.PivotT1.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.p1t1Name = ko.observable("");

    this.j1t1Url = ko.observable();
    if (j.JammerT1 != null) {
        this.j1t1Name = ko.observable(j.JammerT1.SkaterName);
        if (j.JammerT1.SkaterLinkId != nullSkater)
            this.j1t1Url = ko.observable(skaterUrl + j.JammerT1.SkaterName.replace(/ /g, "-") + "/" + j.JammerT1.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.j1t1Name = ko.observable("");

    //team 2
    this.b1t2Url = ko.observable();
    if (j.Blocker1T2 != null) {
        this.b1t2Name = ko.observable(j.Blocker1T2.SkaterName);
        if (j.Blocker1T2.SkaterLinkId != nullSkater)
            this.b1t2Url = ko.observable(skaterUrl + j.Blocker1T2.SkaterName.replace(/ /g, "-") + "/" + j.Blocker1T2.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.b1t2Name = ko.observable("");

    this.b2t2Url = ko.observable();
    if (j.Blocker2T2 != null) {
        this.b2t2Name = ko.observable(j.Blocker2T2.SkaterName);
        if (j.Blocker2T2.SkaterLinkId != nullSkater)
            this.b2t2Url = ko.observable(skaterUrl + j.Blocker2T2.SkaterName.replace(/ /g, "-") + "/" + j.Blocker2T2.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.b2t2Name = ko.observable("");

    this.b3t2Url = ko.observable();
    if (j.Blocker3T2 != null) {
        this.b3t2Name = ko.observable(j.Blocker3T2.SkaterName);
        if (j.Blocker3T2.SkaterLinkId != nullSkater)
            this.b3t2Url = ko.observable(skaterUrl + j.Blocker3T2.SkaterName.replace(/ /g, "-") + "/" + j.Blocker3T2.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.b3t2Name = ko.observable("");

    this.p1t2Url = ko.observable();
    if (j.PivotT2 != null) {
        this.p1t2Name = ko.observable(j.PivotT2.SkaterName);
        if (j.PivotT2.SkaterLinkId != nullSkater)
            this.p1t2Url = ko.observable(skaterUrl + j.PivotT2.SkaterName.replace(/ /g, "-") + "/" + j.PivotT2.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.p1t2Name = ko.observable("");

    this.j1t2Url = ko.observable();
    if (j.JammerT2 != null) {
        this.j1t2Name = ko.observable(j.JammerT2.SkaterName);
        if (j.JammerT2.SkaterLinkId != nullSkater)
            this.j1t2Url = ko.observable(skaterUrl + j.JammerT2.SkaterName.replace(/ /g, "-") + "/" + j.JammerT2.SkaterLinkId.replace(/-/g, ""));
    }
    else
        this.j1t2Name = ko.observable("");

}

function convertMillisecondsToHuman(milliseconds) {
    var time = new Date(milliseconds);
    var returned = "";
    if (time.getMinutes() < 10)
        returned = "0" + time.getMinutes();
    else
        returned = time.getMinutes();

    returned += ":";

    if (time.getSeconds() < 10)
        returned += "0" + time.getSeconds();
    else
        returned += time.getSeconds();
    return returned;
}

function Clock(data) {
    this.timeRemaining = ko.observable(data.isDone);
}


var gameViewModel = new function () {
    var thisViewModel = this;
    //Data
    this.gameId = ko.observable("");
    this.gameName = ko.observable("");
    this.gameDate = ko.observable("");
    this.gameLocation = ko.observable("");
    this.federationId = ko.observable("");
    this.federationName = ko.observable("");
    this.hasGameStarted = ko.observable("");
    this.hasGameEnded = ko.observable("");

    this.team1PicUrl = ko.observable("");
    this.team1Name = ko.observable("");
    this.team1Id = ko.observable("");
    this.team1Url = ko.observable("");
    this.team1Score = ko.observable("");

    this.team2PicUrl = ko.observable("");
    this.team2Name = ko.observable("");
    this.team2Id = ko.observable("");
    this.team2Url = ko.observable("");
    this.team2Score = ko.observable("");

    this.periodClock = ko.observable();
    this.jamClock = ko.observable();

    this.jams = ko.observableArray([]);
    this.BlocksForTeam1 = ko.observableArray([]);
    this.BlocksForTeam2 = ko.observableArray([]);
    this.AssistsForTeam1 = ko.observableArray([]);
    this.AssistsForTeam2 = ko.observableArray([]);
    this.PenaltiesForTeam1 = ko.observableArray([]);
    this.PenaltiesForTeam2 = ko.observableArray([]);
    this.Team1Members = ko.observableArray([]);
    this.Team2Members = ko.observableArray([]);
    this.pictures = ko.observableArray([]);
    this.chats = ko.observableArray([]);

    this.timer = $.timer(function () {
        if (thisViewModel.hasGameEnded != "true")
            getGame(thisViewModel.gameId);
    });

    this.getConversationTimer = $.timer(function () {
        if (thisViewModel.hasGameEnded != "true")
            getConversationFirst(thisViewModel.gameId);
    });

    //initialize
    this.initialize = function (gameIdd, callback) {
        thisViewModel.gameId = gameIdd;

        thisViewModel.timer.set({ time: 25000, autostart: true });
        thisViewModel.getConversationTimer.set({ time: 5000, autostart: true });

        getGame(gameIdd);
        getPictures(gameIdd);
        getConversationFirst(gameIdd);
    };

    this.wall = $.timer(function () {
        CurrentlyViewingPaywall();
    });

    //posts the conversation to the server.
    this.postConversation = function () {
        var chat = $("#chat").val();
        if (chat.length > 2) {
            $.getJSON("/utilities/PostConversation", { id: thisViewModel.gameId, chat: chat, convoType: "Game" });
            thisViewModel.chats.unshift(new Chat(chat, "Anonymous", 0, ""));
            $("#chat").val("");
        }
        $("#chat").focus();
    }

    function getPictures(gameIdd) {
        $.getJSON("/game/picturesOfGame/" + gameIdd, function (result) {
            $(result.pictures).each(function (index) {
                thisViewModel.pictures.push(new Picture(this));
            });
        });
    }

    function getConversationFirst(gameIdd) {
        $.getJSON("/utilities/GetConversation/" + gameIdd + "?convoType=Game", function (result) {
            thisViewModel.chats.removeAll();
            $(result.convo.reverse()).each(function (index) {
                thisViewModel.chats.push(new Chat(this.Chat, this.MemberName, this.Id, this.Time));
            });
        });
    }

    //private methods
    function getGame(gameIdd) {
        $.getJSON(urlForLiveGame + gameIdd + "?callback=?", function (result) {
            thisViewModel.team1Score(result.game.CurrentTeam1Score);
            thisViewModel.team2Score(result.game.CurrentTeam2Score);
            thisViewModel.team1Name(result.game.Team1.TeamName);
            thisViewModel.team2Name(result.game.Team2.TeamName);
            if (result.game.Team1.TeamLinkId != "00000000-0000-0000-0000-000000000000")
                thisViewModel.team1Id(result.game.Team1.TeamLinkId);
            if (result.game.Team2.TeamLinkId != "00000000-0000-0000-0000-000000000000")
                thisViewModel.team2Id(result.game.Team2.TeamLinkId);

            thisViewModel.gameName(result.game.GameName);
            if (result.game.GameLocation != null)
                thisViewModel.gameLocation(" - " + result.game.GameLocation);
            if (result.game.HasGameEnded)
                thisViewModel.hasGameEnded("Game Is Over");
            if (result.game.HasGameStarted)
                thisViewModel.hasGameStarted("Started: ");

            thisViewModel.team1PicUrl(result.game.Team1.Logo.ImageUrl);
            thisViewModel.team2PicUrl(result.game.Team2.Logo.ImageUrl);

            thisViewModel.jams.removeAll();

            $(result.game.Jams.reverse()).each(function (index) {
                thisViewModel.jams.push(new Jam(this));
            });

            thisViewModel.Team1Members.removeAll();
            $(result.game.Team1.TeamMembers).each(function (index) {
                thisViewModel.Team1Members.push(new Skater(this));
            });
            thisViewModel.Team2Members.removeAll();
            $(result.game.Team2.TeamMembers).each(function (index) {
                thisViewModel.Team2Members.push(new Skater(this));
            });


            thisViewModel.BlocksForTeam1.removeAll();
            $(result.game.BlocksForTeam1).each(function (index) {
                //alert("block");
                for (var j = 0; j < thisViewModel.Team1Members().length; j++) {
                    if (this.PlayerWhoBlocked.SkaterId === thisViewModel.Team1Members()[j].id()) {
                        thisViewModel.Team1Members()[j].Blocks.push(new Block(this));
                        break;
                    }
                }
                thisViewModel.BlocksForTeam1.push(new Block(this));
            });
            thisViewModel.BlocksForTeam2.removeAll();
            $(result.game.BlocksForTeam2).each(function (index) {
                //alert("block");
                for (var j = 0; j < thisViewModel.Team2Members().length; j++) {
                    if (this.PlayerWhoBlocked.SkaterId === thisViewModel.Team2Members()[j].id()) {
                        thisViewModel.Team2Members()[j].Blocks.push(new Block(this));
                        break;
                    }
                }
                thisViewModel.BlocksForTeam2.push(new Block(this));
            });
            thisViewModel.AssistsForTeam1.removeAll();
            $(result.game.AssistsForTeam1).each(function (index) {
                // alert("ass");
                for (var j = 0; j < thisViewModel.Team1Members().length; j++) {
                    if (this.PlayerWhoAssisted.SkaterId === thisViewModel.Team1Members()[j].id()) {
                        thisViewModel.Team1Members()[j].Assists.push(new Assist(this));
                        break;
                    }
                }
                thisViewModel.AssistsForTeam1.push(new Assist(this));
            });
            thisViewModel.AssistsForTeam2.removeAll();
            $(result.game.AssistsForTeam2).each(function (index) {
                //alert("ass");
                for (var j = 0; j < thisViewModel.Team2Members().length; j++) {
                    if (this.PlayerWhoAssisted.SkaterId === thisViewModel.Team2Members()[j].id()) {
                        thisViewModel.Team2Members()[j].Assists.push(new Assist(this));
                        break;
                    }
                }
                thisViewModel.AssistsForTeam2.push(new Assist(this));
            });
            thisViewModel.PenaltiesForTeam1.removeAll();
            $(result.game.PenaltiesForTeam1).each(function (index) {
                //  alert("pen");
                for (var j = 0; j < thisViewModel.Team1Members().length; j++) {
                    if (this.PenaltyAgainstMember.SkaterId === thisViewModel.Team1Members()[j].id()) {
                        thisViewModel.Team1Members()[j].Penalties.push(new Penalty(this));
                        break;
                    }
                }
                thisViewModel.PenaltiesForTeam1.push(new Penalty(this));
            });
            thisViewModel.PenaltiesForTeam2.removeAll();
            $(result.game.PenaltiesForTeam2).each(function (index) {
                //  alert("pen");
                for (var j = 0; j < thisViewModel.Team2Members().length; j++) {
                    if (this.PenaltyAgainstMember.SkaterId === thisViewModel.Team2Members()[j].id()) {
                        thisViewModel.Team2Members()[j].Penalties.push(new Penalty(this));
                        break;
                    }
                }
                thisViewModel.PenaltiesForTeam2.push(new Penalty(this));
            });
        });
    }
}

function CurrentlyViewingPaywall() {
    var id = $("#Paywall_PaywallId").val();
    var pp = $("#Paywall_PasswordForPaywall").val();
    $.getJSON("/Utilities/CurrentlyViewingPaywall", { pId: id, p: pp });
}

