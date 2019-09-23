var extension_id = chrome.runtime.id
var port;
window.addEventListener("message", function (event) {
	if (event.source != window) return;
	var message = event.data;
	console.log(message.destination);
	console.log(extension_id);
	if (message.destination && (message.destination === extension_id))
	{
		if (message.action && (message.action === "disconnect")) {
			if (typeof port !== "undefined") port.disconnect();
		} else {
			if (typeof port === "undefined") {
				port = chrome.runtime.connect();
				port.onMessage.addListener( function(message) {
					if (message.destination && (message.destination === extension_id)) {
						console.log("invalid message destination");
						console.log(message);
					}
					else {
						window.postMessage(message,"*");
					}
				} );
			}
			if (message.source && (message.source === extension_id)) {
				console.log("invalid message source");
				console.log(message);
			}
			else {
				port.postMessage(message);
			}
		}

	}
}, false );