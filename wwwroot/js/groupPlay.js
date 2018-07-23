var ClueCard1 = '<li><div class="row p-0 m-0"> <div class="col-6 card border-dark mb-1 mr-1 ml-1 p-0 rounded-0 d-flex justify-content-center"> <div class="card-header text-center pt-1 pb-1">#1</div> <div class="card-body text-dark p-0 mb-1"> <h3 class="card-title text-center text-primary mt-2 mb-1"><b>Helicopter</b></h3> <p class="card-text text-center">Point Value: 10</p> </div></li>';


var ClueCard2 = '<div class="col-6 card border-dark mb-1 p-0 rounded-0 d-flex justify-content-center" style="max-width: 11rem;"><div class="card-header text-center pt-1 pb-1">#2</div><div class="card-body text-dark p-0 mb-1"><h3 class="card-title text-center text-primary mt-2 mb-1"><b>Henry Hill</b></h3><p class="card-text text-center">Point Value: 9</p></div></div></div></div>';


var ClueCard3 = '<div class="text-center">Movie Clue</div><br>';

var ClueCard4 = '<div class="row"><div class="col-2 text-center">1</div><div class="col-6 text-center">clue text</div><div class="col-4 text-center">Worth 10</div></div>';

var ClueRow = '<tr class="text-white" ><th scope="row" class="text-center pl-0 pr-0 pt-3 pb-3 bg-success">1</th><td class="text-center pl-0 pr-0 pt-0 pb-0 bg-success txt">Clue txt</td><td class="text-center pl-0 pr-0 pt-3 pb-3 bg-success">10</td></tr><tr><td colspan="3"></td></tr>';


$("#GroupPlay_Container").click(function () {
    console.log("container clicked");

    $("#TableBody").append(ClueRow);

});


// function GetClue () {
//     $("#GroupPlay_Clues").click(function() {
//         $.get("GetClue", function(res){

//             $("ul#guessResponse").empty();

//             // ALL MOVIE CLUES ---> returns array of 10 [object object] of clues
//             var allMovieClues = res.clues;

//             // CONTENT LENGTH ---> content length starts at -1 and keeps counting up; errors out after final clue
//             var contentLength = $('ul#clueText > li').length - 1 ;

//             // var ClueDifficulty = res.clues[contentLength+1].clueDifficulty;
//             // if (ClueDifficulty == 1) { console.table(allMovieClues); }

//             // CLUE POINTS --> '10', then '9', then '8' etc.
//             var CluePoints = res.clues[contentLength+1].cluePoints;

//             var currentClue = allMovieClues[contentLength+1].clueText;

//             // CLUE ---> lists off each clue  after each click; e.g., '<li>School bus</li>' etc.
//             var clue = "<li class='bg-light swing-in-top-fwd'>" + allMovieClues[contentLength+1].clueText + "</li>";

//             // if you reach the 10th clue, disable the 'getClueButton'
//             if (contentLength == 8)
//             {
//                 // append 'clue' list item to ul list called 'clueText'
//                 $('ul#clueText').append(clue);

//                 $('#cluePoints').empty();
//                 $('#cluePoints').append("Current Clue Point Value: ");

//                 $('#justPoints').empty();
//                 $('#justPoints').append(CluePoints);


//                 $("#getClueButton").prop("disabled", true);
//             }

//             // do not disable the 'getClueButton' for every clue before the final clue
//             else {
//                 $('#cluePoints').empty();
//                 $('#cluePoints').append("Current Clue Point Value: ");

//                 $('#justPoints').empty();
//                 $('#justPoints').append(CluePoints);

//                 // append 'clue' list item to ul list called 'clueText'
//                 $('ul#clueText').append(clue);
//             }
//         });
//     });
// }