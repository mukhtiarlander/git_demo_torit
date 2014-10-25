$(document).ready(function () {
    $.ajaxSetup({ cache: false });
});

jQuery.fn.dataTableExt.oSort['natural-asc'] = function (a, b) {
    return naturalSort(a, b);
};

jQuery.fn.dataTableExt.oSort['natural-desc'] = function (a, b) {
    return naturalSort(a, b) * -1;
};

var currentJamNumber = 0;
var currentJamId = 0;
var row = 0;
var penRows = 0;
var addPenaltyPopup = false;
var showPenaltyPopup = false;
var temporaryPenaltyPlayerId;
var temporaryPenaltyPlayerName;
var temporaryPenaltyTeamNumber;
var temporaryPenatlyType = 1;

jQuery.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", Math.max(0, (($(window).height() - this.outerHeight()) / 2) + $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - this.outerWidth()) / 2) + $(window).scrollLeft()) + "px");
    return this;
}


function CloseAddPenalty() {
    if (addPenaltyPopup === true) {
        $("#tr-" + temporaryPenaltyPlayerId).remove();
        addPenaltyPopup = false;
    }
}

function CloseShowPenalty() {
    if (showPenaltyPopup === true) {
        $("#showpen-" + temporaryPenaltyPlayerId).remove();
        showPenaltyPopup = false;
        currentTeamViewModel.tempSkaterPenalties.removeAll();
    }
}

function PenaltyItem(penalty) {
    penRows += 1;
    this.rowNumber = ko.observable(1);
    if (penRows & 2 > 0)
        this.rowNumber = ko.observable(0);
    this.penaltyName = ko.observable(penalty.name);
    this.penaltyId = ko.observable(penalty.id);
}

function TeamMember(member, teamNumber) {
    this.rowNumber = ko.observable(1);
    if (row & 2 > 0)
        this.rowNumber = ko.observable(0);

    this.memberName = ko.observable(member.memberName);
    this.memberId = ko.observable(member.memberId);
    this.memberNumber = ko.observable(member.memberNumber);
    this.teamNumber = ko.observable(teamNumber);
    this.totalBlocks = ko.observable(member.totalBlocks);
    this.blocksForJam = ko.observable(member.blocksForJam);
    this.totalAssists = ko.observable(member.totalAssists);
    this.assistsForJam = ko.observable(member.assistsForJam);
    this.totalPenalties = ko.observable(member.totalPenalties);
    this.penaltiesForJam = ko.observable(member.penaltiesForJam);
    this.totalScores = ko.observable(member.totalScores);
    this.scoreForJam = ko.observable(member.scoreForJam);
    this.scoreForPass = ko.observable(member.scoreForPass);
    this.linedUp = ko.observable(member.linedUp);
    this.isJammer = ko.observable(member.isJammer);
    this.lostLead = ko.observable(member.lostLead);
    this.isLead = ko.observable(member.isLead);
    this.isPivot = ko.observable(member.isPivot);
    if (member.isPivot === true || member.isJammer === true)
        this.isPivotOrJammer = ko.observable(true);
    else
        this.isPivotOrJammer = ko.observable(false);
    this.isBlocker1 = ko.observable(member.isBlocker1);
    this.isBlocker2 = ko.observable(member.isBlocker2);
    this.isBlocker3 = ko.observable(member.isBlocker3);
    this.isBlocker4 = ko.observable(member.isBlocker4);
    this.isPBox = ko.observable(member.isPBox);
    this.pass = ko.observable(member.scorePass);
    this.scoreForPass = ko.observable(member.scoreForPass);
    this.SkaterPenalties = ko.observableArray([]);
}

var jamNumberTimer = $.timer(function () {
    currentTeamViewModel.PullJamNumber(false);
});
jamNumberTimer.set({ time: 4000, autostart: true });

var scoringPageTimer = $.timer(function () {
    if (currentTeamViewModel.scorers().length < 2)
        currentTeamViewModel.PullCurrentTeamMembers();
});



var currentTeamViewModel = new function () {
    var currentViewModel = this;
    //call back initiallizes a datatable so we can have pretty jquery datatables.
    this.callback = null;
    //precall helps clear a table from datatables so we can stuff it again.
    this.preCall = null;
    this.currentMembers = ko.observableArray([]);
    this.currentMembersTeam2 = ko.observableArray([]);
    this.scorers = ko.observableArray([]);
    this.teamName = ko.observable();
    this.teamId = ko.observable();
    this.team2Name = ko.observable();
    this.team2Id = ko.observable();
    this.currentJam = ko.observable('0');
    this.currentJamId = ko.observable('00000000-0000-0000-0000-000000000000');
    this.totalJams = ko.observable();
    this.teamNumber = ko.observable();
    this.team2Number = ko.observable();
    this.availablePositions = ko.observableArray(['Jammer', 'Pivot', 'Blocker1', 'Blocker2', 'Blocker3', 'Blocker4']);
    this.penaltyTypes = ko.observableArray([]);
    this.tempSkaterPenalties = ko.observableArray([]);
    this.selectedPenalty = ko.observable();
    this.passId = ko.observable();
    this.ruleSet = ko.observable();
    this.hidePenaltyScale = ko.observable();
    this.isJamOver = ko.observable();

    //initialize
    this.initialize = function (preCall, callback) {
        currentViewModel.preCall = preCall;
        currentViewModel.callback = callback;
        currentViewModel.GetAllPenalties();
        currentViewModel.PullJamNumber(true);
        currentViewModel.PullRuleSet();
    }

    this.deletePenalty = function (btn) {
        var teamNumber = 0;
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            if (currentViewModel.currentMembers()[i].memberId() === temporaryPenaltyPlayerId) {
                currentViewModel.currentMembers()[i].totalPenalties(currentViewModel.currentMembers()[i].totalPenalties() - 1);
                currentViewModel.currentMembers()[i].penaltiesForJam(currentViewModel.currentMembers()[i].penaltiesForJam() - 1);
                teamNumber = 1;
                break;
            }
        }
        if (teamNumber === 0) {
            for (var i = 0; i < currentViewModel.currentMembersTeam2().length; i++) {
                if (currentViewModel.currentMembersTeam2()[i].memberId() === temporaryPenaltyPlayerId) {
                    currentViewModel.currentMembersTeam2()[i].totalPenalties(currentViewModel.currentMembersTeam2()[i].totalPenalties() - 1);
                    currentViewModel.currentMembersTeam2()[i].penaltiesForJam(currentViewModel.currentMembersTeam2()[i].penaltiesForJam() - 1);
                    teamNumber = 2;
                    break;
                }
            }
        }

        $.getJSON("deletepenalty?penaltyId=" + $(btn).attr('id')).success(function (result) {
        }).error(function () {
        });
        CloseShowPenalty();
    }

    //shows the add penalty popup
    this.showAddPenalty = function (player) {
        if (addPenaltyPopup === false) {
            var popup = $('#popUpAddPenalty').clone();
            $("<tr id='tr-" + player.memberId() + "'><td colspan='5'>" + popup.html() + "</td></tr>").hide().insertAfter($("#" + player.memberId())).slideDown('fast');
            addPenaltyPopup = true;
            temporaryPenaltyPlayerId = player.memberId();
            temporaryPenaltyTeamNumber = player.teamNumber();
        }
    }

    function loadPenaltiesToUI(player) {
        if (showPenaltyPopup === false) {
            var popup = $('#popUpShowPenalties').clone();
            $("<tr id='showpen-" + player.memberId() + "'><td colspan='5'>" + popup.html() + "</td></tr>").hide().insertAfter($("#" + player.memberId())).slideDown('fast');
            showPenaltyPopup = true;
            temporaryPenaltyPlayerId = player.memberId();
        }
    }

    //shows the penalties for member popup
    this.showMemberPenalty = function (player) {
        currentViewModel.GetPenaltiesForMember(player, loadPenaltiesToUI);

    }

    this.setMajorPenalty = function () {
        $("#minorBtn").toggleClass("isIsSelectedButton", false);
        $("#majorBtn").toggleClass("isIsSelectedButton", true);
        temporaryPenatlyType = 2;
    }

    this.setMinorPenalty = function () {
        $("#minorBtn").toggleClass("isIsSelectedButton", true);
        $("#majorBtn").toggleClass("isIsSelectedButton", false);
        temporaryPenatlyType = 1;
    }

    this.setSelectedPenalty = function (dropdown) {
        var val = $(dropdown).find(":selected").val();
        currentViewModel.selectedPenalty(val);
    }
    this.submitPenalty = function () {
        if (currentViewModel.selectedPenalty() != null) {
            if (temporaryPenaltyTeamNumber === 1) {
                for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
                    if (currentViewModel.currentMembers()[i].memberId() === temporaryPenaltyPlayerId) {
                        currentViewModel.currentMembers()[i].totalPenalties(currentViewModel.currentMembers()[i].totalPenalties() + 1);
                        currentViewModel.currentMembers()[i].penaltiesForJam(currentViewModel.currentMembers()[i].penaltiesForJam() + 1);
                        break;
                    }
                }
            }

            if (temporaryPenaltyTeamNumber === 2) {
                for (var i = 0; i < currentViewModel.currentMembersTeam2().length; i++) {
                    if (currentViewModel.currentMembersTeam2()[i].memberId() === temporaryPenaltyPlayerId) {
                        currentViewModel.currentMembersTeam2()[i].totalPenalties(currentViewModel.currentMembersTeam2()[i].totalPenalties() + 1);
                        currentViewModel.currentMembersTeam2()[i].penaltiesForJam(currentViewModel.currentMembersTeam2()[i].penaltiesForJam() + 1);
                        break;
                    }
                }
            }

            $.getJSON("addpenalty?playerId=" + temporaryPenaltyPlayerId + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + temporaryPenaltyTeamNumber + "&pid=" + currentViewModel.selectedPenalty() + "&mm=" + temporaryPenatlyType).success(function (result) {
            }).error(function () {
            });
            CloseAddPenalty();
        }
    }

    //adds a penalty
    this.addPenalty = function (penalty) {
        console.log(penalty.penaltyId());
        if (penalty.penaltyId() != null) {
            if (temporaryPenaltyTeamNumber === 1) {
                for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
                    if (currentViewModel.currentMembers()[i].memberId() === temporaryPenaltyPlayerId) {
                        currentViewModel.currentMembers()[i].totalPenalties(currentViewModel.currentMembers()[i].totalPenalties() + 1);
                        currentViewModel.currentMembers()[i].penaltiesForJam(currentViewModel.currentMembers()[i].penaltiesForJam() + 1);
                        break;
                    }
                }
            }

            if (temporaryPenaltyTeamNumber === 2) {
                for (var i = 0; i < currentViewModel.currentMembersTeam2().length; i++) {
                    if (currentViewModel.currentMembersTeam2()[i].memberId() === temporaryPenaltyPlayerId) {
                        currentViewModel.currentMembersTeam2()[i].totalPenalties(currentViewModel.currentMembersTeam2()[i].totalPenalties() + 1);
                        currentViewModel.currentMembersTeam2()[i].penaltiesForJam(currentViewModel.currentMembersTeam2()[i].penaltiesForJam() + 1);
                        break;
                    }
                }
            }
            CloseAddPenalty();
            $.getJSON("addpenalty?playerId=" + temporaryPenaltyPlayerId + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + teamNumber + "&pid=" + penalty.penaltyId()).success(function (result) {
            }).error(function () {
            });
        }
    }
    //user clicks next Jam
    this.nextJam = function () {
        currentViewModel.PullJamNumber(true);
    }
    //adds an assist
    this.addAssist = function (player) {
        player.totalAssists(player.totalAssists() + 1);
        player.assistsForJam(player.assistsForJam() + 1);
        $.getJSON("addAssist?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
        });
    }
    //removes and assist
    this.removeAssist = function (player) {
        player.totalAssists(player.totalAssists() - 1);
        player.assistsForJam(player.assistsForJam() - 1);
        $.getJSON("removeAssist?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
        });
    }
    //adds a block
    this.addBlock = function (player) {
        player.totalBlocks(player.totalBlocks() + 1);
        player.blocksForJam(player.blocksForJam() + 1);
        $.getJSON("addBlock?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
        });
    }
    //removes a block
    this.removeBlock = function (player) {
        player.totalBlocks(player.totalBlocks() - 1);
        player.blocksForJam(player.blocksForJam() - 1);
        $.getJSON("removeBlock?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
        });
    }
    //adds a score
    this.addPass = function (player) {
        $.getJSON("addJamPass?playerId=" + player.memberId() + "&t=" + currentViewModel.teamNumber() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId()).success(function (result) {
        }).error(function () {
        });
        player.pass(player.pass() + 1);
        player.scoreForPass(0);
    }
    this.isLead = function (player) {
        $.getJSON("isLead?playerId=" + player.memberId() + "&t=" + currentViewModel.teamNumber() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId()).success(function (result) {
        }).error(function () {
        });
        player.isLead(true);
    }
    this.lostLead = function (player) {
        $.getJSON("lostLead?playerId=" + player.memberId() + "&t=" + currentViewModel.teamNumber() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId()).success(function (result) {
        }).error(function () {
        });
        player.lostLead(true);
    }
    this.addScore = function (player) {
        addPoints(player, 1);
    }
    this.addScore4 = function (player) {
        addPoints(player, 4);
    }
    this.addScore5 = function (player) {
        addPoints(player, 5);
    }
    function addPoints(player, points) {
        player.scoreForPass(player.scoreForPass() + points);
        player.totalScores(player.totalScores() + points);
        player.scoreForJam(player.scoreForJam() + points);
        $.getJSON("addScore?passId=" + currentViewModel.passId() + "&points=" + points + "&playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
        });
    }
    //removes a score
    this.removeScore = function (player) {
        player.totalScores(player.totalScores() - 1);
        player.scoreForJam(player.scoreForJam() - 1);
        player.scoreForPass(player.scoreForPass() - 1);
        $.getJSON("removeScore?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
        });
    }
    //sets blocker 1
    this.setB1 = function (player) {
        //clears all other blockers
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            currentViewModel.currentMembers()[i].isBlocker1(false);
        }
        player.isJammer(false);
        player.isPivot(false);
        player.isPivotOrJammer(false);
        player.isBlocker1(true);
        player.isBlocker2(false);
        player.isBlocker3(false);
        player.isBlocker4(false);
        player.isPBox(false);

        $.getJSON("setBlocker1?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setB1(player);
        });
    }
    //sets blocker 2 
    this.setB2 = function (player) {
        //clears all other blockers
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            currentViewModel.currentMembers()[i].isBlocker2(false);
        }
        player.isBlocker2(true);
        player.isJammer(false);
        player.isPivot(false);
        player.isPivotOrJammer(false);
        player.isBlocker1(false);
        player.isBlocker3(false);
        player.isBlocker4(false);
        player.isPBox(false);

        $.getJSON("setBlocker2?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setB2(player);
        });
    }
    //sets blocker 3
    this.setB3 = function (player) {
        //clears all blocker 3s
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            currentViewModel.currentMembers()[i].isBlocker3(false);
        }
        player.isBlocker3(true);
        player.isJammer(false);
        player.isPivot(false);
        player.isPivotOrJammer(false);
        player.isBlocker1(false);
        player.isBlocker2(false);
        player.isBlocker4(false);
        player.isPBox(false);
        $.getJSON("setBlocker3?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setB3(player);
        });
    }
    this.setB4 = function (player) {
        //clears all blocker 4s
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            currentViewModel.currentMembers()[i].isBlocker4(false);
        }
        player.isBlocker4(true);
        player.isJammer(false);
        player.isPivot(false);
        player.isPivotOrJammer(false);
        player.isBlocker1(false);
        player.isBlocker2(false);
        player.isBlocker3(false);
        player.isPBox(false);
        $.getJSON("setBlocker4?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setB4(player);
        });
    }
    //set pivot 
    this.setP1 = function (player) {
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            currentViewModel.currentMembers()[i].isPivot(false);
            if (!currentViewModel.currentMembers()[i].isJammer())
                currentViewModel.currentMembers()[i].isPivotOrJammer(false);
        }
        player.isPivot(true);
        player.isPivotOrJammer(true);
        player.isJammer(false);
        player.isBlocker1(false);
        player.isBlocker2(false);
        player.isBlocker3(false);
        player.isBlocker4(false);
        player.isPBox(false);
        $.getJSON("setPivot?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setP1(player);
        });
    }

    this.setJ1 = function (player) {
        for (var i = 0; i < currentViewModel.currentMembers().length; i++) {
            currentViewModel.currentMembers()[i].isJammer(false);
            if (!currentViewModel.currentMembers()[i].isPivot())
                currentViewModel.currentMembers()[i].isPivotOrJammer(false);
        }
        player.isJammer(true);
        player.isPivotOrJammer(true);
        player.isPivot(false);
        player.isBlocker1(false);
        player.isBlocker2(false);
        player.isBlocker3(false);
        player.isBlocker4(false);
        player.isPBox(false);
        $.getJSON("setJammer?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setJ1(player);
        });
    }
    this.setPBox = function (player) {
        player.isJammer(false);
        player.isPivot(false);
        player.isPivotOrJammer(false);
        player.isBlocker1(false);
        player.isBlocker2(false);
        player.isBlocker3(false);
        player.isBlocker4(false);
        player.isPBox(true);
        $.getJSON("setPBox?playerId=" + player.memberId() + "&jamNumber=" + currentViewModel.currentJam() + "&jamId=" + currentViewModel.currentJamId() + "&t=" + currentViewModel.teamNumber()).success(function (result) {
        }).error(function () {
            currentViewModel.setPBox(player);
        });
    }
    //score pages were loaded.
    this.scoringLoaded = function () {
        $.getJSON("scoringLoaded", function (result) {
        }).error(function () {
            currentViewModel.scoringLoaded();
        });
    }
    this.initializeScoringPage = function () {
        currentViewModel.PullJamNumber(false);
        scoringPageTimer.set({ time: 2000, autostart: true });
    }
    //gets the current jam number.
    //PullJamNumberByUser == if the user wants to manually pull jam number which could happen by clicking the NEXT Jam Button.
    this.PullJamNumber = function (PullJamNumberByUser) {
        $.getJSON("getJamNumber").success(function (result) {
            //jam number changes
            if (currentViewModel.currentJam() != result.currentJam) {
                //make sure we aren't using the line up or score page.
                if ((window.location.href.indexOf("lineup.html") === -1 && window.location.href.indexOf("score.html") === -1) || PullJamNumberByUser === true) {
                    currentViewModel.isJamOver(false);
                    addPenaltyPopup = false;
                    showPenaltyPopup = false;
                    currentViewModel.currentJam(result.currentJam);
                    currentViewModel.currentJamId(result.currentJamId);
                    currentViewModel.totalJams(result.totalJams);
                    currentViewModel.PullCurrentTeamMembers();
                }
                else //if we are using the score or linup page, the user has to click the button for next jam.
                {
                    currentViewModel.isJamOver(true);
                }
            }
        }).error(function () {
            currentViewModel.PullJamNumber(false);
        });
    }
    //gets the current jam number.
    this.PullRuleSet = function () {
        $.getJSON("getRuleSet").success(function (result) {
            //wftda rule set hides the major and minor buttons...
            if (result.ruleSet === "WFTDA") {
                currentViewModel.hidePenaltyScale(true);
                $("#minorBtn").toggleClass("hidden", true);
                $("#majorBtn").toggleClass("hidden", true);
            }
            else {
                currentViewModel.hidePenaltyScale(false);
                $("#minorBtn").toggleClass("hidden", false);
                $("#majorBtn").toggleClass("hidden", false);
            }
        }).error(function () {
        });
    }
    //gets all the penalties for the player
    this.GetPenaltiesForMember = function (player, callBack) {
        $.getJSON("getpenaltiesformember?playerId=" + player.memberId() + "&t=" + player.teamNumber()).success(function (result) {
            if (result.penalties != null) {
                currentViewModel.tempSkaterPenalties.removeAll();
                $(result.penalties).each(function (index) {
                    currentViewModel.tempSkaterPenalties.push(new PenaltyItem(this));
                });
            }
            callBack(player);
        }).error(function () {
            currentViewModel.GetPenaltiesForMember(player);
        });
    }
    //gets all the penalty information
    this.GetAllPenalties = function () {
        $.getJSON("getallpenaltytypes").success(function (result) {
            if (result.penaltyTypes != null) {
                currentViewModel.penaltyTypes.removeAll();
                $(result.penaltyTypes).each(function (index) {
                    currentViewModel.penaltyTypes.push(new PenaltyItem(this));
                });
            }
        }).error(function () {
            // alert("g");
            currentViewModel.GetAllPenalties();
        });
    }

    this.PullTeam2Members = function () {
        $.getJSON("GetTeam2Members").success(function (result) {
            if (result.teamName != null) {
                //team 2 table
                if (currentViewModel.preCall != null)
                    currentViewModel.preCall(2);
                currentViewModel.team2Name(result.teamName);
                currentViewModel.team2Id(result.teamId);
                currentViewModel.team2Number(2);
                currentViewModel.currentMembersTeam2.removeAll();
                currentViewModel.scorers.removeAll();
                $(result.members).each(function (index) {
                    currentViewModel.currentMembersTeam2.push(new TeamMember(this, 2));
                    if (this.isJammer || this.isPivot)
                        currentViewModel.scorers.push(new TeamMember(this, 2));

                    row += 1;
                });
                //team 2 table
                if (currentViewModel.callback != null)
                    currentViewModel.callback(2);
            }
        }).error(function () {
            currentViewModel.PullTeam2Members();
        });
    }

    this.PullTeam1Members = function () {
        $.getJSON("GetTeam1Members").success(function (result) {
            if (result.teamName != null) {
                //team 1 table
                if (currentViewModel.preCall != null)
                    currentViewModel.preCall(1);
                currentViewModel.teamName(result.teamName);
                currentViewModel.teamId(result.teamId);
                currentViewModel.teamNumber(1);
                currentViewModel.currentMembers.removeAll();
                currentViewModel.scorers.removeAll();
                $(result.members).each(function (index) {
                    currentViewModel.currentMembers.push(new TeamMember(this, 1));
                    if (this.isJammer || this.isPivot)
                        currentViewModel.scorers.push(new TeamMember(this, 1));

                    row += 1;
                });
                //team 1 table
                if (currentViewModel.callback != null)
                    currentViewModel.callback(1);
            }
        }).error(function () {
            currentViewModel.PullTeam1Members();
        });
    }

    //gets the current team members
    this.PullCurrentTeamMembers = function () {
        if (window.location.href.indexOf("t=1") > 0) {
            $.getJSON("GetTeam1Members").success(function (result) {
                if (result.teamName != null) {
                    if (currentViewModel.preCall != null)
                        currentViewModel.preCall();
                    currentViewModel.teamName(result.teamName);
                    currentViewModel.teamId(result.teamId);
                    currentViewModel.teamNumber(1);
                    currentViewModel.currentMembers.removeAll();
                    currentViewModel.scorers.removeAll();
                    $(result.members).each(function (index) {
                        currentViewModel.currentMembers.push(new TeamMember(this, 1));
                        if (this.isJammer || this.isPivot) {
                            currentViewModel.scorers.push(new TeamMember(this, 1));
                        }
                        row += 1;
                    });
                    if (currentViewModel.callback != null)
                        currentViewModel.callback();
                }
            });
            row = 0;
        }
        else if (window.location.href.indexOf("t=2") > 0) {
            $.getJSON("GetTeam2Members").success(function (result) {
                if (result.teamName != null) {
                    if (currentViewModel.preCall != null)
                        currentViewModel.preCall();
                    currentViewModel.teamName(result.teamName);
                    currentViewModel.teamId(result.teamId);
                    currentViewModel.teamNumber(2);
                    currentViewModel.currentMembers.removeAll();
                    currentViewModel.scorers.removeAll();
                    $(result.members).each(function (index) {
                        currentViewModel.currentMembers.push(new TeamMember(this, 2));
                        if (this.isJammer || this.isPivot)
                            currentViewModel.scorers.push(new TeamMember(this, 2));
                        row += 1;
                    });
                    if (currentViewModel.callback != null)
                        currentViewModel.callback();
                }
            }).error(function () {
                currentViewModel.PullCurrentTeamMembers();
            });
            row = 0;
        }
        else if (window.location.href.indexOf("penaltyboth") > 0) {

            currentViewModel.PullTeam1Members();
            currentViewModel.PullTeam2Members();

            row = 0;
        }
    }
}


var announcerViewModel = new function () {
    var currentViewModel = this;
    //call back initiallizes a datatable so we can have pretty jquery datatables.
    this.callback = null;
    //precall helps clear a table from datatables so we can stuff it again.
    this.preCall = null;
    this.team1Name = ko.observable();
    this.team2Name = ko.observable();
    this.JammerT1 = ko.observable(new MemberForAnnouncerJson());
    this.PivotT1 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker1T1 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker2T1 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker3T1 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker4T1 = ko.observable(new MemberForAnnouncerJson());
    this.JammerT2 = ko.observable(new MemberForAnnouncerJson());
    this.PivotT2 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker1T2 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker2T2 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker3T2 = ko.observable(new MemberForAnnouncerJson());
    this.Blocker4T2 = ko.observable(new MemberForAnnouncerJson());
    this.currentJam = ko.observable();
    this.currentJamId = ko.observable();
    this.totalJams = ko.observable();
    this.PlayerStatsT1 = ko.observableArray([]);
    this.PlayerStatsT2 = ko.observableArray([]);

    this.initialize = function (preCall, callback) {
        currentViewModel.preCall = preCall;
        currentViewModel.callback = callback;
        currentViewModel.pullAnnouncerPage();
    }

    this.pullAnnouncerPage = function () {
        $.getJSON("fullAnnouncerPage").success(function (result) {
            if (currentViewModel.preCall != null)
                currentViewModel.preCall();
            currentViewModel.team1Name(result.team1Name);
            currentViewModel.team2Name(result.team2Name);
            currentViewModel.totalJams(result.totalJams);
            currentViewModel.currentJam(result.currentJam);
            currentViewModel.currentJamId(result.currentJamId);
            currentViewModel.JammerT1(new MemberForAnnouncerJson(result.JammerT1));
            currentViewModel.PivotT1(new MemberForAnnouncerJson(result.PivotT1));
            currentViewModel.Blocker1T1(new MemberForAnnouncerJson(result.Blocker1T1));
            currentViewModel.Blocker2T1(new MemberForAnnouncerJson(result.Blocker2T1));
            currentViewModel.Blocker3T1(new MemberForAnnouncerJson(result.Blocker3T1));
            currentViewModel.Blocker4T1(new MemberForAnnouncerJson(result.Blocker4T1));
            currentViewModel.JammerT2(new MemberForAnnouncerJson(result.JammerT2));
            currentViewModel.PivotT2(new MemberForAnnouncerJson(result.PivotT2));
            currentViewModel.Blocker1T2(new MemberForAnnouncerJson(result.Blocker1T2));
            currentViewModel.Blocker2T2(new MemberForAnnouncerJson(result.Blocker2T2));
            currentViewModel.Blocker3T2(new MemberForAnnouncerJson(result.Blocker3T2));
            currentViewModel.Blocker4T2(new MemberForAnnouncerJson(result.Blocker4T2));

            currentViewModel.PlayerStatsT1.removeAll();
            currentViewModel.PlayerStatsT2.removeAll();
            $(result.PlayerStatsT2).each(function (index) {
                currentViewModel.PlayerStatsT2.push(new AnnouncerPlayerStat(this));
                row += 1;
            });
            $(result.PlayerStatsT1).each(function (index) {
                currentViewModel.PlayerStatsT1.push(new AnnouncerPlayerStat(this));
                row += 1;
            });
            if (currentViewModel.callback != null)
                currentViewModel.callback();

        }).error(function () {
        });

    }
}

function MemberForAnnouncerJson(member) {
    if (typeof member === 'undefined' || member === null) {
        this.Name = ko.observable();
        this.Number = ko.observable();
        this.Jams = ko.observable();
        this.PointsPerJam = ko.observable();
        this.Points = ko.observable();
        this.PointsPerMinute = ko.observable();
        this.LeadJamPc = ko.observable();
    } else {
        this.Name = ko.observable(member.Name);
        this.Number = ko.observable(member.Number);
        this.Jams = ko.observable(member.Jams);
        this.PointsPerJam = ko.observable(member.PointsPerJam);
        this.Points = ko.observable(member.Points);
        this.PointsPerMinute = ko.observable(member.PointsPerMinute);
        this.LeadJamPc = ko.observable(member.LeadJamPc);
    }
}

function AnnouncerPlayerStat(member) {
    this.rowNumber = ko.observable(1);
    if (row & 2 > 0)
        this.rowNumber(0);
    this.rosterName = ko.observable(member.rosterName);
    this.rosterNumber = ko.observable(member.rosterNumber);
    this.rosterJammerJams = ko.observable(member.rosterJammerJams);
    this.rosterJammerPts = ko.observable(member.rosterJammerPts);
    this.rosterBlockerJams = ko.observable(member.rosterBlockerJams);
    this.currentJam = ko.observable(member.currentJam);
    this.rosterBlockerPointsPer = ko.observable(member.rosterBlockerPointsPer);
    this.rosterPens = ko.observable(member.rosterPens);
}

var announcerPageTimer = $.timer(function () {
    announcerViewModel.pullAnnouncerPage();
});