/*
    Ovo je javascript biblioteka za komunikaciju sa ekstenzijom.
    Koristi se na sajtu koji bi hteo da pristupi licnoj karti
*/
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = y[op[0] & 2 ? "return" : op[0] ? "throw" : "next"]) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [0, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
cardReader = {};
/*
   ID-jevi ekstenzije, izmeniti po potrebi.
   ID za Chrome dodeljuje Google prodavnica
   ID za Firefox stoji u manifestu
*/
cardReader.chromeID = 'lammmgffjnohfeoiceccbmenhcjadooj';
cardReader.firefoxID = 'messenger.cardreader@brzeboljejeftinije.rs';
cardReader.init = function () {
    cardReader.requestCount = 0;
    var nativeHostResponded = false;
    var ack_wait_timeout = 10000;
    cardReader.promises = {};
    cardReader.rejects = {};
    function isChrome() {
        var isChromium = window.chrome, winNav = window.navigator, vendorName = winNav.vendor, isOpera = winNav.userAgent.indexOf("OPR") > -1, isIEedge = winNav.userAgent.indexOf("Edge") > -1, isIOSChrome = winNav.userAgent.match("CriOS");
        if (isIOSChrome) {
            return true;
        }
        else if (isChromium !== null && isChromium !== undefined && vendorName === "Google Inc." && isOpera == false && isIEedge == false) {
            return true;
        }
        else {
            return false;
        }
    }
    if (isChrome()) {
        cardReader.extensionId = 'chrome-extension://' + cardReader.chromeID + '/';
    }
    else {
        cardReader.extensionId = cardReader.firefoxID;
    }
    function listener(event) {
        if (event.data.source == cardReader.extensionId) {
            if (event.data.status == "OK") {
                cardReader.promises[event.data.request.requestId](event);
            }
            else {
                cardReader.rejects[event.data.request.requestId](event);
            }
        }
    }
    window.addEventListener('message', listener);
    setTimeout(function () {
    }, ack_wait_timeout);
};
cardReader.getAesKey = function () {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "getAesKey", "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.aesEncrypt = function (payload, key) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "aesEncrypt", "payload": btoa(payload), key: key, "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.aesDecrypt = function (payload, key) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "aesDecrypt", "payload": payload, key: key, "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, atob(result.data.payload)];
            }
        });
    });
};
cardReader.rsaEncrypt = function (payload, key, cert) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "rsaEncrypt", "payload": btoa(payload), key: key, cert: cert, "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.rsaDecrypt = function (payload) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "rsaDecrypt", "payload": payload, "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, atob(result.data.payload)];
            }
        });
    });
};
cardReader.sign = function (payload) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "sign", "payload": payload, "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.verify = function (payload, signature, key, cert) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "verify", "payload": payload, signature: signature, key: key, cert: cert, "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.getCertificate = function () {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "getCertificate", "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.check = function () {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "check", "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload == "true"];
            }
        });
    });
};
cardReader.getPublic = function () {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "getPublic", "requestId": rqId });
                    });
                    return [4 /*yield*/, promise];
                case 1:
                    result = _a.sent();
                    return [2 /*return*/, result.data.payload];
            }
        });
    });
};
cardReader.ping = function () {
    return __awaiter(this, void 0, void 0, function () {
        var result, ex_1;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    promise = new Promise(function (resolve, reject) {
                        var rqId = cardReader.requestCount++;
                        cardReader.promises[rqId] = resolve;
                        cardReader.rejects[rqId] = reject;
                        cardReader.sendMessage({ "type": "echo", "requestId": rqId });
                        setTimeout(function () {
                            reject("fail");
                        }, 2000);
                    });
                    _a.label = 1;
                case 1:
                    _a.trys.push([1, 3, , 4]);
                    return [4 /*yield*/, promise];
                case 2:
                    result = _a.sent();
                    return [3 /*break*/, 4];
                case 3:
                    ex_1 = _a.sent();
                    return [2 /*return*/, false];
                case 4: return [2 /*return*/, true];
            }
        });
    });
};
cardReader.sendMessage = function (message) {
    message.source = window.location.href;
    message.destination = cardReader.extensionId;
    window.postMessage(message, "*");
};
