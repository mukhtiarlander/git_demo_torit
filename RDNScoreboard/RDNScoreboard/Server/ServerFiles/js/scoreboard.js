function hexToRgb(hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
});

var showJamScore = false;

function setupMainDiv4x3(div) {
    //debugger;
    div.css({ position: "fixed" });
    var mainDiv = $("#mainDiv");
    var aspect4x3 = _windowFunctions.get4x3Dimensions();
    mainDiv.toggleClass("mainContainer4x3", true);
    $("#rdnationBackground").toggleClass("rdnationBackground4x3", true);

    div.css(aspect4x3);
    if ($(window).height() > 900)
        mainDiv.css('font-size', '28px');
    else if ($(window).height() > 800)
        mainDiv.css('font-size', '26px');
    else if ($(window).height() > 700)
        mainDiv.css('font-size', '24px');
    else if ($(window).height() > 650)
        mainDiv.css('font-size', '23px');
    else if ($(window).height() > 600)
        mainDiv.css('font-size', '16px');
    else if ($(window).height() > 550)
        mainDiv.css('font-size', '16px');
    else if ($(window).height() > 500)
        mainDiv.css('font-size', '16px');
    else if ($(window).height() > 400)
        mainDiv.css('font-size', '13px');
    else if ($(window).height() > 300)
        mainDiv.css('font-size', '9px');
}

function setupMainDiv16x9(div) {
    div.css({ position: "fixed" });
    var mainDiv = $("#mainDiv");
    var aspect4x3 = _windowFunctions.get16x9Dimensions();
    mainDiv.toggleClass("mainContainer16x9", true);
    $("#rdnationBackground").toggleClass("rdnationBackground16x9", true);

    div.css(aspect4x3);
    if ($(window).width() > 1150)
        mainDiv.css('font-size', '27px');
    else if ($(window).width() > 1100)
        mainDiv.css('font-size', '26px');
    else if ($(window).width() > 1050)
        mainDiv.css('font-size', '25px');
    else if ($(window).width() > 1000)
        mainDiv.css('font-size', '24px');
    else if ($(window).width() > 950)
        mainDiv.css('font-size', '22px');
    else if ($(window).width() > 900)
        mainDiv.css('font-size', '21px');
    else if ($(window).width() > 850)
        mainDiv.css('font-size', '20px');
    else if ($(window).width() > 800)
        mainDiv.css('font-size', '19px');
    else if ($(window).width() > 700)
        mainDiv.css('font-size', '16px');
    else if ($(window).width() > 650)
        mainDiv.css('font-size', '14px');
    else if ($(window).width() > 600)
        mainDiv.css('font-size', '14px');
    else if ($(window).width() > 550)
        mainDiv.css('font-size', '14px');
    else if ($(window).width() > 500)
        mainDiv.css('font-size', '14px');
    else if ($(window).width() > 400)
        mainDiv.css('font-size', '12px');
    else if ($(window).width() > 300)
        mainDiv.css('font-size', '9px');
    else if ($(window).width() > 200)
        mainDiv.css('font-size', '9px');
}



function convertMillisecondsToHuman(milliseconds) {
    if (milliseconds != null) {
        //debugger;
        if (milliseconds <= 0)
            return "0:00";
        var returned = "";
        var x = milliseconds / 1000;
        var seconds = Math.floor(x % 60);
        x /= 60;
        var minutes = Math.floor(x % 60);
        returned = minutes;
        returned += ":";
        if (seconds < 10)
            returned += "0" + seconds;
        else
            returned += seconds;

        return returned;
    }
    return "";
}

var timerStats = $.timer(function () {
    $.getJSON("GrabOverlayUpdate", function (result) {
        populateOverlay(result);
    });
});

timerStats.set({ time: 500, autostart: true });

function startOverlay() {
    $.getJSON("GrabOverlayUpdate", function (result) {
        populateOverlay(result);
    });
    var logoOn = getParameterByName("logo");
    if (logoOn === "")
        $("#mainScreen").toggleClass("logo", true);
    else
        $("#mainScreen").toggleClass("logo", false);
    var mod = getParameterByName("mod");
    if (mod === "top")
        $("#mainDiv").toggleClass("topContainer", true);
    else
        $("#mainDiv").toggleClass("bottomContainer", true);

    var bottomRow = getParameterByName("brow");
    if (bottomRow === "on")
        $("#thirdRow").toggleClass("hidden", false);

    var jamScore = getParameterByName("sjam");
    if (jamScore === "on") {
        $("#Team1Score").toggleClass("TeamJamScore", true);
        $("#Team2Score").toggleClass("TeamJamScore", true);
        showJamScore = true;
    }
    var modColor = getParameterByName("modcolor");
    var modtrans = getParameterByName("modtrans");
    var gscreen = getParameterByName("gscreen");
    var t1Color = getParameterByName("t1color");
    var b1Color = getParameterByName("b1color");
    var f1Color = getParameterByName("f1color");
    var t2Color = getParameterByName("t2color");
    var b2Color = getParameterByName("b2color");
    var f2Color = getParameterByName("f2color");
    var teColor = getParameterByName("tecolor");
    var toColor = getParameterByName("tocolor");
    if (gscreen === "")
        gscreen = "00ff01";
    if (t1Color === "")
        t1Color = "872f95";
    if (b1Color === "")
        b1Color = "000000";
    if (f1Color === "")
        f1Color = "ffffff";
    if (t2Color === "")
        t2Color = "872f95";
    if (b2Color === "")
        b2Color = "000000";
    if (f2Color === "")
        f2Color = "ffffff";
    if (toColor === "")
        toColor = "ffffff";
    if (teColor === "")
        teColor = "ffffff";
    if (modColor === "")
        modColor = "872f95";
    if (modtrans === "")
        modtrans = "1";
    var backgroundColor = hexToRgb("#" + modColor);
    var modBack = "background-color:rgba(" + backgroundColor.r + "," + backgroundColor.g + "," + backgroundColor.b + "," + modtrans + ");";
    var stylet1 = "color:#" + f1Color + ";background: #" + t1Color + "; background: -moz-linear-gradient(top, #" + t1Color + " 0%, #" + b1Color + " 100%); background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #" + t1Color + "), color-stop(100%, #" + b1Color + ")); background: -webkit-linear-gradient(top, #" + t1Color + " 0%, #" + b1Color + " 100%); background: -o-linear-gradient(top, #" + t1Color + " 0%, #" + b1Color + " 100%); background: -ms-linear-gradient(top, #" + t1Color + " 0%, #" + b1Color + " 100%); background: linear-gradient(top, #" + t1Color + " 0%, #" + b1Color + " 100%);";
    var stylet2 = "color:#" + f2Color + ";background: #" + t2Color + "; background: -moz-linear-gradient(top, #" + t2Color + " 0%, #" + b2Color + " 100%); background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #" + t2Color + "), color-stop(100%, #" + b2Color + ")); background: -webkit-linear-gradient(top, #" + t2Color + " 0%, #" + b2Color + " 100%); background: -o-linear-gradient(top, #" + t2Color + " 0%, #" + b2Color + " 100%); background: -ms-linear-gradient(top, #" + t2Color + " 0%, #" + b2Color + " 100%); background: linear-gradient(top, #" + t2Color + " 0%, #" + b2Color + " 100%);";
    var toStyle = "background:#" + toColor + ";";
    var teStyle = "color:#" + teColor + ";";
    var gsStyle = "background:#" + gscreen + ";";
    $("#mainDiv").attr("style", modBack);
    $("#body").attr("style", gsStyle);
    $("#JammerNameTeam1").attr("style", stylet1);
    $("#JammerNameTeam2").attr("style", stylet2);
    //$("#Team1Name").attr("style", stylet1);
   // $("#Team2Name").attr("style", stylet2);
    $("#timeOut1Team1").attr("style", toStyle);
    $("#timeOut2Team1").attr("style", toStyle);
    $("#timeOut3Team1").attr("style", toStyle);
    $("#timeOut1Team2").attr("style", toStyle);
    $("#timeOut2Team2").attr("style", toStyle);
    $("#timeOut3Team2").attr("style", toStyle);
    //$("#Team1Score").attr("style", teStyle);
    //$("#Team2Score").attr("style", teStyle);
    //$("#ClockPeriodNumber").attr("style", teStyle);
   // $("#ClockPeriodTime").attr("style", teStyle);
    //$("#JamNumber").attr("style", teStyle);
   // $("#ClockJamTime").attr("style", teStyle);
    $("#message").attr("style", teStyle);
}
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}


function populateOverlay(jsonObject) {
    if (jsonObject.gameStarted === "True") {
        //debugger;
        var leadJT1 = jsonObject.leadJT1;
        var leadJT2 = jsonObject.leadJT2;
        var leadPT1 = jsonObject.leadPT1;
        var leadPT2 = jsonObject.leadPT2;

        if (leadJT1 == "True" || leadPT1=="True") {
            $("#WftdaT1LD").toggleClass("Show", true);
            }
        else {
            $("#WftdaT1LD").toggleClass("Show", false);
        }

        if (leadJT2 == "True" || leadPT2=="True") {
            $("#WftdaT2LD").toggleClass("Show", true);
        }
        else {
            $("#WftdaT2LD").toggleClass("Show", false);
        }

        //if (leadPT1 == "True")
        //    $("#WftdaT1LD").toggleClass("Show", true);
        //else
        //    $("#WftdaT1LD").toggleClass("Show", false);

        //if (leadPT2 == "True")
        //    $("#WftdaT2LD").toggleClass("Show", true);
        //else
        //    $("#WftdaT2LD").toggleClass("Show", false);

        $("#ClockPeriodTime").text(convertMillisecondsToHuman(jsonObject.periodClock));
        $("#ClockPeriodNumber").text(jsonObject.periodNumber);
        $("#JamNumber").text(jsonObject.jamNumber);
        $("#JammerNameTeam1").html(jsonObject.activeTeam1Jammer);
        $("#JammerNameTeam2").html(jsonObject.activeTeam2Jammer);
        var tot2 = parseFloat(jsonObject.team2TimeOut)
        if (tot2 == 3)
            $("#WftdaT2T3").toggleClass("Show", false);
        else
            $("#WftdaT2T3").toggleClass("Show", true);
        if (tot2 >= 2)
            $("#WftdaT2T2").toggleClass("Show", false);
        else
            $("#WftdaT2T2").toggleClass("Show", true);
        if (tot2 >= 1)
            $("#WftdaT2T1").toggleClass("Show", false);
        else
            $("#WftdaT2T1").toggleClass("Show", true);

        var tot1 = parseFloat(jsonObject.team1TimeOut);
        if (tot1 == 3)
            $("#WftdaT1T3").toggleClass("Show", false);
        else
            $("#WftdaT1T3").toggleClass("Show", true);
        if (tot1 >= 2)
            $("#WftdaT1T2").toggleClass("Show", false);
        else
            $("#WftdaT1T2").toggleClass("Show", true);
        if (tot1 >= 1)
            $("#WftdaT1T1").toggleClass("Show", false);
        else
            $("#WftdaT1T1").toggleClass("Show", true);
        if (jsonObject.isJamRunning === "True") {
            $("#ClockJamTime").text(convertMillisecondsToHuman(jsonObject.jamClock));
        }
        if (jsonObject.isTimeOutRunning === "True") {
            $("#StatusBar").text("Time Out");
            $("#ClockJamTime").text(convertMillisecondsToHuman(jsonObject.timeOutClock));
        }
        
        if (jsonObject.isLineUpRunning === "True") {
            $("#StatusBar").text("Line Up");
            $("#ClockJamTime").text(convertMillisecondsToHuman(jsonObject.lineUpClock));
        }
       
        if (jsonObject.isIntermissionRunning === "True") {
            $("#ClockPeriodNumber").text(jsonObject.intermissionName);
            $("#ClockPeriodTime").text(convertMillisecondsToHuman(jsonObject.intermissionClock));
        }
        if (jsonObject.isJamRunning != "True" && jsonObject.isTimeOutRunning != "True" && jsonObject.isLineUpRunning != "True")
            $("#ClockJamTime").text(convertMillisecondsToHuman(jsonObject.jamClock));
        if (jsonObject.isTimeOutRunning != "True" && jsonObject.isLineUpRunning != "True")
            $("#StatusBar").text("");

    }
    else {
        $("#ClockPeriodNumber").text("NotStarted");
        $("#StatusBar").text("Standby");
    }
    if (jsonObject.team1Name != null)
        $("#Team1Name").text(jsonObject.team1Name);
    if (jsonObject.team1Score != null)
        $("#Team1Score").text(jsonObject.team1Score);
    if (showJamScore === true && jsonObject.team1JamScore != null)
        $("#Team1Score").append(":" + jsonObject.team1JamScore);
    if (jsonObject.team2Name != null)
        $("#Team2Name").text(jsonObject.team2Name);
    if (jsonObject.team2Score != null)
        $("#Team2Score").text(jsonObject.team2Score);
    if (showJamScore === true && jsonObject.team2JamScore != null)
        $("#Team2Score").append(":" + jsonObject.team2JamScore);
}