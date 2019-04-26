$(document).ready(function() {
    $("#remainingGuesses").append("3"); // CloneDiv();
    $("#buttonText").on("click", function() {
        var el = $(this);
        if (el.text() == el.data("text-swap")) {
            el.text(el.data("text-original"));
        } else {
            el.data("text-original", el.text());
            el.text(el.data("text-swap"));
        }
    });
    var divClone = $("#formContainer").clone(true);
});
function CloneDiv() {
    var divClone = $("#formContainer").clone(true);
    $("#getClueButton").click(function() {
        $("#formContainer")
            .empty()
            .replaceWith(divClone.clone(true));
    });
    $("#GuessAgainNoCluesLeft").click(function() {
        $("#formContainer")
            .empty()
            .replaceWith(divClone.clone(true));
    });
}

//#region [Red]
$("#guessButton").click(function() {
    console.log("clicked #guessButton");
    var playersGuess = $("input:first")
        .val()
        .toString()
        .toUpperCase();
    console.log("Players guess: ", playersGuess);
    $.get("single/guess_movie", function(res) {
        var currentPoints = Number($("#justPoints").html());
        var thisGamesMovieTitle = res[0].toUpperCase();
        var guessCount = res[1];
        console.log("Movie title: ", thisGamesMovieTitle); // the player WON
        if (playersGuess == thisGamesMovieTitle) {
            console.log("OUTCOME: the player WON");
            UpdatePlayerPoints(currentPoints);
            ViewSingleGameOverWinPage();
        } // wrong guess
        else {
            // wrong guess; player still has guesses
            if (guessCount > 0) {
                console.log("OUTCOME: WRONG guess; guesses remaining");
                UpdateMovieGuessInput();
                RespondToWrongGuessContinueGame(guessCount, playersGuess);
            } // out of guesses and the player lost
            if (guessCount == 0) {
                console.log("OUTCOME: player lost");
                UpdatePlayerPoints(0);
                ViewSingleGameOverLossPage();
            }
        }
    });
});

//#endregion
function GetTextFromGuessBox() {
    $("#movieGuessInput").keyup(function(event) {
        // 'godfa' OR 'godfathe' OR 'godfather' etc.
        var searchText = $("#movieGuessInput").val();
        $("#searchRes").html("");
        $.ajax({
            url:
                "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
            method: "GET",
            success: function(serverResponse) {
                // returns array of movie objects (e.g., title, release year) that meet search criteria
                var imdbMovieInfo = serverResponse["Search"];
                if (imdbMovieInfo.length > 0) {
                    $("#searchResult").empty();
                    for (var h = 0; h < imdbMovieInfo.length; h++) {
                        // returns any movie with search term in it (e.g., 'good' leads to 'good will hunting' AND 'a few good men')
                        var oneResult = imdbMovieInfo[h]["Title"];
                        $("#searchResult").append(
                            '<li class="text-danger">' + oneResult + "</li>"
                        );
                    } // when you click the movie title in the drop-down list, it populates the text box
                    $("#searchResult li").bind("click", function() {
                        SetGuessText(this);
                    });
                }
            }
        });
    });
}

function SetGuessText(element) {
    // 'godfa' OR 'godfathe' OR 'godfather' etc.
    var searchText = $("#movieGuessInput").val();
    console.log("searchText is: ", searchText); // movie title selected from dropdown
    var value = $(element).text();
    $("#movieGuessInput").val(value);
    $("#searchResult").empty();
    $.ajax({
        url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
        method: "GET",
        success: function(serverResponse) {
            // returns array of movie objects (e.g., title, release year) that meet search criteria
            var imdbMovieInfo = serverResponse["Search"];
        }
    });
}

function EmptyAndAppendRemainingGuesses(string) {
    $("#remainingGuesses")
        .empty()
        .append(string);
}

function UpdateMovieGuessInput() {
    $("#movieGuessInput").val("");
}

function SendGuessAgainMessage(str) {
    var responseToWrongGuess = "<span><b>" + str + "</b>" + " is WRONG. ";
    var guessAgainMessage = responseToWrongGuess + "Guess Again.</span>";
    $("ul#guessResponse")
        .empty()
        .append(guessAgainMessage);
}

function RespondToWrongGuessContinueGame(guessCount, playersGuess) {
    EmptyAndAppendRemainingGuesses(guessCount);
    $(".hiddenDiv")
        .empty()
        .append(guessCount);
    SendGuessAgainMessage(playersGuess);
}

// -----> END REGION : GUESSING <-----
// ----------------------------------------------

// ----------------------------------------------
// -----> REGION : CLUES <-----
$("#getClueButton").click(function() {
    $.get("single/get_clue", function(res) {
        $("#clueButtonText")
            .empty()
            .append("Get Clue");
        $("ul#guessResponse").empty(); // new CloneDiv();
        new GetTextFromGuessBox(); // var HiddenObject = document.getElementById("hiddenDiv");
        var comparisonString = $(".hiddenDiv").get(0).innerHTML;
        var comparisonNumber = Number(comparisonString);
        if (comparisonNumber > 0) {
            $("#remainingGuesses")
                .empty()
                .append(comparisonString);
        } // CONTENT LENGTH --> starts at -1 and counts up; error after last clue
        // CLUE POINTS --> count down from 10
        var allMovieClues = res.clues;
        var contentLength = $("ul#clueText > li").length - 1;
        var clueDifficulty = res.clues[contentLength + 1].clueDifficulty;
        var cluePoints = res.clues[contentLength + 1].cluePoints;
        if (clueDifficulty == 1) {
            console.table(allMovieClues);
        }
        var currentClue = allMovieClues[contentLength + 1].clueText;
        new SendClueToController(currentClue); // lists off each clue  after each click; e.g., '<li>School bus</li>' etc.
        var clueListItem =
            "<li>" + allMovieClues[contentLength + 1].clueText + "</li>";
        $("ul#clueText").append(clueListItem);
        $("#cluePoints")
            .empty()
            .append("Clue Value: ");
        $("#justPoints")
            .empty()
            .append(cluePoints);
        if (contentLength == 8) {
            DisableGetNextClueButton();
        }
    });
});
function SendClueToController(element) {
    $.ajax({
        type: "GET",
        url: "single/get_clue_from_javascript",
        data: {
            ClueText: element
        },
        success: function(data) {
            console.log("SendClueToController(element): ", element); // console.log("TO CONTROLLER: " + element);
        },
        error: function(jqXHR, textStatus, errorThrown) {
            console.log("ERROR: CLUE NOT SENT TO CONTROLLER");
        }
    });
}

function DisableGetNextClueButton() {
    $("#getClueButton").prop("disabled", true);
}

// -----> ENDREGION : CLUES <-----
// ----------------------------------------------
// ----------------------------------------------
// -----> REGION : HINTS <-----
$("#GetMovieDecadeButton").click(function(event) {
    $.get("single/get_movie_release_year", function(res) {
        AppendHint("Decade: " + res);
    });
});
$("#GetMovieGenreButton").click(function(event) {
    console.log("EVENT: ", event);
    $.get("single/get_movie_genre", function(res) {
        AppendHint("Genre: " + res);
    });
});
$("#GetMovieDirectorButton").click(function(event) {
    $.get("single/get_movie_director", function(res) {
        AppendHint("Director: " + res);
    });
});
function AppendHint(string) {
    console.log("appending hint: ", string);
    var clueHtml = '<div class="col text-center">' + string + "</div>";
    $("#MovieHints")
        .empty()
        .append(clueHtml);
}

// -----> ENDREGION : HINTS <-----
// ----------------------------------------------
// ----------------------------------------------
// -----> REGION : SEND POINTS TO CONTROLLER <-----
function UpdatePlayerPoints(element) {
    $.ajax({
        type: "POST",
        url: "update_player_points",
        data: {
            cluePoints: $("#justPoints").html(),
            MovieId: element
        },
        success: function(data) {
            console.log("PLAYER POINTS SUCCESSFULLY UPDATED");
            console.log(" -- element: ", element);
        },
        error: function(jqXHR, textStatus, errorThrown) {}
    });
}

function ViewSingleGameOverWinPage() {
    $.ajax({
        type: "GET",
        url: "single/game_over_win",
        success: function() {
            window.location.href = "single/game_over_win";
        },
        error: function() {
            console.log("failed directing to game_over_win");
        }
    });
}

function ViewSingleGameOverLossPage() {
    $.ajax({
        type: "GET",
        url: "single/game_over_loss",
        success: function() {
            window.location.href = "single/game_over_loss";
        },
        error: function() {
            console.log("failed directing to game_over_loss");
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

// let firstTeam;
// let secondTeam;

// var responseFromController = $.get("group/send_team_ids_to_javascript", function (res)
// {
//   firstTeam = {
//     id: res[0],
//     name: res[2],
//     points: res[4]
//   };
//   console.log("firstTeam: ", firstTeam);

//   secondTeam = {
//     id: res[1],
//     name: res[3],
//     points: res[5]
//   };
//   console.log("secondTeam: ", secondTeam);

//   return res;
// });

// // var responseJSON = responseFromController.responseJSON;

// $(document).ready(function ()
// {
//   new GetNextGroupClue();
//   new GuessGroupMovie();
//   new GiveUpAndAdmitFailure();
// });

// // #region MANAGE TEAMS
// // ---------------------------------------------------------------------------------------

// let firstTeamScoreHtml = $("#first_team_score").get();
// let firstTeamNameAndScoreHtml = $(".first_team_name_and_score").get();
// let secondTeamScoreHtml = $("#second_team_score").get();
// let secondTeamNameAndScoreHtml = $(".second_team_name_and_score").get();

// let currentTeamName;
// let currentTeamId;
// let sittingTeamName;
// let sittingTeamId;
// // let winningTeamName;
// // let winningTeamId;

// function ToggleTeamGuessing()
// {
//   console.log(clueNumber);
//   if (clueNumber == 1 || clueNumber == 3 || clueNumber == 5 || clueNumber == 7 || clueNumber == 9)
//   {
//     console.log("first team guessing");
//     currentTeamName = firstTeam.name;
//     currentTeamId = firstTeam.id;
//     sittingTeamName = secondTeam.name;
//   }
//   if (clueNumber == 2 || clueNumber == 4 || clueNumber == 6 || clueNumber == 8 || clueNumber == 10)
//   {
//     console.log("second team guessing");
//     currentTeamName = secondTeam.name;
//     currentTeamId = secondTeam.id;
//     sittingTeamName = firstTeam.name;
//   }

//   if (clueNumber > 1)
//   {
//     $(firstTeamNameAndScoreHtml).toggleClass("this_teams_turn");
//     $(secondTeamNameAndScoreHtml).toggleClass("this_teams_turn");
//   }

//   // console.log("currentTeamName: ", currentTeamName);
//   return currentTeamName;
// }

// //

// // ---------------------------------------------------------------------------------------
// // #endregion MANAGE TEAMS

// // #region MANAGE CLUES
// // ---------------------------------------------------------------------------------------
// let groupCluesList = $("#group_clues_list").get();
// let clueNumber = 1;
// let clueIndex = 0;
// let cluePoints = 10;
// // let cluesRevealed = 0;

// function GetNextGroupClue()
// {
//   // console.log("GetNextGroupClue");
//   $("#group_clues_list").click(function ()
//   {
//     if (clueNumber >= 10)
//     {
//       $("#group_clues_list").prop("disabled", true);
//     }

//     $.get("group/get_clues_object", function (res)
//     {
//       new GetTextFromGroupGuessBox();
//       let allClues = res.clues;
//       let oneClue = allClues[clueIndex];
//       let clueText = oneClue.clueText;
//       let clueDifficulty = oneClue.clueDifficulty;
//       cluePoints = oneClue.cluePoints;

//       RemoveInitialInstructions();
//       ToggleTeamGuessing();
//       AppendClueToList(clueText);
//       SendClueNumberToController(clueDifficulty);

//       clueNumber++;
//       clueIndex++;
//     });
//   });
// }

// function SendClueNumberToController(element)
// {
//   // console.log("SendClueNumberToController");
//   $.ajax
//     ({
//       type: "GET",
//       url: "group/get_clue_number_from_javascript",
//       data: { ClueNumber: element },
//       success: function (data) { },
//       error: function (jqXHR, textStatus, errorThrown) { }
//     });
// }

// function RemoveInitialInstructions()
// {
//   if (clueNumber == 1)
//   {
//     $(groupCluesList).empty();
//   }
// }

// function AppendClueToList(element)
// {
//   var clueHtml = '<div class="group_clue">' + element + '</div>';
//   $(groupCluesList).append(clueHtml);
// }

// // ---------------------------------------------------------------------------------------
// // #endregion MANAGE CLUES

// // #region MANAGE GUESSES
// // ---------------------------------------------------------------------------------------

// function GetTextFromGroupGuessBox()
// {
//   $("#group_guess_input").keyup(function (event)
//   {
//     var searchText = $("#group_guess_input").val();

//     $("#group_search_res_list").html("");

//     $.ajax
//       ({
//         url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
//         method: "GET",
//         success: function (serverResponse)
//         {
//           var imdbMovieInfo = serverResponse["Search"];
//           if (imdbMovieInfo.length > 0)
//           {
//             $("#group_search_res_list").empty();
//             for (var h = 0; h < imdbMovieInfo.length; h++)
//             {
//               var oneResult = imdbMovieInfo[h]["Title"];
//               AppendListItemToList(oneResult);
//             }
//             // click movie title in drop-down list and it populates the input box
//             $("#group_search_res_list li").bind("click", function ()
//             {
//               SetGroupGuessText(this);
//             });
//           }
//         }
//       });
//   });
// }

// // returns any movie with search term in it (e.g., 'good' leads to 'good will hunting' AND 'a few good men')
// function AppendListItemToList(listItem)
// {
//   $("#group_search_res_list").append('<li class="text-danger">' + listItem + "</li>");
// }

// function SetGroupGuessText(element)
// {
//   var searchText = $("#group_guess_input").val();
//   var value = $(element).text();

//   $("#group_guess_input").val(value);
//   $("#group_search_res_list").empty();

//   $.ajax
//     ({
//       url: "https://www.omdbapi.com/?s=" + searchText + "&apikey=4514dc2d",
//       method: "GET",
//       success: function (serverResponse)
//       {
//         var imdbMovieInfo = serverResponse["Search"];
//       }
//     });
// }

// // --- 'replace' finds any double spaces between words and trims to one space
// // --- 'trim' removes spaces at the end of the guess
// function GuessGroupMovie()
// {
//   $(".group_guess_button").click(function ()
//   {
//     // gets text entered in html input
//     let movieTitleGuessed = $("#group_guess_input:first").val().toString().replace(/\s+/g, ' ').trim().toUpperCase();
//     $.get("group/get_movie_json", function (res)
//     {
//       let movieTitleActual = res["title"].replace(/\s+/g, ' ').trim().toUpperCase();
//       let movieId = res["movieId"];

//       // If correct guess
//       if (movieTitleGuessed == movieTitleActual)
//       {
//         if (currentTeamName = firstTeam.name)
//         {
//           ManageCorrectGuessOutcome(movieId, cluePoints, clueNumber);
//         }
//       }

//       else TriggerWrongGuessAlertBox();
//     });
//   });
// }

// function ManageCorrectGuessOutcome(movieId, pointsReceived, clueGameWonAt)
// {
//   RouteToCorrectGuessPage();
//   UpdateHtmlWithNewScore();
//   SendMovieTeamJoinInfoToController(currentTeamId, movieId, pointsReceived, clueGameWonAt);
//   SendGameTeamJoinUpdatesToController(currentTeamId, pointsReceived);
//   SendRoundInfoToController();
// }

// function TriggerWrongGuessAlertBox()
// {
//   alert("That guess is WRONG (and embarrassing)");
// }

// // ---------------------------------------------------------------------------------------
// // #endregion MANAGE GUESSES

// // #region MANAGE GAME OUTCOMES
// // ---------------------------------------------------------------------------------------
// function SendMovieTeamJoinInfoToController(teamId, movieId, pointsReceived, clueGameWonAt)
// {
//   // console.log("SendMovieTeamJoinInfoToController");
//   $.ajax
//     ({
//       type: "POST",
//       url: "group/create_movie_team_join",
//       data:
//       {
//         TeamId: teamId,
//         MovieId: movieId,
//         PointsReceived: pointsReceived,
//         ClueGameWonAt: clueGameWonAt,
//       },
//       success: function (data) { }
//     });
// }

// function SendGameTeamJoinUpdatesToController(teamId, pointsReceived)
// {
//   console.log("SendGameTeamJoinUpdatesToController");
//   $.ajax({
//     type: "POST",
//     url: "group/update_game_team_join",
//     data:
//     {
//       TeamId: teamId,
//       PointsReceived: pointsReceived,
//     },
//     success: function (data) { }
//   });
// }

// function UpdateHtmlWithNewScore()
// {
//   // winningTeamName = GetCurrentTeamGuessing().trim();
//   if (currentTeamName == firstTeam.name)
//   {
//     firstTeam.points = firstTeam.points + cluePoints;
//     $(firstTeamScoreHtml).empty().append(firstTeam.points.toString());
//   }
//   if (currentTeamName == secondTeam.name)
//   {
//     secondTeam.points = secondTeam.points + cluePoints;
//     $(secondTeamScoreHtml).empty().append(secondTeam.points.toString());
//   }
// }

// function RouteToCorrectGuessPage()
// {
//   console.log("RouteToCorrectGuessPage");
//   $.ajax
//     ({
//       type: "GET",
//       url: "group/correct_guess_page",
//       success: function ()
//       {
//         window.location.href = "group/correct_guess_page";
//       }
//     });
// }

// function GiveUpAndAdmitFailure()
// {
//   $("#group_quit_button").click(function ()
//   {
//     console.log("clicked quit button");

//     $.ajax
//       ({
//         type: "GET",
//         url: "group/quit_movie_page",
//         success: function ()
//         {
//           window.location.href = "group/quit_movie_page";
//         }
//       });
//   });
// }

// function SendRoundInfoToController()
// {
//   $.ajax
//     ({
//       type: "POST",
//       url: "group/update_round",
//       data:
//       {
//         WinningTeam: currentTeamName,
//         LosingTeam: sittingTeamName
//       },
//       success: function (data) { }
//     });
// }

// // ---------------------------------------------------------------------------------------
// // #endregion MANAGE GAME OUTCOMES

// function PrintClueDetails(text, difficulty)
// {
//   let logClue = 0;
//   if (logClue == 1)
//   {
//     console.log("----------------------------------------");
//     console.log("NEW CLUE");
//     console.log("----------------------------------------");
//     console.log();
//     console.log("CLUE: ", text);
//     console.log("DIFFICULTY: ", difficulty);
//     console.log("POINTS: ", cluePoints);
//     console.log("----------------------------------------");
//     console.log("----------------------------------------");
//     console.log();
//   }
// }

//   // currentTeamName = GetCurrentTeamGuessing().trim();
//   // if (currentTeamName == firstTeamName) firstTeamGuesses++;
//   // if (currentTeamName == secondTeamName) secondTeamGuesses++;
//   // let firstTeamGuessesResponse = firstTeamName + " has guessed " + firstTeamGuesses + " times";
//   // let secondTeamGuessesResponse = secondTeamName + " has guessed " + secondTeamGuesses + " times";
//   // alert("That guess is WRONG (and embarrassing)\n" + firstTeamGuessesResponse + "\n" + secondTeamGuessesResponse);

// // function GetCurrentTeamGuessingSnake()
// // {
// //   if (clueNumber == 1 || clueNumber == 4 || clueNumber == 5 || clueNumber == 8 || clueNumber == 9)
// //   {
// //     // console.log("first team guessing");
// //     currentTeamName = firstTeamName;
// //     currentTeamId = firstTeamId;
// //     winningTeamId = currentTeamId;
// //   }
// //   if (clueNumber == 2 || clueNumber == 3 || clueNumber == 6 || clueNumber == 7 || clueNumber == 10)
// //   {
// //     // console.log("second team guessing");
// //     currentTeamName = secondTeamName;
// //     currentTeamId = secondTeamId;
// //     winningTeamId = currentTeamId;
// //   }
// //   console.log("currentTeamName: ", currentTeamName);
// //   return currentTeamName;
// // }

// // let firstTeamId;
// // let firstTeamNameHtml = $("#first_team_name").get();
// // let firstTeamName;
// // try { firstTeamName = firstTeamNameHtml[0].innerText.trim(); }
// // catch (error) { }

// // let firstTeamScoreString;
// // try { firstTeamScoreString = firstTeamScoreHtml[0].innerText.trim(); }
// // catch (error) { }

// // let firstTeamScore = Number(firstTeamScoreString);
// // let firstTeamNameAndScore;
// // try { firstTeamNameAndScore = firstTeamNameAndScoreHtml[0].innerText.trim(); }
// // catch (error) { }

// // ----- SECOND TEAM -----
// // let secondTeamId;
// // let secondTeamNameHtml = $("#second_team_name").get();
// // let secondTeamName;
// // try { secondTeamName = secondTeamNameHtml[0].innerText.trim(); }
// // catch (error) { }

// // let secondTeamScoreString;
// // try { secondTeamScoreString = secondTeamScoreHtml[0].innerText.trim(); }
// // catch (error) { }

// // let secondTeamScore = Number(secondTeamScoreString);
// // let secondTeamNameAndScore;
// // try { secondTeamNameAndScore = secondTeamNameAndScoreHtml[0].innerText.trim(); }
// // catch (error) { }

// // let groupButtons = $("#group_buttons").get();
// // let groupGuessButton = $("#group_guess_button").get();

// // function GetSittingTeam()
// // {
// //   if (GetCurrentTeamGuessing() == firstTeam.name)
// //   {
// //     sittingTeamName = secondTeam.name;
// //   }
// //   if (GetCurrentTeamGuessing() == secondTeam.name)
// //   {
// //     sittingTeamName = firstTeam.name;
// //   }
// //   // console.log("sittingTeamName: ", sittingTeamName);
// //   return sittingTeamName;
// // }
