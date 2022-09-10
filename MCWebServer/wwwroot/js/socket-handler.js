let address = document.body.getAttribute("data-webaddress")
const mcSocket = new MCSocket(address);

/*mcSocket.logReceived = (message, type) => {
    let logType;
    switch (type){
        case 0:
            logType = SYSTEM_MESSAGE;
            break;
        case 1:
            logType = SYSTEM_ERROR_MESSAGE;
            break;
        default:
            logType = USER_MESSAGE;
            break;
    }

    addLog(message, logType);
}*/

/*
mcSocket.playerJoined = addPlayer;
mcSocket.playerLeft = removePlayer;
mcSocket.cpuMemoryDataReceived = setCpuAndMemory;
mcSocket.statusUpdated = setStatus;
mcSocket.logoutFunction = function () { logoutReceived() };*/