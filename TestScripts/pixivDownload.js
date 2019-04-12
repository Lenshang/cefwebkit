//url = "https://www.pixiv.net/member_illust.php?mode=manga&illust_id=74098866"

async function __main() {
    //await waitDebug();
    writeLog("PixivDownload Start");
    await scriptBrowser.setProxy("http://127.0.0.1:8388");//设置代理
    //await waitClick("click");
    writeLog("请输入要下载图片的P站URL");
    var url = await waitInput("");
    writeLog("你输入的图片url是:" + url);
    var monitorId = await scriptBrowser.setUrlMonitor("https://i.pximg.net/img-master/img", "CONTAIN");
    //writeLog("添加图片拦截ID：" + monitorId);
    await scriptBrowser.loadUrl(url);
    await waitClick("请等待全部图片加载后点击这里保存");
    var files = await scriptBrowser.saveUrlMonitorAllContents(monitorId, "testImg");
    for (var i = 0; i < files.length; i++) {
        writeLog("文件：" + files[i]+" 已储存");
    }
}