//actual league model
var MemberDisplay = function (firstName, lastName, derbyName, playerNumber, team, email, phoneNumber, gender, id, MadeClassRank) {
    this.firstName = ko.observable(firstName).extend({ required: { message: 'Required' } });
    this.lastName = ko.observable(lastName);
    this.derbyName = ko.observable(derbyName).extend({ required: { message: 'Required' } });
    this.playerNumber = ko.observable(playerNumber);
    this.team = ko.observable(team).extend({ required: { message: 'Required' } });
    this.email = ko.observable(email).extend({ required: { message: 'Email Required' } });
    this.phoneNumber = ko.observable(phoneNumber);
    this.gender = ko.observable(gender);
    this.id = ko.observable(id);
    this.MadeClassRank = ko.observable(MadeClassRank);
}
var MemberDisplayForLeague = function (firstName, lastName, derbyName, playerNumber, email, phoneNumber, gender, id) {
    this.firstName = ko.observable(firstName).extend({ required: { message: 'Required' } });
    this.lastName = ko.observable(lastName);
    this.derbyName = ko.observable(derbyName).extend({ required: { message: 'Required' } });
    this.playerNumber = ko.observable(playerNumber);
    this.email = ko.observable(email).extend({ required: { message: 'Email Required' } });
    this.phoneNumber = ko.observable(phoneNumber);
    this.gender = ko.observable(gender);
    this.id = ko.observable(id);
}
var LeagueNames = ko.observableArray();
var genderList = ko.observableArray([
        { name: "Male", type: "1" },
    { name: "Female", type: "2" }
]);
//class rank for the made federation.
var classRank = ko.observableArray([
        { name: "I", type: "1" },
    { name: "II", type: "2" },
    { name: "III", type: "3" }
]);

//view model contains logic for the member
function MembersViewModel() {
    var self = this;

    self.members = ko.observableArray();

    self.addMember = function () {
        var memberTemp = new MemberDisplay("", "", "", "", "", "", "", "", "", "");
        memberTemp.errors = ko.validation.group(memberTemp);
        self.members.push(memberTemp);
        //alert("boom");
        $("#addedSuccessfully").toggleClass("displayNone", true);
    };

    self.addMemberForLeague = function () {
        var memberTemp = new MemberDisplayForLeague("", "", "", "", "", "", "", "");
        memberTemp.errors = ko.validation.group(memberTemp);
        self.members.push(memberTemp);
        //alert("boom");
        $("#addedSuccessfully").toggleClass("displayNone", true);
    };

    self.removeAll = function () {
        self.members.removeAll();
    }

    self.removeMember = function (MemberDisplay) {
        self.members.remove(MemberDisplay);
    }

    self.show_messages = function () {
        self.errors.showAllMessages();
        ko.utils.arrayForEach(self.members(), function (memberTemp) {
            memberTemp.errors.showAllMessages();
        });
    };

    self.issValid = function () {
        var valid = true;
        ko.utils.arrayForEach(self.members(), function (memberTemp) {
            if (memberTemp.errors().length > 0)
                valid = false;
        });
        return valid;
    };

    self.saveMembers = function () {
        if (self.issValid()) {
            $("#savingMembers").toggleClass("displayNone", false);
            $.ajax("/Federation/AddMembers", {
                data: ko.toJSON(self.members),
                type: "post",
                contentType: 'application/json',
                dataType: 'json',
                success: function (response) {
                    if (response.result === "true") {
                        $("#addedSuccessfully").toggleClass("displayNone", false);
                        $("#savingMembers").toggleClass("displayNone", true);
                        self.removeAll();
                    }
                }
            });
        }
        else {
            self.show_messages();
        }
    };
    self.saveMembersForLeague = function () {
        if (self.issValid()) {
            $("#savingMembers").toggleClass("displayNone", false);
            $.ajax("/League/AddMembers", {
                data: ko.toJSON(self.members),
                type: "post",
                contentType: 'application/json',
                dataType: 'json',
                success: function (response) {
                    if (response.result === "true") {
                        $("#addedSuccessfully").toggleClass("displayNone", false);
                        $("#savingMembers").toggleClass("displayNone", true);

                        self.removeAll();
                    }
                }
            });
        }
        else {
            self.show_messages();
        }
    }
}




