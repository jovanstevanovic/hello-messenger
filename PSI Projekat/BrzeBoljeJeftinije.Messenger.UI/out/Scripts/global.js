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
var touchScrollStartY = null;
var datePickerParams = { format: 'dd.mm.yyyy' };
var modalStack = [];
var currentModal = null;
$(document).ready(function () {
    initSignalR();
});
var navMenuTimeout = null;
var activePrompt = null;
function openNavMenu() {
    if (navMenuTimeout != null) {
        clearTimeout(navMenuTimeout);
        navMenuTimeout = null;
    }
    $("#nav-menu").show();
    navMenuTimeout = setTimeout(function () {
        navMenuTimeout = null;
        $("#nav-menu").hide();
    }, 5000);
}
function showMessageBox(message) {
    $("#message-box-modal").find(".message-box-text").html(message);
    $("#message-box-modal").find(".info-part").show();
    $("#message-box-modal").find(".alert-part").hide();
    showModal("#message-box-modal");
}
function showAlertBox(message) {
    $("#message-box-modal").find(".message-box-text").html(message);
    $("#message-box-modal").find(".info-part").hide();
    $("#message-box-modal").find(".alert-part").show();
    showModal("#message-box-modal");
}
function showGenericModal(html) {
    $("#generic-modal").find(".generic-content").html(html);
    showModal("#generic-modal");
}
function openPrompt(message, callback) {
    $("#prompt-modal").find(".message-box-text").html(message);
    showModal("#prompt-modal");
    activePrompt = callback;
}
function promptAccept() {
    closeModal();
    if (activePrompt)
        activePrompt(true);
}
function promptReject() {
    closeModal();
    if (activePrompt)
        activePrompt(false);
}
function showModal(id) {
    if (currentModal != null) {
        modalStack.push(currentModal);
        $(currentModal).find(".modal-inner").hide();
        $(id).find(".modal-inner").show();
        $(id).show();
        $(currentModal).hide();
        currentModal = id;
    }
    else {
        currentModal = id;
        $(id).find(".modal-inner").show();
        $(id).show();
    }
}
function closeModal() {
    $(currentModal).find(".modal-inner").hide();
    if (modalStack.length > 0) {
        oldModal = modalStack.pop();
        $(oldModal).find(".modal-inner").show();
        $(oldModal).show();
        $(currentModal).hide();
        currentModal = oldModal;
    }
    else {
        $(currentModal).hide();
        currentModal = null;
    }
}
function handleCardErrorMessage(msg) {
    var status = msg.data.status;
    var errorMessage = null;
    switch (status) {
        case "NO_CARD":
            errorMessage = "Lična karta nije ubačena ili je sertifikat na njoj neispravan";
            break;
        case "EXC":
            errorMessage = "Došlo je do greške: " + msg.data.message;
            break;
        default:
            errorMessage = "Došlo je do nepoznate greške prilikom obavljanja kriptografskih operacija";
            break;
    }
    showAlertBox(errorMessage);
}
function asyncAjax(url, data, method) {
    if (data === void 0) { data = {}; }
    if (method === void 0) { method = 'POST'; }
    return __awaiter(this, void 0, void 0, function () {
        var promise, promise;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!(data instanceof FormData)) return [3 /*break*/, 2];
                    promise = new Promise(function (accept, reject) {
                        $.ajax(url, {
                            method: method,
                            data: data,
                            processData: false,
                            contentType: false,
                            success: function (result) {
                                accept(result);
                            },
                            error: function (xhr, error, thrown) {
                                err = "";
                                if (error)
                                    err = error;
                                if (thrown)
                                    err += " " + thrown;
                                showAlertBox("Došlo je do greške u mrežnoj komunikaciji: " + err);
                                reject(err);
                            }
                        });
                    });
                    return [4 /*yield*/, promise];
                case 1: return [2 /*return*/, _a.sent()];
                case 2:
                    promise = new Promise(function (accept, reject) {
                        $.ajax(url, {
                            method: method,
                            data: data,
                            success: function (result) {
                                accept(result);
                            },
                            error: function (xhr, error, thrown) {
                                err = "";
                                if (error)
                                    err = error;
                                if (thrown)
                                    err += " " + thrown;
                                showAlertBox("Došlo je do greške u mrežnoj komunikaciji: " + err);
                                reject(err);
                            }
                        });
                    });
                    return [4 /*yield*/, promise];
                case 3: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
function initSignalR() {
    var hub = $.connection.messengerHub;
    hub.client.Refresh = function (name, message) {
        if (window.refreshGroups)
            refreshGroups(true);
    };
    hub.client.NewMessagesForGroup = function (groupId) {
        if (window.messagesCameForGroup)
            messagesCameForGroup(groupId);
    };
    $.connection.hub.start().done(function () {
    });
}
function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
