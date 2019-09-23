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
var groupsScrollbar = null;
var currentGroup = null;
var newGroup = null;
var sendingMessage = false;
var userSearchResult = null;
var previousUserSearchQuery = null;
var uploadedAttachment = null;
var uploadedAttachmentType = null;
var uploadedAttachmentName = null;
var firstReady = true;
$(document).ready(function () {
    if (!firstReady)
        return;
    firstReady = false;
    cardReader.init();
    groupsScrollbar = new PerfectScrollbar('#groups', {
        suppressScrollX: true
    });
    $(".modal-inner").each(function (index, element) {
        new PerfectScrollbar(element);
    });
    refreshGroups(true);
    groupsScrollbar.update();
});
function getMyGroups(searchString) {
    var returnedGroups = [];
    groups.forEach(function (group) {
        if (!(searchString) || group.name.indexOf(searchString) != -1) {
            returnedGroups.push(group);
        }
    });
    return groups;
}
function getGroupById(id) {
    for (var g in groups) {
        if (groups[g].id == id)
            return groups[g];
    }
}
function createGroup(group) {
    groups.add(group);
}
function editGroup() {
    return __awaiter(this, void 0, void 0, function () {
        var html;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Group/Edit", { Id: currentGroup.id }, "GET")];
                case 1:
                    html = _a.sent();
                    showGenericModal(html);
                    return [2 /*return*/];
            }
        });
    });
}
function loadGroup(group) {
    if (group == null) {
        currentGroup = null;
        newGroup = {
            members: []
        };
        showModal("#newgroup");
    }
    else {
        group = getGroupById(group);
        currentGroup = group;
        if (group.type == "request") {
            $("#group-edit").hide();
            $("#friend-delete").hide();
            $("#group-leave").hide();
        }
        else if (group.admin) {
            $("#group-edit").show();
            $("#friend-delete").hide();
            $("#group-leave").hide();
        }
        else {
            $("#group-edit").hide();
            if (group.binary) {
                $("#friend-delete").show();
                $("#group-leave").hide();
            }
            else {
                $("#friend-delete").hide();
                $("#group-leave").show();
            }
        }
        $("#group-picture").attr("src", group.picture);
        $("#group-name").html(group.name);
        $("#conversation").html("");
        if (group.type == "request") {
            $("#fileupload").attr('readonly', "true");
            $("#sent-text").attr('readonly', "true");
            if (group.sent) {
                $("#conversation").append(renderMessage(true, true, 'Poslali ste zahtev za prijateljstvo ' + group.name, null, new Date()));
            }
            else {
                $("#conversation").append(renderMessage(false, true, group.name + ' vam je poslao zahtev za prijateljstvo', null, new Date()));
            }
        }
        else {
            $("#sent-text").removeAttr("readonly");
            $("#fileupload").removeAttr("readonly");
            showMessagesForCurrentGroup(currentGroup);
        }
        $("#group-picture").show();
        redrawGroups();
    }
    setTimeout(function () {
        var objDiv = document.getElementById("conversation");
        objDiv.scrollTop = objDiv.scrollHeight;
    }, 100);
}
function attachFile() {
    sendingMessage = true;
    $("#fileupload").click();
}
function uploadFile() {
    attachmentSet = true;
    readURL($("#fileupload")[0]);
}
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            if (e.target.result.length > 50 * 1024 * 1024) {
                uploadedAttachment = null;
                showAlertBox("Veličina poslatih fajlova je ograničena na 50MB");
                return;
            }
            uploadedAttachment = e.target.result;
        };
        uploadedAttachmentType = input.files[0].type;
        uploadedAttachmentName = input.files[0].name;
        reader.readAsBinaryString(input.files[0]);
    }
}
function accept() {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/User/Accept", { otherId: currentGroup.otherId })];
                case 1:
                    result = _a.sent();
                    refreshGroups(true);
                    return [2 /*return*/];
            }
        });
    });
}
function reject() {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/User/Reject", { otherId: currentGroup.otherId })];
                case 1:
                    result = _a.sent();
                    refreshGroups(true);
                    return [2 /*return*/];
            }
        });
    });
}
function redrawGroups() {
    $("#group-list").html("");
    $("#group-list").append(renderChip('createNewGroup()', 'fa-plus', null, null, "Nova grupa"));
    groups.forEach(function (group) {
        if ($("#group-search").val().trim() != "" && group.name.toUpperCase().indexOf($("#group-search").val().trim().toUpperCase()) == -1)
            return;
        $("#group-list").append(renderChip('loadGroup("' + group.id + '")', null, group.picture, null, group.name, currentGroup != null && group.id == currentGroup.id, group.unread));
    });
    $(".chiptext").ellipsis();
}
function refreshGroups(loadFirst) {
    return __awaiter(this, void 0, void 0, function () {
        var fetchedGroups, knownGroups;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, fetchGroups()];
                case 1:
                    fetchedGroups = _a.sent();
                    groups = [];
                    requests = {};
                    friends = {};
                    knownGroups = {};
                    fetchedGroups.forEach(function (group) {
                        if (group.binary) {
                            friends[group.otherId] = true;
                        }
                        if (group.type == "request") {
                            requests[group.otherId] = true;
                        }
                        groups.push(group);
                        knownGroups[group.id] = true;
                    });
                    groups.sort(function (a, b) {
                        if (a.timestamp > b.timestamp)
                            return -1;
                        else if (a.timestamp < b.timestamp)
                            return 1;
                        else
                            return 0;
                    });
                    redrawGroups();
                    if (loadFirst || (currentGroup != null && !(knownGroups[currentGroup.id]))) {
                        if (groups.length > 0) {
                            loadGroup(groups[0].id);
                        }
                        else {
                            $("#group-picture").hide();
                            $("#friend-delete").hide();
                            $("#group-leave").hide();
                            $("#group-edit").hide();
                        }
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function searchUsers(event) {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (event.key.length > 1)
                        return [2 /*return*/];
                    $("#zahtev-poslat").hide();
                    $("#friend-list").html("");
                    if (!($("#friend-search").val().trim() != "")) return [3 /*break*/, 3];
                    if (!!(previousUserSearchQuery != null && $("#friend-search").val().trim().indexOf(previousUserSearchQuery) != -1 && userSearchResult != null)) return [3 /*break*/, 2];
                    userSearchResult = null;
                    return [4 /*yield*/, asyncAjax("/User/Search", {
                            name: $("#friend-search").val().trim()
                        })];
                case 1:
                    userSearchResult = _a.sent();
                    _a.label = 2;
                case 2:
                    previousUserSearchQuery = $("#friend-search").val().trim();
                    userSearchResult.forEach(function (user) {
                        if ($("#friend-search").val().trim() != "" && user.name.toUpperCase().indexOf($("#friend-search").val().trim().toUpperCase()) == -1)
                            return;
                        if (friends[user.id] || requests[user.id])
                            return;
                        $("#friend-list").append(renderChip('sendFriendRequest(this, "' + user.id + '")', null, user.picture, 'fa-plus', user.name));
                    });
                    _a.label = 3;
                case 3: return [2 /*return*/];
            }
        });
    });
}
function sendFriendRequest(source, id) {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/User/SendRequest", {
                        id: id
                    })];
                case 1:
                    _a.sent();
                    return [4 /*yield*/, refreshGroups()];
                case 2:
                    _a.sent();
                    $(source).remove();
                    showMessageBox("Zahtev uspešno poslat");
                    return [2 /*return*/];
            }
        });
    });
}
function renderChip(onclick, fa, image, rightFa, text, selected, unread) {
    if (fa) {
        return "<a href='#' onclick='" + onclick + "'>" +
            "<li> <div class='chip " + (selected ? "selected-chip" : "") + "'> " +
            "<div style='float:left;margin-right:25px'><i class='fa " + fa + " fa-2x' style='margin-left:-10px;margin-top:10px' aria-hidden='true'></i></div> <span class='chiptext'> <div style='float:left'>" + text + "</div>" +
            (rightFa ? "<div style='float:left;margin-right:25px'><i class='fa " + rightFa + " fa-2x' style='margin-left:-10px;margin-top:10px' aria-hidden='true'></i></div>" : "") + "</span>" +
            "</div> </li>" +
            "</a>";
    }
    else if (image) {
        return "<a href='#' onclick='" + onclick + "'>" +
            "<li><div class='chip " + (selected ? "selected-chip" : "") + (unread ? " unread-chip" : "") + "'> " +
            "<img src='" + image + "' width='96' height='96'> <span class='chiptext'> <div style='float:left'>" +
            text +
            "</div>" +
            (rightFa ? "<div style='float:right'><i class='fa " + rightFa + " fa-2x' style='margin-left:-10px;margin-top:10px' aria-hidden='true'></i></div>" : "") + "</span>" +
            "</div>  </li></a>" +
            "";
    }
}
function renderMessage(sent, friendRequest, text, image, date, senderpic, senderText, notrash) {
    if (notrash === void 0) { notrash = false; }
    type = "receiver";
    if (friendRequest)
        type = "friend-request";
    else if (sent)
        type = "sender";
    return '<div class="row message-body">' +
        (senderpic && !sent ? '<img src="' + senderpic + '" class="senderpic"/> <span class="sender-text">' + senderText + '</span>' : '') +
        '<div class="col-sm-12 message-main-' + type + '">' +
        '<div class="' + type + '">' +
        (image ? '<img class="sent-img" src="' + image + '")" onclick="displaySentImg(\'' + image + '\')"/>' : '<div class="message-text">' + text + '</div>') +
        (friendRequest && !sent ? "<div class='message-text' style='text-align:center'><a href='#'><span onclick='accept()' style='color:green;font-weight:bold'>PRIHVATI</span><a/>|<a href='#'><span onclick='reject()' style='color:red;font-weight:bold'>ODBIJ</span><a/></div>" : "") +
        '<span class="message-time pull-right">' +
        date.toLocaleDateString() + " " + date.toLocaleTimeString() +
        '</span>' +
        (sent && (notrash) ? '<a href="#" onclick="deleteMessage(this)"><i class="fa fa-trash search-icon" aria-hidden="true"></a>' : '') +
        '</div>' +
        '</div>' +
        '</div>';
}
function deleteMessage(row) {
    $(row).closest(".row").remove();
}
function leaveGroup() {
    return __awaiter(this, void 0, void 0, function () {
        var _this = this;
        return __generator(this, function (_a) {
            openPrompt("Da li ste sigurni da želite da napustite grupu?", function (accept) { return __awaiter(_this, void 0, void 0, function () {
                var result;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!accept) return [3 /*break*/, 2];
                            return [4 /*yield*/, asyncAjax("/Group/Leave/" + currentGroup.id)];
                        case 1:
                            result = _a.sent();
                            if (result == "OK") {
                                showMessageBox("Uspešno ste napustili grupu");
                                refreshGroups(false);
                            }
                            else {
                                showAlertBox(result);
                            }
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            }); });
            return [2 /*return*/];
        });
    });
}
function deleteFriend() {
    var _this = this;
    openPrompt("Da li ste sigurni da želite da obrišete prijatelja?", function (accept) { return __awaiter(_this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!accept) return [3 /*break*/, 2];
                    return [4 /*yield*/, asyncAjax("/User/DeleteFriendship/" + currentGroup.otherId)];
                case 1:
                    result = _a.sent();
                    if (result == "OK") {
                        showMessageBox("Uspešno ste obrisali prijatelja");
                        refreshGroups(false);
                    }
                    else {
                        showAlertBox(result);
                    }
                    refreshGroups(false);
                    _a.label = 2;
                case 2: return [2 /*return*/];
            }
        });
    }); });
}
function displaySentImg(image) {
    $("#picture-display").find(".display-picture").attr("src", image);
    showModal("#picture-display");
}
function fetchGroups() {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Group/My")];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
function createNewGroup() {
    return __awaiter(this, void 0, void 0, function () {
        var html;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Group/Edit", {}, "GET")];
                case 1:
                    html = _a.sent();
                    showGenericModal(html);
                    return [2 /*return*/];
            }
        });
    });
}
function getPksForGroup(id) {
    return __awaiter(this, void 0, void 0, function () {
        var pks;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Messages/GetPksForGroup", { "id": id })];
                case 1:
                    pks = _a.sent();
                    return [2 /*return*/, pks];
            }
        });
    });
}
function encryptLongFile(file, key) {
    return __awaiter(this, void 0, void 0, function () {
        var CHUNK_SIZE, start, end, chunk, ciphertext;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    result = "";
                    CHUNK_SIZE = 256 * 1024;
                    start = 0;
                    end = CHUNK_SIZE;
                    _a.label = 1;
                case 1:
                    if (!(start < file.length)) return [3 /*break*/, 3];
                    end = Math.min(end, file.length);
                    chunk = file.slice(start, end);
                    return [4 /*yield*/, cardReader.aesEncrypt(chunk, key)];
                case 2:
                    ciphertext = _a.sent();
                    if (result != "")
                        result += "|";
                    result += ciphertext;
                    start += CHUNK_SIZE;
                    end = start + CHUNK_SIZE;
                    return [3 /*break*/, 1];
                case 3: return [2 /*return*/, result];
            }
        });
    });
}
function decryptLongFile(file, key) {
    return __awaiter(this, void 0, void 0, function () {
        var chunks, plaintext, _a, _b, _i, chunk, _c;
        return __generator(this, function (_d) {
            switch (_d.label) {
                case 0:
                    chunks = file.split("|");
                    plaintext = "";
                    _a = [];
                    for (_b in chunks)
                        _a.push(_b);
                    _i = 0;
                    _d.label = 1;
                case 1:
                    if (!(_i < _a.length)) return [3 /*break*/, 4];
                    chunk = _a[_i];
                    _c = plaintext;
                    return [4 /*yield*/, cardReader.aesDecrypt(chunks[chunk], key)];
                case 2:
                    plaintext = _c + _d.sent();
                    _d.label = 3;
                case 3:
                    _i++;
                    return [3 /*break*/, 1];
                case 4: return [2 /*return*/, plaintext];
            }
        });
    });
}
var rsaDecryptionCache = {};
function getSymmetricKey(encKey) {
    return __awaiter(this, void 0, void 0, function () {
        var result;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (rsaDecryptionCache[encKey])
                        return [2 /*return*/, rsaDecryptionCache[encKey]];
                    return [4 /*yield*/, cardReader.rsaDecrypt(encKey)];
                case 1:
                    result = _a.sent();
                    rsaDecryptionCache[encKey] = result;
                    return [2 /*return*/, result];
            }
        });
    });
}
function decryptMessage(message) {
    return __awaiter(this, void 0, void 0, function () {
        var encryptedKey, signature, actualKey, _a, plaintext;
        return __generator(this, function (_b) {
            switch (_b.label) {
                case 0:
                    encryptedKey = message.CryptoMaterial.split("$")[0];
                    signature = message.CryptoMaterial.split("$")[1];
                    if (!simUser) return [3 /*break*/, 1];
                    _a = atob(encryptedKey);
                    return [3 /*break*/, 3];
                case 1: return [4 /*yield*/, getSymmetricKey(encryptedKey)];
                case 2:
                    _a = (_b.sent());
                    _b.label = 3;
                case 3:
                    actualKey = _a;
                    return [4 /*yield*/, cardReader.aesDecrypt(message.Content, actualKey)];
                case 4:
                    plaintext = _b.sent();
                    message.Content = escapeHtml(decodeURI(plaintext));
                    return [2 /*return*/];
            }
        });
    });
}
/*---------------------------------------------------------------------------*/
/**
 * Korisne funkcije
 * refreshGroups(true) - osvezava listu grupa
 * showMessageBox(message) - prikazuje popup korisniku
 * showAlertBox(message) - isto, samo drugi znak na popup-u
 * openPrompt(poruka, function callback(yes)) - prikazje korisniku YES/NO prompt i poziva callback kad ovaj klikne
 *
 *
 */
/**
 * Dohvata attachment za zadatu poruku
 * Vraća attachment kao binarni niz
 * @param message objekat poruke
 */
function fetchAttachment(message) {
    return __awaiter(this, void 0, void 0, function () {
        var content, encryptedKey, actualKey, _a, content;
        return __generator(this, function (_b) {
            switch (_b.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Messages/GetAttachment", {
                        messageId: message.Id,
                        id: message.Attachments[0].Id
                    })];
                case 1:
                    content = _b.sent();
                    encryptedKey = message.CryptoMaterial.split("$")[0];
                    if (!simUser) return [3 /*break*/, 2];
                    _a = atob(encryptedKey);
                    return [3 /*break*/, 4];
                case 2: return [4 /*yield*/, getSymmetricKey(encryptedKey)];
                case 3:
                    _a = (_b.sent());
                    _b.label = 4;
                case 4:
                    actualKey = _a;
                    return [4 /*yield*/, decryptLongFile(content, actualKey)];
                case 5:
                    content = _b.sent();
                    return [2 /*return*/, content];
            }
        });
    });
}
/*
 * Dohvata zadati deo poruka za zadatu grupu.
 * Vraća niz objekata
 */
function fetchMessages(groupId, pageId) {
    return __awaiter(this, void 0, void 0, function () {
        var response, _a, _b, _i, message;
        return __generator(this, function (_c) {
            switch (_c.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Messages/GetMessages", {
                        "groupId": groupId,
                        "page": pageId
                    })];
                case 1:
                    response = _c.sent();
                    _a = [];
                    for (_b in response)
                        _a.push(_b);
                    _i = 0;
                    _c.label = 2;
                case 2:
                    if (!(_i < _a.length)) return [3 /*break*/, 5];
                    message = _a[_i];
                    return [4 /*yield*/, decryptMessage(response[message])];
                case 3:
                    _c.sent();
                    _c.label = 4;
                case 4:
                    _i++;
                    return [3 /*break*/, 2];
                case 5: return [2 /*return*/, response];
            }
        });
    });
}
/**
 * Briše poruku sa zadatim ID-jem
 * @param id ID poruke
 */
function deleteMessage(id) {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, asyncAjax("/Messages/Delete", { "id": id })];
                case 1: return [2 /*return*/, _a.sent()];
            }
        });
    });
}
/**
 * Šalje poruku sa zadatim tekstom na zadatu grupu.
 * Ako postoji attachment, šalje i njega
 * @param {any} messageText
 * @param {any} groupId
 */
function sendAndEncrypt(messageText, groupId) {
    return __awaiter(this, void 0, void 0, function () {
        var pks, key, encodedText, encryptedText, keys, signature, signature, _a, _b, _i, i, userId, pubkey, encryptedKey, sentData, _c, result;
        return __generator(this, function (_d) {
            switch (_d.label) {
                case 0: return [4 /*yield*/, getPksForGroup(groupId)];
                case 1:
                    pks = _d.sent();
                    return [4 /*yield*/, cardReader.getAesKey()];
                case 2:
                    key = _d.sent();
                    encodedText = encodeURI(messageText);
                    return [4 /*yield*/, cardReader.aesEncrypt(encodedText, key)];
                case 3:
                    encryptedText = _d.sent();
                    keys = {};
                    signature = "sim";
                    return [4 /*yield*/, cardReader.check()];
                case 4:
                    if (!_d.sent()) return [3 /*break*/, 6];
                    return [4 /*yield*/, cardReader.sign(key)];
                case 5:
                    signature = _d.sent();
                    _d.label = 6;
                case 6:
                    _a = [];
                    for (_b in pks)
                        _a.push(_b);
                    _i = 0;
                    _d.label = 7;
                case 7:
                    if (!(_i < _a.length)) return [3 /*break*/, 10];
                    i = _a[_i];
                    userId = pks[i]["id"];
                    pubkey = pks[i]["key"];
                    return [4 /*yield*/, cardReader.rsaEncrypt(key, pubkey, null)];
                case 8:
                    encryptedKey = _d.sent();
                    keys[userId] = encryptedKey + "$" + signature;
                    _d.label = 9;
                case 9:
                    _i++;
                    return [3 /*break*/, 7];
                case 10:
                    sentData = {
                        GroupId: groupId,
                        Ciphertext: encryptedText,
                        Materials: JSON.stringify(keys)
                    };
                    if (!(uploadedAttachment != null)) return [3 /*break*/, 12];
                    _c = sentData;
                    return [4 /*yield*/, encryptLongFile(uploadedAttachment, key)];
                case 11:
                    _c.Attachment = _d.sent();
                    sentData.AttachmentName = uploadedAttachmentName;
                    sentData.AttachmentType = uploadedAttachmentType;
                    _d.label = 12;
                case 12: return [4 /*yield*/, asyncAjax("/Messages/Send", sentData)];
                case 13:
                    result = _d.sent();
                    if (result == "OK") {
                        uploadedAttachment = null;
                    }
                    else {
                        showAlertBox(result);
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function messagesCameForGroup(groupId) {
    return __awaiter(this, void 0, void 0, function () {
        var messagesTemp, i, time, messages, found, messagesList, i, j, i, date, dateString, usersPictureSource, j, _a, _b, _c, _d, msgJson, objDiv;
        return __generator(this, function (_e) {
            switch (_e.label) {
                case 0:
                    if (!(groupId == currentGroup.id)) return [3 /*break*/, 9];
                    return [4 /*yield*/, fetchMessages(groupId, 0)];
                case 1:
                    messagesTemp = _e.sent();
                    for (i = 0; i < messagesTemp.length; i++) {
                        time = messagesTemp[i].Time.substring(6, 19);
                        messagesTemp[i].Time = new Date(parseInt(time));
                    }
                    messagesTemp = messagesTemp.sort(function (a, b) { return a.Time.getTime() - b.Time.getTime(); });
                    messages = new Array();
                    found = false;
                    messagesList = $("#conversation").children("div.row");
                    for (i = 0; i < messagesTemp.length; i++) {
                        for (j = 0; j < messagesList.length; j++) {
                            if (messagesTemp[i].Id == messagesList[j].id) {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            messages[messages.length] = messagesTemp[i]; // messages contain all new messages. messagesTemp contain 10 messages, including messages
                        found = false; // that already existis.
                    }
                    i = 0;
                    _e.label = 2;
                case 2:
                    if (!(i < messages.length)) return [3 /*break*/, 8];
                    date = messages[i].Time;
                    dateString = ("0" + date.getDate()).slice(-2) + "/" +
                        ("0" + (date.getMonth() + 1)).slice(-2) + "/" +
                        date.getFullYear() + "  " +
                        ("0" + date.getHours()).slice(-2) + ":" +
                        ("0" + date.getMinutes()).slice(-2) + ":" +
                        ("0" + date.getSeconds()).slice(-2);
                    usersPictureSource = "/User/Picture?id=" + messages[i].Sender + "";
                    if (messages[i].Content.length > 0) {
                        $("#conversation").append('<div class="row message-body" id=' + messages[i].Id + '>' +
                            '<img src=' + usersPictureSource + ' class="senderpic">' +
                            '<div class="col-sm-12 message-main-sender">' +
                            '<div class="sender">' +
                            '<div class="message-text">' +
                            messages[i].Content +
                            '</div>' +
                            '<span class="message-time pull-right">' +
                            dateString +
                            '</span>' +
                            '<a href="#" onclick="deleteMessage(this)">' +
                            '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
                            '</a>' +
                            '</div>' +
                            '</div>' +
                            '</div>');
                    }
                    if (!(messages[i].Attachments.length > 0)) return [3 /*break*/, 7];
                    j = 0;
                    _e.label = 3;
                case 3:
                    if (!(j < messages[i].Attachments.length)) return [3 /*break*/, 7];
                    if (!(messages[i].Attachments[j].Type == "image/png" || messages[i].Attachments[j].Type == "image/jpg"
                        || messages[i].Attachments[j].Type == "image/jpeg")) return [3 /*break*/, 5];
                    _b = (_a = $("#conversation")).append;
                    _c = '<div class="row message-body" id=' + messages[i].Id + '>' +
                        '<img src=' + usersPictureSource + ' class="senderpic">' +
                        '<div class="col-sm-12 message-main-receiver">' +
                        '<div class="receiver">' +
                        '<img src=' + 'data:image/PNG;base64,';
                    _d = btoa;
                    return [4 /*yield*/, fetchAttachment(messages[i])];
                case 4:
                    _b.apply(_a, [_c + _d.apply(void 0, [_e.sent()]) + ' />' +
                            '<span class="message-time pull-right">' + dateString + '</span>' +
                            '<a href="#" onclick="deleteMessage(this)">' +
                            '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
                            '</a>' +
                            '</div>' +
                            '</div>' +
                            '</div>']);
                    return [3 /*break*/, 6];
                case 5:
                    msgJson = JSON.stringify(messages[i]);
                    $("#conversation").append('<div class="row message-body" id=' + messages[i].Id + '>' +
                        '<img src=' + usersPictureSource + ' class="senderpic">' +
                        '<div class="col-sm-12 message-main-receiver">' +
                        '<div class="receiver">' +
                        '<div class="message-text">' +
                        '<a href="#" onclick=\'saveAttachment(' + msgJson + ')\'>' + messages[i].Attachments[j].Name + '</a>' +
                        '</div>' +
                        '<span class="message-time pull-right">' + dateString + '</span>' +
                        '<a href="#" onclick="deleteMessage(this)">' +
                        '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
                        '</a>' +
                        '</div>' +
                        '</div>' +
                        '</div>');
                    _e.label = 6;
                case 6:
                    j++;
                    return [3 /*break*/, 3];
                case 7:
                    i++;
                    return [3 /*break*/, 2];
                case 8:
                    objDiv = document.getElementById("conversation");
                    objDiv.scrollTop = objDiv.scrollHeight;
                    return [3 /*break*/, 10];
                case 9:
                    refreshGroups(true);
                    _e.label = 10;
                case 10: return [2 /*return*/];
            }
        });
    });
}
var currentPage;
function showMessagesForCurrentGroup(current, page) {
    return __awaiter(this, void 0, void 0, function () {
        var messages, i, time, i, date, dateString, usersPictureSource, msgString, j, msgString, _a, _b, msgJson, msgString, objDiv;
        return __generator(this, function (_c) {
            switch (_c.label) {
                case 0:
                    if (!current)
                        current = currentGroup;
                    $("#toplink").remove();
                    if (page == null)
                        page = 0;
                    currentPage = page;
                    return [4 /*yield*/, fetchMessages(current.id, page)];
                case 1:
                    messages = _c.sent();
                    for (i = 0; i < messages.length; i++) {
                        time = messages[i].Time.substring(6, 19);
                        messages[i].Time = new Date(parseInt(time));
                    }
                    messages = messages.sort(function (a, b) { if (page == 0)
                        return a.Time.getTime() - b.Time.getTime();
                    else
                        return b.Time.getTime() - a.Time.getTime(); });
                    i = 0;
                    _c.label = 2;
                case 2:
                    if (!(i < messages.length)) return [3 /*break*/, 8];
                    date = messages[i].Time;
                    dateString = ("0" + date.getDate()).slice(-2) + "/" +
                        ("0" + (date.getMonth() + 1)).slice(-2) + "/" +
                        date.getFullYear() + "  " +
                        ("0" + date.getHours()).slice(-2) + ":" +
                        ("0" + date.getMinutes()).slice(-2) + ":" +
                        ("0" + date.getSeconds()).slice(-2);
                    usersPictureSource = "/User/Picture?id=" + messages[i].Sender + "";
                    if (messages[i].Content.length > 0) {
                        msgString = '<div class="row message-body" id=' + messages[i].Id + '>' +
                            '<img src=' + usersPictureSource + ' class="senderpic">' +
                            '<div class="col-sm-12 message-main-sender">' +
                            '<div class="sender">' +
                            '<div class="message-text">' +
                            messages[i].Content +
                            '</div>' +
                            '<span class="message-time pull-right">' +
                            dateString +
                            '</span>' +
                            '<a href="#" onclick="deleteMessage(this)">' +
                            '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
                            '</a>' +
                            '</div>' +
                            '</div>' +
                            '</div>';
                        if (page == 0)
                            $("#conversation").append(msgString);
                        else
                            $("#conversation").prepend(msgString);
                    }
                    if (!(messages[i].Attachments.length > 0)) return [3 /*break*/, 7];
                    j = 0;
                    _c.label = 3;
                case 3:
                    if (!(j < messages[i].Attachments.length)) return [3 /*break*/, 7];
                    if (!(messages[i].Attachments[j].Type == "image/png" || messages[i].Attachments[j].Type == "image/jpg"
                        || messages[i].Attachments[j].Type == "image/jpeg")) return [3 /*break*/, 5];
                    _a = '<div class="row message-body" id=' + messages[i].Id + '>' +
                        '<img src=' + usersPictureSource + ' class="senderpic">' +
                        '<div class="col-sm-12 message-main-receiver">' +
                        '<div class="receiver">' +
                        '<img src=' + 'data:image/PNG;base64,';
                    _b = btoa;
                    return [4 /*yield*/, fetchAttachment(messages[i])];
                case 4:
                    msgString = _a + _b.apply(void 0, [_c.sent()]) + ' />' +
                        '<span class="message-time pull-right">' + dateString + '</span>' +
                        '<a href="#" onclick="deleteMessage(this)">' +
                        '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
                        '</a>' +
                        '</div>' +
                        '</div>' +
                        '</div>';
                    if (page == 0)
                        $("#conversation").append(msgString);
                    else
                        $("#conversation").prepend(msgString);
                    return [3 /*break*/, 6];
                case 5:
                    msgJson = JSON.stringify(messages[i]);
                    msgString = '<div class="row message-body" id=' + messages[i].Id + '>' +
                        '<img src=' + usersPictureSource + ' class="senderpic">' +
                        '<div class="col-sm-12 message-main-receiver">' +
                        '<div class="receiver">' +
                        '<div class="message-text">' +
                        '<a href="#" onclick=\'saveAttachment(' + msgJson + ')\'>' + messages[i].Attachments[j].Name + '</a>' +
                        '</div>' +
                        '<span class="message-time pull-right">' + dateString + '</span>' +
                        '<a href="#" onclick="deleteMessage(this)">' +
                        '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
                        '</a>' +
                        '</div>' +
                        '</div>' +
                        '</div>';
                    if (page == 0)
                        $("#conversation").append(msgString);
                    else
                        $("#conversation").prepend(msgString);
                    _c.label = 6;
                case 6:
                    j++;
                    return [3 /*break*/, 3];
                case 7:
                    i++;
                    return [3 /*break*/, 2];
                case 8:
                    if (messages.length > 0) {
                        $("#conversation").prepend('<div class="row message-body" id="toplink" style="text-align:center">' + "<a href='#' onclick='showMessagesForCurrentGroup(null, " + (page + 1) + ")'> Učitaj još</a > " + "</div > ");
                    }
                    if (page == 0) {
                        objDiv = document.getElementById("conversation");
                        objDiv.scrollTop = objDiv.scrollHeight;
                    }
                    return [2 /*return*/];
            }
        });
    });
}
var attachmentSet = false; // Detect if message contain attachment, but not text.
function sendMessage() {
    return __awaiter(this, void 0, void 0, function () {
        var textValue;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    textValue = document.getElementById("sent-text").value;
                    if (textValue.length == 0 && !attachmentSet) {
                        showAlertBox("Niste uneli tekst ili prilog!");
                        return [2 /*return*/];
                    }
                    if (textValue.length == 256) {
                        showAlertBox("Tekst je predugacak!");
                        return [2 /*return*/];
                    }
                    attachmentSet = false;
                    document.getElementById("sent-text").value = "";
                    return [4 /*yield*/, sendAndEncrypt(textValue, currentGroup.id)];
                case 1:
                    _a.sent();
                    return [2 /*return*/];
            }
        });
    });
}
function saveAttachment(message) {
    return __awaiter(this, void 0, void 0, function () {
        var content, bytes, i, blob;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0: return [4 /*yield*/, fetchAttachment(message)];
                case 1:
                    content = _a.sent();
                    bytes = new Uint8Array(content.length);
                    for (i = 0; i < content.length; i++)
                        bytes[i] = content.charCodeAt(i);
                    blob = new Blob([bytes], { type: message.Attachments[0].Type });
                    saveAs(blob, message.Attachments[0].Name);
                    return [2 /*return*/];
            }
        });
    });
}
function checkIfEnterPress(event) {
    return __awaiter(this, void 0, void 0, function () {
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (!(event.keyCode == 13)) return [3 /*break*/, 2];
                    return [4 /*yield*/, sendMessage()];
                case 1:
                    _a.sent();
                    _a.label = 2;
                case 2: return [2 /*return*/];
            }
        });
    });
}
