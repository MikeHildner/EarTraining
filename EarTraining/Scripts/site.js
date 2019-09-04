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
    var scale = getMajorScale(theDo);
    var isMinor = false;
    var chord;

    switch (numeral) {
        case 'I':
            chord = scale[0];
            break;
        case 'iii':
            chord = scale[2];
            isMinor = true;
            break;
        case 'IV':
            chord = scale[3];
            break;
        case 'V':
            chord = scale[4];
            break;
        case 'vi':
            chord = scale[5];
            isMinor = true;
            break;
        default:
            consoleAndAlert("Numeral '" + numeral + "' is not supported.");
    }

    if (isMinor) {
        chord += 'min';
    }

    return chord;
}

function getMajorScale(theDo) {
    var scale;
    switch (theDo) {
        case 'C':
            scale = ['C', 'D', 'E', 'F', 'G', 'A', 'B'];
            break;
        case 'G':
            scale = ['G', 'A', 'B', 'C', 'D', 'E', 'F#'];
            break;
        case 'D':
            scale = ['D', 'E', 'F#', 'G', 'A', 'B', 'C#'];
            break;
        case 'A':
            scale = ['A', 'B', 'C#', 'D', 'E', 'F#', 'G#'];
            break;
        case 'E':
            scale = ['E', 'F#', 'G#', 'A', 'B', 'C#', 'D#'];
            break;
        case 'B':
            scale = ['B', 'C#', 'D#', 'E', 'F#', 'G#', 'A#'];
            break;
        case 'F#':
            scale = ['F#', 'G#', 'A#', 'B', 'C#', 'D#', 'E#'];
            break;
        case 'C#':
            scale = ['Db', 'Eb', 'F', 'Gb', 'Ab', 'Bb', 'C'];
            break;
        case 'G#':
            scale = ['Ab', 'Bb', 'C', 'Db', 'Eb', 'F', 'G'];
            break;
        case 'D#':
            scale = ['Eb', 'F', 'G', 'Ab', 'Bb', 'C', 'D'];
            break;
        case 'A#':
            scale = ['Bb', 'C', 'D', 'Eb', 'F', 'G', 'A'];
            break;
        case 'F':
            scale = ['F', 'G', 'A', 'Bb', 'C', 'D', 'E'];
            break;	
        default:
            consoleAndAlert("getScale for '" + theDo + "' is not supported.");
    }

    return scale;
}