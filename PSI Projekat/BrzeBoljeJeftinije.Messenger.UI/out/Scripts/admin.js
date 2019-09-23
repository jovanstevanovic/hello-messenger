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
var previousUserSearchQuery = null;
var userSearchResult = null;
$(document).ready(function () {
});
function searchUsers(event) {
    return __awaiter(this, void 0, void 0, function () {
        var query;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    if (event.key.length > 1)
                        return [2 /*return*/];
                    query = $("#user-search").val().trim();
                    $("#user-list-body").html("");
                    if (!($("#user-search").val().trim() != "")) return [3 /*break*/, 3];
                    if (!!(previousUserSearchQuery != null && $("#user-search").val().trim().indexOf(previousUserSearchQuery) != -1 && userSearchResult != null)) return [3 /*break*/, 2];
                    userSearchResult = null;
                    return [4 /*yield*/, asyncAjax("/Admin/Search", {
                            name: $("#user-search").val().trim()
                        })];
                case 1:
                    userSearchResult = _a.sent();
                    _a.label = 2;
                case 2:
                    previousUserSearchQuery = $("#user-search").val().trim();
                    userSearchResult.forEach(function (userObject) {
                        if (userObject.name.toUpperCase().indexOf(query.toUpperCase()) == -1)
                            return;
                        $("#user-list-body").append(renderTableRow(userObject.id, userObject.name, userObject.status, userObject.banDate, userObject.picture));
                    });
                    _a.label = 3;
                case 3: return [2 /*return*/];
            }
        });
    });
}
function renderTableRow(userId, name, status, bandate, image) {
    var row = $('<tr data-user-id="' + userId + '">' +
        '<td>' + name + '<img src="' + image + '" class="user-list-image"></td>' +
        '<td>' +
        '<select class="form-control ban-status">' +
        '<option value="ok">Dozvoljen</option>' +
        '<option value="ban">Banovan</option>' +
        '</select>' +
        '</td>' +
        '<td><input type="text" class="form-control user-ban-date" /></td>' +
        '<td><input type="button" class="form-control" value="Ažuriraj" onclick="updateUserStatus(this)" /><input type="button" class="form-control" value="Obriši" onclick="deleteUser(this)" /></td>' +
        '</tr>');
    row.find(".ban-status").val(status);
    row.find(".user-ban-date").datepicker(datePickerParams);
    row.find(".user-ban-date").datepicker("setDate", bandate);
    return row;
}
function updateUserStatus(sender) {
    return __awaiter(this, void 0, void 0, function () {
        var target, userId, status, date, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    target = $(sender).closest("tr");
                    userId = target.attr("data-user-id");
                    status = target.find(".ban-status").val();
                    date = target.find(".user-ban-date").datepicker("getDate");
                    return [4 /*yield*/, asyncAjax("/Admin/UpdateStatus", { Id: userId, Status: status, ExpiryDate: date.toISOString() })];
                case 1:
                    response = _a.sent();
                    userSearchResult = null;
                    if (response.indexOf("FAIL:") == 0) {
                        showAlertBox(response.replace("FAIL:", ""));
                    }
                    else {
                        showMessageBox("Stanje korisnika uspešno promenjeno");
                    }
                    searchUsers();
                    return [2 /*return*/];
            }
        });
    });
}
var userToDelete = null;
function deleteUser(sender) {
    var target = $(sender).closest("tr");
    var userId = target.attr("data-user-id");
    userToDelete = userId;
    showModal("#confirm-delete");
}
function confirmDelete() {
    delete users[userToDelete];
    searchUsers();
    $(".modal").hide();
}
function registerNewAdmin() {
    showModal("#register-new-admin");
}
function confirmAdminRegister() {
    return __awaiter(this, void 0, void 0, function () {
        var name, password, confirm, response;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    name = $("#new-admin-name").val();
                    password = $("#new-admin-password").val();
                    confirm = $("#new-admin-password-confirm").val();
                    if (name.trim().length == 0 || password.trim().length == 0) {
                        showAlertBox("Niste uneli sve podatke");
                        return [2 /*return*/];
                    }
                    if (password != confirm) {
                        showAlertBox("Lozinke se ne poklapaju");
                        return [2 /*return*/];
                    }
                    return [4 /*yield*/, asyncAjax("/Admin/Register", { Username: name, Password: password })];
                case 1:
                    response = _a.sent();
                    if (response.indexOf("FAIL:") != -1) {
                        showAlertBox(response.replace("FAIL:", ""));
                    }
                    else {
                        showMessageBox("Registracija uspešna");
                    }
                    return [2 /*return*/];
            }
        });
    });
}
function setNewPassword() {
    return __awaiter(this, void 0, void 0, function () {
        var password, confirm;
        return __generator(this, function (_a) {
            switch (_a.label) {
                case 0:
                    password = $("#new-password").val();
                    confirm = $("#new-password-confirm").val();
                    if (password.length == 0) {
                        showAlertBox("Lozinka ne može biti prazna");
                        return [2 /*return*/];
                    }
                    if (password != confirm) {
                        showAlertBox("Lozinke se ne poklapaju");
                        return [2 /*return*/];
                    }
                    return [4 /*yield*/, asyncAjax("/Admin/MyPassword", { password: password })];
                case 1:
                    _a.sent();
                    showMessageBox("Lozinka uspešno promenjena");
                    return [2 /*return*/];
            }
        });
    });
}
