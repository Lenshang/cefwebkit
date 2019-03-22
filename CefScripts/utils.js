async function sleep(_msecond) {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            resolve()
        }, _msecond);
    });
}

function writeLog(msg) {
    document.writeln("<h4>" + new Date().toLocaleString()+":"+msg+"</h4>")
}