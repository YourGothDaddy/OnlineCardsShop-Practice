"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (userId, userFullName, message, receiverId) {

    var idFromUrl = location.pathname.split('/')[3];

    var ulElement = document.getElementById("messagesList");

    var liElement = document.createElement("li");
    liElement.classList.add("d-flex");
    liElement.classList.add("justify-content-between");
    liElement.classList.add("mb-4");

    var cardDiv = document.createElement("div");
    cardDiv.style.backgroundColor = "white";

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

    var messageDiv = document.createElement("p");
    messageDiv.classList.add("mb-0");

    if (userId != idFromUrl) {
        messageDiv.style.color = "#212529";
        messageDiv.style.backgroundColor = "#d6d6d6";
        messageDiv.style.padding = "1rem";
        messageDiv.style.borderRadius = "5px";
    }

    liElement.appendChild(cardDiv);

    if (userId == idFromUrl) {
        cardDiv.appendChild(usernameDiv);
        usernameDiv.appendChild(usernameP);
        usernameDiv.appendChild(sentTimeAgoP);
    }

    liElement.appendChild(cardBodyDiv);

    cardBodyDiv.appendChild(messageDiv);

    ulElement.appendChild(liElement);

    usernameP.textContent = `${userFullName}`;
    messageDiv.textContent = `${message}`;
});

connection.on("ShowHistory", function (messages, idFromUrl) {

    for (let i = 0; i <= messages.length; i++) {
        ShowChatHistory(messages[i], idFromUrl, i, messages.length - 1)
    }
});

connection.on("ShowRecentChats", function (chats, timePassedSinceLastMessage, userFullName, recentChatsSendersProfileImages) {

    var recentMessagesDiv = document.getElementById("recentMessages");
    recentMessagesDiv.innerHTML = '';

    let i = -1;

    chats.forEach((chat) => {
        i++;
        if (chat.users.length > 0) {
            ShowRecentChats(chat, timePassedSinceLastMessage[i], userFullName, recentChatsSendersProfileImages[i]);
        }
    });
    
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

    connection.invoke("RetrieveRecentChats").catch(function (err) {
        return console.error(err.toString());
    });

    connection.invoke("OnConnected").catch(function (err) {
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

    var recentMessagesDiv = document.getElementById("recentMessages");
    while (recentMessagesDiv.firstChild) {
        recentMessagesDiv.removeChild(recentMessagesDiv.lastChild);
    }

    connection.invoke("RetrieveRecentChats").catch(function (err) {
        return console.error(err.toString());
    });

    connection.invoke("RetrieveRecentChatsOfUser", idFromUrl).catch(function (err) {
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
    cardDiv.style.backgroundColor = "white";

    if (messagesCounter == messagesCount) {
        ulElement.scrollTo(0, ulElement.scrollHeight);
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

    var messageDiv = document.createElement("div");
    messageDiv.classList.add("mb-0");

    if (stringifiedMessage.userId != idFromUrl) {
        messageDiv.style.color = "#212529";
        messageDiv.style.backgroundColor = "#d6d6d6";
        messageDiv.style.padding = "1rem";
        messageDiv.style.borderRadius = "5px";
    }

    liElement.appendChild(cardDiv);
    if (stringifiedMessage.userId == idFromUrl) {
        cardDiv.appendChild(usernameDiv);
        usernameDiv.appendChild(usernameP);
        usernameDiv.appendChild(sentTimeAgoP);
    }

    liElement.appendChild(cardBodyDiv);

    cardBodyDiv.appendChild(messageDiv);

    ulElement.appendChild(liElement);

    usernameP.textContent = `${stringifiedMessage.user.fullName}`;
    messageDiv.textContent = `${stringifiedMessage.content}`;
}

function ShowRecentChats(chat, timePassedSinceLastMessage, userFullName, userProfileImageName) {

    var sender;
    var otherChatParticipantId;

    chat.users.forEach((user) => {
        if (user.fullName != userFullName) {
            sender = user.fullName;
            otherChatParticipantId = user.id;
        }
    })

    if (chat.messages.length > 0) {
        var lastMessage = chat.messages[chat.messages.length - 1].content;
        var lastMessageSender = chat.messages[chat.messages.length - 1].user.fullName;

        var recentMessagesDiv = document.getElementById("recentMessages");

        var cardDiv = document.createElement("div");
        cardDiv.classList.add("card");

        var cardBodyDiv = document.createElement("div");
        cardBodyDiv.classList.add("card-body");

        var ulElement = document.createElement("ul");
        ulElement.classList.add("list-unstyled");
        ulElement.classList.add("mb-0");

        var liElement = document.createElement("li");
        liElement.classList.add("p-2");
        liElement.classList.add("border-bottom");
        liElement.style.backgroundColor = "#eee";

        var anchorElement = document.createElement("a");
        anchorElement.classList.add("d-flex");
        anchorElement.classList.add("justify-content-between");

        var innerCard = document.createElement("div");
        innerCard.classList.add("d-flex");
        innerCard.classList.add("flex-row");

        var imgElement = document.createElement("img");
        imgElement.src = "/" + userProfileImageName;
        imgElement.alt = "avatar";
        imgElement.style.borderRadius = "25px";
        imgElement.style.maxHeight = "60px";
        imgElement.style.maxWidth = "60px";
        imgElement.style.objectFit = "cover";
        imgElement.style.objectPosition = "center";
        imgElement.classList.add("d-flex");
        imgElement.classList.add("align-self-center");
        imgElement.classList.add("me-3");
        imgElement.classList.add("shadow-1-strong");
        imgElement.width = 60;

        var userTitleAndMessageDiv = document.createElement("div");
        userTitleAndMessageDiv.classList.add("pt-1");

        var usernameP = document.createElement("p");
        usernameP.classList.add("fw-bold");
        usernameP.classList.add("mb-0");
        usernameP.textContent = sender;

        var messageOverviewP = document.createElement("p");
        messageOverviewP.classList.add("small");
        messageOverviewP.classList.add("text-muted");

        if (lastMessage != null) {
            if (lastMessage.length <= 50 && lastMessage.length > 0) {
                if (lastMessageSender == userFullName) {
                    messageOverviewP.textContent = 'You: ' + lastMessage;
                } else {
                    messageOverviewP.textContent = lastMessage;
                }
            } else {
                if (lastMessageSender == userFullName) {
                    messageOverviewP.textContent = 'You: ' + lastMessage.slice(0, 50) + "..";
                } else {
                    messageOverviewP.textContent = lastMessage.slice(0, 50) + "..";
                }
            }
        }

        var timeSentDiv = document.createElement("div");
        timeSentDiv.classList.add("small");
        timeSentDiv.classList.add("text-muted");
        timeSentDiv.classList.add("mb-1");
        timeSentDiv.textContent = timePassedSinceLastMessage;

        var mainAnchorElement = document.createElement("a");
        mainAnchorElement.href = otherChatParticipantId;

        mainAnchorElement.appendChild(cardDiv);

        cardDiv.appendChild(cardBodyDiv);

        cardBodyDiv.appendChild(ulElement);

        ulElement.appendChild(liElement);

        liElement.appendChild(anchorElement);

        anchorElement.appendChild(innerCard);

        innerCard.appendChild(imgElement);

        innerCard.appendChild(userTitleAndMessageDiv);

        userTitleAndMessageDiv.appendChild(usernameP);
        userTitleAndMessageDiv.appendChild(messageOverviewP);

        anchorElement.appendChild(timeSentDiv);

        recentMessagesDiv.appendChild(mainAnchorElement);
    } else {
        var recentMessagesDiv = document.getElementById("recentMessages");

        var cardDiv = document.createElement("div");
        cardDiv.classList.add("card");

        var cardBodyDiv = document.createElement("div");
        cardBodyDiv.classList.add("card-body");

        var ulElement = document.createElement("ul");
        ulElement.classList.add("list-unstyled");
        ulElement.classList.add("mb-0");

        var liElement = document.createElement("li");
        liElement.classList.add("p-2");
        liElement.classList.add("border-bottom");
        liElement.style.backgroundColor = "#eee";

        var anchorElement = document.createElement("a");
        anchorElement.classList.add("d-flex");
        anchorElement.classList.add("justify-content-between");

        var innerCard = document.createElement("div");
        innerCard.classList.add("d-flex");
        innerCard.classList.add("flex-row");

        var imgElement = document.createElement("img");
        imgElement.src = "/" + userProfileImageName;
        imgElement.alt = "avatar";
        imgElement.style.borderRadius = "25px";
        imgElement.style.maxHeight = "60px";
        imgElement.style.maxWidth = "60px";
        imgElement.style.objectFit = "cover";
        imgElement.style.objectPosition = "center";
        imgElement.classList.add("d-flex");
        imgElement.classList.add("align-self-center");
        imgElement.classList.add("me-3");
        imgElement.classList.add("shadow-1-strong");
        imgElement.width = 60;

        var userTitleAndMessageDiv = document.createElement("div");
        userTitleAndMessageDiv.classList.add("pt-1");

        var usernameP = document.createElement("p");
        usernameP.classList.add("fw-bold");
        usernameP.classList.add("mb-0");
        usernameP.textContent = sender;

        var messageOverviewP = document.createElement("p");
        messageOverviewP.classList.add("small");
        messageOverviewP.classList.add("text-muted");

        var mainAnchorElement = document.createElement("a");
        mainAnchorElement.href = otherChatParticipantId;

        mainAnchorElement.appendChild(cardDiv);

        cardDiv.appendChild(cardBodyDiv);

        cardBodyDiv.appendChild(ulElement);

        ulElement.appendChild(liElement);

        liElement.appendChild(anchorElement);

        anchorElement.appendChild(innerCard);

        innerCard.appendChild(imgElement);

        innerCard.appendChild(userTitleAndMessageDiv);

        userTitleAndMessageDiv.appendChild(usernameP);
        userTitleAndMessageDiv.appendChild(messageOverviewP);

        recentMessagesDiv.appendChild(mainAnchorElement);
    }
}