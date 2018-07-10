$(document).ready(function(){

    $("#remainingGuesses").append("3");

    $("#getClueButton").click(function() {
        $.get("getClue", function(res){

            $("#clueButtonText").empty();
            $("#clueButtonText").append("Get Next Clue");

            $("ul#guessResponse").empty();

            new ResetForm();

            // var HiddenObject = document.getElementById("hiddenDiv");
            var HiddenString = $(".hiddenDiv").get(0).innerHTML;
            var HiddenNumber = Number(HiddenString);

            // console.log("HIDDEN OBJECT: ", HiddenObject);
            // console.log("HIDDEN STRING: ", HiddenString);
            // console.log("HIDDEN NUMBER: ", HiddenNumber);

            if(HiddenNumber > 0)
            {
                $("#remainingGuesses").empty();
                $("#remainingGuesses").append(HiddenString);
            }

            // ALL MOVIE CLUES ---> returns array of 10 [object object] of clues
            var allMovieClues = res.clues;

            // CONTENT LENGTH ---> content length starts at -1 and keeps counting up; errors out after final clue
            var contentLength = $('ul#clueText > li').length - 1 ;

            // var ClueDifficulty = res.clues[contentLength+1].clueDifficulty;
            // if (ClueDifficulty == 1) { console.table(allMovieClues); }

            // CLUE POINTS --> '10', then '9', then '8' etc.
            var CluePoints = res.clues[contentLength+1].cluePoints;

            var currentClue = allMovieClues[contentLength+1].clueText;

            new SendClueToController(currentClue);

            var valUp     = 1;
            var percentUp = 10;

            var NewProgress = '<div class="progress-bar progBarOrange" role="progressbar" style="width: ' + percentUp + '%" aria-valuenow=' + valUp + ' aria-valuemin="0" aria-valuemax="10"><span id="cluesLeft"></span></div>';

            $("#progressBar").append(NewProgress);

            if(CluePoints == 1)
            {
                $("#progressBar").empty();

                var FullBar = '<div class="progress-bar progBarOrange" role="progressbar" style="width: 100%" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100"><a href="/viewInstructions">You are out clues. Click Here for Help</a></div>';
                $("#progressBar").append(FullBar);

            }

            // CLUE ---> lists off each clue  after each click; e.g., '<li>School bus</li>' etc.
            var clue = "<li class='bg-light swing-in-top-fwd'>" + allMovieClues[contentLength+1].clueText + "</li>";

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

        // USER GUESS ---> what the player types in to the guess box; e.g., 'Goodfellas' etc.
        var UserGuess = $("input:first").val().toString();

        $.get("guessMovie", function(res) {

            var currentPointsInt = $("#justPoints").html();
            var xPointsInt = Number(currentPointsInt);
            console.log(xPointsInt);

            // SESSION MOVIE TITLE ---> 'Goodfellas' etc.
            var SessionMovieTitle = res[0];

            // SESSION MOVIE TITLE TO CAPS --> e.g., 'goodfellas' TO 'GOODFELLAS'
            var SessionMovieTitleToCaps = SessionMovieTitle.toUpperCase();

            // GUESS COUNT ---> 2, then 1, then 0
            var guessCount     = res[1];
            // var guessCountSpan = "<span class='guessCount'>" + guessCount + "</span>";

            // console.log(currentPointsInt);
            // console.log(SessionMovieTitle);
            // console.log(SessionMovieTitleToCaps);
            // console.log("GUESS COUNT: ", guessCount);

            // USER GUESS TO CAPS ---> 'GOODFELLAS' etc.
            var UserGuessToCaps = UserGuess.toUpperCase();

            // GUESS RESPONSE WRONG --> concatenated response to wrong answer
            var guessResponseWrong = '<b>' + UserGuessToCaps + "</b>" + " is WRONG.";

            if(guessCount > 0)
            {
                console.log("HIT GUESS COUNT > 0");
                // guesses remaining; the player WON
                if(UserGuessToCaps == SessionMovieTitleToCaps)
                {
                    console.log("__________ user still has guesses and won __________");

                    // clears the 'remaining guesses'div if players wins the game
                    $("#remainingGuesses").empty();

                    // clears the 'movie guess input' text box if the player wins the game
                    $("#movieGuessInput").val('');

                    // disable the 'guess button' if the player wins the game
                    $("#guessButton").prop("disabled", true);

                    // calls the 'UpdatePlayerPoints' function; this sends the point value of the clue the movie was guessed on back to the controller
                    new UpdatePlayerPoints("#guessMovieForm", xPointsInt);

                    var dialog = bootbox.dialog({
                        message: '<br><div class="endGameMessage row border border-dark"><div class="col"><span class="align-middle"><b class="endWin align-middle">Correct. You win.</b> <br> Points received: ' + currentPointsInt + '</span></div><br><div class="col"><img class="align-middle" src="https://image.tmdb.org/t/p/w92/gKDNaAFzT21cSVeKQop7d1uhoSp.jpg" alt="ASP.NET" class="img-responsive"></div></div>',
                        buttons: {
                            cancel: {
                                label    : "Quit (I'm a loser)",
                                className: 'btn-danger endButton',
                                callback : function() {
                                    console.log("QUIT GAME");
                                    window.location.href = "/";
                                }
                            },

                            playagain:
                            {
                                label    : "Play again (I'm a nerd)",
                                className: 'btn-success endButton',
                                callback : function()
                                {
                                    console.log("PLAY AGAIN");
                                    window.location.href = "/initiateGame";
                                }
                            }
                        }
                    });
                }
                // guesses remaining; the player's guess was WRONG
                else
                {
                    console.log("__________ user still has guesses and lost __________");

                    $("#remainingGuesses").empty();
                    $("#remainingGuesses").append(guessCount);
                    $(".hiddenDiv").empty();
                    $(".hiddenDiv").append(guessCount);

                    $("#movieGuessInput").val('');

                    $("ul#guessResponse").empty();
                    $('ul#guessResponse').append(guessResponseWrong);
                }
            }

            // user is out of guesses
            if(guessCount == 0)
            {
                // out of guesses and the player won
                if(UserGuessToCaps == SessionMovieTitleToCaps)
                {
                    console.log("__________ user is out of guesses and won __________");

                    $("#remainingGuesses").empty();
                    $('#remainingGuesses').append("0");

                    $("ul#guessResponse").empty();
                    $("#movieGuessInput").val('');

                    $("#guessButton").prop("disabled", true);

                    // calls the 'UpdatePlayerPoints' function; this sends the point value of the clue the movie was guessed on back to the controller
                    new UpdatePlayerPoints("#guessMovieForm", xPointsInt);

                    var dialog = bootbox.dialog({
                        message: '<p class="endGameMessage"><b class="endWin">Correct. You win.</b> <br> Points received: ' + currentPointsInt + '<br> </p>',
                        buttons: {
                            cancel: {
                                label    : "Quit (I'm a loser)",
                                className: 'btn-danger endButton',
                                callback : function() {
                                    console.log("QUIT GAME");
                                    window.location.href = "/";
                                }
                            },
                            playagain:
                            {
                                label    : "Play again (I'm a nerd)",
                                className: 'btn-success endButton',
                                callback : function()
                                {
                                    console.log("PLAY AGAIN");
                                    window.location.href = "/initiateGame";
                                }
                            }
                        }
                    });
                }

                // out of guesses and the player lost
                else
                {
                    console.log("__________ user is out of guesses and lost __________");

                    $("#remainingGuesses").empty();

                    $("ul#guessResponse").empty();
                    $("#movieGuessInput").val('');

                    $("#guessButton").prop("disabled", true);

                    new UpdatePlayerPoints("#guessMovieForm", 0);

                    var dialog = bootbox.dialog({
                        message: '<p class="endGameMessage"><b class="endLoss">You lost</b> <br> Please take some time to reflect on your failure. </p>',
                        buttons: {
                            cancel: {
                                label    : "Quit (I'm a loser)",
                                className: 'btn-danger endButton',
                                callback : function() {
                                    console.log("QUIT GAME");
                                    window.location.href = "/";
                                }
                            },
                            playagain:
                            {
                                label    : "Play again (I'm a nerd)",
                                className: 'btn-success endButton',
                                callback : function()
                                {
                                    console.log("PLAY AGAIN");
                                    window.location.href = "/initiateGame";
                                }
                            }
                        }
                    });
                }
            }
        });

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
                        $("#searchResult").append('<li class="text-danger bg-light">' + oneResult + '</li>')
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

    function ResetForm()
    {
        $("#getClueButton").click(function() {
            $("#formContainer").empty();
            $("#formContainer").replaceWith(divClone.clone(true));

        })

        // console.log("FORM WAS RESET");
        var toggle = document.getElementById('toggle');
        var wrapper = document.querySelectorAll('.subscribe');
        var submit = document.getElementById('submit');
        var success = document.querySelectorAll('.subscribe__success');
        var content = document.querySelectorAll('.subscribe__wrapper');
        // console.log(toggle);
        // console.log(wrapper);
        // console.log(submit);
        // console.log(success);
        // console.log(content);

        toggle.addEventListener('click', function()
        {
            this.className += ' subscribe__toggle__hidden';
            wrapper[0].className += ' subscribe-1__active';
        });

        submit.addEventListener('click', function()
        {
            success[0].className += ' subscribe__success--active';
            wrapper[0].className += ' subscribe-1__success';
            content[0].style.display = 'none';
        });
    };


    var divClone = $("#formContainer").clone(true);
    // console.log("DIV CLONE", divClone);

});


// #region SET TEXT
function setText(element) {

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
}


// #region UPDATE PLAYER POINTS
function UpdatePlayerPoints (formContainer, element)
{
    console.log();
    console.log("-----'UPDATE USER POINTS' JS FUNCTION STARTED-----");
    $.ajax ({
        type: "POST",
        url : "/updatePlayerPoints",
        data: {
            CluePoints: $("#justPoints").html(),
            MovieId: element
        },
        success: function(data) {
            console.log("PLAYER POINTS SUCCESSFULLY UPDATED");
            console.log(data.cluePoints);
            console.log(element);
        },
        error: function(jqXHR, textStatus, errorThrown) {
        },
    })
    console.log("-----'UPDATE USER POINTS' JS FUNCTION COMPLETED-----");
    console.log();
}


// #region UPDATE PLAYER POINTS
function SendClueToController (element)
{
    $.ajax ({
        type: "GET",
        url : "/getClueFromJavaScript",
        data: {
            ClueText: element,
        },
        success: function(data) {
            console.log("TO CONTROLLER: " + element);
        },
        error: function(jqXHR, textStatus, errorThrown) {
            console.log("ERROR: CLUE NOT SENT TO CONTROLLER");
        },
    })
}


function ShowProgress ()
{
    $("#getClueButton").on("click", function() {

        var valUp     = 1;
        var percentUp = 10;

        var Replace = '<div class="progress-bar" role="progressbar" style="width: ' + percentUp + '%" aria-valuenow=' + valUp + ' aria-valuemin="0" aria-valuemax="10"></div>';

        console.log(Replace);
        $("#progressTest").append(Replace);
    });
}


$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});





$("#GetMovieDecade").click(function(event)
{
    console.log("get hint -----> decade");
    $('#MovieHints').empty();

    var Replacement = '<div class="col text-center text-white">1990s</div>';

    $('#MovieHints').append(Replacement);
});


$("#GetMovieGenre").click(function(event)
{
    console.log("get hint -----> genre");
    $('#MovieHints').empty();
    $('#MovieHints').append("comedy, drama, horror, sci-fi");
});


$("#GetMovieDirector").click(function(event)
{
    console.log("get hint -----> director");
    $('#MovieHints').empty();
    $('#MovieHints').append("Steven Spielberg");
});








$("#GetMovieDecadeButton").click(function(event)
{
    $.get("GetMovieDecade", function(res)
    {
        console.log("get hint -----> decade");
        console.log(res);
    })
});
