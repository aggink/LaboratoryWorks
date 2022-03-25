const url = "https://localhost:5001/";
let _connection;

$('button#Connect').on('click', connect);
$('button#Disconnect').on('click', disconnect);
$('button#Send').on('click', send);

let _users = [];

function buildConnection() {

    $('button#Disconnect').slideToggle();
    $('button#Send').slideToggle();

    _connection = new signalR.HubConnectionBuilder()
        .withUrl("/message")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    _connection.onclose(() => {

        $('.chat-message-right').remove();
        $('.chat-message-left').remove();
        $('.list-group-item').remove();
    });

    _connection.on("SendMessageAsync", (user, message) => {
        AddMessage(user, message);
    });

    _connection.on("UpdateUsersAsync", (users) => {
        _users = users;
        UpdateUser();
    });

    _connection.on("SendUrlAsync", (user, url, fileName) => {
        AddMessageUrl(user, url, fileName);
    });

    _connection.on("SendImageAsync", (user, imageUrl) => {
        AddMessageImg(user, imageUrl);
    });

    _connection.on("SendErrorAsync", (message) => {
        AddMessageError(message);
    });
}

function UpdateUser() {
    $('.list-group-item').remove();

    for (var i = 0; i < _users.length; i++) {
        $('.ListUsers').before(
            '<a href=\"#\" class=\"list-group-item list-group-item-action border-0\">' +
                '<div class=\"d-flex align-items-start\">' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"'+ _users[i] +'\" width=\"40\" height=\"40\">' +
                    '<div class=\"flex-grow-1 ml-3\">' +
                        _users[i] +
                        '<div class=\"small\"><span class=\"fas fa-circle chat-online\"></span> Online</div>' +
                    '</div>' +
				'</div>' +
			'</a>'
        );   
    }
}

function AddMessage(user, message) {

    var name = user;
    if(user == $('strong#userName').text()){
        $('.chat-messages').append(
            '<div class=\"chat-message-right pb-4\">' +
                '<div>' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"' + name + '\" width=\"40\" height=\"40\">' +
                    '<div class=\"text-muted small text-nowrap mt-2\"></div>' +
		    	'</div>' +
                '<div class=\"flex-shrink-1 bg-light rounded py-2 px-3 mr-3\">' +
                    message +
                '</div>' +
             '</div>'
        );
    }
    else{
        name = user;
        $('.chat-messages').append(
            '<div class=\"chat-message-left pb-4\">' +
                '<div>' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"' + name + '\" width=\"40\" height=\"40\">' +
                    '<div class=\"text-muted small text-nowrap mt-2\"></div>' +
				'</div>' +
                '<div class=\"flex-shrink-1 bg-light rounded py-2 px-3 ml-3\">' +
                    '<div class=\"font-weight-bold mb-1\">' + name + '</div>' +
                    message +
                '</div>' +
            '</div>'
        );
    }

    var scroll = $('.chat-messages');
    scroll.scrollTop(scroll.prop('scrollHeight'));
}

function AddMessageImg(user, url) {
    var name = user;
    if (user == $('strong#userName').text()) {
        $('.chat-messages').append(
            '<div class=\"chat-message-right pb-4\">' +
                '<div>' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"' + name + '\" width=\"40\" height=\"40\">' +
                    '<div class=\"text-muted small text-nowrap mt-2\"></div>' +
                '</div>' +
                '<div class=\"flex-shrink-1 bg-light rounded py-2 px-3 mr-3\">' +
                    '<img src=\"' + url + '\" class=\"rounded float-right\" style=\"width:50%\" >' +
                '</div>' +
            '</div>'
        );
    }
    else {
        $('.chat-messages').append(
            '<div class=\"chat-message-left pb-4\">' +
                '<div>' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"' + name + '\" width=\"40\" height=\"40\">' +
                    '<div class=\"text-muted small text-nowrap mt-2\"></div>' +
                '</div>' +
                '<div class=\"flex-shrink-1 bg-light rounded py-2 px-3 ml-3\">' +
                    '<div class=\"font-weight-bold mb-1\">' + name + '</div>' +
                    '<img src=\"' + url + '\" class=\"rounded float-left\" style=\"width:50%\">' +
                '</div>' +
            '</div>'
        );
    }

    var scroll = $('.chat-messages');
    scroll.scrollTop(scroll.prop('scrollHeight'));
}

function AddMessageUrl(user, url, fileName) {
    var name = user;
    if (user == $('strong#userName').text()) {
        $('.chat-messages').append(
            '<div class=\"chat-message-right pb-4\">' +
                '<div>' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"' + name + '\" width=\"40\" height=\"40\">' +
                    '<div class=\"text-muted small text-nowrap mt-2\"></div>' +
                '</div>' +
                '<div class=\"flex-shrink-1 bg-light rounded py-2 px-3 mr-3\">' +
                    '<a href=\"' + url + '\" >' + fileName + '</a>' +
                '</div>' +
            '</div>'
        );
    }
    else {
        $('.chat-messages').append(
            '<div class=\"chat-message-left pb-4\">' +
                '<div>' +
                    '<img src=\"/images/avatar.jpg\" class=\"rounded-circle mr-1\" alt=\"' + name + '\" width=\"40\" height=\"40\">' +
                    '<div class=\"text-muted small text-nowrap mt-2\"></div>' +
                '</div>' +
                '<div class=\"flex-shrink-1 bg-light rounded py-2 px-3 ml-3\">' +
                    '<div class=\"font-weight-bold mb-1\">' + name + '</div>' +
                        '<a href=\"' + url + '\" >' + fileName + '</a>' +
                '</div>' +
            '</div>'
        );
    }

    var scroll = $('.chat-messages');
    scroll.scrollTop(scroll.prop('scrollHeight'));
}

function AddMessageError(message) {
    $('.chat-messages').append(
        '<div class=\"chat-message-right pb-4\">' +
            '<div class=\"flex-shrink-1 bg-danger rounded py-2 px-3 mr-3\">' +
                message +  
            '</div>' +
        '</div>'
    );

    var scroll = $('.chat-messages');
    scroll.scrollTop(scroll.prop('scrollHeight'));
}

async function start() {
    try {
        await _connection.start();
        console.log(`SignalR Connected...`);
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

async function send() {
    var $file = $('input#messageFile');
    var $message = $('input#messageText').val();

    if (!$message && $file[0].files.length <= 0) {
        AddMessageError("Сообщение или файл не заданы");
        return;
    }

    try {
        if ($message) {
            await _connection.invoke("SendMessageAsync", $message);
            $('input#messageText').val("");
        }

        if ($file[0].files.length > 0) {

            $('input#connectionId').val(_connection.connectionId);
            var form = document.getElementById('formFile');

            if (window.FormData !== undefined) {
                var formData = new FormData(form);
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData
                });

                if (response.ok) {
                    var result = response.json();
                    console.log(result);
                }
                else {
                    var result = response.json();
                    console.log(result);
                }
            }
            else {
                AddMessageError("Браузер не поддерживает FormData");
            }

            $file.val("");
        }
    }
    catch (err) {
        console.log(err);
    }
}

function connect() {

    if (_connection) {

        start();

        $('button#Connect').slideToggle(200);
        $('button#Disconnect').slideToggle(200);
        $('button#Send').slideToggle(200);
    }
    return;
}

function disconnect() {

    if (_connection && _connection.state === 'Connected') {

        $('button#Disconnect').slideToggle(200);
        $('button#Connect').slideToggle(200);
        $('button#Send').slideToggle(200);

        console.log("Disconnecting...");
        _connection.stop();

        $('.list-group-item').remove();
    }

    return;
}

// build connection
buildConnection();

$('a#DeleteMessage').on('click', async (event) => {

    event.preventDefault();

    var a = document.getElementById('DeleteMessage');
    const response = await fetch(a.href, {
        method: 'GET'
    });

    if (response.ok) {
        var result = response.json();
        alert("Данные чата успешно удалены");
        console.log(result);
    }
    else {
        var result = response.json();
        console.log(result);
    }
});