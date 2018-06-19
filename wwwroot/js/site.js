$(document).ready(function(){

    $("#getClueButton").click(function() {
        $.get("getClue", function(res){

            $("#clueButtonText").empty();
            $("#clueButtonText").append("Get Next Clue");

            // ALL MOVIE INFO ---> gives you all info about the movie
            var allMovieInfo = res;
                // console.log("ALL MOVIE INFO : ", allMovieInfo);
                // console.log("ALL MOVIE INFO TABLE: ");
                // console.table(allMovieInfo);

            // ALL MOVIE CLUES ---> returns array of 10 [object object] of clues
            var allMovieClues = res.clues;

            // ALL MOVIE CLUES LENGTH ---> 10
            var allMovieCluesLength = Object.keys(allMovieClues).length;

            // CONTENT LENGTH ---> content length starts at -1 and keeps counting up; errors out after final clue
            var contentLength = $('ul#clueText > li').length - 1 ;

            // CLUE ID ---> '1', then '2', OR '11', then '12', etc.
            var ClueId = res.clues[contentLength+1].clueId;

            // CLUE POINTS --> '10', then '9', then '8' etc.
            var CluePoints = res.clues[contentLength+1].cluePoints;

            // CLUE ---> lists off each clue  after each click; e.g., '<li>School bus</li>' etc.
            var clue = "<li>" + allMovieClues[contentLength+1].clueText + "</li>";

            // if you reach the 10th clue, disable the 'getClueButton'
            if (contentLength == 8)
            {
                // append 'clue' list item to ul list called 'clueText'
                $('ul#clueText').append(clue);

                $('#cluePoints').empty();
                $('#cluePoints').append("Current Clue Point Value: ");

                $('#justPoints').empty();
                $('#justPoints').append(CluePoints);

                $("#getClueButton").prop("disabled", true);
            }

            // do not disable the 'getClueButton' for every clue before the final clue
            else {
                $('#cluePoints').empty();
                $('#cluePoints').append("Current Clue Point Value: ");

                $('#justPoints').empty();
                $('#justPoints').append(CluePoints);

                // append 'clue' list item to ul list called 'clueText'
                $('ul#clueText').append(clue);
            }
        });
    });


    $("#buttonText").on("click", function() {
        var el = $(this);
        if (el.text() ==el.data("text-swap")) {
            el.text(el.data("text-original"));
        }
        else {
            el.data("text-original", el.text());
            el.text(el.data("text-swap"));
        }
    });


    // #region  --> GUESS MOVIE
    $("#guessMovie").submit(function(event) {

        console.log("-----'GUESS MOVIE' FUNCTION STARTED-----");

        // USER GUESS ---> 'Goodfellas' etc.
        var UserGuess = $("input:first").val().toString();

        $.get("guessMovie", function(res) {

            var currentPointsInt = $("#justPoints").html();
            // console.log("JUST POINTS INT | GUESS MOVIE: ", currentPointsInt);

            // MOVIE GUESS ITEMS ---> array of session movie title and session guess count
                // var MovieGuessItems = res;

            // SESSION MOVIE TITLE ---> 'Goodfellas' etc.
                var SessionMovieTitle = res[0];

            // SESSION MOVIE TITLE TO CAPS --> e.g., 'goodfellas' TO 'GOODFELLAS'
            var SessionMovieTitleToCaps = SessionMovieTitle.toUpperCase();

            // GUESS COUNT ---> 2, then 1, then 0
            var guessCount = res[1];
            console.log("GUESS COUNT: ", guessCount);

            // USER GUESS TO CAPS ---> 'GOODFELLAS' etc.
            var UserGuessToCaps = UserGuess.toUpperCase();

            // GUESS RESPONSE WRONG --> concatenated response to wrong answer
            var guessResponseWrong = "Your Answer: " + "<b>" + UserGuessToCaps + "</b>" + " is WRONG.";

            if(guessCount > 0)
            {
                // guesses remaining; the player WON
                if(UserGuessToCaps == SessionMovieTitleToCaps)
                {
                    // clears the 'remaining guesses'div if players wins the game
                    $("#remainingGuesses").empty();

                    // clears the 'movie guess input' text box if the player wins the game
                    $("#movieGuessInput").val('');

                    // disable the 'guess button' if the player wins the game
                    $("#guessButton").prop("disabled", true);

                    // calls the 'UpdatePlayerPoints' function; this sends the point value of the clue the movie was guessed on back to the controller
                    UpdatePlayerPoints("#guessMovieForm");

                    var dialog = bootbox.dialog({
                        message: "<p>CORRECT! You win the game! <br> Points received: " + currentPointsInt + "<br> </p>",
                        buttons: {
                            cancel: {
                                label    : "Quit (I'm a loser)",
                                className: 'btn-danger',
                                callback : function() {
                                    console.log("LOSER");
                                    window.location.href = "/";
                                }
                            },

                            playagain:
                            {
                                label    : "Play again (I'm a nerd)",
                                className: 'btn-info',
                                callback : function()
                                {
                                    console.log("NERD");
                                    window.location.href = "/initiateGame";
                                }
                            }
                        }
                    });
                }
                // guesses remaining; the player's guess was WRONG
                else
                {
                    $("#remainingGuesses").empty();
                    $("#remainingGuesses").append("Remaining Guesses: ", guessCount);

                    $("#movieGuessInput").val('');

                    $("ul#JQresponse").empty();
                    $('ul#JQresponse').append(guessResponseWrong);
                }
            }

            // user is out of guesses
            if(guessCount == 0)
            {
                // out of guesses and the player won
                if(UserGuessToCaps == SessionMovieTitleToCaps)
                {
                    console.log("USER GUESS: ", UserGuessToCaps," ", "SESSION MOVIE: " , SessionMovieTitleToCaps);

                    $("#remainingGuesses").empty();
                    $('#remainingGuesses').append("THE GAME IS OVER. Click 'Start Over' to begin a new game");

                    $("ul#JQresponse").empty();
                    $("#movieGuessInput").val('');

                    $("#guessButton").prop("disabled", true);

                    // calls the 'UpdatePlayerPoints' function; this sends the point value of the clue the movie was guessed on back to the controller
                    new UpdatePlayerPoints("#guessMovieForm");

                    var dialog = bootbox.dialog({
                        message: "<p>CORRECT! You win the game! <br> Points received: " + currentPointsInt + "</p>",
                        buttons: {
                            cancel: {
                                label    : "Quit (I'm a loser)",
                                className: 'btn-danger',
                                callback : function() {
                                    console.log("LOSER");
                                    window.location.href = "/";
                                }
                            },
                            playagain:
                            {
                                label    : "Play again (I'm a nerd)",
                                className: 'btn-info',
                                callback : function()
                                {
                                    console.log("NERD");
                                    window.location.href = "/initiateGame";
                                }
                            }
                        }
                    });
                }

                // out of guesses and the player lost
                else
                {
                    console.log("USER GUESS: ", UserGuessToCaps," ", "SESSION MOVIE: " , SessionMovieTitleToCaps);

                    $("#remainingGuesses").empty();
                    $('#remainingGuesses').append("THE GAME IS OVER. Click 'Start Over' to begin a new game");

                    $("ul#JQresponse").empty();
                    $("#movieGuessInput").val('');

                    $("#guessButton").prop("disabled", true);
                    var dialog = bootbox.dialog({
                        message: "<p>You lost. Please consider whether or not you are a serious enough player for this game </p>",
                        buttons: {
                            cancel: {
                                label    : "Quit (I'm a loser)",
                                className: 'btn-danger',
                                callback : function() {
                                    console.log("LOSER");
                                    window.location.href = "/";
                                }
                            },
                            playagain:
                            {
                                label    : "Play again (I'm a nerd)",
                                className: 'btn-info',
                                callback : function()
                                {
                                    console.log("NERD");
                                    window.location.href = "/initiateGame";
                                }
                            }
                        }
                    });
                }
            }
        })

        console.log("-----'GUESS MOVIE' FUNCTION COMPLETED-----");

        event.preventDefault();

    });




    $('#movieGuessInput').keyup(function(event)
    {
        // SEARCH QUERY --> 'godfa' OR 'godfathe' OR 'godfather' etc.
        var SearchQuery = $('#movieGuessInput').val();

        $('#searchres').html("");
        // SEARCH RES HTML --> returns and object with a bunch of info about the htlml element (e.g., a div or list)
        var SearchResHTML = $('#searchres').html("");

        $.ajax({
            // url example: "https://www.omdbapi.com/?s=godfather&apikey=4514dc2d",
            url    : "https://www.omdbapi.com/?s=" + SearchQuery + "&apikey=4514dc2d",
            method : 'GET',
            success: function (serverResponse) {

                // REAL RES --> returns an array of movie objects (including 'title', 'release year' , etc.) that meet search criteria
                var realres = serverResponse["Search"];

                // REAL RES LENGTH --> seems to always be 10?
                var realreslength = realres.length;

                if(realres.length>0)
                {
                    $("#searchResult").empty();

                    for(var h = 0; h<realreslength; h++) {
                        // ONE RESULT --> returns any movies with search term in it (e.g., 'good' leads to 'good will hunting' AND 'a few good men' etc. )
                        var oneResult = realres[h]["Title"];
                        $("#searchResult").append("<li>" + oneResult + "</li>")
                    }

                    // when you click the movie title in the dropdown list, it populates the text box
                    $("#searchResult li").bind("click",function()
                    {
                        setText(this);
                    });

                }
            }
        })
    })
});


// #region SET TEXT
function setText(element) {

    console.log("-----'SET TEXT' FUNCTION STARTED-----");

    // SEARCH QUERY --> 'godfa' OR 'godfathe' OR 'godfather' etc.
    var SearchQuery = $('#movieGuessInput').val();

    // VALUE --> the movie title you select from the dropdown
    var value = $(element).text();

    $("#movieGuessInput").val(value);
    $("#searchResult").empty();

    $.ajax({
        // url example: "https://www.omdbapi.com/?s=godfather&apikey=4514dc2d",
        url: "https://www.omdbapi.com/?s=" + SearchQuery + "&apikey=4514dc2d",
        // url    : "https://www.omdbapi.com/?s=" + SearchQuery + "&apikey=4514dc2d",
        method : 'GET',
        success: function (serverResponse) {

            // REAL RES --> returns an array of movie objects (including 'title', 'release year' , etc.) that meet search criteria
            var realres = serverResponse["Search"];

        }
    })

    console.log("-----'SET TEXT' FUNCTION COMPLETED-----");
}


// #region UPDATE PLAYER POINTS
function UpdatePlayerPoints (formContainer)
{
    console.log();
    console.log("-----'UPDATE USER POINTS' JS FUNCTION STARTED-----");

    $.ajax ({
        type: "POST",
        url : "/updatePlayerPoints",
        data: {
            CluePoints: $("#justPoints").html(),
        },
        success: function(data) {
            console.log("SUCCESSSSS!!!!");
        },
        error: function(jqXHR, textStatus, errorThrown) {
        },
    })
    console.log("-----'UPDATE USER POINTS' JS FUNCTION COMPLETED-----");
    console.log();
}


// function InitiateGame ()
// {
//     console.log();
//     console.log("-----'INITIATE GAME' JS FUNCTION STARTED-----");

//     $.ajax ({
//         type   : "POST",
//         url    : "/startOver",
//         success: function() {
//             console.log("SUCCESS!");
//         },
//         error: function() {
//             console.log("ERROR");
//         }
//     })

//     console.log("-----'INITIATE GAME' JS FUNCTION COMPLETED-----");
//     console.log();
// }

