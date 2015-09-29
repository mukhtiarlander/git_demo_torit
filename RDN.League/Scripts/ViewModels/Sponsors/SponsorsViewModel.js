var Sponsors = new function () {
	var thisViewModel = this;
	
}

function SponsorsViewModel(UsedCount, SponsorId, LeagueId) {
	var self = this;
	self.usedCount = ko.observable(UsedCount);
	self.sponsorId = SponsorId;
	self.leagueGuidId = LeagueId;

	this.useCode = function (btn) {
		var url = '/Sponsor/UseCode';
		$.ajax({
			url: url,
			dataType: 'json',
			contentType: 'application/json',
			data: { id: this.sponsorId, leagueId: this.leagueGuidId },
			cache: false,
			success: function (data) {
				self.usedCount(data);
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
	};

	return self;
}