function login() {
    var settings = {
        "async": true,
        "crossDomain": true,
        "url": config.authority,
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
        document.getElementById("response_lbl").value = JSON.stringify(response);
    });
}