/**
 * Container which holds the server instances.
 */
class ServerPark{
    #mcNetworking;
    #servers = {}; // {"name1": MinecraftServer}
    #errorMessages = [];

    #observers = {
        "serverNameChange": [], // ([oldName, newName]) => {},
        "serverAdded": [], // ([mcServer, isSetup]) => {},
        "serverDeleted": [], // ([name]) => {},
        "activeServerChange": [], // ([name]) => {},
        "statusChange": [], // ([serverName, status, onlineFrom, storage]) => {},
        "pcUsage": [], // ([serverName, cpu, memory]) => {},
        "playerLeft": [], // ([serverName, username]) => {},
        "playerJoined": [], // ([serverName, minecraftPlayer]) => {},
        "logReceived": [], // ([serverName, [messages]]) => {}
        "errorMessage": [], // ([message]) => {},
    }


    constructor(serverAddress) {
        this.#mcNetworking = new MCNetworking(serverAddress);
        this.#setUpListeners();
    }

    /**
     * Sets up the Minecraft servers from the data parameter.
     * @param data the setup data.
     */
    #loadSetupData(data){
        if(data == null)
            return;

        for (const server of data) {

            let name = server["name"];
            let status = server["serverStatus"];
            let players = [];
            let logs = [];
            let cpu = "0 %";
            let memory = "0 B";
            let storage = server["storageSpace"];
            let onlineFrom = server.onlineFrom == null ? null : new Date(server.onlineFrom); //from text;

            for (const player of server["onlinePlayers"]) {
                let username = player["username"];
                let dateOnlineFrom = new Date(player["onlineFrom"]);
                let pastUptime = SimpleTimeObject.fromText(player["pastOnline"]);

                let mcPlayer = new MinecraftPlayer(username, dateOnlineFrom, pastUptime);
                players.push(mcPlayer);
            }

            for (const log of server["logs"]) {
                let message = log["message"];
                let type = log["messageType"];

                let logMessage = new ServerLog(message, type);
                logs.push(logMessage);
            }



            this.#servers[name] = new MinecraftServer(this.#mcNetworking, name,
                status, players, logs, cpu, memory, storage, onlineFrom);

            this.#invokeObserver("serverAdded", this.#servers[name], true);
        }
    }

    /**
     * Gets and loads the setup data from the server.
     */
    setup(){
        this.#mcNetworking.sendHandler.getSetupData(data => this.#loadSetupData(data));
    }

    /**
     * Gets a server by name
     * @param name the name of the server
     * @returns {MinecraftServer}
     */
    getServer(name){
        return this.#servers[name];
    }

    /**
     * Gets all the servers.
     * @returns {MinecraftServer[]}
     */
    getAllServers(){
        return Object.values(this.#servers);
    }

    /**
     * Sends a AddServer request to the server.
     * @param name
     */
    addServer(name){
        this.#mcNetworking.sendHandler.sendAddServer(name);
    }

    /**
     * Gets all the error messages.
     * @returns {string[]}
     */
    getAllErrorMessages(){
        return [...this.#errorMessages];
    }

    /**
     * Refreshes the discord user's data with the latest info.
     * @param discordId discord id of the user
     * @param callback callback which will be called when the data is ready. It should take a DiscordUser as param.
     */
    getRefreshedProfile(discordId, callback){
        let convertingCallback = obj => {
            let discordUser = new DiscordUser(
                obj["profPic"],
                obj["id"],
                obj["username"],
            );

            callback(discordUser);
        };

        this.#mcNetworking.sendHandler.getRefreshedProfPic(discordId, convertingCallback);
    }

    /**
     * Adds a listener to the ServerPark.
     * @param event event to subscribe to.
     * @param listener the listener which will be invoked.
     */
    addListener(event, listener){
        if(Object.keys(this.#observers).includes(event))
            this.#observers[event].push(listener);
    }

    /**
     * Invoke a list of observers with the parameters
     * @param eventName name of the event
     * @param args arguments
     */
    #invokeObserver(eventName, ...args){
        if(!Object.keys(this.#observers).includes(eventName))
            return;

        let observers = this.#observers[eventName];

        for (const observer of observers) {
            observer(args);
        }
    }

    /**
     * Sets up the received listeners for the sockets.
     */
    #setUpListeners(){
        this.#mcNetworking.receiveHandler.serverAddedReceived = (name, storage) => this.#serverAdded(name, storage);
        this.#mcNetworking.receiveHandler.serverDeletedReceived = name => this.#serverDeleted(name);
        this.#mcNetworking.receiveHandler.serverNameChangeReceived = (oldName, newName) => this.#serverRenamed(oldName, newName);
        this.#mcNetworking.receiveHandler.activeServerChangeReceived = serverName => this.#activeServerChange(serverName);

        this.#mcNetworking.receiveHandler.statusChangeReceived = (name, stat, onlineFrom, storage) => this.#statusChangeReceived(name, stat, onlineFrom, storage);
        this.#mcNetworking.receiveHandler.pcUsageReceived = (serverName, cpu, memory) => this.#pcUsage(serverName, cpu, memory);
        this.#mcNetworking.receiveHandler.playerLeftReceived = (serverName, username) => this.#playerLeft(serverName, username);
        this.#mcNetworking.receiveHandler.playerJoinedReceived = (serverName, username, onlineFrom, pastUptime) => this.#playerJoined(serverName, username, onlineFrom, pastUptime);
        this.#mcNetworking.receiveHandler.logReceived = (serverName, messages) => this.#logReceived(serverName, messages);

        this.#mcNetworking.receiveHandler.logoutReceived = () => Logout.logout();
        this.#mcNetworking.receiveHandler.errorReceived = (message) => this.#errorMessage(message);
    }

    /**
     * Server Added event handler.
     * @param name name of the server.
     * @param storage storage of the server.
     */
    #serverAdded(name, storage){
        this.#servers[name] = new MinecraftServer(this.#mcNetworking, name, ServerStatus.Offline, [],
            [], "0%", "0 B", storage, new Date());

        this.#invokeObserver("serverAdded", this.#servers[name], false);
    }

    /**
     * Server Deleted event handler.
     * @param name name of the server.
     */
    #serverDeleted(name){
        delete this.#servers[name];
        this.#invokeObserver("serverDeleted", name);
    }

    /**
     * Server Renamed event handler.
     * @param oldName the old name of the server.
     * @param newName the new name of the server.
     */
    #serverRenamed(oldName, newName){
        let temp = this.#servers[oldName];
        delete this.#servers[oldName];
        temp.serverName = newName;

        this.#servers[newName] = temp;


        this.#invokeObserver("serverNameChange", oldName, newName);
    }

    /**
     * ActiveServerChanged ??? might get removed, wtf is this event.
     * @param name
     */
    #activeServerChange(name){
        this.#invokeObserver("activeServerChange", name);
    }

    /**
     * Status change of the server.
     * @param serverName the name of the server that changed status.
     * @param status new status.
     * @param onlineFrom online from (might be null).
     * @param storage storage space for the server.
     */
    #statusChangeReceived(serverName, status, onlineFrom, storage) {
        let server = this.getServer(serverName);
        server.status = status;
        server.storage = storage;
        
        if (onlineFrom != null) {
            onlineFrom = onlineFrom.replace("T", " ");
            server.onlineFrom = new Date(onlineFrom);
        }
        this.#invokeObserver("statusChange", serverName, status, server.onlineFrom, storage);
    }

    /**
     * PC usage information received.
     * @param serverName name of the server which sent the data.
     * @param cpu cpu usage
     * @param memory memory usage
     */
    #pcUsage(serverName, cpu, memory) {
        let server = this.getServer(serverName);
        server.cpuUsage = cpu;
        server.memoryUsage = memory;

        this.#invokeObserver("pcUsage", serverName, cpu, memory);
    }

    /**
     * Event when a player leaves the server.
     * @param serverName name of the server the player left.
     * @param username username of the player.
     */
    #playerLeft(serverName, username){
        let server = this.getServer(serverName);
        delete server.players[username];

        this.#invokeObserver("playerLeft", serverName, username);
    }

    /**
     * Event when a player joins the server.
     * @param serverName name of the server the player joined.
     * @param username username of the player.
     * @param onlineFrom date when the player joined the server.
     * @param pastUptime past uptime timespan.
     */
    #playerJoined(serverName, username, onlineFrom, pastUptime){
        let date = new Date(onlineFrom.replace("T", " "));
        let simpleTime = new SimpleTimeObject(pastUptime.h, pastUptime.m, pastUptime.s);

        let player = new MinecraftPlayer(username, date, simpleTime);

        let server = this.getServer(serverName);
        server.players.push(player);

        this.#invokeObserver("playerJoined", serverName, player);
    }

    /**
     * Log received from the server
     * @param serverName name of the server that sent the log message.
     * @param logs log messages that are received.
     */
    #logReceived(serverName, logs){
        let server = this.#servers[serverName];
        let messages = []

        for (const log of logs) {
            let message = new ServerLog(log["message"], log["type"]);
            server.logs.push(message);
            messages.push(message);
        }

        this.#invokeObserver("logReceived", serverName, messages);
    }

    /**
     * Error message received from the server.
     * @param message error message.
     */
    #errorMessage(message){
        this.#errorMessages.push(message);

        this.#invokeObserver("errorMessage", message);
    }
}