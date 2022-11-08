// copy server address to clipboard
(function(){
    const serverAddress = document.getElementById("server-address");
    serverAddress.addEventListener("click", ()=>{
        copy()
    })
    serverAddress.addEventListener("keypress", ev => {
        if(ev.key !== "Enter")
            return;

        copy();
    })

    function copy(){
        serverAddress.classList.remove("copied")
        serverAddress.offsetHeight; //trigger reflow xdddd


        navigator.permissions.query({ name: "clipboard-read" }).then((result) => {
            if (result.state == "granted" || result.state == "prompt") {
                navigator.clipboard.writeText(serverAddress.textContent)
                serverAddress.classList.add("copied");
            }
            else {
                addLog('Cannot copy message to clipboard. Please use the https website for that!', SYSTEM_ERROR_MESSAGE);
            }
        });
    }
})();


// command
(function () {
    const commandInput = document.getElementById("command-input");
    commandInput.addEventListener("keypress", e => {
        if(e.key !== "Enter")
            return;

        let command = commandInput.value.trim();
        commandInput.value = "";

        if(command.length === 0)
            return;

        sendCommand(command);
    })

    function sendCommand(command){
        mcSocket.sendCommand(command);
    }
})();


// logout
(function(){
    const logoutButton = document.getElementById("logout");
    logoutButton.addEventListener("click", () => sendLogout());
    logoutButton.addEventListener("keypress", e =>{
        if(e.key !== "Enter")
            return;

        sendLogout();
    })

    function sendLogout() {
        logoutReceived();
        mcSocket.logout();
    }
})();


// status change send
(function(){
    const statusChangeButton = document.getElementById("start-button");
    statusChangeButton.addEventListener("click", sendStatusChange);
    statusChangeButton.addEventListener("keypress", e =>{
        if(e.key !== "Enter")
            return;

        sendStatusChange();
    })

    function sendStatusChange(){
        mcSocket.toggleServer();
    }
})();