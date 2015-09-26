var MemberViewModel = new function () {
    var thisViewModel = this;
    this.Members = ko.observableArray([]);
    this.maxId = ko.observable();
    this.pendingRequest = ko.observable(false);
    this.page = ko.observable();
    this.IsFinishedScrolling = ko.observable(false);
    this.SearchText = ko.observable();
    this.ListCountPull = ko.observable();

    this.LoadMemberList = function (count) {
        thisViewModel.ListCountPull(count);
        thisViewModel.page(0);
        getItems(thisViewModel.ListCountPull(), false);
        $(window).scroll(function () {
            if ($(window).scrollTop() >= $(document).height() - $(window).height() - 400) {
                getItems(thisViewModel.ListCountPull(), false);
            }
        });
    }

    this.SearchMembers = function (input) {
        thisViewModel.SearchText($(input).val());
        thisViewModel.IsFinishedScrolling(false);
        thisViewModel.pendingRequest(false);
        thisViewModel.page(0);
        getItems(thisViewModel.ListCountPull(), true);
    }

    function getItems(cnt, isSearch) {
        if (!thisViewModel.pendingRequest() && !thisViewModel.IsFinishedScrolling()) {
            thisViewModel.pendingRequest(true);
            $.ajax({
                type: "POST",
                url: apiUrl + "Skater/GetAllSkaters",
                data: { p: thisViewModel.page(), c: cnt, s: thisViewModel.SearchText() },
                dataType: "json",
                success: function (data) {
                    if (data.members.length > 0) {
                        if (!isSearch) {
                            ko.utils.arrayForEach(data.members, function (entry) {
                                thisViewModel.Members.push(entry);
                            });
                        }
                        else {
                            thisViewModel.Members.removeAll();
                            thisViewModel.Members(data.members);
                        }
                        thisViewModel.page(thisViewModel.page() + 1);
                        thisViewModel.pendingRequest(false);
                        $('#skaters-grid').masonry('reloadItems', {
                            itemSelector: '.skaters-grid-item',
                            percentPosition: true,
                        })
                        $('#skaters-grid').imagesLoaded().progress(function () {
                            $('#skaters-grid').masonry({
                                itemSelector: '.skaters-grid-item',
                            });

                        });
                    }
                    else
                        thisViewModel.IsFinishedScrolling(true);
                }
            });

        }
    }

}