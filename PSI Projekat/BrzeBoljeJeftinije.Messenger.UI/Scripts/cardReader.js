/**
 * JavaScript biblioteka za komunikaciju sa ekstenzijom koja pristupa ličnoj karti
 * Autor: Nikola Pavlović
*/


cardReader = {};

cardReader.chromeID = 'lammmgffjnohfeoiceccbmenhcjadooj'
cardReader.firefoxID = 'messenger.cardreader@brzeboljejeftinije.rs'

cardReader.init = function () {
    cardReader.requestCount = 0
    cardReader.timeouts = {}
    var nativeHostResponded = false;

    var ack_wait_timeout = 10000;
    cardReader.promises = {}
    cardReader.rejects = {}

    function isChrome() {
        var isChromium = window.chrome,
          winNav = window.navigator,
          vendorName = winNav.vendor,
          isOpera = winNav.userAgent.indexOf("OPR") > -1,
          isIEedge = winNav.userAgent.indexOf("Edge") > -1,
          isIOSChrome = winNav.userAgent.match("CriOS");

        if (isIOSChrome) {
            return true;
        } else if (isChromium !== null && isChromium !== undefined && vendorName === "Google Inc." && isOpera == false && isIEedge == false) {
            return true;
        } else {
            return false;
        }
    }
    if (isChrome()) {
        cardReader.extensionId = 'chrome-extension://' + cardReader.chromeID + '/';
    }
    else {
        cardReader.extensionId = cardReader.firefoxID;
    }

    function listener(event)
    {
        if (event.data.source == cardReader.extensionId)
        {
            clearTimeout(cardReader.timeouts[event.data.request.requestId]);
            if (event.data.status == "OK")
            {
                cardReader.promises[event.data.request.requestId](event)
            }
            else 
            {
                cardReader.rejects[event.data.request.requestId](event)
            }
        }
    }

    window.addEventListener('message', listener);
    setTimeout(function () {
        
        }, ack_wait_timeout);
}
cardReader.getAesKey = async function ()
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "getAesKey", "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload;
}
cardReader.aesEncrypt = async function (payload, key)
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "aesEncrypt", "payload": btoa(payload), key:key, "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload;
}

cardReader.aesDecrypt = async function (payload, key)
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "aesDecrypt", "payload": payload, key: key, "requestId": rqId })
    });
    var result = await promise;
    return atob(result.data.payload);
}
cardReader.rsaEncrypt = async function (payload, key, cert)
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "rsaEncrypt", "payload": btoa(payload), key: key, cert:cert, "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload;
}

cardReader.rsaDecrypt = async function (payload)
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "rsaDecrypt", "payload": payload, "requestId": rqId, "notimeout": true })
    });
    var result = await promise;
    return atob(result.data.payload);
}
cardReader.rsaDecryptMulti = async function (payload)
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "rsaDecryptMulti", "payload": payload, "requestId": rqId, "notimeout": true })
    });
    var result = await promise;
    return result.data.payload;
}
cardReader.sign=async function(payload)
{
    promise = new Promise(function (resolve, reject) {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "sign", "payload": payload, "requestId":  rqId})
    });
    var result=await promise;
    return result.data.payload;
}
cardReader.verify = async function (payload, signature, key, cert)
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "verify", "payload": payload, signature: signature, key: key, cert: cert, "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload;
}
cardReader.getCertificate = async function () {
    promise = new Promise(function (resolve, reject) {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "getCertificate", "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload;
}
cardReader.check = async function ()
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "check", "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload=="true";
}
cardReader.getPublic = async function ()
{
    promise = new Promise(function (resolve, reject)
    {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "getPublic", "requestId": rqId })
    });
    var result = await promise;
    return result.data.payload;
}
cardReader.ping = async function ()
{
    promise = new Promise(function (resolve,reject) {
        var rqId = cardReader.requestCount++;
        cardReader.promises[rqId] = resolve;
        cardReader.rejects[rqId] = reject;
        cardReader.sendMessage({ "type": "echo", "requestId": rqId, "notimeout":true })
        setTimeout(function () {
            reject("fail");
        }, 10000);
    });
    try {
        var result = await promise;
    }
    catch (ex)
    {
        return false;
    }
    return true;
}
cardReader.sendMessage = function (message)
{
    if (message.requestId !== null && !(message.notimeout))
    {
        cardReader.timeouts[message.requestId] = setTimeout(() =>
        {
            cardReader.rejects[message.requestId]({ data: { status: "TIMEOUT" } });
        }, 10000);
    }
    message.source = window.location.href;
    message.destination = cardReader.extensionId
    window.postMessage(message, "*");
}