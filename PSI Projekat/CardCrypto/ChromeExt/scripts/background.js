function validSender(hostname)
{
    var okHostnames = ["localhost", "psi-messenger-prod.azurewebsites.net"];
    return okHostnames.indexOf(hostname) != -1;
}

chrome.runtime.onConnect.addListener( function(port) {
	
	var manifest = chrome.runtime.getManifest();
	port.port = chrome.runtime.connectNative(manifest.name.toLowerCase());
	console.log(manifest.name.toLowerCase());
	port.port.port = port;
	var url = new URL(port.sender.url);
	if (!validSender(url.hostname))
	{
	    console.log("Refused request from: "+port.sender.url);
	    return;
	}
	else
	{
	    console.log("Accepted request from: " + port.sender.url);
	}
	port.onMessage.addListener(function (message, sender) {
		return sender.port.postMessage(message); 
	} );

	port.port.onMessage.addListener(function (message, sender) {
		return sender.port.postMessage(message);
	} );
	
	port.onDisconnect.addListener(function (sender) {
		sender.port.disconnect();
	} );

} );