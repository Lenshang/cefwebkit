scriptEngine.import("test.js");

async function __main() {
    await waitDebug();
    writeLog("Hello Cef Scripts");
    args = await scriptEngine.getArgs();
    writeLog(args[0]);
    await scriptEngine.writeln("Hello Cef Scripts");
    await sleep(1000);
    writeLog("Lets Do It");
    await sleep(1000);
    await waitClick("点击这里继续");
    writeLog("Yes");
    test();
}