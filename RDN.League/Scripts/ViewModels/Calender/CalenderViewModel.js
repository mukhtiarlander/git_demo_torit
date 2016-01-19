var Calendar = new function () {
    var thisViewModel = this;
    var groupsSelectedIds = [];

    this.checkInRemoveSmall = function (button, memId, derbyName) {
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var memberRow = $("#" + memId + "-row");
        var memberRow2 = $("#" + memId + "-row2");

        $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
        }).error(function () {
            $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
            });
        });

        memberRow.remove();
        memberRow2.remove();
    };


    this.checkInMemberSmall = function () {
        var dropDown = $("#SelectedMemberId");
        var selectedItem = dropDown.val();
        var selectedMember = $("#SelectedMemberId option:selected").text();
        var noteCheckIn = $("#noteCheckIn");
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var present = $("#Present").is(':checked');
        var partial = $("#Partial").is(':checked');
        var notPresent = $("#Not_Present").is(':checked');
        var excused = $("#Excused").is(':checked');
        var isTardy = $("#isTardy").is(':checked');
        var additionalPoints = $("#additionalPoints").val();

        if (selectedItem === "") {
            $("#warning").text("Please Select Member");
            return;
        }
        else {
            $("#warning").text("");
        }

        var pontType;
        var points;
        if (present) {
            pontType = "Present";
            points = Number($("#Present").attr("points"));
        }
        else if (partial) {
            pontType = "Partial";
            points = Number($("#Partial").attr("points"));
        }
        else if (notPresent) {
            pontType = "Not_Present";
            points = Number($("#Not_Present").attr("points"));
        }
        else if (excused) {
            pontType = "Excused";
            points = Number($("#Excused").attr("points"));
        }
        if (isTardy)
            points += Number($("#isTardy").attr("points"));

        $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: selectedItem, pointType: pontType, note: noteCheckIn.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
            if (result.isSuccess === true) {
                $("#warning").text("");
                $('.bottom-right').notify({
                    message: { text: 'Checked In! ' },
                    fadeOut: { enabled: true, delay: 4000 }
                }).show();
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: selectedItem, pointType: pontType, note: noteCheckIn.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
                if (result.isSuccess === true) {
                    $("#warning").text("");
                    $('.bottom-right').notify({
                        message: { text: 'Checked In! ' },
                        fadeOut: { enabled: true, delay: 4000 }
                    }).show();
                } else {
                }
            });
        });

        if (isTardy)
            pontType += " - Tardy";
        var rowCount = $('#checkInMembers tr').length + 1;
        $("#checkInMembers tbody").prepend('<tr class="trBorderB" id="' + selectedItem + '-row"><td></td><td></td><td>' + selectedMember + '</td><td>' + pontType + '</td><td>' + (+points + +additionalPoints) + '</td><td><button  class="btn btn-danger" onclick="javascript:Calendar.checkInRemoveSmall(this,\'' + selectedItem + '\', \'' + selectedMember + '\')"><i class="fa fa-trash"></i></button></td></tr>');
        $("#SelectedMemberId option[value='" + selectedItem + "']").remove();
        dropDown.val("");
        noteCheckIn.val("");
    }

    this.checkInMemberLarge = function (button, memId, derbyName) {
        memId = $.trim(memId);
        derbyName = $.trim(derbyName);
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var note = $("#" + memId + "-Note");
        var notecell = $("#" + memId + "-Note-cell");
        var present = $("#Present").is(':checked');
        var partial = $("#Partial").is(':checked');
        var notPresent = $("#Not_Present").is(':checked');
        var excused = $("#Excused").is(':checked');
        var isTardy = $("#" + memId + "-tardy").is(':checked');
        var additionalPoints = $("#additionalPoints").val();

        var pontType;
        var points;
        if (present) {
            pontType = "Present";
            points = Number($("#Present").attr("points"));
        }
        else if (partial) {
            pontType = "Partial";
            points = Number($("#Partial").attr("points"));
        }
        else if (notPresent) {
            pontType = "Not_Present";
            points = Number($("#Not_Present").attr("points"));
        }
        else if (excused) {
            pontType = "Excused";
            points = Number($("#Excused").attr("points"));
        }
        if (isTardy)
            points += Number($("#TardyPoints").val());


        $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: memId, pointType: pontType, note: note.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
            if (result.isSuccess === true) {
                $('.bottom-right').notify({
                    message: { text: 'Checked In! ' },
                    fadeOut: { enabled: true, delay: 4000 }
                }).show();
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/CheckMemberIntoEvent", { calendarId: calId, eventId: evnId, memberId: memId, pointType: pontType, note: note.val(), isTardy: isTardy, addPoints: additionalPoints }, function (result) {
                if (result.isSuccess === true) {
                    $('.bottom-right').notify({
                        message: { text: 'Checked In! ' },
                        fadeOut: { enabled: true, delay: 4000 }
                    }).show();
                } else {
                }
            });
        });
        $(button).parent().html("<button type='button' class='btn btn-warning btn-sm' onclick='javascript:Calendar.checkInRemoveLarge(this,\"" + memId + "\", \"" + derbyName + "\")'>Remove</button>");

        if (isTardy)
            pontType += " - Tardy";
        $("#" + memId).text(pontType);
        notecell.html('<span class="i">' + note.val() + '</span>');
        $("#" + memId + "-tardy-cell").text("");
        $("#" + memId + "-points").text(+points + +additionalPoints);
    }

    this.DeleteCalendarEventType = function (e) {
        var eventTypeId = e.attr("data-id");
        $.getJSON("/Calendar/CalendarDeleteEventType", { eventTypeId: eventTypeId }, function (result) {
            if (result.isSuccess === true) {
                e.parent().parent().remove();
                $('.bottom-right').notify({
                    message: { text: "Event Type Removed" },
                    fadeOut: { enabled: true, delay: 4000 }
                }).show();
            }
        });
    };

    this.checkInRemoveLarge = function (button, memId, derbyName) {
        memId = $.trim(memId);
        derbyName = $.trim(derbyName);
        var calId = $("#CalendarId").val();
        var evnId = $("#CalendarItemId").val();
        var tardyCell = $("#" + memId + "-tardy-cell");
        var noteCell = $("#" + memId + "-Note-cell");
        var pointCell = $("#" + memId + "-points");
        var pointTypeCell = $("#" + memId);
        var tardyPoints = $("#TardyPoints").val();


        $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
            if (result.isSuccess === true) {
            } else {
            }
        }).error(function () {
            $.getJSON("/Calendar/CheckInMemberRemove", { calendarId: calId, eventId: evnId, memberId: memId }, function (result) {
                if (result.isSuccess === true) {
                } else {
                }
            });
        });

        $(button).parent().html('<input type="button" class="btn btn-primary btn-sm" value="Check In" onclick="javascript:Calendar.checkInMemberLarge(this,\'' + memId + '\', \'' + derbyName + '\')" />');
        tardyCell.html('<input type="checkbox" id="' + memId + '-tardy" points="' + tardyPoints + '"  title="Is Tardy" /><span>Tardy?</span>');
        noteCell.html('<input type="text" id="' + memId + '-Note" class="form-control" />');
        pointCell.text("");
        pointTypeCell.text("None");
    }

    this.ExpandGroupMemberList = function () {
        $("#groupMemberLists").slideToggle();
    };
    this.SetAvailForEventHomePage = function (calId, evenId) {
        calendarId = calId;
        eventId = evenId;
        $(".popover").hide().remove();
        $('button[data-toggle="popover"]').popover('destroy');
        var popup = $('#availablePopUp').clone();
        addEventPopup = true;
        $("#" + eventId + "-setAvail").popover({ content: popup.html() });
        $("#" + eventId + "-setAvail").popover('show');
    }
    this.InitializeNewEvent = function () {
        var gList = $("#groupList");
        var currentIds = $("#ToGroupIds").val();
        $.getJSON("/league/GetGroupsOfCurrentMember", {}, function (result) {
            if (result.isSuccess === true) {
                $(result.groups).each(function () {
                    if (currentIds.indexOf(this[1]) === -1)
                        gList.append("<li><label style='font-weight:normal'><input groupName='" + this[0].replace(/'/g, "") + "' id='" + this[1] + "' name='" + this[1] + "' onchange='Calendar.ChangeGroupDictionaryItem(this)' type='checkbox' > " + this[0].replace(/'/g, "") + "</label></li>");
                    else { // if the group already is in the event.
                        gList.append("<li><label style='font-weight:normal'><input  checked='checked' groupName='" + this[0].replace(/'/g, "") + "' id='" + this[1] + "' name='" + this[1] + "' onchange='Calendar.ChangeGroupDictionaryItem(this)' type='checkbox' > " + this[0].replace(/'/g, "") + "</label></li>");
                        var group = { name: this[0].replace(/'/g, ""), idd: this[1] };
                        groupsSelectedIds.push(group);
                    }
                });
            }
        }).error(function () {
        });
    };
    this.InitializeNewReport = function () {
        var gList = $("#groupList");
        $.getJSON("/league/GetGroupsOfCurrentMember", {}, function (result) {
            if (result.isSuccess === true) {
                gList.append("<li><label class='no-bold'><input groupName='Entire League' id='0' name='groupToReportOn' onchange='Calendar.ChangeGroupDictionaryItemReport(this)' type='radio' > Entire League</label></li>");
                $(result.groups).each(function () {
                    gList.append("<li><label class='no-bold'><input groupName='" + this[0] + "' id='" + this[1] + "' name='groupToReportOn' onchange='Calendar.ChangeGroupDictionaryItemReport(this)' type='radio' > " + this[0] + "</label></li>");
                });
            }
        }).error(function () {
        });
    };
    this.InitializeCalenderLarger = function (type, id) {
        var date = new Date();
        var d = date.getDate();
        var m = date.getMonth();
        var y = date.getFullYear();

        var calendar = $('#calendar').fullCalendar({
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay'
            },
            eventBackgroundColor: "#872f95",
            eventBorderColor: "#c5c5c5",
            selectable: true,
            events: function (start, end, callback) {
                start = start.getTime() / 1000;
                end = end.getTime() / 1000;
                $.getJSON("/calendar/LoadCalendarView", { type: type, id: id, startDt: start, endDt: end }, function (result) {
                    var events = [];
                    $.each(result.events, function (i, field) {
                        events.push({
                            title: $(field).attr('title'),
                            start: $(field).attr('start'),
                            id: $(field).attr('id'),
                            end: $(field).attr('end'),
                            url: $(field).attr('url'),
                            allDay: $(field).attr('allDay'),
                            backgroundColor: $(field).attr('backColor'),
                            location: field["Locaton"],
                            description: field["Description"]
                        });
                    });
                    callback(events);
                });
            },
            eventRender: function (event, element) {
                var calenderEventTemplate = $(".divEventDetail").clone();
                if ((event.location != undefined && event.location != "") || (event.description != undefined && event.description != "")) {
                    if (event.location != undefined && event.location != "") { calenderEventTemplate.find(".divLocation").html(event.location).parent().removeClass("display-none"); }
                    if (event.description != undefined && event.description != "") { calenderEventTemplate.find(".divDescription").html(event.description).parent().removeClass("display-none"); }
                    element.popover({
                        title: "My Title",
                        html: true,
                        placement: "auto right",
                        trigger: "hover",
                        container: "body",
                        title: event.title,
                        content: calenderEventTemplate.html()
                    });
                }
            }
        });
    };
    this.InitializeCalenderList = function () {
        $('[data-toggle="offcanvas"]').click(function () {
            $('.row-offcanvas').toggleClass('active');
        });

        $("button[data-attendance]").each(function () {
            $(this).tooltip({
                placement: 'bottom',
                title: $(this).attr("data-attendance")
            });
        });
        $("button[data-rsvp]").each(function () {
            $(this).tooltip({
                placement: 'bottom',
                title: $(this).attr("data-rsvp")
            });
        });
        $(".eventTitle[data-note!='']").popover({
            html: true,
            placement: "auto right",
            content: function () {
                var location = $(this).attr("data-location");
                var description = $(this).attr("data-note");
                var contentTemplate = $(".divEventDetail").clone();
                if (location != undefined && location != "") { contentTemplate.find(".divLocation").html(location).parent().removeClass("display-none"); }
                contentTemplate.find(".divDescription").html(description).parent().removeClass("display-none");
                return contentTemplate.html();
            }
        });
        $(".eventTitle[data-location!='']").popover({
            html: true,
            placement: "auto right",
            content: function () {
                var location = $(this).attr("data-location");
                var description = $(this).attr("data-note");
                var contentTemplate = $(".divEventDetail").clone();
                contentTemplate.find(".divLocation").html(location).parent().removeClass("display-none");
                if (description != undefined && description != "") { contentTemplate.find(".divDescription").html(description).parent().removeClass("display-none"); }
                return contentTemplate.html();
            }
        });
    };
    this.ChangeGroupDictionaryItem = function (checkbox) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToGroupIds");
        var box = $(checkbox);
        var checked = box.is(":checked");
        if (checked) {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds.push(group);
        }
        else {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds = jQuery.grep(groupsSelectedIds, function (value) {
                return value.idd != group.idd;
            });
        }
        var text = "";
        var ids = "";
        if (groupsSelectedIds.length > 0) {
            $.each(groupsSelectedIds.reverse(), function (i, val) {
                text += '<span class="label label-primary font12 padding-top-5">' + val.name + '</span> ';
                ids += val.idd + ",";
            });
        } else {
            text += '<span class="label label-primary font12 padding-top-5">Entire League</span> ';
        }

        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    }
    this.ChangeGroupDictionaryItemReport = function (checkbox) {
        var memNames = $("#ToMemberNamesSelected");
        var memIds = $("#ToGroupIds");
        var box = $(checkbox);
        var checked = box.is(":checked");
        groupsSelectedIds = [];
        if (checked) {
            var group = { name: box.attr("groupName"), idd: box.attr("id") };
            groupsSelectedIds.push(group);
        }

        var text = "";
        var ids = "";
        if (groupsSelectedIds.length > 0) {
            $.each(groupsSelectedIds.reverse(), function (i, val) {
                text += '<span class="label label-primary font12 padding-top-5">' + val.name + '</span> ';
                ids += val.idd + ",";
            });
            $("#pullGroupEvents").toggleClass("displayNone", false);
        } else {
            text += '<span class="label label-primary font12 padding-top-5">Entire League</span> ';
            $("#pullGroupEvents").toggleClass("displayNone", true);
        }

        if (document.getElementById('ToMemberNamesSelected') !== null)
            document.getElementById('ToMemberNamesSelected').innerHTML = text;
        memIds.val(ids);
    };
}