"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (userId, userFullName, message, receiverId, messagesCounter, messagesCount) {

    //Uncomment if I decide to load the messages on parts

    //var lastMessageEl = document.getElementById("lastMessage");
    //lastMessageEl.style.backgroundColor = "#fff";
    //lastMessageEl.removeAttribute("id");

    var ulElement = document.getElementById("messagesList");

    var liElement = document.createElement("li");
    liElement.classList.add("d-flex");
    liElement.classList.add("justify-content-between");
    liElement.classList.add("mb-4");

    var cardDiv = document.createElement("div");
    cardDiv.classList.add("card");

    if (userId != receiverId) {
        cardDiv.style.color = "red";
    }

    if (messagesCounter == messagesCount) {
        ulElement.scrollTo(0, ulElement.scrollHeight);
    }

    //Uncomment if I decide to load the messages on parts

    //if (messagesCounter == messagesCount) {
    //    cardDiv.setAttribute("id", "lastMessage");
    //}

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

    usernameP.textContent = `${userFullName}`;
    messageP.textContent = `${message}`;

    //Uncomment if I decide to load the messages on parts

    //var lastMessageEl = document.getElementById("lastMessage");

    //var test = document.getElementById("messagesList");

    //$(test).scroll(function () {
    //    var hT = $('#lastMessage').offset().top,
    //        hH = $('#lastMessage').outerHeight(),
    //        wH = $(test).height(),
    //        wS = $(this).scrollTop();
    //    if (wS > (hT + hH - wH)) {
    //        if (lastMessageEl != null) {
    //            $(test).off('scroll');
    //            lastMessageEl.style.backgroundColor = "yellow";
    //        }
    //    }
    //});
});

connection.on("ShowHistory", function (messages, idFromUrl) {

    for (let i = 0; i <= messages.length; i++) {
        ShowChatHistory(messages[i], idFromUrl, i, messages.length - 1)
    }
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

function ShowChatHistory(message, idFromUrl, messagesCounter, messagesCount) {

    var stringifiedMessage = JSON.parse(JSON.stringify(message));

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

    if (messagesCounter == messagesCount) {
        ulElement.scrollTo(0, ulElement.scrollHeight);
    }

    //Uncomment if I decide to load the messages on parts

    //if (messagesCounter == messagesCount) {
    //    cardDiv.setAttribute("id", "lastMessage");
    //}

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

    //Uncomment if I decide to load the messages on parts

    //var lastMessageEl = document.getElementById("lastMessage");

    //var test = document.getElementById("messagesList");

    //$(test).scroll(function () {
    //    var hT = $('#lastMessage').offset().top,
    //        hH = $('#lastMessage').outerHeight(),
    //        wH = $(test).height(),
    //        wS = $(this).scrollTop();
    //    if (wS > (hT + hH - wH)) {
    //        if (lastMessageEl != null) {
    //            $(test).off('scroll');
    //            lastMessageEl.style.backgroundColor = "yellow";
    //        }
    //    }
    //});
}