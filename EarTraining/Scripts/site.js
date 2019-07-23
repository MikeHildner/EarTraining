//$(document).ready(function () {
//    $('.spoiler').hover(
//        function () {
//            var t = $(this)[0];
//            $(this)[0].animate({
//                "color": "#FFFFFF"//,
//                //backgroundColor: "blue"
//            });
//        },
//        function () {
//            //alert('mouse leave');
//        }
//    );
//});

function getRandomInt(min, max, exclude) {
    min = Math.ceil(min);
    max = Math.floor(max);
    var ri = Math.floor(Math.random() * (max - min + 1)) + min;
    if (exclude && exclude.includes(ri)) {
        ri = getRandomInt(min, max, exclude);
    }
    return ri;
}

function consoleAndAlert(msg) {
    console.log(msg);
    alert(msg);
}

function getNewDo() {
    var request = $.ajax({
        url: rootPath + "DO/getNewDO",
        method: "GET"
    });

    request.done(function (data) {
        var pitchName = data.PitchName;
        var frequency = data.Hertz;

        $('#divSpoiler').text(pitchName + ' - ' + frequency + ' Hz');
        //$('.doAccepter')
    });

    request.fail(function (jqXHR, textStatus) {
        alert('There was an error processing the request.\n\n' + textStatus + ': ' + jqXHR.status + ' - ' + jqXHR.statusText + '.');
    });

}
