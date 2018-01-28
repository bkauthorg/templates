/// <reference path="oidc-client.js" />

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("loginWithGoogle").addEventListener("click", loginWithGoogle, false);
document.getElementById("loginRO").addEventListener("click", loginRO, false);

var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user);
        console.log(JSON.stringify(user));
    }
    else {
        log("User not logged in");
    }
});

function loginWithGoogle() {
    config.acr_values = "idp:Google tenant:applecorp";
    mgr = new Oidc.UserManager(config);
    mgr.signinRedirect();
}

function loginRO() {
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": config.authority+config.tokenPart,
        "method": "POST",
        "headers": {
            "Content-Type": "application/x-www-form-urlencoded",
            "Cache-Control": "no-cache"
        },
        "data": {
            "client_id": config.client_id,
            "grant_type": "password",
            "username": document.getElementById("username_txt").value,
            "password": document.getElementById("password_txt").value,
            "acr_values": config.acr_values,
            "scope": config.scope
        }
    }

    $.ajax(settings).done(function (response) {
        console.log(response);
        log(JSON.stringify(response));
    });
}