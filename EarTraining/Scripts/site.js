﻿$(document).ready(function () {
    $('#spinner').hide();
});

function getRandomInt(min, max, exclude) {
    min = Math.ceil(min);
    max = Math.floor(max);
    var ri = Math.floor(Math.random() * (max - min + 1)) + min;
    if (exclude && exclude.includes(ri)) {
        ri = getRandomInt(min, max, exclude);
    }
    return ri;
}

function consoleAndToastr(msg, title, severity) {
    console.log(msg);
    switch (severity) {
        case "error":
            toastr.error(msg, title);
            break;

        case "info":
            toastr.info(msg, title);
            break;

        case "success":
            toastr.success(msg, title);
            break;

        case "warning":
            toastr.warning(msg, title);
            break;

        default:
            toastr.warning(msg, title);
    }
    
}

function getNewDo(callback, msg, friendlyMessage) {
    var request = $.ajax({
        url: rootPath + "DO/getNewDO",
        method: "GET"
    });

    request.done(function (data) {
        var pitchName = data.PitchName;
        //pitchName = pitchName.split('/')[0];
        pitchName = encodeURIComponent(pitchName);
        var frequency = data.Hertz;

        //$('#doSpoiler').text(pitchName + ' - ' + frequency + ' Hz');
        //var doInfo = pitchName + ' - ' + frequency + ' Hz';
        callback(pitchName, msg, friendlyMessage);
    });

    request.fail(function (jqXHR, textStatus) {
        consoleAndToastr('There was an error processing the request.\n\n' + textStatus + ': ' + jqXHR.status + ' - ' + jqXHR.statusText + '.', null, 'error');
    });
}

function buildProgressionTable(doInfo, progInfo, friendlyMessage, inversionsOnly) {
    doInfo = decodeURIComponent(doInfo);
    // Get just the note name from doInfo.
    var theDo = doInfo.split('-')[0].split('/')[0].trim();
    theDo = theDo.replace(/[0-9]/g, '');

    // Split the progInfo, keeping inversion info.
    var progsWithInversionInfo = progInfo.split('-');

    // Get progression.
    // Remove parenthesis and text inside them.
    progInfo = progInfo.replace(/ *\([^)]*\) */g, '');

    // Remove whitespace.
    progInfo = progInfo.replace(/\s+/g, "");

    // Get the roman numerals as an array.
    var progs = progInfo.split('-');

    var progTable = '<table class="tdcenter">';

    // Friendly message, if we have one.
    if (friendlyMessage) {
        var colspan = progsWithInversionInfo.length;
        progTable += '<tr>';
        progTable += '<td colspan="' + colspan + '">' + friendlyMessage + '</td>';
        progTable += '<tr>';
    }

    // Numerals with inversion info.
    progTable += '<tr>';
    for (var i = 0; i < progsWithInversionInfo.length; i++) {
        var tdInfo;
        if (inversionsOnly) {
            // Grab just the text in parenthesis.
            tdInfo = progsWithInversionInfo[i].match(/\((.*?)\)/);
            tdInfo = tdInfo[0];
        }
        else {
            tdInfo = progsWithInversionInfo[i];
        }
        progTable += '<td class="bottom-border">' + tdInfo + '</td>';
    }
    progTable += '<tr>';

    // Chords.
    progTable += '<tr>';
    for (var j = 0; j < progs.length; j++) {
        progTable += '<td class="font-weight-bold">' + getChord(theDo, progs[j]) + '</td>';
    }
    progTable += '<tr>';

    progTable += '</table>';
    return progTable;
}

function getChord(theDo, numeral) {
    var scale = getMajorScale(theDo);
    var isMinor = false;
    var chord;

    switch (numeral) {
        case 'I':
            chord = scale[0];
            break;
        case 'bII':
            if (theDo === 'B') {
                chord = 'C';
            }
            else if (theDo === 'E') {
                chord = 'F';
            }
            else if (theDo.endsWith('#')) {
                chord = scale[0].charAt(0);
            }
            else {
                chord = scale[1] + 'b';
            }
            break;
        case 'ii':
            chord = scale[1];
            isMinor = true;
            break;
        case 'iii':
            chord = scale[2];
            isMinor = true;
            break;
        case 'IV':
            chord = scale[3];
            break;
        case 'bV':
            console.log('=== bV');
            console.log('theDo:' + theDo);
            chord = scale[4] + 'b';
            if (chord.endsWith('bb')){
                chord = chord.replace('bb', '');
                chord = previousLetterName(chord);
            }
            break;
        case 'V':
            chord = scale[4];
            break;
        case 'vi':
            chord = scale[5];
            isMinor = true;
            break;
        case 'bVI':
            console.log('=== bVI');
            console.log('theDo:' + theDo);
            console.log('scale[4]:' + scale[4]);
            if (scale[4] === 'B') {
                chord = 'C';
            }
            else if (scale[4] === 'E') {
                chord = 'F';
            }
            else if (scale[4].endsWith('#')) {
                chord = scale[5].charAt(0);
            }
            else if (scale[4].endsWith('b')) {
                chord = scale[4].charAt(0);
            }
            else {
                chord = scale[5] + 'b';
            }
            break;
        case 'VII':
            chord = scale[6];
            break;
        default:
            consoleAndToastr("Numeral '" + numeral + "' is not supported.");
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
        case 'Cb':
        case 'B':
            scale = ['B', 'C#', 'D#', 'E', 'F#', 'G#', 'A#'];
            break;
        case 'F#':
        case 'Gb':
            scale = ['Gb', 'Ab', 'Bb', 'Cb', 'Db', 'Eb', 'F'];
            break;
        case 'C#':
        case 'Db':
            scale = ['Db', 'Eb', 'F', 'Gb', 'Ab', 'Bb', 'C'];
            break;
        case 'G#':
        case 'Ab':
            scale = ['Ab', 'Bb', 'C', 'Db', 'Eb', 'F', 'G'];
            break;
        case 'D#':
        case 'Eb':
            scale = ['Eb', 'F', 'G', 'Ab', 'Bb', 'C', 'D'];
            break;
        case 'A#':
        case 'Bb':
            scale = ['Bb', 'C', 'D', 'Eb', 'F', 'G', 'A'];
            break;
        case 'F':
            scale = ['F', 'G', 'A', 'Bb', 'C', 'D', 'E'];
            break;
        default:
            consoleAndToastr("getScale for '" + theDo + "' is not supported.");
    }

    return scale;
}

function getExcluded(excluded, index) {
    excluded = excluded || [];
    $('.cb-include').each(function () {
        var cb = $(this);
        var checked = cb.is(':checked');
        if (!checked) {
            var num = cb.attr('id').replace('cb-include-', '');
            num = parseInt(num.split('-')[index]);
            excluded.push(num);
        }
    });
    //console.log(excluded);
    return excluded;
}

function getDoFromLocalStorage() {
    var doNoteName = window.localStorage.getItem('doNoteName');
    if (!doNoteName) {
        doNoteName = 'C4';
        window.localStorage.setItem('doNoteName', doNoteName);
    }

    return doNoteName;
}

function changeAllDoNoteNames(theDo) {
    //console.log('***Entered changeAllDoNoteNames. theDo: ' + theDo);
    if (!theDo) {
        theDo = getDoFromLocalStorage();
    }
    var allSources = $('audio source');
    allSources.each(function (index, element) {
        //console.log('===allSources[' + index + ']: ' + element);
        var src = $(element).attr('src');
        //console.log('src: ' + src);
        var regex = /doNoteName=.*?(&|$)/;
        var doNoteNameParams = src.match(regex);
        //console.log('doNoteNameParams: ' + doNoteNameParams);

        $(doNoteNameParams).each(function (i, el) {
            //console.log('doNoteNameParam[' + i + ']: ' + el);
            if (el.endsWith('&')) {
                el = el.slice(0, -1); // Trim last character.
                //console.log('doNoteNameParam[' + i + '] trim last char: ' + el);
            }

            if (el.includes('doNoteName')) {
                src = src.replace(el, 'doNoteName=' + theDo);
                //console.log('new src: ' + src);
                $(element).attr('src', src);
                var parent = $(element).parent();
                parent.attr('src', src);
            }
        });
    });
    //console.log('***Exiting changeAllDoNoteNames');
}

function previousLetterName(letter) {
    var i = letter.charCodeAt(0);
    i = i - 1;
    if (i === 64) {
        i = 71;
    }

    var previousLetter = String.fromCharCode(i);
    return previousLetter;
}

function invertSelections(callback) {
    $('.cb-include:visible').each(function () {
        this.checked = !this.checked;
    });

    callback();
}