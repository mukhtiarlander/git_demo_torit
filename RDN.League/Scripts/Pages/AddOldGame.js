//Adding Old Games //
var GameModel = function (team1Members, team2Members) {
    var membersTeam1 = ko.observable(team1Members);
    var membersTeam2 = ko.observable(team2Members);
}

function GamePageModel() {

    var self = this;

    self.Game = ko.observable();

    self.MembersTeam1 = ko.observable();
    self.MembersTeam2 = ko.observable();

    self.newGame = function () {
        var ga = new GameModel(new MembersViewModel(), new MembersViewModel());
        ga.errors = ko.validation.group(ga);
        self.Game = ga;

        self.MembersTeam1 = new MembersViewModel();
        ko.applyBindings(self.MembersTeam1);
        self.MembersTeam2 = new MembersViewModel();
        ko.applyBindings(self.MembersTeam2);

    };

}


//loads the game page.
function loadAddGamePage() {




    $('#ddlTeam1').change(function () {
        getMembersInLeague($('#ddlTeam1').val(), "team1");
    });

    $('#ddlTeam2').change(function () {
        getMembersInLeague($('#ddlTeam2').val(), "team2");
    });

    var getMembersInLeague = function (leagueId, team) {
        $.ajax({
            url: '/League/GetMembersOfLeague?leagueId=' + leagueId,
            type: 'GET',
            success: function (data) {
                $.each(data, function (key, value) {
                    var mem = new MemberDisplay("", "", value.SkaterName, "", "", "", "", "", value.SkaterId);
                    if (team === "team1")
                        gameDisplay.MembersTeam1.addMember(mem);
                    else
                        gameDisplay.membersTeam2.addMember(mem);

                });
            }
        });
    }
}

