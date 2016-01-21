var Sponsors = new function () {
	var thisViewModel = this;
	thisViewModel.usedCount = ko.observable();
	thisViewModel.sponsorId = "";
	thisViewModel.leagueGuidId = "";

	this.Initialize = function (UsedCount, SponsorId, LeagueId) {
		var self = this;
		thisViewModel.usedCount(UsedCount);
		thisViewModel.sponsorId = SponsorId;
		thisViewModel.leagueGuidId = LeagueId;
	}

    this.InitializeAddSponsorship = function() {
        tinymce.init({
            mode: "textareas",
            elements: "elm2",
            theme: "modern",
            plugins: "layer,table,preview,media,contextmenu,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,link",
        });
        $("#ExpiresDate").datepicker();
        $("#Price").numeric({ negative: false });
    };
    
    this.InitializeEditSponsorship = function() {
        tinymce.init({
            mode: "textareas",
            elements: "elm2",
            theme: "modern",
            plugins: "layer,table,preview,media,contextmenu,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,link",
        });
        $("#ExpiresDate").datepicker();
        $("#Price").numeric({ negative: false });
    }

	this.UseCode = function (btn) {
		var url = '/Sponsor/UseCode';
		$.ajax({
			url: url,
			dataType: 'json',
			contentType: 'application/json',
			data: { id: this.sponsorId, leagueId: this.leagueGuidId },
			cache: false,
			success: function (data) {
				thisViewModel.usedCount(data);
				$('.bottom-right').notify({
					message: { text: 'Successfully Used' },
					type: "success",
					fadeOut: { enabled: true, delay: 4000 }
				}).show();
			},
			error: function () {
				$('.bottom-right').notify({
					message: { text: 'An error has occurred' },
					type: "danger",
					fadeOut: { enabled: true, delay: 4000 }
				}).show();
			}
		});
	}	
}