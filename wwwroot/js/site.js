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


// function RespondToCorrectGuess(currentPoints)
// {
//   console.log('RespondToCorrectGuess');
//   UpdatePlayerPoints("#guessMovieForm", currentPoints);

//   $.get("single/game_over_win", function (res)
//   {
//     console.log("single/game_over_win");
//     // console.log(res);
//   })
// }


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
        // console.log(" -- data.cluePoints: ", data.cluePoints);
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



$("#change_team_button").click(function (selector)
{
  console.log(firstTeamDiv);
  console.log(secondTeamDiv);
  $(firstTeamDiv).toggleClass("this_teams_turn");
  $(secondTeamDiv).toggleClass("this_teams_turn");

})


let firstTeamDiv = $("#firstTeam").get();
let secondTeamDiv = $("#secondTeam").get();

















// ----------------------------------------------
// ----------------------------------------------
// ----------------------------------------------

// function ResetForm()
// {
//   var divClone = $("#formContainer").clone(true);
//   $("#getClueButton").click(function ()
//   {
//     $("#formContainer").empty().replaceWith(divClone.clone(true));
//   });

//   $("#GuessAgainNoCluesLeft").click(function ()
//   {
//     $("#formContainer").empty().replaceWith(divClone.clone(true));
//   });

//   // console.log("FORM WAS RESET");
//   var toggle = document.getElementById("toggle");
//   var wrapper = document.querySelectorAll(".subscribe");
//   var submit = document.getElementById("submit");
//   var success = document.querySelectorAll(".subscribe__success");
//   var content = document.querySelectorAll(".subscribe__wrapper");
//   // console.log(toggle);
//   // console.log(wrapper);
//   // console.log(submit);
//   // console.log(success);
//   // console.log(content);

//   toggle.addEventListener("click", function ()
//   {
//     this.className += " subscribe__toggle__hidden";
//     wrapper[0].className += " subscribe-1__active";
//   });

//   submit.addEventListener("click", function ()
//   {
//     success[0].className += " subscribe__success--active";
//     wrapper[0].className += " subscribe-1__success";
//     //   content[0].style.display = "none";
//   });
// }


// $(function ()
// {
//   $('[data-toggle="tooltip"]').tooltip();
// });



// if (guessCount > 0)
// {
//   UpdateMovieGuessInput();

//   // guesses remaining; the player WON
//   if (playersGuess == thisGamesMovieTitle)
//   {
//     RespondToCorrectGuess(currentPoints);
//     // EmptyRemainingGuesses();
//     // DisableGuessButton();
//     // UpdatePlayerPoints("#guessMovieForm", currentPoints);
//     // ShowBootBoxResponse(currentPoints);
//   }
//   // guesses remaining; the player's guess was WRONG
//   else
//   {
//     RespondToWrongGuessContinueGame(guessCount, playersGuess);
//     // EmptyAndAppendRemainingGuesses(guessCount);
//     // $(".hiddenDiv").empty().append(guessCount);
//     // SendGuessAgainMessage(playersGuess);
//   }
// }


// if (guessCount == 0)
// {
//   DisableGetNextClueButton();
//   UpdateMovieGuessInput();

//   if (playersGuess == thisGamesMovieTitle)
//   {
//     // out of guesses and the player won
//     EmptyAndAppendRemainingGuesses("0");
//     EmptyGuessResponseList();
//     UpdatePlayerPoints("#guessMovieForm", currentPoints);
//     ShowBootBoxResponse(currentPoints);
//   }

//   else
//   {
//     // out of guesses and the player lost
//     EmptyRemainingGuesses();
//     UpdatePlayerPoints("#guessMovieForm", 0);
//     ShowBootBoxResponse(currentPoints);
//   }
// }




// function RespondToCorrectGuess(currentPoints)
// {
//   // EmptyRemainingGuesses();
//   // DisableGuessButton();
//   UpdatePlayerPoints("#guessMovieForm", currentPoints);
//   // ShowBootBoxResponse(currentPoints);

// }




// // ----------------------------------------------
// // -----> REGION : BOOTBOX MESSAGES <-----

// function ShowBootBoxResponse(int)
// {
//   var dialog = bootbox.dialog(
//     {
//       message: PlayerWonBootBoxMessage(int),
//       buttons: {
//         cancel: {
//           label: bootboxQuitButtonLabel,
//           className: bootboxQuitButtonStyle,
//           callback: function ()
//           {
//             window.location.href = "/";
//           }
//         },

//         playagain:
//         {
//           label: bootboxPlayAgainButtonLabel,
//           className: bootboxPlayAgainButtonStyle,
//           callback: function ()
//           {
//             // console.log("PLAY AGAIN");
//             window.location.href = "InitiateSinglePlayerGame";
//           }
//         }
//       }
//     });
//   return dialog;
// }

// function PlayerWonBootBoxMessage(string)
// {
//   var message = '<br><div class="endGameMessage row border border-dark"><div class="col"><span class="align-middle"><b class="endWin align-middle">Correct. You win.</b> <br> Points received: ' +
//     string +
//     '</span></div><br><div class="col"><img class="align-middle" src="https://image.tmdb.org/t/p/w92/gKDNaAFzT21cSVeKQop7d1uhoSp.jpg" alt="ASP.NET" class="img-responsive"></div></div>'

//   return message;
// }

// function PlayerLostBootBoxMessage()
// {
//   var message = '<p class="endGameMessage"><b class="endLoss">You lost</b> <br> Please take some time to reflect on your failure. </p>';
//   return message;
// }

// let bootboxQuitButtonLabel = "Quit (I'm a loser)";
// let bootboxPlayAgainButtonLabel = "Play again (I'm a nerd)";

// let bootboxQuitButtonStyle = "btn-danger endButton";
// let bootboxPlayAgainButtonStyle = "btn-success endButton";


// // -----> ENDREGION : BOOTBOX MESSAGES <-----
// // ----------------------------------------------








// $("#guessMovie").submit(function (event)
// {
//   var playersGuess = $("input:first")
//     .val()
//     .toString()
//     .toUpperCase();

//   console.log("Players guess: ", playersGuess);

//   $.get("single/guess_movie", function (res)
//   {
//     var currentPoints = Number($("#justPoints").html());
//     var thisGamesMovieTitle = res[0].toUpperCase();
//     var guessCount = res[1];

//     console.log("Movie title: ", thisGamesMovieTitle);

//     if (guessCount > 0)
//     {
//       UpdateMovieGuessInput();

//       // guesses remaining; the player WON
//       if (playersGuess == thisGamesMovieTitle)
//       {
//         UpdatePlayerPoints("#guessMovieForm", currentPoints);
//         ViewSingleGameOverWinPage();
//       }
//       // guesses remaining; the player's guess was WRONG
//       else
//       {
//         RespondToWrongGuessContinueGame(guessCount, playersGuess);
//       }
//     }

//     if (guessCount == 0)
//     {
//       // DisableGetNextClueButton();
//       // UpdateMovieGuessInput();

//       if (playersGuess == thisGamesMovieTitle)
//       {
//         // out of guesses and the player won
//         RespondToCorrectGuess(currentPoints);
//         ViewSingleGameOverWinPage();
//       }

//       else
//       {
//         // out of guesses and the player lost
//         // EmptyRemainingGuesses();
//         UpdatePlayerPoints("#guessMovieForm", 0);
//         // ShowBootBoxResponse(currentPoints);
//       }
//     }
//   });
//   event.preventDefault();
// });