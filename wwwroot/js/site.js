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






// ------------------------------------------------------------------
// ------------------------------------------------------------------
// GROUP PLAY SECTION
// ------------------------------------------------------------------
// ------------------------------------------------------------------

let firstTeamName = $("#first_team_name").get();
let firstTeamScore = $("#first_team_score").get();
let firstTeamNameAndScore = $(".first_team_name_and_score").get();
let secondTeamName = $("#second_team_name").get();
let secondTeamScore = $("#second_team_score").get();
let secondTeamNameAndScore = $(".second_team_name_and_score").get();
let counterDiv = $("#counter_div").get();
let groupCluesList = $("#group_clues_list").get();

let movieObject = $.get("group/get_movie_info_object", function (res) { });




$(document).ready(function ()
{
  new GetNextGroupClue();
  new GuessGroupMovie();
})



function SendClueNumberToController(element)
{
  // console.log('SendClueNumberToController');
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

function GetNextGroupClue()
{
  $("#group_clues_list").click(function ()
  {
    // console.log("clicked group clues list");

    $.get("group/get_movie_clues_object", function (res)
    {
      new GetTextFromGroupGuessBox();

      let allClues = res.clues;
      let oneClue = allClues[index];
      let clueText = oneClue.clueText;
      let clueDifficulty = oneClue.clueDifficulty;
      let cluePoints = oneClue.cluePoints;

      // $(counterDiv).empty().append('<div>' + 'Clue: ' + clueNumber.toString() + '</div>');

      if (clueNumber == 1)
      {
        $(firstTeamNameAndScore).toggleClass("this_teams_turn");
        $(groupCluesList).empty();
      }

      if (clueNumber % 2 == 0)
      {
        $(firstTeamNameAndScore).toggleClass("this_teams_turn");
        $(secondTeamNameAndScore).toggleClass("this_teams_turn");
      }

      clueNumber++;

      let logClue = 0;
      if (logClue == 1)
      {
        console.log("----------------------------------------");
        console.log("NEW CLUE");
        console.log("----------------------------------------");
        console.log();
        console.log("CLUE: ", clueText);
        console.log("DIFFICULTY: ", clueDifficulty);
        console.log("POINTS: ", cluePoints);
        console.log("----------------------------------------");
        console.log("----------------------------------------");
        console.log();
      }

      AppendClueToList(clueText);
      SendClueNumberToController(clueDifficulty);
      index++;
    })
  })
}


function AppendClueToList(element)
{
  var clueHtml = '<div class="group_clue">' + element + '</div>';
  $(groupCluesList).append(clueHtml);
}



function GetTextFromGroupGuessBox()
{
  $("#group_guess_input").keyup(function (event)
  {
    var searchText = $("#group_guess_input").val();
    // console.log("Search text: ", searchText);

    $("#group_search_res_list").html("");

    $.ajax(
      {
        url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
        method: "GET",
        success: function (serverResponse)
        {
          var imdbMovieInfo = serverResponse["Search"];
          // console.log("GetTextFromGroupGuessBox() : 'imdbMovieInfo' --> ", imdbMovieInfo);

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

              // console.log("GetTextFromGroupGuessBox() : 'oneResult' --> ", oneResult);
            }

            // when you click the movie title in the drop-down list, it populates the text box
            $("#group_search_res_list li").bind("click", function ()
            {
              SetGroupGuessText(this);
              console.log("this = ", this);
            });
          }
        }
      });
  });
}



function SetGroupGuessText(element)
{
  var searchText = $("#group_guess_input").val();
  console.log("SetGroupGuessText() : 'searchText' --> ", searchText);

  var value = $(element).text();
  console.log("SetGroupGuessText() : 'value' --> ", value);

  $("#group_guess_input").val(value);
  $("#group_search_res_list").empty();

  $.ajax(
    {
      url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
      method: "GET",
      success: function (serverResponse)
      {
        var imdbMovieInfo = serverResponse["Search"];
        console.log("SetGroupGuessText() : 'imdbMovieInfo' --> ", imdbMovieInfo);
      }
    });
}


function GuessGroupMovie()
{
  console.log("GuessGroupMovie()");
  $("#group_guess_button").click(function ()
  {
    console.log('clicked #group_guess_button');

    // gets text entered in html input
    // 'replace' finds any double spaces between words and trims to one space
    // 'trim' removes spaces at the end of the guess
    let movieTitleGuessed = $("#group_guess_input:first")
      .val()
      .toString()
      .replace(/\s+/g, ' ')
      .trim()
      .toUpperCase();

    console.log("movieTitleGuessed: ", movieTitleGuessed);

    // gets movie from the controller
    $.get("group/get_movie_info", function (res)
    {
      // 'replace' finds any double spaces between words and trims to one space
      // 'trim' removes spaces at the end of the guess
      let movieTitleActual = res["title"].replace(/\s+/g, ' ').trim().toUpperCase();
      console.log("movieTitleActual: ", movieTitleActual);

      if (movieTitleGuessed == movieTitleActual)
      {
        console.log("you win the game");
      }

      else
      {
        console.log("you lose the game");
      }

    })
  })
}






function CheckIfGuessIsCorrect()
{
  let testString = "test";
  return testString;
}








// $("#change_team_button").click(function (selector)
// {
//   $(counterDiv).empty().append('<div>' + 'Clue: ' + clueNumber.toString() + '</div>');

//   if (clueNumber == 1)
//     $(firstTeamNameAndScore).toggleClass("this_teams_turn");

//   if (clueNumber % 2 == 0)
//   {
//     $(firstTeamNameAndScore).toggleClass("this_teams_turn");
//     $(secondTeamNameAndScore).toggleClass("this_teams_turn");
//   }
//   SendClueNumberToController(clueNumber);
//   clueNumber++;
// })