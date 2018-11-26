$(document).ready(function ()
{
  $("#remainingGuesses").append("3");
  // CloneDiv();

  $("#buttonText").on("click", function ()
  {
    var el = $(this);
    if (el.text() == el.data("text-swap"))
    {
      el.text(el.data("text-original"));
    } else
    {
      el.data("text-original", el.text());
      el.text(el.data("text-swap"));
    }
  });

  var divClone = $("#formContainer").clone(true);
});


function CloneDiv()
{
  var divClone = $("#formContainer").clone(true);
  $("#getClueButton").click(function ()
  {
    $("#formContainer").empty().replaceWith(divClone.clone(true));
  });

  $("#GuessAgainNoCluesLeft").click(function ()
  {
    $("#formContainer").empty().replaceWith(divClone.clone(true));
  });
}



//#region [Red]
$("#guessButton").click(function ()
{
  console.log('clicked #guessButton');
  var playersGuess = $("input:first")
    .val()
    .toString()
    .toUpperCase();

  console.log("Players guess: ", playersGuess);


  $.get("single/guess_movie", function (res)
  {
    var currentPoints = Number($("#justPoints").html());
    var thisGamesMovieTitle = res[0].toUpperCase();
    var guessCount = res[1];

    console.log("Movie title: ", thisGamesMovieTitle);

    // the player WON
    if (playersGuess == thisGamesMovieTitle)
    {
      console.log("OUTCOME: the player WON");
      UpdatePlayerPoints(currentPoints);
      ViewSingleGameOverWinPage();
    }

    // wrong guess
    else
    {
      // wrong guess; player still has guesses
      if (guessCount > 0)
      {
        console.log("OUTCOME: WRONG guess; guesses remaining");
        UpdateMovieGuessInput();
        RespondToWrongGuessContinueGame(guessCount, playersGuess);
      }

      // out of guesses and the player lost
      if (guessCount == 0)
      {
        console.log("OUTCOME: player lost");
        UpdatePlayerPoints(0);
        ViewSingleGameOverLossPage();
      }
    }
  });
});
//#endregion

function GetTextFromGuessBox()
{
  $("#movieGuessInput").keyup(function (event)
  {
    // 'godfa' OR 'godfathe' OR 'godfather' etc.
    var searchText = $("#movieGuessInput").val();

    $("#searchRes").html("");

    $.ajax(
      {
        url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
        method: "GET",
        success: function (serverResponse)
        {
          // returns array of movie objects (e.g., title, release year) that meet search criteria
          var imdbMovieInfo = serverResponse["Search"];

          if (imdbMovieInfo.length > 0)
          {
            $("#searchResult").empty();
            for (var h = 0; h < imdbMovieInfo.length; h++)
            {
              // returns any movie with search term in it (e.g., 'good' leads to 'good will hunting' AND 'a few good men')
              var oneResult = imdbMovieInfo[h]["Title"];
              $("#searchResult").append(
                '<li class="text-danger">' + oneResult + "</li>"
              );
            }
            // when you click the movie title in the drop-down list, it populates the text box
            $("#searchResult li").bind("click", function ()
            {
              SetGuessText(this);
            });
          }
        }
      });
  });
}


function SetGuessText(element)
{
  // 'godfa' OR 'godfathe' OR 'godfather' etc.
  var searchText = $("#movieGuessInput").val();
  console.log("searchText is: ", searchText);

  // movie title selected from dropdown
  var value = $(element).text();

  $("#movieGuessInput").val(value);
  $("#searchResult").empty();

  $.ajax(
    {
      url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
      method: "GET",
      success: function (serverResponse)
      {
        // returns array of movie objects (e.g., title, release year) that meet search criteria
        var imdbMovieInfo = serverResponse["Search"];
      }
    });
}


//#region [RegionRed]
let hello;
//#endregion

function EmptyAndAppendRemainingGuesses(string)
{
  $("#remainingGuesses").empty().append(string);
}


function UpdateMovieGuessInput()
{
  $("#movieGuessInput").val("");
}


function SendGuessAgainMessage(str)
{
  var responseToWrongGuess = '<span><b>' + str + '</b>' + ' is WRONG. ';
  var guessAgainMessage = responseToWrongGuess + 'Guess Again.</span>';
  $("ul#guessResponse").empty().append(guessAgainMessage);
}


function RespondToWrongGuessContinueGame(guessCount, playersGuess)
{
  EmptyAndAppendRemainingGuesses(guessCount);
  $(".hiddenDiv").empty().append(guessCount);
  SendGuessAgainMessage(playersGuess);
}

// -----> END REGION : GUESSING <-----
// ----------------------------------------------







// ----------------------------------------------
// -----> REGION : CLUES <-----

$("#getClueButton").click(function ()
{
  $.get("single/get_clue", function (res)
  {
    $("#clueButtonText").empty().append("Get Clue");
    $("ul#guessResponse").empty();

    // new CloneDiv();
    new GetTextFromGuessBox();

    // var HiddenObject = document.getElementById("hiddenDiv");
    var comparisonString = $(".hiddenDiv").get(0).innerHTML;
    var comparisonNumber = Number(comparisonString);

    if (comparisonNumber > 0)
    {
      $("#remainingGuesses").empty().append(comparisonString);
    }

    // CONTENT LENGTH --> starts at -1 and counts up; error after last clue
    // CLUE POINTS --> count down from 10
    var allMovieClues = res.clues;
    var contentLength = $("ul#clueText > li").length - 1;
    var clueDifficulty = res.clues[contentLength + 1].clueDifficulty;
    var cluePoints = res.clues[contentLength + 1].cluePoints;

    if (clueDifficulty == 1)
    {
      console.table(allMovieClues);
    }

    var currentClue = allMovieClues[contentLength + 1].clueText;

    new SendClueToController(currentClue);

    // lists off each clue  after each click; e.g., '<li>School bus</li>' etc.
    var clueListItem = "<li>" + allMovieClues[contentLength + 1].clueText + "</li>";

    $("ul#clueText").append(clueListItem);
    $("#cluePoints").empty().append("Clue Value: ");
    $("#justPoints").empty().append(cluePoints);

    if (contentLength == 8)
    {
      DisableGetNextClueButton();
    }
  });
});

function SendClueToController(element)
{
  $.ajax(
    {
      type: "GET",
      url: "single/get_clue_from_javascript",
      data:
      {
        ClueText: element
      },
      success: function (data)
      {
        console.log("SendClueToController(element): ", element);
        // console.log("TO CONTROLLER: " + element);
      },
      error: function (jqXHR, textStatus, errorThrown)
      {
        console.log("ERROR: CLUE NOT SENT TO CONTROLLER");
      }
    });
}

function DisableGetNextClueButton()
{
  $("#getClueButton").prop("disabled", true);
}

// -----> ENDREGION : CLUES <-----
// ----------------------------------------------




// ----------------------------------------------
// -----> REGION : HINTS <-----

$("#GetMovieDecadeButton").click(function (event)
{
  $.get("single/get_movie_release_year", function (res)
  {
    AppendHint('Decade: ' + res);
  });
});


$("#GetMovieGenreButton").click(function (event)
{
  console.log("EVENT: ", event);
  $.get("single/get_movie_genre", function (res)
  {
    AppendHint('Genre: ' + res);
  });
});


$("#GetMovieDirectorButton").click(function (event)
{
  $.get("single/get_movie_director", function (res)
  {
    AppendHint('Director: ' + res);
  });
});


function AppendHint(string)
{
  console.log("appending hint: ", string);
  var clueHtml = '<div class="col text-center">' + string + '</div>';
  $("#MovieHints").empty().append(clueHtml);
};

// -----> ENDREGION : HINTS <-----
// ----------------------------------------------



// ----------------------------------------------
// -----> REGION : SEND POINTS TO CONTROLLER <-----


function UpdatePlayerPoints(element)
{
  $.ajax(
    {
      type: "POST",
      url: "update_player_points",
      data: {
        cluePoints: $("#justPoints").html(),
        MovieId: element
      },
      success: function (data)
      {
        console.log("PLAYER POINTS SUCCESSFULLY UPDATED");
        console.log(" -- element: ", element);
      },
      error: function (jqXHR, textStatus, errorThrown) { }
    });
}


function ViewSingleGameOverWinPage()
{
  $.ajax(
    {
      type: "GET",
      url: "single/game_over_win",
      success: function ()
      {
        window.location.href = "single/game_over_win";
      },
      error: function ()
      {
        console.log('failed directing to game_over_win');
      }
    });
}

function ViewSingleGameOverLossPage()
{
  $.ajax(
    {
      type: "GET",
      url: "single/game_over_loss",
      success: function ()
      {
        window.location.href = "single/game_over_loss";
      },
      error: function ()
      {
        console.log('failed directing to game_over_loss');
      }
    });
}

// ----------------------------------------------
// -----> ENDREGION : SEND POINTS TO CONTROLLER <-----





// ---------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------
// GROUP PLAY SECTION
// ---------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------



$(document).ready(function ()
{
  new GetNextGroupClue();
  new GuessGroupMovie();

  $.get("group/send_team_ids_to_javascript", function (res)
  {
    firstTeamId = res[0];
    secondTeamId = res[1];
  })

  // console.log(firstTeamScore);
  // console.log(typeof (firstTeamScore));

  // console.log(secondTeamScore);
  // console.log(typeof (secondTeamScore));
})


// #region [Yellow] MANAGE TEAMS
// ---------------------------------------------------------------------------------------




let firstTeamNameHtml = $("#first_team_name").get();
let firstTeamName = firstTeamNameHtml[0].innerText.trim();
let firstTeamScoreHtml = $("#first_team_score").get();
let firstTeamScoreString = firstTeamScoreHtml[0].innerText.trim();
let firstTeamScore = Number(firstTeamScoreString);
let firstTeamNameAndScoreHtml = $(".first_team_name_and_score").get();
let firstTeamNameAndScore = firstTeamNameAndScoreHtml[0].innerText.trim();
let firstTeamId;

let secondTeamNameHtml = $("#second_team_name").get();
let secondTeamName = secondTeamNameHtml[0].innerText.trim();
let secondTeamScoreHtml = $("#second_team_score").get();
let secondTeamScoreString = secondTeamScoreHtml[0].innerText.trim();
let secondTeamScore = Number(secondTeamScoreString);
let secondTeamNameAndScoreHtml = $(".second_team_name_and_score").get();
let secondTeamNameAndScore = secondTeamNameAndScoreHtml[0].innerText.trim();
let secondTeamId;

let currentTeamName;
let currentTeamId;
let winningTeamName;
let winningTeamId;

let groupGuessButtons = $("#group_guess_buttons").get();
let groupGuessButton = $("#group_guess_button").get();


function GetCurrentTeamGuessing()
{
  if (clueNumber == 1 || clueNumber == 4 || clueNumber == 5 || clueNumber == 8 || clueNumber == 9)
  {
    currentTeamName = firstTeamName;
    currentTeamId = firstTeamId;
    winningTeamId = currentTeamId;
  }

  if (clueNumber == 2 || clueNumber == 3 || clueNumber == 6 || clueNumber == 7 || clueNumber == 10)
  {
    currentTeamName = secondTeamName;
    currentTeamId = secondTeamId;
    winningTeamId = currentTeamId;
  }
  return currentTeamName;
}



// ---------------------------------------------------------------------------------------
// #endregion MANAGE TEAMS


// #region [Blue] MANAGE CLUES
// ---------------------------------------------------------------------------------------

let groupCluesList = $("#group_clues_list").get();

function SendClueNumberToController(element)
{
  $.ajax({
    type: "GET",
    url: "group/get_clue_number_from_javascript",
    data: { ClueNumber: element },
    success: function (data) { },
    error: function (jqXHR, textStatus, errorThrown) { }
  })
}


let clueNumber = 1;
let index = 0;
let cluePoints = 10;

function GetNextGroupClue()
{
  $("#group_clues_list").click(function ()
  {
    if (clueNumber >= 10)
    {

      ManageGameOutOfClues();
      // console.log("disable clues list");
      // console.log(clueNumber);
      // $("#group_clues_list").prop("disabled", true);
      // $(groupGuessButtons).empty();

      // var newGroupGuessButton = '<button id="group_guess_button" class="btn group_guess_buttons" style="width: 50%">Click to Guess</button>';

      // var groupQuitButton = '<button id="group_quit_button" class="btn btn-danger group_guess_buttons" style="width: 50%">Quit Game and Admit Failure</button>';

      // $(groupGuessButtons).append(newGroupGuessButton);
      // $(groupGuessButtons).append(groupQuitButton);
    }

    $.get("group/get_movie_clues_object", function (res)
    {
      new GetTextFromGroupGuessBox();

      let allClues = res.clues;
      let oneClue = allClues[index];
      let clueText = oneClue.clueText;
      let clueDifficulty = oneClue.clueDifficulty;
      cluePoints = oneClue.cluePoints

      if (clueNumber == 1)
      {
        // console.table(allClues);
        $(firstTeamNameAndScoreHtml).toggleClass("this_teams_turn");
        $(groupCluesList).empty();
      }

      if (clueNumber % 2 == 0)
      {
        $(firstTeamNameAndScoreHtml).toggleClass("this_teams_turn");
        $(secondTeamNameAndScoreHtml).toggleClass("this_teams_turn");
      }

      clueNumber++;

      PrintClueDetails(clueText, clueDifficulty);
      AppendClueToList(clueText);
      SendClueNumberToController(clueDifficulty);

      index++;
    })
  })
}


function PrintClueDetails(text, difficulty)
{
  let logClue = 0;
  if (logClue == 1)
  {
    console.log("----------------------------------------");
    console.log("NEW CLUE");
    console.log("----------------------------------------");
    console.log();
    console.log("CLUE: ", text);
    console.log("DIFFICULTY: ", difficulty);
    console.log("POINTS: ", cluePoints);
    console.log("----------------------------------------");
    console.log("----------------------------------------");
    console.log();
  }
}


function AppendClueToList(element)
{
  var clueHtml = '<div class="group_clue">' + element + '</div>';
  $(groupCluesList).append(clueHtml);
}

function ManageGameOutOfClues()
{
  console.log("disable clues list");
  console.log(clueNumber);
  $("#group_clues_list").prop("disabled", true);
  $(groupGuessButtons).empty();

  var newGroupGuessButton = '<button id="group_guess_button" class="btn group_guess_buttons" style="width: 50%">Click to Guess</button>';

  var groupQuitButton = '<button id="group_quit_button" class="btn btn-danger group_guess_buttons" style="width: 50%">Quit Game and Admit Failure</button>';

  $(groupGuessButtons).append(newGroupGuessButton);
  $(groupGuessButtons).append(groupQuitButton);
}

// ---------------------------------------------------------------------------------------
// #endregion MANAGE CLUES




// #region [Purple] MANAGE GUESSES
// ---------------------------------------------------------------------------------------

// let movieObject = $.get("group/get_movie_info_object", function (res) { });

let firstTeamGuesses = 0;
let secondTeamGuesses = 0;

function GetTextFromGroupGuessBox()
{
  $("#group_guess_input").keyup(function (event)
  {
    var searchText = $("#group_guess_input").val();

    $("#group_search_res_list").html("");

    $.ajax(
      {
        url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
        method: "GET",
        success: function (serverResponse)
        {
          var imdbMovieInfo = serverResponse["Search"];

          if (imdbMovieInfo.length > 0)
          {
            $("#group_search_res_list").empty();

            for (var h = 0; h < imdbMovieInfo.length; h++)
            {
              // returns any movie with search term in it (e.g., 'good' leads to 'good will hunting' AND 'a few good men')
              var oneResult = imdbMovieInfo[h]["Title"];
              $("#group_search_res_list").append(
                '<li class="text-danger">' + oneResult + "</li>"
              );
            }
            // click movie title in drop-down list and it populates the input box
            $("#group_search_res_list li").bind("click", function ()
            {
              SetGroupGuessText(this);
            });
          }
        }
      });
  });
}


function SetGroupGuessText(element)
{
  var searchText = $("#group_guess_input").val();
  var value = $(element).text();

  $("#group_guess_input").val(value);
  $("#group_search_res_list").empty();

  $.ajax(
    {
      url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
      method: "GET",
      success: function (serverResponse) { var imdbMovieInfo = serverResponse["Search"]; }
    });
}


function GuessGroupMovie()
{
  $("#group_guess_button").click(function ()
  {
    // gets text entered in html input
    // --- 'replace' finds any double spaces between words and trims to one space
    // --- 'trim' removes spaces at the end of the guess
    let movieTitleGuessed = $("#group_guess_input:first")
      .val()
      .toString()
      .replace(/\s+/g, ' ')
      .trim()
      .toUpperCase();

    // gets movie from the controller
    $.get("group/get_movie_info", function (res)
    {
      // 'replace' finds any double spaces between words and trims to one space
      // 'trim' removes spaces at the end of the guess
      let movieTitleActual = res["title"].replace(/\s+/g, ' ').trim().toUpperCase();
      let movieId = res["movieId"];

      // the guess was correct
      if (movieTitleGuessed == movieTitleActual)
      {
        ManageGameWonOutcome(movieId);
        ResponseToCorrectGuess();
      }

      // the guess was wrong
      else
      {
        RespondToWrongGuess();
      }

    })
  })
}

function ResponseToCorrectGuess()
{
  alert("correct");
}


function RespondToWrongGuess()
{
  currentTeamName = GetCurrentTeamGuessing().trim();

  if (currentTeamName == firstTeamName)
    firstTeamGuesses++;

  if (currentTeamName == secondTeamName)
    secondTeamGuesses++;

  let firstTeamGuessesResponse = firstTeamName + " has guessed " + firstTeamGuesses + " times";
  let secondTeamGuessesResponse = secondTeamName + " has guessed " + secondTeamGuesses + " times";
  // console.log(firstTeamGuessesResponse);

  alert("That guess is WRONG (and embarrassing)\n" + firstTeamGuessesResponse + "\n" + secondTeamGuessesResponse);
}

// ---------------------------------------------------------------------------------------
// #endregion MANAGE GUESSES




// #region [Green] MANAGE GAME OUTCOMES
// ---------------------------------------------------------------------------------------


function ManageGameWonOutcome(movieId)
{
  // winningTeamName = GetCurrentTeamGuessing().trim();
  SendWinningGameOutcomeToController(winningTeamId, cluePoints, movieId);
  IdentifyWinningTeam();

  SendGameTeamJoinInfoToController(winningTeamId, true, cluePoints, clueNumber);
  // first team is the winner
  // if (winningTeamName == firstTeamName)
  // {
  //   // console.log("FIRST TEAM WON --> ", winningTeamName, "[ ", winningTeamId, " ]", "WON", cluePoints, "points");
  //   firstTeamScore = firstTeamScore + cluePoints;
  //   $(firstTeamScoreHtml).empty().append(firstTeamScore.toString());
  // }

  // // second team is the winner
  // if (winningTeamName == secondTeamName)
  // {
  //   // console.log("SECOND TEAM WON --> ", winningTeamName, "[ ", winningTeamId, " ]", "WON", cluePoints, "points");
  //   secondTeamScore = secondTeamScore + cluePoints;
  //   $(secondTeamScoreHtml).empty().append(secondTeamScore.toString());
  // }
}

function IdentifyWinningTeam()
{
  winningTeamName = GetCurrentTeamGuessing().trim();
  // first team is the winner
  if (winningTeamName == firstTeamName)
  {
    // console.log("FIRST TEAM WON --> ", winningTeamName, "[ ", winningTeamId, " ]", "WON", cluePoints, "points");
    firstTeamScore = firstTeamScore + cluePoints;
    $(firstTeamScoreHtml).empty().append(firstTeamScore.toString());
  }

  // second team is the winner
  if (winningTeamName == secondTeamName)
  {
    // console.log("SECOND TEAM WON --> ", winningTeamName, "[ ", winningTeamId, " ]", "WON", cluePoints, "points");
    secondTeamScore = secondTeamScore + cluePoints;
    $(secondTeamScoreHtml).empty().append(secondTeamScore.toString());
  }
}


function SendGameTeamJoinInfoToController(teamId, winLoseBool, pointsReceived, clueGameWonAt)
{
  console.log("SENDING INFO FOR JOIN");
  $.ajax(
    {
      type: "POST",
      url: "group/create_game_team_join",
      data: {
        TeamId: teamId,
        WinLoss: winLoseBool,
        PointsReceived: pointsReceived,
        ClueGameWonAt: clueGameWonAt
      },
      success: function (data)
      {
        console.log(data.TeamId);
        console.log(data.WinLoss);
        console.log(data.PointsReceived);
        console.log(data.ClueGameWonAt);
      }
    }
  )
}



function SendWinningGameOutcomeToController(teamId, pointsWon, movieId)
{
  // console.log("SendWinningGameOutcomeToController():", teamId, "", pointsWon);

  $.ajax(
    {
      type: "POST",
      url: "group/get_winning_game_outcome_from_javascript",
      data: {
        cluePoints: pointsWon,
        winningTeamId: teamId,
        movieId: movieId
      },
      success: function (data)
      {
        // console.log("SENT WINNING OUTCOME TO CONTROLLER");
        // console.log("DATA: ", data.cluePoints);
      },
      error: function (jqXHR, textStatus, errorThrown) { }
    });
}




// ---------------------------------------------------------------------------------------
// #endregion MANAGE GAME OUTCOMES












// winningTeamName = GetCurrentTeamGuessing().trim();
// SendWinningGameOutcomeToController(winningTeamId, cluePoints, movieId);

// // first team is the winner
// if (winningTeamName == firstTeamName)
// {
//   // console.log("FIRST TEAM WON --> ", winningTeamName, "[ ", winningTeamId, " ]", "WON", cluePoints, "points");
//   firstTeamScore = firstTeamScore + cluePoints;
//   $(firstTeamScoreHtml).empty().append(firstTeamScore.toString());
// }

// // second team is the winner
// if (winningTeamName == secondTeamName)
// {
//   // console.log("SECOND TEAM WON --> ", winningTeamName, "[ ", winningTeamId, " ]", "WON", cluePoints, "points");
//   secondTeamScore = secondTeamScore + cluePoints;
//   $(secondTeamScoreHtml).empty().append(secondTeamScore.toString());
// }









// console.log("WINNING TEAM: ", winningTeamId, " ", winningTeamName);
// console.log("CURRENT TEAM: ", currentTeamId, " ", currentTeamName);
// console.log("FIRST TEAM: ", firstTeamId, " ", firstTeamName);
// console.log("SECOND TEAM: ", secondTeamId, " ", secondTeamName);



// function EmptyRemainingGuesses()
// {
//   $("#remainingGuesses").empty();
// }


// function DisableGuessButton()
// {
//   $("#guessButton").prop("disabled", true);
// }


// function EmptyGuessResponseList()
// {
//   $("ul#guessResponse").empty();
// }