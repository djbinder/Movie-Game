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




// ----------------------------------------------
// -----> REGION : GUESSING <-----

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


function GetTextFromGuessBox()
{
  $("#movieGuessInput").keyup(function (event)
  {
    // 'godfa' OR 'godfathe' OR 'godfather' etc.
    var searchText = $("#movieGuessInput").val();

    $("#searchRes").html("");
    // SEARCH RES HTML --> returns and object with a bunch of info about the html element (e.g., a div or list)
    // var SearchResHTML = $("#searchRes").html("");

    $.ajax(
      {
        url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
        method: "GET",
        success: function (serverResponse)
        {
          // returns array of movie objects (e.g., title, release year) that meet search criteria
          // length seems to always be 10?
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


function EmptyRemainingGuesses()
{
  $("#remainingGuesses").empty();
}


function EmptyAndAppendRemainingGuesses(string)
{
  $("#remainingGuesses").empty().append(string);
}


function DisableGuessButton()
{
  $("#guessButton").prop("disabled", true);
}


function EmptyGuessResponseList()
{
  $("ul#guessResponse").empty();
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

// -----> END REGION : GUESSING <-----
// ----------------------------------------------





function RespondToWrongGuessContinueGame(guessCount, playersGuess)
{
  EmptyAndAppendRemainingGuesses(guessCount);
  $(".hiddenDiv").empty().append(guessCount);
  SendGuessAgainMessage(playersGuess);
}

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

// function UpdatePlayerPoints(formContainer, element)
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

// ----------------------------------------------
// ----------------------------------------------
// ----------------------------------------------

var clueNumber = 1;

$("#change_team_button").click(function (selector)
{
  $(counterDiv).empty().append('<div>' + 'Clue: ' + clueNumber.toString() + '</div>');

  if (clueNumber == 1)
    $(firstTeamNameAndScore).toggleClass("this_teams_turn");

  if (clueNumber % 2 == 0)
  {
    $(firstTeamNameAndScore).toggleClass("this_teams_turn");
    $(secondTeamNameAndScore).toggleClass("this_teams_turn");
  }
  SendClueNumberToController(clueNumber);
  clueNumber++;
})

function SendClueNumberToController(element)
{
  console.log('SendClueNumberToController');
  $.ajax({
    type: "GET",
    url: "group/get_clue_number_from_javascript",
    data: { ClueNumber: element },
    success: function (data) { },
    error: function (jqXHR, textStatus, errorThrown) { }
  })
}


let firstTeamName = $("#first_team_name").get();
let firstTeamScore = $("#first_team_score").get();
let firstTeamNameAndScore = $(".first_team_name_and_score").get();
let secondTeamName = $("#second_team_name").get();
let secondTeamScore = $("#second_team_score").get();
let secondTeamNameAndScore = $(".second_team_name_and_score").get();
let counterDiv = $("#counter_div").get();


let index = 0;

$("#get_team_clue_button").click(function ()
{
  console.log("----------------------------------------");
  console.log("NEW CLUE");
  console.log("----------------------------------------");
  console.log();
  $.get("group/get_movie_clues_object", function (res)
  {
    let allClues = res.clues;
    let lengthOfAllClues = allClues.length;
    // console.log("LENGTH: ", lengthOfAllClues);

    let oneClue = allClues[index];

    let clueText = oneClue.clueText;
    console.log("CLUE: ", clueText);

    let clueDifficulty = oneClue.clueDifficulty;
    console.log("DIFFICULTY: ", clueDifficulty);

    let cluePoints = oneClue.cluePoints;
    console.log("POINTS: ", cluePoints);

    SendClueNumberToController(cluePoints);

    index++;
    console.log("----------------------------------------");
    console.log("----------------------------------------");
    console.log();

    // var movieId = res.MovieId;
    // console.log(movieId);
  })
})



// ----------------------------------------------
// ----------------------------------------------
// ----------------------------------------------