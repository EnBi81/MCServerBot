/**
 * Creates and listens to the socket. Handles received data, and can send data to the server.
 */
class MCNetworking {
    #socket;

    receiveHandler;
    sendHandler;


    constructor(address) {
        this.receiveHandler = new MCSocketReceived();

        try{
            this.#socket = new WebSocket(address);
            this.#socket.addEventListener('message', e => this.#dataReceived(e));
            this.#socket.addEventListener('open', () => console.log("Websocket ready!"));
            this.#socket.addEventListener('close', () => this.receiveHandler.errorReceived("Connection to Server could not be established :(("));

            this.sendHandler = new MCSender(this.receiveHandler);
        } catch (e){
        }
    }

    /**
     * Input handlers.
     * @type {{serverNameChange: function(*): *, playerLeft: function(*): *, logout: function(): *, pcUsage: function(*): *, log: function(*): *, serverAdded: function(*): *, serverDeleted: function(*): *, error: function(*): *, playerJoin: function(*): *, activeServerChange: function(*): *, status: function(*): *}}
     */
    #receiveHandlers = {
        "error": e => this.receiveHandler.errorReceived((e["errorMessage"])),
        "logout": () => this.receiveHandler.logoutReceived(),
        "serverNameChange": e => this.receiveHandler.serverNameChangeReceived(e["oldName"], e["newName"]),
        "serverDeleted": e => this.receiveHandler.serverDeletedReceived(e["name"]),
        "serverAdded": e => this.receiveHandler.serverAddedReceived(e["name"], e["storage"]),
        "activeServerChange": e => this.receiveHandler.activeServerChangeReceived(e["newActive"]),
        "log": e => this.receiveHandler.logReceived(e["server"], e["logs"]),
        "playerJoin": e => this.receiveHandler.playerJoinedReceived(e["server"], e["username"], e["onlineFrom"], e["pastUptime"]),
        "playerLeft": e => this.receiveHandler.playerLeftReceived(e["server"], e["username"]),
        "pcUsage": e => this.receiveHandler.pcUsageReceived(e["server"], e["cpu"], e["memory"]),
        "status": e => this.receiveHandler.statusChangeReceived(e["server"], e["status"], e["onlineFrom"], e["storage"])
    }

    /**
     * Handles the received data by forwarding to the appropriate handler method.
     * @param event received data.
     */
    #dataReceived(event){
        let data = event.data;

        let parsedData; // all the received data
        let dataType; // the name of the received data/event

        try{
            parsedData = JSON.parse(data);
            dataType = parsedData["datatype"];
        }catch (e){
            console.log("Error: " + e);
            return;
        }

        // check if datatype is null, or if an event handler does not exist.
        if(dataType == null || !(dataType in this.#receiveHandlers)){
            console.log("Null datatype received: " + dataType);
            return;
        }

        // get and execute the handler.
        this.#receiveHandlers[dataType](parsedData);
    }
}
