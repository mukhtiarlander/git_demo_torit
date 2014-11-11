var LeagueViewModel = new function () {
    var thisViewModel = this;
    this.Leagues = ko.observableArray([]);
    this.maxId = ko.observable();
    this.pendingRequest = ko.observable(false);
    this.page = ko.observable();
    this.IsFinishedScrolling = ko.observable(false);
    this.SearchText = ko.observable();

    this.LoadLeagueList = function () {
        thisViewModel.page(0);
        getItems(50, false);
        $(window).scroll(function () {
            if ($(window).scrollTop() >= $(document).height() - $(window).height() - 400) {
                getItems(50, false);
            }
        });
    }

    this.SearchLeagues = function (input) {
        console.log(thisViewModel.SearchText($(input).val()));
        thisViewModel.IsFinishedScrolling(false);
        thisViewModel.pendingRequest(false);
        thisViewModel.page(0);
        getItems(50, true);
    }

    function getItems(cnt, isSearch) {
        if (!thisViewModel.pendingRequest() && !thisViewModel.IsFinishedScrolling()) {
            thisViewModel.pendingRequest(true);
            $.ajax({
                type: "POST",
                url: apiUrl + "League/GetAllLeagues",
                data: { p: thisViewModel.page(), c: cnt, s: thisViewModel.SearchText() },
                dataType: "json",
                success: function (data) {
                    if (data.leagues.length > 0) {
                        if (!isSearch) {
                            ko.utils.arrayForEach(data.leagues, function (entry) {
                                thisViewModel.Leagues.push(entry);
                            });
                        }
                        else {
                            thisViewModel.Leagues.removeAll();
                            thisViewModel.Leagues(data.leagues);
                        }
                        thisViewModel.page(thisViewModel.page() + 1);
                        thisViewModel.pendingRequest(false);
                    }
                    else
                        thisViewModel.IsFinishedScrolling(true);
                }
            });

        }
    }

}