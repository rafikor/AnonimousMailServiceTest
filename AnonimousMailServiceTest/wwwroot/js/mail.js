"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/MailHub?userName=" + currentUserName).build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessages", function (messagesJson) {
    var messages = JSON.parse(messagesJson);
    
    for (var i = 0; i < messages.length; i++) {
        var buttonAsHeader = document.createElement("button");
        buttonAsHeader.className = "collapsible";
        buttonAsHeader.addEventListener("click", function () {
            this.classList.toggle("active");
            var content = this.nextElementSibling;
            if (content.style.maxHeight) {
                content.style.maxHeight = null;
            } else {
                content.style.maxHeight = content.scrollHeight * 0 + "px";
            }
        });

        buttonAsHeader.textContent = messages[i].TimeSent + "," + messages[i].Author + ': ' + messages[i].Title;
        var div = document.createElement("div");
        div.className = 'content';

        div.style.maxHeight = 0;
        let body = document.createElement("p");
        body.textContent = messages[i].Body;
        div.appendChild(body);

        let messagesNode = document.getElementById("messagesList");
        messagesNode.insertBefore(div, messagesNode.firstChild);
        messagesNode.insertBefore(buttonAsHeader, div);
    }
});

connection.on("ReceivePossibleRecipients", function (usersJson) {
    var users = JSON.parse(usersJson);
    let possibleUsersOptions = document.getElementById("possibleUsersOptions");
    for (var i = 0; i < users.length; i++) {
        var option = document.createElement("option");
        option.textContent = users[i];
        possibleUsersOptions.appendChild(option);
    }
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var author = currentUserName;
    var recipient = document.getElementById("recipientInput").value;
    var message = document.getElementById("messageInput").value;
    var theme = document.getElementById("themeInput").value;
    
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("POST", "PostMessage", true);
    xmlHttp.setRequestHeader("Content-Type", "multipart/form-data");

    const requestOptions = {
        method: 'POST',
        headers: {
            Author: author, Recipient: recipient, Title: theme, Body: message
        }
    };
    fetch("PostMessage", requestOptions);
    
    event.preventDefault();
});

