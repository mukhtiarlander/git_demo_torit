function retrieveLeagues(partialname, outputdiv) {
    $.post("/Manager/GetLeagues/" + partialname, {},
  function (data) {
      $.each(data, function (row) {
          var country = '';
          if (data[row].Country != null)
              country = data[row].Country;
          var state = '';
          if (data[row].State != null)
              state = data[row].State;

          $('#' + outputdiv + ' > tbody:last').append('<tr><td><a href="/Manager/JoinLeagueById/' + data[row].LeagueId + '">' + data[row].Name + ' [' + country + ' ' + state + ']</a></td></tr>');
      });
  }, "json");
}