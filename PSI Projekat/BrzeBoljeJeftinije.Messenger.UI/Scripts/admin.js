/**
 * Skripta koja se učitava na admin stranici
 * Autori:
 * Jovan Stevanović
 * Nikola Pavlović
 */
var previousUserSearchQuery = null;
var userSearchResult = null;
$(document).ready(function ()
{

});
async function searchUsers(event)
{
    if (event.key.length > 1) return;
    var query = $("#user-search").val().trim();
    $("#user-list-body").html("");
    if ($("#user-search").val().trim() != "")
    {
        if (!(previousUserSearchQuery != null && $("#user-search").val().trim().indexOf(previousUserSearchQuery) != -1 && userSearchResult != null))
        {
            userSearchResult = null;
            blockElementWithMessage("Tražim", "#user-list-body");
            userSearchResult = await asyncAjax("/Admin/Search",
                {
                    name: $("#user-search").val().trim()
                });
            $("#user-list-body").unblock();
        }
        previousUserSearchQuery = $("#user-search").val().trim();
        userSearchResult.forEach(function (userObject)
        {
            if (userObject.name.toUpperCase().indexOf(query.toUpperCase()) == -1) return;
            $("#user-list-body").append(renderTableRow(userObject.id, userObject.name, userObject.status, userObject.banDate, userObject.picture));
        });
    }
}
function renderTableRow(userId, name, status, bandate, image)
{
    var row=$('<tr data-user-id="'+userId+'">'+
                '<td>'+name+'<img src="' + image + '" class="user-list-image"></td>'+
                '<td>'+
                    '<select class="form-control ban-status">'+
                        '<option value="ok">Dozvoljen</option>'+
                        '<option value="ban">Banovan</option>'+
                    '</select>'+
    '</td>' +
    '<td><input type="text" class="form-control user-ban-date" /></td>' +
    '<td><input type="button" class="form-control" value="Ažuriraj" onclick="updateUserStatus(this)" /><input type="button" class="form-control" value="Obriši" onclick="deleteUser(this)" /></td>' +
'</tr>');
    row.find(".ban-status").val(status);
    row.find(".user-ban-date").datepicker(datePickerParams);
    row.find(".user-ban-date").datepicker("setDate", bandate);
    return row;
}
async function updateUserStatus(sender)
{
    var target = $(sender).closest("tr");
    var userId = target.attr("data-user-id");
    var status = target.find(".ban-status").val();
    var date = target.find(".user-ban-date").datepicker("getDate");
    blockElementWithMessage("Ažuriram", target);
    var response = await asyncAjax("/Admin/UpdateStatus", { Id: userId, Status: status, ExpiryDate: date.toISOString() });
    target.unblock();
    userSearchResult = null;
    if (response.indexOf("FAIL:") == 0)
    {
        showAlertBox(response.replace("FAIL:", ""));
    }
    else
    {
        showMessageBox("Stanje korisnika uspešno promenjeno");
    }
    searchUsers();
}
var userToDelete = null;
function deleteUser(sender)
{
    var target = $(sender).closest("tr");
    var userId = target.attr("data-user-id");
    userToDelete = userId;
    showModal("#confirm-delete")
}
function confirmDelete()
{
    delete users[userToDelete];
    searchUsers();
    $(".modal").hide();
}
function registerNewAdmin()
{
    showModal("#register-new-admin")
}
async function confirmAdminRegister()
{
    var name = $("#new-admin-name").val();
    var password = $("#new-admin-password").val();
    var confirm = $("#new-admin-password-confirm").val();
    if (name.trim().length == 0 || password.trim().length == 0)
    {
        showAlertBox("Niste uneli sve podatke");
        return;
    }
    if (password != confirm)
    {
        showAlertBox("Lozinke se ne poklapaju");
        return;
    }
    blockElementWithMessage("Kreiram");
    var response = await asyncAjax("/Admin/Register", { Username: name, Password: password });
    $("body").unblock();
    if (response.indexOf("FAIL:") != -1)
    {
        showAlertBox(response.replace("FAIL:", ""));
    }
    else
    {
        showMessageBox("Registracija uspešna");
    }
}
async function setNewPassword()
{
    var password = $("#new-password").val();
    var confirm = $("#new-password-confirm").val();
    if (password.length == 0)
    {
        showAlertBox("Lozinka ne može biti prazna");
        return;
    }
    if (password != confirm)
    {
        showAlertBox("Lozinke se ne poklapaju");
        return;
    }
    blockElementWithMessage("Menjam");
    await asyncAjax("/Admin/MyPassword", { password: password });
    showMessageBox("Lozinka uspešno promenjena");
    $("body").unblock();
}