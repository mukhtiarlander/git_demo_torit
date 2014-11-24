var apiUrl = "http://localhost:16106/";

apiUrl = "https://api.rdnation.com/";

OpenLayers.ImgPath = "/content/images/";

var urlForLiveCurrentGames = "https://api.rdnation.com/livegame/currentgames?callback=?";
//var urlForLiveCurrentGames = "http://localhost:19237/livegame/currentgames?callback=?";

function Chat(chatTemp, memberName, id, created) {
    this.chat = ko.observable(chatTemp);
    this.name = ko.observable(memberName);
    this.Id = ko.observable(id);
    this.Created = ko.observable("");
}

function HideShowCCInfo(hideShow) {
    if (hideShow === 'show') {
        $("#expirationDateTR").toggleClass('displayNone', false);
        $("#securityCodeTR").toggleClass('displayNone', false);
        $("#cardNumberTR").toggleClass('displayNone', false);
        toggleSubscriptionValidationRules(true);
    }
    else if (hideShow === 'hide') {
        $("#expirationDateTR").toggleClass('displayNone', true);
        $("#securityCodeTR").toggleClass('displayNone', true);
        $("#cardNumberTR").toggleClass('displayNone', true);
        toggleSubscriptionValidationRules(false);
    }
}

function toggleSubscriptionValidationRules(onOff) {
    var settings = $('#PaymentForm').validate().settings;
    //on
    if (onOff === true) {
        $.extend(settings, {
            rules: {
                "Paywall.CCNumber": "required",
                "Paywall.SecurityCode": "required",
                MonthOfExpiration: "required",
                YearOfExpiration: "required",
                "Paywall.EmailAddress": "required"
            },
            submitHandler: function (form) {
                // disable the submit button to prevent repeated clicks
                $('#submitButton1').toggleClass("displayNone", true);
                $('#working1').toggleClass("displayNone", false);
                Stripe.createToken({
                    number: $('.card-number').val(),
                    cvc: $('.card-cvc').val(),
                    exp_month: $('.card-expiry-month').val(),
                    exp_year: $('.card-expiry-year').val(),
                }, stripeResponseHandler);
            }
        });
    }
    else if (onOff === false) {
        $.extend(settings, {
            rules: {
                "Paywall.EmailAddress": "required"
            },
            submitHandler: function (form) {
                // disable the submit button to prevent repeated clicks
                $('#submitButton1').toggleClass("displayNone", true);
                $('#working1').toggleClass("displayNone", false);
                var form$ = $("#PaymentForm");
                // and submit
                form$.get(0).submit();
            }
        });
    }
}

function stripeResponseHandler(status, response) {
    if (response.error) {
        // show the errors on the form
        $(".paymentErrors").html(response.error.message);
        $('#submitButton1').toggleClass("displayNone", false);
        $('#working1').toggleClass("displayNone", true);
    } else {
        var form$ = $("#PaymentForm");
        // token contains id, last4, and card type
        var token = response['id'];
        // insert the token into the form so it gets submitted to the server
        form$.append("<input type='hidden' name='stripeToken' value='" + token + "'/>");
        // and submit
        form$.get(0).submit();
    }
}


function IsConnectedToDerby() {
    var settings = $('#signUp').validate().settings;
    var checkbox = $("#IsConnectedToRollerDerby");
    var isChecked = $(checkbox).is(":checked");
    if (!isChecked) {
        $("#derbyNameRow").toggleClass("displayNone", true);
        $("#firstNameRow").toggleClass("displayNone", true);
        $("#genderRow").toggleClass("displayNone", true);
        $("#positionRow").toggleClass("displayNone", true);
        $.extend(settings, {
            rules: {
                Email: "required",
                ConfirmEmail: "required",
                Password: "required"
            }
        });
    } else {
        $("#derbyNameRow").toggleClass("displayNone", false);
        $("#firstNameRow").toggleClass("displayNone", false);
        $("#genderRow").toggleClass("displayNone", false);
        $("#positionRow").toggleClass("displayNone", false);
        $.extend(settings, {
            rules: {
                DerbyName: "required",
                Firstname: "required",
                Email: "required",
                ConfirmEmail: "required",
                Password: "required",
                PositionType: "required"
            }
        });
    }
}


function AddSiteMapNode(url, modified) {
    $.getJSON("/Utilities/AddNodeToSiteMap", { url: url, modified: modified });
}

//verifys the derby name if we already have it in our system.  If we do, it will display possible matches to the user signing up.
function SearchForDerbyName(control) {
    $("#loadingDerbyName").toggleClass("displayNone");
    $.getJSON("/Utilities/SearchForDerbyName", { name: control.value }, function (result) {
        $("#loadingDerbyName").toggleClass("displayNone");
        if (result.names.length > 0) {
            $("#signUpFindDerbyNameHeader").html("<div class='green'>Is This You? Click The Name if it is!</div>");
            $("#signUpFindDerbyName").html("");
            $.each(result.names, function (i, field) {
                var memId = field.MemberId.replace(/\-/g, '');
                $("#signUpFindDerbyName").append("<div><a href='http://" + window.location.host + "/verifyderbyname/" + memId + "/" + field.DerbyName.replace(/\#/g, '') + "'>" + field.DerbyName + "</a> - " + field.PlayerNumber + "</div>");
            });
        }
    });
}


function CurrentGame(game) {
    //Data
    this.gameId = ko.observable(game.GameId);
    var team1NTemp = "Home";
    var team2NTemp = "Away";
    if (game.Team1Name != null)
        team1NTemp = game.Team1Name;

    if (game.Team2Name != null)
        team2NTemp = game.Team2Name;

    this.gameName = ko.observable(game.GameName.substring(0, 12));
    //we trim chars to make room for scores.

    this.team1Name = ko.observable(team1NTemp.substring(0, 12));
    this.team1Score = ko.observable(game.Team1Score);

    this.team2Name = ko.observable(team2NTemp.substring(0, 12));
    this.team2Score = ko.observable(game.Team2Score);

    this.jamNumber = ko.observable(game.JamNumber);
    this.periodNumber = ko.observable(game.PeriodNumber + ":");
    this.ruleSet = ko.observable(game.RuleSet);
    this.isGameStreaming = ko.observable(game.IsLiveStreaming);

    this.gameUrl = ko.observable("http://" + window.location.host + "/roller-derby-game/" + game.GameId.replace(/-/g, "") + "/" + game.GameName.replace(/ /g, "-") + "/" + team1NTemp.replace(/ /g, "-") + "/" + team2NTemp.replace(/ /g, "-"));
}
var currentGamesTimer = $.timer(function () {
    currentGamesViewModel.PullCurrentGamesTicker();
});
currentGamesTimer.set({ time: 180000, autostart: true });

var currentGamesViewModel = new function () {
    var currentGameViewModel = this;
    this.currentGames = ko.observableArray([]);

    //initialize
    this.initialize = function (callback) {
        currentGameViewModel.PullCurrentGamesTicker();
    }

    //private functions
    this.PullCurrentGamesTicker = function () {
        $.getJSON(urlForLiveCurrentGames, function (result) {
            currentGameViewModel.currentGames.removeAll();
            $(result.games).each(function (index) {
                currentGameViewModel.currentGames.push(new CurrentGame(this));
            });
        });
    }
}



function createmarkerforleage(lon, lat, data, index) {
    if (lon != "0" && lat != "0") {
        //console.log(data[6]);
        var popupinfo = "<table class='popuptable'><tr><td class='popuptd'><div class='popupleagename'><a style='text-decoration: none;color:#BA2B3C;' target='_blank' href='" + data[8] + "' >" + data[2] + "</a></div></br><div class='popupleagemem'>" + data[7] + " Members</div></br><div class='popupaddress'>" + data[3] + " " + data[4] + "</br>" + data[5] + "</div></br><div class='popupbold'></div></td><td style='width:50px;padding-top:5px;padding-right:5px'>";
        if (data[6] != null)
            popupinfo += "<img src='" + data[6] + "' width='100px'/>";
        popupinfo += "</td></tr></table>";
        var lonLat = new OpenLayers.LonLat(lon, lat)
              .transform(
                new OpenLayers.Projection(PROJECTIONFROM, PROJECTIONTO), // transform from WGS 1984
                map.getProjectionObject() // to Spherical Mercator Projection
              );
        var myLocation = new OpenLayers.Geometry.Point(lon, lat)
            .transform(PROJECTIONFROM, PROJECTIONTO);
        var zoom = 0;
        var markers = new OpenLayers.Layer.Markers("Markers");
        markers.setZIndex(100);
        map.addLayer(markers);
        var size = new OpenLayers.Size(21, 25);
        var offset = new OpenLayers.Pixel(-(size.w / 2), -size.h);
        var icon = new OpenLayers.Icon('../Content/images/marker3.png', size, offset);
        markers.addMarker(new OpenLayers.Marker(lonLat, icon));

        markers.events.register('mousedown', markers, function (evt) {
            $(".olPopup").hide();
                        var popup = new OpenLayers.Popup.FramedCloud("Popup",
            myLocation.getBounds().getCenterLonLat(), null,
            popupinfo, null,
            true // <-- true if we want a close (X) button, false otherwise
        );

            map.addPopup(popup);

            OpenLayers.Event.stop(evt);
            $(".olPopup").css("z-index", 10000);
        });
        map.setCenter(lonLat);
        map.zoomToMaxExtent();

    }
}


