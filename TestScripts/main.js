scriptEngine.import("test.js");

async function main() {
    writeLog("Hello Cef Scripts");
    await scriptEngine.writeln("Hello Cef Scripts");
    await sleep(1000);
    writeLog("Lets Do It");
    await sleep(1000);
    test();
    //await scriptEngine.loadUrl("https://www.baidu.com");
    //await sleep(1000);
    //await scriptEngine.writeln("load Url Success");

    //await scriptEngine.writeln(document.querySelector("h1").innerHTML)
}
main();