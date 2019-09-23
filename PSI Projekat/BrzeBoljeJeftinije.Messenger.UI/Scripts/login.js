/**
 * Skripta koja se pokreće na login stranici
 * Autori:
 * Jovan Stevanović
 * Nikola Pavlović
 */
$(document).ready(() =>
{
    cardReader.init();
});
async function doCardLogin()
{
    try
    {
        var pingStatus = await cardReader.ping();
        if(!pingStatus)
        {
            showAlertBox("Ekstenzija ili program nisu instalirani");
            return false;
        }
        var certificate = await cardReader.getCertificate();
        var signature = await cardReader.sign(await asyncAjax("/Auth/GetSignatureToken"));
    }
    catch(err)
    {
        handleCardErrorMessage(err);
        return false;
    }
    var result = await asyncAjax("/Auth/", { Signature: signature, Certificate: certificate });
    if (result == "OK") {
        window.location.href = "/Messages";
    }
    else {
        showAlertBox(result.replace("FAIL:", ""));
        return false;
    }
    return false;
}
function getSimCredentials()
{
    if(localStorage.simId)
    {
        return localStorage.simId+":"+localStorage.simName+":"+localStorage.simSecret;
    }
    else
    {
        localStorage.simId="SIM-"+sessionId;
        localStorage.simName="Simulacioni korisnik "+Math.round(Math.random()*10000);
        localStorage.simSecret = ""+Math.round(Math.random()*10000000000);
        return localStorage.simId+":"+localStorage.simName+":"+localStorage.simSecret;
    }
}
async function doSimLogin()
{
    var pingStatus = await cardReader.ping();
    if (!pingStatus)
    {
        showAlertBox("Ekstenzija ili program nisu instalirani");
        return false;
    }
    var result = await asyncAjax("/Auth/", {
        Signature: "sim",
        Certificate: getSimCredentials()
    });
    if (result == "OK")
    {
        window.location.href = "/Messages";
    }
    else
    {
        showAlertBox(result.replace("FAIL:", ""));
    }
}
async function doLogin()
{
    blockElementWithMessage("Sačekajte");
    if($("#login_real")[0].checked)
    {
        doCardLogin();
    }
    else
    {
        doSimLogin();
    }
    $("#body").unblock();
    return false;
}