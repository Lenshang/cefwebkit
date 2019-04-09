async function sleep(_msecond) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve()
        }, _msecond);
    });
}
var _isClick = false;
function btClick() {
    _isClick = true;
}
async function waitClick(buttonName) {
    document.writeln("<button id=\"waitclick\" type=\"button\" onclick=\"btClick()\">" + buttonName+"</button>")

    while (_isClick == false) {
        await sleep(100);
    }
    _isClick = false;
    document.querySelector("body").removeChild(document.getElementById("waitclick"));
}

function writeLog(msg) {
    document.writeln("<h4>" + new Date().toLocaleString()+":"+msg+"</h4>")
}