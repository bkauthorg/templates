var config = {
    authority: "http://localhost:5000/idsvr",
    //authority: "https://35.227.255.109/idsvr",
    tokenPart: "/connect/token",
    client_id: "apple.js.client",
    redirect_uri: "http://localhost:5013/callback.html",
    response_type: "id_token token",
    scope: "openid profile apple.scope.api1",
    post_logout_redirect_uri: "http://localhost:5013/index.html",
    acr_values: "tenant:applecorp"
};


//var config = {
//    tenant: "applecorp",
//    authority: "http://localhost:5000/idsvr",
//    tokenPart: "/connect/token",
//    client_id: "apple.js.client",
//    response_type: "id_token token",
//    scope: "openid profile apple.scope.api1",
//    acr_values: "tenant:applecorp"
//};