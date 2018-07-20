"use strict"

$("#GroupPlay_Clues").click(function () {
    console.log("container clicked");

    $('#GroupPlay_Clues').append("test");
});


function GetClue () {
    $("#GroupPlay_Clues").click(function() {
        $.get("getClue", function(res){

            $("ul#guessResponse").empty();

            // ALL MOVIE CLUES ---> returns array of 10 [object object] of clues
            var allMovieClues = res.clues;

            // CONTENT LENGTH ---> content length starts at -1 and keeps counting up; errors out after final clue
            var contentLength = $('ul#clueText > li').length - 1 ;

            // var ClueDifficulty = res.clues[contentLength+1].clueDifficulty;
            // if (ClueDifficulty == 1) { console.table(allMovieClues); }

            // CLUE POINTS --> '10', then '9', then '8' etc.
            var CluePoints = res.clues[contentLength+1].cluePoints;

            var currentClue = allMovieClues[contentLength+1].clueText;

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
}