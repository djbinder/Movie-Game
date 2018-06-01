$(document).ready(function(){

    $("#getClue").click(function() {
        $.get("getClue", function(res){

            // ALL MOVIE INFO ---> gives you all info about the movie
            var allMovieInfo = res;
                // console.log("ALL MOVIE INFO : ", allMovieInfo);

            // ALL MOVIE CLUES ---> returns array of 10 [object object]
            var allMovieClues = res.clues;
                // console.log("ALL MOVIE CLUES: ", allMovieClues);

            // ALL MOVIE CLUES LENGHT ---> 10
            var allMovieCluesLength = Object.keys(allMovieClues).length;

            // CONTENT LENGTH ---> content length starts at -1 and keeps counting up; errors out after final clue
            var contentLength = $('ul#clueText > li').length - 1 ;
                // console.log("CONTENT LENGTH: ", contentLength);

            // CLUE ID ---> '1', then '2', OR '11', then '12', etc.
            var ClueId = res.clues[contentLength+1].clueId;
                // console.log("CLUE ID: ", ClueId);


            var CluePoints = res.clues[contentLength+1].cluePoints;
                console.log("CLUE POINTS: ", CluePoints);

            // CLUE ---> lists off each clue  after each click; e.g., '<li>School bus</li>' etc.
            var clue = "<li>" + allMovieClues[contentLength+1].clueText + "<br>" + "Pts: " + CluePoints + "</li>";
                // console.log("CLUE: ", clue);

            var addedPoints = CluePoints;
            // console.log("ADDED POINTS: ", addedPoints);


            // append 'clue' list item to ul list called 'clueText'
            $('ul#clueText').append(clue);
        });
    })



    $("#guessMovie").submit(function(event) {

        // USER GUESS ---> 'Goodfellas' etc.
        var UserGuess = $("input:first").val().toString();
        // console.log("USER GUESS: ", UserGuess);

        $.get("guessMovie", function(res) {

            // MOVIE GUESS ITEMS ---> array of session movie title and session guess count
            var MovieGuessItems = res;
                // console.log("MOVIE GUESS ITEMS: ", MovieGuessItems);

            // SESSION MOVIE TITLE ---> 'Goodfellas' etc.
            var SessionMovieTitle = res[0];
                // console.log("SESSION MOVIE TITLE: ", SessionMovieTitle);

            var SessionMovieTitleToCaps = SessionMovieTitle.toUpperCase();
                console.log("SESSION TITLE TO CAPS: ", SessionMovieTitleToCaps);

            // GUESS COUNT ---> 2, then 1, then 0
            var guessCount = res[1];
                console.log("GUESS COUNT: ", guessCount);


            // CONTENT LENGTH ---> 2, then 1, then stays at 1
            // var contentLength = $('ul#JQresponse > li').length - 1;
            // console.log("CONTENT LENGTH: ", contentLength);



            // USER GUESS TO CAPS ---> 'GOODFELLAS' etc.
            var UserGuessToCaps = UserGuess.toUpperCase();
                // console.log(UserGuessToCaps);

            // GUESS RESPONSE WRONG --> concatenated response to wrong answer
            var guessResponseWrong = "Your Answer: " + "<b>" + UserGuessToCaps + "</b>" + " is WRONG. " + "<br>" + "You have " + guessCount + " remaining guesses" + "<br>";
                // console.log("GUESS RESPONSE WRONG: ", guessResponseWrong);


            if(guessCount > 0)
            {

                if(UserGuessToCaps == SessionMovieTitleToCaps)
                {
                    console.log('------------------------------------');
                    console.log("EQUAL --- MARK 1");
                    console.log(UserGuessToCaps," ",SessionMovieTitleToCaps);
                    console.log('------------------------------------');

                    $('ul#JQresponse').append("YOU WIN THE GAME!!!!");
                }

                else{
                    console.log('------------------------------------');
                    console.log("NOT EQUAL --- MARK 1");
                    console.log(UserGuessToCaps," ",SessionMovieTitleToCaps);
                    console.log('------------------------------------');

                    $('ul#JQresponse').append(guessResponseWrong);

                }

            }


            if(guessCount == 0)
            {
                if(UserGuessToCaps == SessionMovieTitleToCaps)
                {
                    console.log('------------------------------------');
                    console.log("EQUAL --- MARK 2");
                    console.log(UserGuessToCaps," ",SessionMovieTitleToCaps);
                    console.log('------------------------------------');

                    $('ul#JQresponse').append("YOU WIN THE GAME!!!!");
                }
                else {
                    console.log('------------------------------------');
                    console.log("NOT EQUAL --- MARK 2");
                    console.log(UserGuessToCaps," ",SessionMovieTitleToCaps);
                    console.log('------------------------------------');

                    $('ul#JQresponse').append("THE GAME IS OVER" + "<br>" + guessResponseWrong);
                }
            }

        })

        event.preventDefault();


        // $('ul#JQresponse').append(UserGuess);

    })

    // var xTriggered = 0;
    $('#movieGuessInput').keyup(function(event){

        // this code created a 'handler' log; not necessary but kind of interesting
            // xTriggered++;
            // var msg = "Handler for .keyup() called " + xTriggered + " time(s).";
            // console.log('------------------------------------');
                // MESSAGE --> 'Handler for .keyup() called 4 time(s). html' etc.
                    // console.log("MESSAGE: ", msg, "html" );
                // EVENT --> logs a ton of information within an object about the 'event' (i.e., 'keyup');
                    // console.log("EVENT: ", event );
            // console.log('------------------------------------');


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
                var realres = serverResponse["Search"]

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
            var realres = serverResponse["Search"]

            var realresLength = realres.length;
            console.log("REAL RES LENGTH: ", realresLength);


            $("#movieDetail").empty();
            if(realresLength > 0){

                var oneResult = realres[0]["Title"];
                console.log("SET TEXT ONE RESULT: ", oneResult);

                $("#movieDetail").append("Title : " + oneResult + "<br/>");

            }

        }
    })


}
