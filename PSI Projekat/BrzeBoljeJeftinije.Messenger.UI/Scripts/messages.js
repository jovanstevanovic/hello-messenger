/**
 * Skripta koja se pokreće na stranici za poruke
 * Autori:
 * Jovan Stevanović
 * Nikola Pavlović
 */
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
$(document).ready(function ()
{
    if (!firstReady) return;
    try
    {
        if (!cardReader.ping())
        {
            showAlertBox("Neuspela komunikacija sa ekstenzijom ili programom za šifrovanje");
            return;
        }
    }
    catch (err)
    {
        handleCardErrorMessage(err);
        return;
    }
    firstReady = false;
    cardReader.init();
    groupsScrollbar = new PerfectScrollbar('#groups',
        {
            suppressScrollX: true
        });
    $(".modal-inner").each(function (index, element)
    {
        new PerfectScrollbar(element);
    });
    refreshGroups(true);
    groupsScrollbar.update();
});

function getMyGroups(searchString)
{
    var returnedGroups = [];
    groups.forEach((group) =>
    {
        if (!(searchString) || group.name.indexOf(searchString) != -1)
        {
            returnedGroups.push(group);
        }
    });
    return groups;
}

function getGroupById(id)
{
    for (var g in groups)
    {
        if (groups[g].id == id) return groups[g];
    }
}

function createGroup(group)
{
    groups.add(group);
}

async function editGroup()
{
    blockElementWithMessage("Sačekajte");
    var html = await asyncAjax("/Group/Edit", { Id: currentGroup.id }, "GET");
    $("body").unblock();
    showGenericModal(html);
}
function setAttachmentIndicator()
{
    if (uploadedAttachment)
    {
        $("#attachment-indicator").addClass("attachment-indicator-active")
    }
    else
    {
        $("#attachment-indicator").removeClass("attachment-indicator-active")
    }
}

function loadGroup(group)
{
    $("#message-content").unblock();
    uploadedAttachment = null;
    setAttachmentIndicator();
    if (group == null)
    {
        currentGroup = null;
        newGroup = {
            members: []
        }
        showModal("#newgroup");
    }
    else
    {
        group = getGroupById(group);
        currentGroup = group;
        if (group.type == "request")
        {
            $("#group-edit").hide();
            $("#friend-delete").hide();
            $("#group-leave").hide();
        }
        else if (group.admin)
        {
            $("#group-edit").show();
            $("#friend-delete").hide();
            $("#group-leave").hide();
        }
        else
        {
            $("#group-edit").hide();
            if (group.binary)
            {
                $("#friend-delete").show();
                $("#group-leave").hide();
            }
            else
            {
                $("#friend-delete").hide();
                $("#group-leave").show();
            }
        }
        $("#group-picture").attr("src", group.picture);
        $("#group-name").html(group.name)
        $("#conversation").html("");
        if (group.type == "request")
        {
            $("#fileupload").attr('readonly', "true");
            $("#sent-text").attr('readonly', "true");
            if (group.sent)
            {
                $("#conversation").append(renderMessage(true, true, 'Poslali ste zahtev za prijateljstvo ' + group.name, null, new Date()));
            }
            else
            {
                $("#conversation").append(renderMessage(false, true, group.name + ' vam je poslao zahtev za prijateljstvo', null, new Date()));
            }
        }
        else
        {
            $("#sent-text").removeAttr("readonly");
            $("#fileupload").removeAttr("readonly");
            showMessagesForCurrentGroup(currentGroup);
        }
        $("#group-picture").show();
        redrawGroups(true);
    }
    setTimeout(function ()
    {
        var objDiv = document.getElementById("conversation");
        objDiv.scrollTop = objDiv.scrollHeight;
    }, 100);
}

function attachFile()
{
    sendingMessage = true;
    $("#fileupload").click();
}

function uploadFile()
{
    attachmentSet = true;
    readURL($("#fileupload")[0])
}

function readURL(input)
{
    if (input.files && input.files[0])
    {
        var reader = new FileReader();

        reader.onload = function (e)
        {
            if (e.target.result.length > 50 * 1024 * 1024)
            {
                uploadedAttachment = null;
                setAttachmentIndicator();
                showAlertBox("Veličina poslatih fajlova je ograničena na 50MB");
                return;
            }
            uploadedAttachment = e.target.result;
            setAttachmentIndicator();
        };
        uploadedAttachmentType = input.files[0].type;
        if (!uploadedAttachmentType)
        {
            uploadedAttachmentType = "application/octet-stream";
        }
        uploadedAttachmentName = input.files[0].name;
        reader.readAsBinaryString(input.files[0]);
    }
}

async function accept()
{
    blockElementWithMessage("Sačekajte");
    var result = await asyncAjax("/User/Accept", { otherId: currentGroup.otherId });
    $("body").unblock();
    refreshGroups(true);
}

async function reject()
{
    blockElementWithMessage("Sačekajte");
    var result = await asyncAjax("/User/Reject", { otherId: currentGroup.otherId });
    $("body").unblock();
    refreshGroups(true);
}

function redrawGroups(onlyRedrawUnread)
{
    if (!onlyRedrawUnread)
    {
        $("#group-list").html("");
        $("#group-list").append(renderChip('createNewGroup()', 'fa-plus', null, null, "Nova grupa"));
    }
    groups.forEach(function (group)
    {
        if (onlyRedrawUnread)
        {
            if (group.unread)
            {
                $("#grp-" + group.id).find(".chip").addClass("unread-chip");
            }
            else
            {
                $("#grp-" + group.id).find(".chip").removeClass("unread-chip");
            }
            if (group.id == currentGroup.id) $("#grp-" + group.id).find(".chip").addClass("selected-chip");
            else $("#grp-" + group.id).find(".chip").removeClass("selected-chip");
        }
        else
        {
            if ($("#group-search").val().trim() != "" && group.name.toUpperCase().indexOf($("#group-search").val().trim().toUpperCase()) == -1) return;
            $("#group-list").append(renderChip('loadGroup("' + group.id + '")', null, group.picture, null, group.name, currentGroup != null && group.id == currentGroup.id, group.unread, "grp-"+group.id));
        }
    })
    $(".chiptext").ellipsis();
}
async function refreshGroups(loadFirst, onlyRedrawUnread)
{
    if (!onlyRedrawUnread) blockElementWithMessage("Učitavam", "#groups");
    var fetchedGroups = await fetchGroups();
    groups = [];
    requests = {};
    friends = {};
    var knownGroups = {}
    console.log(fetchedGroups);
    fetchedGroups.forEach((group) =>
    {
        if (group.binary)
        {
            friends[group.otherId] = true;
        }
        if (group.type == "request")
        {
            requests[group.otherId] = true;
        }
        groups.push(group);
        knownGroups[group.id] = true;
    });
    groups.sort((a, b) =>
    {
        if (a.timestamp > b.timestamp) return -1;
        else if (a.timestamp < b.timestamp) return 1;
        else return 0;
    });
    redrawGroups(onlyRedrawUnread);
    if (loadFirst || (currentGroup != null && !(knownGroups[currentGroup.id])))
    {
        if (groups.length > 0)
        {
            loadGroup(groups[0].id);
        }
        else
        {
            $("#group-picture").hide();
            $("#friend-delete").hide();
            $("#group-leave").hide();
            $("#group-edit").hide();
        }
    }
    if (!onlyRedrawUnread) $("#groups").unblock();
}

async function searchUsers(event)
{
    if (event.key.length > 1) return;
    $("#zahtev-poslat").hide();
    $("#friend-list").html("");
    if ($("#friend-search").val().trim() != "")
    {
        if (!(previousUserSearchQuery != null && $("#friend-search").val().trim().indexOf(previousUserSearchQuery) != -1 && userSearchResult != null))
        {
            userSearchResult = null;
            blockElementWithMessage("#friend-list", "Pretražujem")
            userSearchResult = await asyncAjax("/User/Search",
                {
                    name: $("#friend-search").val().trim()
                }, "POST");
            $("#friend-list").unblock();
        }
        previousUserSearchQuery = $("#friend-search").val().trim();
        userSearchResult.forEach(function (user)
        {
            if ($("#friend-search").val().trim() != "" && user.name.toUpperCase().indexOf($("#friend-search").val().trim().toUpperCase()) == -1) return;
            if (friends[user.id] || requests[user.id]) return;
            $("#friend-list").append(renderChip('sendFriendRequest(this, "' + user.id + '")', null, user.picture, 'fa-plus', user.name))
        });
    }
}

async function sendFriendRequest(source, id)
{
    blockElementWithMessage("Sačekajte");
    await asyncAjax("/User/SendRequest",
        {
            id: id
        });
    $("body").unblock();
    await refreshGroups();
    $(source).remove();
    showMessageBox("Zahtev uspešno poslat");
}

function renderChip(onclick, fa, image, rightFa, text, selected, unread, chipId)
{
    if (fa)
    {
        return "<a href='#' onclick='" + onclick + "' " + ((chipId) ? ("id='" + chipId+"'"):(""))+">" +
            "<li> <div class='chip " + (selected ? "selected-chip" : "") + "'> " +
            "<div style='float:left;margin-right:25px'><i class='fa " + fa + " fa-2x' style='margin-left:-10px;margin-top:10px' aria-hidden='true'></i></div> <span class='chiptext'> <div style='float:left'>" + text + "</div>" +
            (rightFa ? "<div style='float:left;margin-right:25px'><i class='fa " + rightFa + " fa-2x' style='margin-left:-10px;margin-top:10px' aria-hidden='true'></i></div>" : "") + "</span>" +
            "</div> </li>" +
            "</a>";
    }
    else if (image)
    {
        return "<a href='#' onclick='" + onclick + "' " + ((chipId) ? ("id='" + chipId + "'") : ("")) + ">" +
            "<li><div class='chip " + (selected ? "selected-chip" : "") + (unread ? " unread-chip" : "") + "'> " +
            "<img src='" + image + "' width='96' height='96'> <span class='chiptext'> <div style='float:left'>" +
            text +
            "</div>" +
            (rightFa ? "<div style='float:right'><i class='fa " + rightFa + " fa-2x' style='margin-left:-10px;margin-top:10px' aria-hidden='true'></i></div>" : "") + "</span>" +
            "</div>  </li></a>" +
            "";
    }
}
async function renderActualMessage(message)
{
    var attString = [];
    if (message.Attachments.length > 0) 
    {
        for (var i = 0; i < message.Attachments.length;i++)
        {
            var attachment = message.Attachments[i];
            if (attachment.Type.indexOf("image") == 0)
            {
                attString.push('<img onclick="displaySentImg(this)" class="sent-img" src=' + 'data:' + attachment.Type + ';base64,' + btoa(await fetchAttachment(message)) + ' />');
            }
            else
            {
                var msgJson = JSON.stringify(message);
                attString.push('<a href="#" onclick=\'saveAttachment(' + msgJson + ')\'>' + attachment.Name + '</a>');
            }
        }
    }
    var date = message.Time;
    var dateString =
        ("0" + date.getDate()).slice(-2) + "/" +
        ("0" + (date.getMonth() + 1)).slice(-2) + "/" +
        date.getFullYear() + "  " +
        ("0" + date.getHours()).slice(-2) + ":" +
        ("0" + date.getMinutes()).slice(-2) + ":" +
        ("0" + date.getSeconds()).slice(-2);
    attString = attString.join("<br/>");
    if (attString) attString = "<br/>" + attString;
    var usersPictureSource = "/User/Picture?id=" + message.Sender + "";
    return '<div class="row message-body" id=' + message.Id + '>' +
                    ((message.Sender != currentUserId)?('<img src=' + usersPictureSource + ' class="senderpic">'):'') +
                    ((message.Sender == currentUserId) ? ('<div class="col-sm-12 message-main-sender">') : ('<div class="col-sm-12 message-main-receiver">')) +
                    ((message.Sender == currentUserId) ? ('<div class="sender">') : ('<div class="receiver">')) +
                    '<div class="message-text">' +
                    message.Content +
                    attString +
                    '</div>' +
                    '<span class="message-time pull-right">' +
                    dateString +
                    '</span>' +
        ((message.Sender == currentUserId) ? ('<a href="#" onclick="deleteMessage(this)">' +
            '<i class="fa fa-trash search-icon" aria-hidden="true"></i>' +
        '</a>') : "") +
                    '</div>' +
                    '</div>' +
                    '</div>';
}

function renderMessage(sent, friendRequest, text, image, date, senderpic, senderText, notrash = false)
{
    type = "receiver";
    if (friendRequest) type = "friend-request";
    else if (sent) type = "sender";
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
        '</div>'
}

async function leaveGroup()
{
    openPrompt("Da li ste sigurni da želite da napustite grupu?", async (accept) =>
    {
        if (accept)
        {
            blockElementWithMessage("Sačekajte");
            var result = await asyncAjax("/Group/Leave/" + currentGroup.id);
            $("body").unblock();
            if (result == "OK")
            {
                showMessageBox("Uspešno ste napustili grupu");
                refreshGroups(false);
            }
            else
            {
                showAlertBox(result);
            }
        }
    });
}

function deleteFriend()
{
    openPrompt("Da li ste sigurni da želite da obrišete prijatelja?", async (accept) =>
    {
        if (accept)
        {
            blockElementWithMessage("Sačekajte");
            var result = await asyncAjax("/User/DeleteFriendship/" + currentGroup.otherId);
            $("body").unblock();
            if (result == "OK")
            {
                showMessageBox("Uspešno ste obrisali prijatelja");
                refreshGroups(false);
            }
            else
            {
                showAlertBox(result);
            }
            refreshGroups(false);
        }
    });
}

function displaySentImg(image)
{
    $("#picture-display").find(".display-picture").attr("src", $(image).attr("src"));
    showModal("#picture-display")
}
async function fetchGroups()
{
    return await asyncAjax("/Group/My");
}
async function createNewGroup()
{
    blockElementWithMessage("Sačekajte");
    var html = await asyncAjax("/Group/Edit", {}, "GET");
    $("body").unblock();
    showGenericModal(html);
}
async function getPksForGroup(id)
{
    var pks = await asyncAjax("/Messages/GetPksForGroup", { "id": id });
    return pks;
}
async function encryptLongFile(file, key)
{
    result = "";
    var CHUNK_SIZE = 256 * 1024;
    var start = 0;
    var end = CHUNK_SIZE;
    while (start < file.length)
    {
        end = Math.min(end, file.length);
        var chunk = file.slice(start, end);
        try
        {
            var ciphertext = await cardReader.aesEncrypt(chunk, key);
        }
        catch (err)
        {
            handleCardErrorMessage(err);
            throw err;
        }
        if (result != "") result += "|";
        result += ciphertext;
        start += CHUNK_SIZE;
        end = start + CHUNK_SIZE;
    }
    return result;
}
async function decryptLongFile(file, key)
{
    try
    {
        var chunks = file.split("|");
        var plaintext = "";
        for (var chunk in chunks)
        {
            plaintext += await cardReader.aesDecrypt(chunks[chunk], key);
        }
        return plaintext;
    }
    catch (err)
    {
        handleCardErrorMessage(err);
        throw err;
    }
}
var rsaDecryptionCache = {};
async function getSymmetricKey(encKey)
{
    if (rsaDecryptionCache[encKey]) return rsaDecryptionCache[encKey];
    try
    {
        var result = await cardReader.rsaDecrypt(encKey);
    }
    catch (err)
    {
        handleCardErrorMessage(err);
        throw err;
    }
    rsaDecryptionCache[encKey] = result;
    return result;
}
async function decryptMessage(message)
{
    var encryptedKey = message.CryptoMaterial.split("$")[0];
    var signature = message.CryptoMaterial.split("$")[1];
    var actualKey = simUser ? atob(encryptedKey) : (await getSymmetricKey(encryptedKey));
    try
    {
        var plaintext = await cardReader.aesDecrypt(message.Content, actualKey);
    }
    catch (err)
    {
        handleCardErrorMessage(err);
        throw err;
    }
    message.Content = escapeHtml(decodeURI(plaintext));
}
async function predecryptKeys(messages)
{
    if (simUser) return;
    keys = [];
    for (var i = 0; i < messages.length; i++)
    {
        message = messages[i];
        var encryptedKey = message.CryptoMaterial.split("$")[0];
        if (!rsaDecryptionCache[encryptedKey]) keys.push(encryptedKey);
    }
    try
    {
        var decrypted = await cardReader.rsaDecryptMulti(keys);
    }
    catch (err)
    {
        handleCardErrorMessage(err);
        throw err;
    }
    for (var i = 0; i < keys.length; i++)
    {
        rsaDecryptionCache[keys[i]] = atob(decrypted[i]);
    }
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
async function fetchAttachment(message)
{
    var content = await asyncAjax("/Messages/GetAttachment", {
        messageId: message.Id,
        id: message.Attachments[0].Id
    });
    var encryptedKey = message.CryptoMaterial.split("$")[0];
    var actualKey = simUser ? atob(encryptedKey) : (await getSymmetricKey(encryptedKey));
    var content = await decryptLongFile(content, actualKey);
    return content;
}

/*
 * Dohvata zadati deo poruka za zadatu grupu.
 * Vraća niz objekata
 */
async function fetchMessages(groupId, pageId, toBlock)
{
    if (!toBlock) toBlock = "#message-content";
    blockElementWithMessage("Dohvatam poruke", toBlock);
    var response = await asyncAjax("/Messages/GetMessages", {
        "groupId": groupId,
        "page": pageId
    });
    blockElementWithMessage("Dеšifrujem poruke: 0%", toBlock);
    await predecryptKeys(response);
    var cnt = 0;
    for (var message in response)
    {
        await decryptMessage(response[message]);
        cnt += 1;
        var percentage = Math.round(cnt * 100 / response.length);
        blockElementWithMessage("Dеšifrujem poruke: " + percentage + "%", toBlock);
    }
    return response;
}

/**
 * Briše poruku sa zadatim ID-jem
 * @param id ID poruke
 */
async function deleteMessage(message)
{
    message = $(message).closest(".message-body");
    blockElementWithMessage(message, "Brišem");
    await asyncAjax("/Messages/Delete", { "id": message.attr("id") });
    message.remove();

}

/**
 * Šalje poruku sa zadatim tekstom na zadatu grupu.
 * Ako postoji attachment, šalje i njega
 * @param {any} messageText
 * @param {any} groupId
 */
async function sendAndEncrypt(messageText, groupId)
{
    try
    {
        var pks = await getPksForGroup(groupId);
        var key = await cardReader.getAesKey();
        var encodedText = encodeURI(messageText);
        var encryptedText = await cardReader.aesEncrypt(encodedText, key);
        var keys = {};
        var signature = "sim";
        if (await cardReader.check())
        {
            var signature = await cardReader.sign(key);
        }
        for (var i in pks)
        {
            var userId = pks[i]["id"];
            var pubkey = pks[i]["key"];
            var encryptedKey = await cardReader.rsaEncrypt(key, pubkey, null);
            keys[userId] = encryptedKey + "$" + signature;
        }
        var sentData = {
            GroupId: groupId,
            Ciphertext: encryptedText,
            Materials: JSON.stringify(keys)
        };
        if (uploadedAttachment != null)
        {
            sentData.Attachment = await encryptLongFile(uploadedAttachment, key);
            sentData.AttachmentName = uploadedAttachmentName;
            sentData.AttachmentType = uploadedAttachmentType;
        }
        var result = await asyncAjax("/Messages/Send", sentData);
        if (result == "OK")
        {
            uploadedAttachment = null;
            setAttachmentIndicator();
        }
        else
        {
            showAlertBox(result);
        }
    }
    catch (err)
    {
        handleCardErrorMessage(err);
        throw err;
    }

}

async function messagesCameForGroup(groupId)
{
    if (groupId == currentGroup.id)
    {
        $("#conversation").append($('<div class="row message-body" id="dummydiv" style="height:50px"></div>'));
        var objDiv = document.getElementById("conversation");
        objDiv.scrollTop = objDiv.scrollHeight;
        var messagesTemp = await fetchMessages(groupId, 0, "#dummydiv");
        if (currentGroup.id != groupId) return;
        for (var i = 0; i < messagesTemp.length; i++)
        {
            var time = messagesTemp[i].Time.substring(6, 19);
            messagesTemp[i].Time = new Date(parseInt(time));
        }

        messagesTemp = messagesTemp.sort(function (a, b) { return a.Time.getTime() - b.Time.getTime() });

        var messages = new Array();
        var found = false;
        var messagesList = $("#conversation").children("div.row");
        for (var i = 0; i < messagesTemp.length; i++)
        {
            for (var j = 0; j < messagesList.length; j++)
            {
                if (messagesTemp[i].Id == messagesList[j].id)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                messages[messages.length] = messagesTemp[i]; // messages contain all new messages. messagesTemp contain 10 messages, including messages
            found = false;                                   // that already existis.
        }

        var renderedHtml = "";
        for (var i = 0; i < messages.length; i++) 
        {
            renderedHtml += (await renderActualMessage(messages[i]));
        }
        if (currentGroup.id != groupId) return;
        $("#dummydiv").unblock();
        $("#dummydiv").remove();
        $("#conversation").append(renderedHtml);
        var objDiv = document.getElementById("conversation");
        objDiv.scrollTop = objDiv.scrollHeight;
    }
    else
    {
        refreshGroups(false, true);
    }
}
var currentPage;
async function showMessagesForCurrentGroup(current, page)
{
    var currentTopMessage = $($('#conversation').children()[1]);
    if (!current) current = currentGroup;
    $("#toplink").remove();
    if (page == null) page = 0;
    currentPage =page;
    var messages = await fetchMessages(current.id, page);
    if (current.id != currentGroup.id) return;
    for (var i = 0; i < messages.length; i++)
    {
        var time = messages[i].Time.substring(6, 19); // Extracting milliseconds from string Time.
        messages[i].Time = new Date(parseInt(time));
    }

    messages = messages.sort(function (a, b) { if (page == 0) return a.Time.getTime() - b.Time.getTime(); else return b.Time.getTime() - a.Time.getTime(); });
    var renderedHtml = "";
    for (var i = 0; i < messages.length; i++) 
    {
        if (page == 0) renderedHtml += (await renderActualMessage(messages[i]));
        else renderedHtml = (await renderActualMessage(messages[i])) + renderedHtml;
    }
    if (current.id == currentGroup.id)
    {
        if (page == 0) $("#conversation").append(renderedHtml);
        else $("#conversation").prepend(renderedHtml);
    }
    if (messages.length > 0)
    {
        $("#conversation").prepend('<div class="row message-body" id="toplink" style="text-align:center">' + "<a href='#' onclick='showMessagesForCurrentGroup(null, "+(page+1)+")'> Učitaj još</a > " + "</div > ");
    }
    if (page == 0)
    {
        var objDiv = document.getElementById("conversation");
        objDiv.scrollTop = objDiv.scrollHeight;
    }
    else
    {
        $("#conversation").scrollTop(currentTopMessage.offset().top - 2*currentTopMessage.height());
    }
    $("#message-content").unblock();
    refreshGroups(false, true);
}

var attachmentSet = false; // Detect if message contain attachment, but not text.

async function sendMessage()
{
    var textValue = document.getElementById("sent-text").value;
    if (textValue.length == 0 && !attachmentSet) 
    {
        showAlertBox("Niste uneli tekst ili prilog!");
        return;
    }

    if (textValue.length == 256) 
    {
        showAlertBox("Tekst je predugacak!");
        return;
    }

    blockElementWithMessage("Šaljem", "#message-entry");
    attachmentSet = false;
    await sendAndEncrypt(textValue, currentGroup.id);
    document.getElementById("sent-text").value = "";
    $("#message-entry").unblock();
}

async function saveAttachment(message)
{
    blockElementWithMessage("Sačekajte");
    var content = await fetchAttachment(message);
    $("body").unblock();
    var bytes = new Uint8Array(content.length);
    for (var i = 0; i < content.length; i++) bytes[i] = content.charCodeAt(i);
    var blob = new Blob([bytes], { type: message.Attachments[0].Type });
    saveAs(blob, message.Attachments[0].Name);
}

async function checkIfEnterPress(event)
{
    if (event.keyCode == 13)
    {
        await sendMessage();
    }
}