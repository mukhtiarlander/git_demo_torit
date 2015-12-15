var tournamentViewModel = new function () {
    var thisViewModel = this;
    //Data
    this.tournamentId = ko.observable("");
    this.tournamentName = ko.observable("");
    this.pictures = ko.observableArray([]);
    this.chats = ko.observableArray([]);
    $("#countdown-text").html("00:00:00:00");
    //this.CountDown = ko.observable("00:00:00:00");

    this.InitiateTournamentCountdown = function (seconds) {
        var myCounter = new Countdown({
            seconds: seconds,  // number of seconds to count down
            onUpdateStatus: function (sec) {
                var time = secondsToTime(sec);
                console.dir(time);
                //thisViewModel.CountDown(time.d + ":" + time.h + ":" + time.m + ":" + time.s);
                $("#countdown-text").html(time.d + ":" + time.h + ":" + time.m + ":" + time.s);

            }, // callback for each second
            onCounterEnd: function () { } // final action
        });

        myCounter.start();
    }

    this.getConversationTimer = $.timer(function () {
        getConversationFirst(thisViewModel.tournamentId);
    });


    this.wall = $.timer(function () {
        CurrentlyViewingPaywall();
    });

    //initialize
    this.initialize = function (tournamentIdd, callback) {
        thisViewModel.tournamentId = tournamentIdd;
        thisViewModel.getConversationTimer.set({ time: 5000, autostart: true });
        getConversationFirst(tournamentIdd);
    };

    //posts the conversation to the server.
    this.postConversation = function (idd, conType) {
        var chat = $("#chat").val();
        if (chat.length > 2) {
            $.getJSON("/utilities/PostConversation", { id: thisViewModel.tournamentId, chat: chat, convoType: "Tournament" });
            thisViewModel.chats.unshift(new Chat(chat, "Anonymous", 0, ""));
            $("#chat").val("");
        }
        $("#chat").focus();
    }

    function getConversationFirst(tournamentIdd) {
        $.getJSON("/utilities/GetConversation/" + tournamentIdd + "?convoType=Tournament", function (result) {
            thisViewModel.chats.removeAll();
            $(result.convo.reverse()).each(function (index) {
                thisViewModel.chats.push(new Chat(this.Chat, this.MemberName, this.Id, this.Time));
            });
        });
    }
}