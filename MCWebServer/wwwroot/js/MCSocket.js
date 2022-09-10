class MCSocket{
    #socket;

    /*logReceived(message, type){}
    playerJoined(username, onlineFrom, pastUptime){}
    playerLeft(username){}
    cpuMemoryDataReceived(cpu, memory){}
    //storageReceived(storage){}
    statusUpdated(status){}
    logoutFunction = () => { };*/

    constructor(address) {
        this.#socket = new WebSocket(address);
        this.#socket.addEventListener('message', this.#dataReceived);
        this.#socket.addEventListener('open', () => console.log("Websocket ready!"));
        this.#socket.addEventListener('close', e => addLog("Connection to Server could not be established :((", SYSTEM_ERROR_MESSAGE));
    }

    #dataReceived(event){
        let data = event.data;
        //console.log(data);
        let parsedData
        let dataType;

        try{
            parsedData = JSON.parse(data);
            dataType = parsedData["datatype"];
        }catch (e){
            console.log(e);
            return;
        }

        if(dataType == null){
            console.log("null data received");
            return;
        }

        if (dataType === "log") {
            let logs = parsedData["logs"];

            for (let log of logs) {
                let message = log["message"]; // string
                let type = log["type"]; // int

                addLog(message, type);
            }
        }

        else if(dataType === "playerJoin"){
            let username = parsedData["username"]; //string
            let onlineFrom = new Date(parsedData["onlineFrom"]); //string
            let pastUptime = parsedData["pastUptime"]; // {h:int, m: int, s:int}

            addPlayer(username, onlineFrom, pastUptime);
        }

        else if(dataType === "playerLeft"){
            let username = parsedData["username"]; //string

            removePlayer(username);
        }

        else if(dataType === "pcUsage"){
            let cpu = parsedData["cpu"]; //string
            let memory = parsedData["memory"]; //string

            setCpuAndMemory(cpu, memory);
        }

        /*else if(dataType === "storageReceived"){
            let storage = parsedData["storage"]; //string

            this.storageReceived(storage);
        }*/

        else if(dataType === "status"){
            let status = parsedData["status"]; //"online", "offline", "starting", "shutting-down"

            let onlineFrom = parsedData["onlineFrom"];
            if (onlineFrom != null) {
                serverOnline = new Date(onlineFrom);
            }

            let storageData = parsedData["storage"];
            setStorage(storageData);

            setStatus(status);
        }

        else if (dataType === "logout") {
            logoutReceived();
        }
    }

    toggleServer(){
        this.#sendToServer("toggle");
    }

    logout(){
        this.#sendToServer("logout");
    }

    sendCommand(command){
        this.#sendToServer("addCommand", command)
    }

    #sendToServer(requestName, data = null){
        if(this.#socket.readyState === 3){
            addLog("No connection with the server! Please refresh the page.", SYSTEM_ERROR_MESSAGE);
            return;
        }

        let request = {request: requestName, data: data};
        let json = JSON.stringify(request);
        this.#socket.send(json);
    }
}