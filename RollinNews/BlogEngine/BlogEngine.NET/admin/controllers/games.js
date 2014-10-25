angular.module('blogAdmin').controller('GamesController', ["$rootScope", "$scope", "$location", "$http", "$filter", "dataService", function ($rootScope, $scope, $location, $http, $filter, dataService) {
    $scope.items = [];
    $scope.ruleSets = [];
    $scope.federations = [];
    $scope.id = ($location.search()).id;
    $scope.filter = ($location.search()).fltr;
    $scope.editItem = {};
    
    $scope.loadRuleSets = function (id) {

        dataService.getItems('/api/ruleset')
        .success(function (data) {
            angular.copy(data, $scope.ruleSets);

        })
        .error(function () {
        });
    }

    $scope.loadFederations = function (id) {
        dataService.getItems('/api/federations')
        .success(function (data) {
            angular.copy(data, $scope.federations);

        })
        .error(function () {
        });
    }

  
    $scope.editGame = function (id) {
        $scope.id = id;
        $scope.loadCurrentGame();
    }

    $scope.loadCurrentGame = function () {
        spinOn();
        $scope.loadRuleSets($scope.id);
        $scope.loadFederations($scope.id);
        dataService.getItems('/api/games/', { id: $scope.id })
        .success(function (data) {
            angular.copy(data, $scope.editItem);
            $('#txtTeam1Name').val($scope.editItem.Team1Name);
            $('#txtTeam2Name').val($scope.editItem.Team2Name);

            $('input[name=RuleSetEnumDisplay]').each(function () {
                if ($(this).attr('title') === $scope.editItem.RuleSetEnumDisplay) {
                    $(this).prop("checked", true);
                }
            });
            $('input[name=SanctionedByFederationEnumDisplay]').each(function () {
                if ($(this).attr('title') === $scope.editItem.SanctionedByFederationEnumDisplay) {
                    $(this).prop("checked", true);
                }
            });

            $("#modal-edit").modal();
            spinOff();
        })
        .error(function () {
            toastr.error($rootScope.lbl.errorLoadingRole);
            spinOff();
        });
    }


    $scope.load = function () {
        spinOn();
        if ($scope.id) {
            $scope.loadCurrentGame();
        }
        var url = '/api/games';
        var p = { take: 0, skip: 0 }
        dataService.getItems(url, p)
        .success(function (data) {
            angular.copy(data.ManualGames, $scope.items);
            gridInit($scope, $filter);
            if ($scope.filter) {
                $scope.setFilter($scope.filter);
            }
            spinOff();
        })
        .error(function () {
            toastr.error("Error Loading Games");
            spinOff();
        });

        $('#txtTeam1Name, #txtTeam2Name').typeahead({
            hint: true,
            highlight: true,
            minLength: 1
        },
        {
            displayKey: 'Title',
            name: 'IdOfItem',
            source: function (query, process) {
                var searchUrl = '/api/search';
                var p = { search: query, type: "l" };
                dataService.getItems(searchUrl, p)
                .success(function (data) {
                    return process(data);
                })
                .error(function () {
                    toastr.error("Error Searching");
                });
            }
        });
        $('#txtTeam1Name').on('typeahead:selected', function (evt, item) {
            $scope.editItem.Team1Id = item.IdOfItem;
            $scope.editItem.Team1Name = $(this).val();
        });
        $('#txtTeam2Name').on('typeahead:selected', function (evt, item) {
            $scope.editItem.Team2Id = item.IdOfItem;
            $scope.editItem.Team2Name = $(this).val();
        });
        $('#txtTeam1Name').blur(function () {
            if ($(this).val() !== $scope.editItem.Team1Name) {
                $scope.editItem.Team1Id = "";
                $scope.editItem.Team1Name = $(this).val();
            }
        });
        $('#txtTeam2Name').blur(function () {
            if ($(this).val() !== $scope.editItem.Team2Name) {
                $scope.editItem.Team2Id = "";
                $scope.editItem.Team2Name = $(this).val();
            }
        });
    }


    $scope.load();

    $scope.loadGameForm = function (id) {
        $scope.loadRuleSets(id);
        $scope.loadFederations(id);

        if (!id) {
            id = "Anonymous";
            $scope.editItem = {};
            $scope.newItem = true;
            $scope.editItem.TimeEntered = moment().format("YYYY-MM-DD HH:MM");
            $("#modal-edit").modal();
        }
        else {
            $scope.newItem = false;
            $scope.loadCurrentGame(id);
            spinOn();
        }

    }


    $scope.FederationChange = function (btn) {
        $scope.editItem.SanctionedByFederationEnumDisplay = $(btn).attr("title");

    }
    $scope.RulesChange = function (btn) {
        $scope.editItem.RuleSetEnumDisplay = $(btn).attr("title");
    }


    $scope.save = function () {
        if ($scope.newItem) {
            if (!$('#form').valid()) {
                return false;
            }
            $scope.saveGame();
        }
        else {
            $scope.saveGame();
        }
    }
    $scope.saveAndPublish = function () {
        $scope.editItem.IsPublished = true;
        $scope.editItem.IsApproved = true;
        if ($scope.newItem) {
            if (!$('#form').valid()) {
                return false;
            }
            $scope.saveGame();
        }
        else {
            $scope.saveGame();
        }
    }

    $scope.saveGame = function () {
        spinOn();

        dataService.addItem("/api/games", $scope.editItem)
        .success(function (data) {
            toastr.success("Game Saved");

            spinOff();
            $("#modal-edit").modal('hide');
            $scope.id = null;
            $scope.load();
        })
        .error(function () {
            toastr.error("update failed");
            spinOff();
            $("#modal-edit").modal('hide');
        });
    }


    $scope.processChecked = function (action) {
        spinOn();
        var i = $scope.items.length;
        var checked = [];
        while (i--) {
            var item = $scope.items[i];
            if (item.IsChecked === true) {
                checked.push(item);
            }
        }
        if (checked.length < 1) {
            spinOff();
            return false;
        }
        dataService.processChecked("/api/posts/processchecked/" + action, $scope.items)
		.success(function (data) {
		    $scope.load();
		    gridInit($scope, $filter);
		    toastr.success($rootScope.lbl.completed);
		    spinOff();
		})
		.error(function () {
		    toastr.error($rootScope.lbl.failed);
		    spinOff();
		});
    }

    $scope.setFilter = function (filter) {
        if ($scope.filter === 'pub') {
            $scope.gridFilter('IsPublished', true, 'pub');
        }
        if ($scope.filter === 'dft') {
            $scope.gridFilter('IsPublished', false, 'dft');
        }
    }

}]);