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

        $('#doSpoiler').text(pitchName + ' - ' + frequency + ' Hz');
    });

    request.fail(function (jqXHR, textStatus) {
        alert('There was an error processing the request.\n\n' + textStatus + ': ' + jqXHR.status + ' - ' + jqXHR.statusText + '.');
    });

}

function buildProgressionText(doInfo, progInfo) {
    // Get just the note name from doInfo.
    var theDo = doInfo.split('-')[0].split('/')[0].trim();
    theDo = theDo.replace(/[0-9]/g, '');
    console.log('theDo: ' + theDo);

    // Get progression.
    // Remove parenthesis and text inside them.
    progInfo = progInfo.replace(/ *\([^)]*\) */g, '');
    //console.log('progInfo: ' + progInfo);

    // Remove whitespace.
    progInfo = progInfo.replace(/\s+/g, "");
    console.log('progInfo: ' + progInfo);

    // Get the roman numerals as an array.
    var progs = progInfo.split('-');

    var progText = '';
    for (var i = 0; i < progs.length; i++) {
        progText += getChord(theDo, progs[i]);
        if (i + 1 < progs.length) {
            progText += ' - ';
        }
    }

    console.log('progText: ' + progText);
    return progText;
}

function getChord(theDo, numeral) {
    var hasSharp = theDo.includes('#');
    var doNoSharp = theDo.replace('#', '');

    var charCode = doNoSharp.charCodeAt(0);
    //console.log(doNoSharp + ': ' + charCode);

    switch (numeral) {
        case 'I':
            charCode += 0;
            break;
        case 'IV':
            charCode += 3;
            break;
        case 'V':
            charCode += 4;
            break;
        default:
            consoleAndAlert("Numeral '" + numeral + "' is not supported.");
    }

    // Handle 'overflow'.
    //console.log('charCode: ' + charCode);
    if (charCode > 71) {
        charCode -= 7;
        //console.log('New charCode: ' + charCode);
    }

    var chord = String.fromCharCode(charCode);
    if (hasSharp) {
        chord += '#';
    }

    if (theDo === 'F' && chord === 'B') {
        chord = 'Bb';
    }
    return chord;
}