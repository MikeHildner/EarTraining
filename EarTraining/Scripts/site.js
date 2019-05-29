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
