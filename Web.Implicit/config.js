var config = {
    authority: "##AUTHORITY_URL##",
    client_id: "##CLIENT_ID##",
    redirect_uri: "http://localhost:5004/callback.html",
    response_type: "id_token token",
    scope: "openid profile ##SCOPE##",
    post_logout_redirect_uri: "http://localhost:5004/index.html",
    acr_values: "tenant:##TENANT##"
};