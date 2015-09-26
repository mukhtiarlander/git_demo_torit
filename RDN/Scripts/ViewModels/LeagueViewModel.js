﻿var LeagueViewModel = new function () {
    var thisViewModel = this;
    this.Leagues = ko.observableArray([]);
    this.maxId = ko.observable();
    this.pendingRequest = ko.observable(false);
    this.page = ko.observable();
    this.IsFinishedScrolling = ko.observable(false);
    this.SearchText = ko.observable();
    this.ListCountPull = ko.observable();
    var lastLeagueSearch = "";

    this.LoadLeagueList = function (count) {
        thisViewModel.ListCountPull(count);
        thisViewModel.page(0);
        getItems(thisViewModel.ListCountPull(), false);
        $(window).scroll(function () {
            if ($(window).scrollTop() >= $(document).height() - $(window).height() - 400) {
                getItems(thisViewModel.ListCountPull(), false);
            }
        });
    }

    this.LoadTweets = function (userName) {
        userName = userName.substring(userName.lastIndexOf("/") + 1, userName.length);
        $.ajax({
            type: "POST",
            url: apiUrl + "Utilities/GetTweets",
            data: { userName: userName },
            dataType: "json",
            success: function (data) {
               
            }
        });
    };
    this.SearchLeagues = function (input) {
        thisViewModel.SearchText($(input).val());
        thisViewModel.IsFinishedScrolling(false);
        thisViewModel.pendingRequest(false);
        thisViewModel.page(0);
        getItems(thisViewModel.ListCountPull(), true);
    };

    var delay = (function () {
        var timer = 0;
        return function (callback, ms) {
            clearTimeout(timer);
            timer = setTimeout(callback, ms);
        };
    })();


    function getItems(cnt, isSearch) {
        if (thisViewModel.SearchText() != null) {
            var text = thisViewModel.SearchText();
            thisViewModel.SearchText(text.trim());
        }
        if (lastLeagueSearch == thisViewModel.SearchText()) return;
        var waitTime = 0;
        if (isSearch)
            waitTime = 1500;
        delay(function () {
            if (!thisViewModel.pendingRequest() && !thisViewModel.IsFinishedScrolling()) {
                thisViewModel.pendingRequest(true);
                $.ajax({
                    type: "POST",
                    url: apiUrl + "League/GetAllLeagues",
                    data: { p: thisViewModel.page(), c: cnt, s: thisViewModel.SearchText() },
                    dataType: "json",
                    success: function (data) {
                        lastLeagueSearch = thisViewModel.SearchText();
                        if (data.leagues.length > 0) {
                            if (!isSearch) {
                                ko.utils.arrayForEach(data.leagues, function (entry) {
                                    thisViewModel.Leagues.push(entry);
                                });
                            } else {
                                thisViewModel.Leagues.removeAll();
                                thisViewModel.Leagues(data.leagues);
                            }
                            thisViewModel.page(thisViewModel.page() + 1);
                            thisViewModel.pendingRequest(false);
                        } else {
                            thisViewModel.IsFinishedScrolling(true);
                            thisViewModel.Leagues.removeAll();
                        }
                    }
                });

            }
        }, waitTime);
    }

}