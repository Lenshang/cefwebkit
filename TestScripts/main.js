﻿scriptEngine.import("test.js");

async function main() {
    writeLog("Hello Cef Scripts");
    await scriptEngine.writeln("Hello Cef Scripts");
    await sleep(1000);
    writeLog("Lets Do It");
    await sleep(1000);
    await waitClick("点击这里继续");
    writeLog("Yes");
    test();
}
main();