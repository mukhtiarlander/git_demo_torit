var EventViewModel = new function () {
    var thisViewModel = this;
    //Data
    this.eventId = ko.observable("");
    this.chats = ko.observableArray([]);
    this.CountDown = ko.observable("00:00:00:00");

    this.InitiateEventCountdown = function (seconds) {
        var myCounter = new Countdown({
            seconds: seconds,  // number of seconds to count down
            onUpdateStatus: function (sec) {
                var time = secondsToTime(sec);
                thisViewModel.CountDown(time.d + ":" + time.h + ":" + time.m + ":" + time.s);

            }, // callback for each second
            onCounterEnd: function () { } // final action
        });

        myCounter.start();
    }

    this.getConversationTimer = $.timer(function () {
        getConversationFirst(thisViewModel.eventId);
    });

    //initialize
    this.initialize = function (eventIdd, callback) {
        thisViewModel.eventId = eventIdd;
        thisViewModel.getConversationTimer.set({ time: 30000, autostart: true });
        getConversationFirst(eventIdd);
    };

    //posts the conversation to the server.
    this.postConversation = function (idd, conType) {
        var chat = $("#chat").val();
        if (chat.length > 2) {
            $.getJSON("/utilities/PostConversation", { id: thisViewModel.eventId, chat: chat, convoType: "Event" });
            thisViewModel.chats.unshift(new ChatItem(chat, "Anonymous", 0, ""));
            $("#chat").val("");
        }
        $("#chat").focus();
    }

    function getConversationFirst(eventIdd) {
        $.getJSON("/utilities/GetConversation/" + eventIdd + "?convoType=Event", function (result) {
            thisViewModel.chats.removeAll();
            $(result.convo).each(function (index) {
                thisViewModel.chats(result.convo);
            });
        });
    }
}
