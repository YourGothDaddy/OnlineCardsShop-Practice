"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {

    var ulElement = document.getElementById("messagesList");

    var liElement = document.createElement("li");
    liElement.classList.add("d-flex");
    liElement.classList.add("justify-content-between");
    liElement.classList.add("mb-4");

    var cardDiv = document.createElement("div");
    cardDiv.classList.add("card");

    var usernameDiv = document.createElement("div");
    usernameDiv.classList.add("card-header");
    usernameDiv.classList.add("d-flex");
    usernameDiv.classList.add("justify-content-between");
    usernameDiv.classList.add("p-3");

    var usernameP = document.createElement("p");
    usernameP.classList.add("fw-bold");
    usernameP.classList.add("mb-0");

    var sentTimeAgoP = document.createElement("p");
    sentTimeAgoP.classList.add("text-muted");
    sentTimeAgoP.classList.add("small");
    sentTimeAgoP.classList.add("mb-0");

    var cardBodyDiv = document.createElement("div");
    cardBodyDiv.classList.add("card-body");

    var messageP = document.createElement("p");
    messageP.classList.add("mb-0");

    liElement.appendChild(cardDiv);

    cardDiv.appendChild(usernameDiv);
    usernameDiv.appendChild(usernameP);
    usernameDiv.appendChild(sentTimeAgoP);

    liElement.appendChild(cardBodyDiv);

    cardBodyDiv.appendChild(messageP);

    ulElement.appendChild(liElement);

    usernameP.textContent = `${user}`;
    messageP.textContent = `${message}`;
});

connection.on("ShowHistory", function (messages, idFromUrl) {

    messages.forEach(message => ShowChatHistory(message, idFromUrl));

});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    var idFromUrl = location.pathname.split('/')[3];

    connection.invoke("CreateGroup", idFromUrl).catch(function (err) {
        return console.error(err.toString());
    });

    connection.invoke("RetrieveChatHistory", idFromUrl).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {

    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;

    var idFromUrl = location.pathname.split('/')[3];

    connection.invoke("SendMessage", message, idFromUrl).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

function ShowChatHistory(message, idFromUrl) {

    var stringifiedMessage = JSON.parse(JSON.stringify(message));

    console.log(stringifiedMessage.userId);
    console.log(idFromUrl);

    var ulElement = document.getElementById("messagesList");

    var liElement = document.createElement("li");
    liElement.classList.add("d-flex");
    liElement.classList.add("justify-content-between");
    liElement.classList.add("mb-4");

    var cardDiv = document.createElement("div");
    cardDiv.classList.add("card");

    if (stringifiedMessage.userId != idFromUrl) {
        cardDiv.style.color = "red";
    }

    var usernameDiv = document.createElement("div");
    usernameDiv.classList.add("card-header");
    usernameDiv.classList.add("d-flex");
    usernameDiv.classList.add("justify-content-between");
    usernameDiv.classList.add("p-3");

    var usernameP = document.createElement("p");
    usernameP.classList.add("fw-bold");
    usernameP.classList.add("mb-0");

    var sentTimeAgoP = document.createElement("p");
    sentTimeAgoP.classList.add("text-muted");
    sentTimeAgoP.classList.add("small");
    sentTimeAgoP.classList.add("mb-0");

    var cardBodyDiv = document.createElement("div");
    cardBodyDiv.classList.add("card-body");

    var messageP = document.createElement("p");
    messageP.classList.add("mb-0");

    liElement.appendChild(cardDiv);

    cardDiv.appendChild(usernameDiv);
    usernameDiv.appendChild(usernameP);
    usernameDiv.appendChild(sentTimeAgoP);

    liElement.appendChild(cardBodyDiv);

    cardBodyDiv.appendChild(messageP);

    ulElement.appendChild(liElement);

    usernameP.textContent = `${stringifiedMessage.user.fullName}`;
    messageP.textContent = `${stringifiedMessage.content}`;
}