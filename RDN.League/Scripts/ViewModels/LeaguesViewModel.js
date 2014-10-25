//actual league model
var League = function (leagueName, contactEmail, contactPhone, city, state, country) {
    this.leagueName = ko.observable(leagueName).extend({ required: { message: 'Required' } });
    this.contactEmail = ko.observable(contactEmail).extend({ required: { message: 'Email Required' } });
    this.contactPhone = ko.observable(contactPhone);
    this.city = ko.observable(city);
    this.state = ko.observable(state);
    this.country = ko.observable(country).extend({ required: { message: 'Required' } });
}

//view model contains logic for the league
function LeaguesViewModel() {
    var self = this;

    self.league = ko.observableArray().extend({ leagueName: true });

    self.addLeague = function () {
        var leagTemp = new League("", "", "", "", "", "");
        leagTemp.errors = ko.validation.group(leagTemp);
        self.league.push(leagTemp);
        $("#addedSuccessfully").toggleClass("displayNone", true);
    };

    self.removeAll = function () {
        self.league.removeAll();
    }

    self.removeLeague = function (League) {
        self.league.remove(League);
    }

    self.show_messages = function () {
        self.errors.showAllMessages();
        ko.utils.arrayForEach(self.league(), function (leagueTemp) {
            leagueTemp.errors.showAllMessages();
        });
    };

    self.issValid = function () {
        var valid = true;
        ko.utils.arrayForEach(self.league(), function (leagueTemp) {
            if (leagueTemp.errors().length > 0)
                valid = false;
        });
        return valid;
    };

    self.saveLeagues = function () {
        if (self.issValid()) {
            $("#addLeagues").toggleClass("displayNone", false);
            $.ajax("/League/Adds", {
                data: ko.toJSON(self.league),
                type: "post",
                contentType: 'application/json',
                dataType: 'json',
                success: function (response) {
                    if (response.result === "true") {
                        $("#addedSuccessfully").toggleClass("displayNone", false);
                        self.removeAll();
                    }
                    $("#addLeagues").toggleClass("displayNone", true);
                }
            });
        }
        else {
            self.show_messages();
        }
    }
}




