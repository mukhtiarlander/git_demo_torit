function gridInit(scope, filter, totalPostCount, currentPage) {
    var sortingOrder = ''; // 'Title';
    scope.sortingOrder = sortingOrder;
    scope.reverse = false;
    scope.filteredItems = [];
    scope.groupedItems = [];
    scope.itemsPerPage = 20;

    scope.unPagedItems = scope.items;
    scope.pagedItems = [];
    if (currentPage !== undefined)
        scope.currentPage = currentPage;
    else
        scope.currentPage = 0;

    scope.totalPosts = totalPostCount;



    scope.range = function (start, end) {
        var ret = [];
        if (!end) {
            end = start;
            start = 0;
        }
        for (var i = start; i < end; i++) {
            ret.push(i);
        }
        return ret;
    };

    scope.prevPage = function () {
        if (scope.currentPage > 0) {
            if (scope.totalPosts !== undefined && scope.totalPosts > 0) {
                scope.currentPage--;
                scope.getPage(scope.currentPage);
            } else {
                scope.currentPage--;
            }
        }
        clearChecks();
    };

    scope.nextPage = function () {
        //console.log(scope.currentPage);
        if (scope.totalPosts !== undefined && scope.totalPosts > 0) {
            scope.currentPage++;
            scope.getPage(scope.currentPage);
        } else {
            if (scope.currentPage < scope.pagedItems.length - 1) {
                scope.currentPage++;
            }
        }
        clearChecks();
    };

    var searchMatch = function (haystack, needle) {
        if (!needle) {
            return true;
        }
        if (!haystack) {
            return false;
        }
        return haystack.toString().toLowerCase().indexOf(needle.toString().toLowerCase()) !== -1; 0.

    };

    scope.search = function () {
        //console.log(scope.query);
        if (scope.totalPosts !== undefined && scope.totalPosts > 0) {
            scope.searchPage(scope.query);
        }
        else {
            scope.filteredItems = filter('filter')(scope.items, function (item) {
                for (var attr in item) {
                    if (searchMatch(item[attr], scope.query))
                        return true;
                }
                return false;
            });
            if (scope.sortingOrder !== '') {
                scope.filteredItems = filter('orderBy')(scope.filteredItems, scope.sortingOrder, scope.reverse);
            }
        }
        scope.currentPage = 0;
        scope.groupToPages();

    };

    scope.groupToPages = function () {
        scope.pagedItems = [];

        if (scope.totalPosts !== undefined && scope.totalPosts > 0) {
            //console.log(scope.totalPosts);
            for (var i = 0; i < scope.totalPosts; i++) {
                if (i % scope.itemsPerPage === 0) {
                    scope.pagedItems[Math.floor(i / scope.itemsPerPage)] = [scope.filteredItems[i]];
                } else {
                    scope.pagedItems[Math.floor(i / scope.itemsPerPage)].push(scope.filteredItems[i]);
                }
            }
        }
        else {
            for (var i = 0; i < scope.filteredItems.length; i++) {
                if (i % scope.itemsPerPage === 0) {
                    scope.pagedItems[Math.floor(i / scope.itemsPerPage)] = [scope.filteredItems[i]];
                } else {
                    scope.pagedItems[Math.floor(i / scope.itemsPerPage)].push(scope.filteredItems[i]);
                }
            }
        }
    };

    scope.setPage = function () {

        if (scope.totalPosts !== undefined && scope.totalPosts > 0) {
            scope.currentPage = this.n;
            scope.getPage(scope.currentPage);
        } else {
            scope.currentPage = this.n;
        }
        clearChecks();
    };

    scope.sort_by = function (newSortingOrder, e) {
        if (scope.sortingOrder == newSortingOrder)
            scope.reverse = !scope.reverse;

        scope.sortingOrder = newSortingOrder;

        $('th i').each(function () {
            $(this).removeClass('fa-sort-asc').removeClass('fa-sort-desc').addClass('fa-sort');
        });

        if (scope.reverse)
            $(e.target).removeClass('fa-sort').addClass('fa-sort-asc');
        else
            $(e.target).removeClass('fa-sort').addClass('fa-sort-desc');
    };

    scope.gridFilter = function (field, value, fltr) {
        $("#fltr-all").removeClass('active');
        $("#fltr-apr").removeClass('active');
        $("#fltr-pnd").removeClass('active');
        $("#fltr-spm").removeClass('active');
        $("#fltr-png").removeClass('active');
        $("#fltr-pub").removeClass('active');
        $("#fltr-dft").removeClass('active');

        $("#fltr-loc").removeClass("active");
        $("#fltr-gal").removeClass("active");

        $("#fltr-" + fltr).addClass('active');

        scope.filter = fltr;

        scope.filteredItems = filter('filter')(scope.items, function (item) {
            if (value == 'all') {
                return true;
            }
            return item[field] === value;
        });
        if (scope.sortingOrder !== '') {
            scope.filteredItems = filter('orderBy')(scope.filteredItems, scope.sortingOrder, scope.reverse);
        }
        scope.currentPage = 0;
        scope.groupToPages();
        return false;
    };
    scope.gridNewFilter = function (field, value, fltr) {
        $("#fltr-all").removeClass('active');
        $("#fltr-apr").removeClass('active');
        $("#fltr-pnd").removeClass('active');
        $("#fltr-spm").removeClass('active');
        $("#fltr-png").removeClass('active');
        $("#fltr-pub").removeClass('active');
        $("#fltr-dft").removeClass('active');

        $("#fltr-loc").removeClass("active");
        $("#fltr-gal").removeClass("active");

        $("#fltr-" + fltr).addClass('active');
        if (fltr === "all") {
            scope.load();
        }
        else if (fltr === "pub") {
            scope.loadPublished();
        }
        else if (fltr === "dft") {
            scope.loadUnPublished();
        }
        return false;
    };


    scope.checkAll = function (e) {
        for (var i = 0; i < scope.pagedItems[scope.currentPage].length; i++) {
            // for packages, do not check local packages
            if (scope.pagedItems[scope.currentPage][i].Location) {
                if (scope.pagedItems[scope.currentPage][i].Location != "L") {
                    scope.pagedItems[scope.currentPage][i].IsChecked = e.target.checked;
                }
            }
                // for comments, do not check if comment has children
            else if (scope.commentsPage) {
                if (scope.pagedItems[scope.currentPage][i].HasChildren === false) {
                    scope.pagedItems[scope.currentPage][i].IsChecked = e.target.checked;
                }
            }
            else {
                // for others toggle all
                scope.pagedItems[scope.currentPage][i].IsChecked = e.target.checked;
            }
        }
    };

    function clearChecks() {
        for (var i = 0; i < scope.items.length; i++)
            scope.items[i].IsChecked = false;

        $('#chkAll').prop('checked', false);
    };

    scope.search();
}