/**
 * Handles the Server Info part of the page. Listens to the server-park, and also listens for the user inputs.
 */
class ServerInfoPage{
    #serverPark;

    #selectedServer; // selected server which was loaded into the gui

    #serverInfoElement;
    #serverNameElement;
    #serverUptimeElement;
    #cpuElement;
    #memoryElement;
    #storageElement;
    #toggleButtonElement;

    #serverAddressView;
    #logView;
    #playerView;
    #updateTimers;


    constructor(serverPark, serverInfoId, serverNameId, serverAddressId, serverUptimeId, playersBoxId, cpuId, memoryId, storageId, toggleButtonId, commandBoxId, writeCommandId) {
        this.#serverPark = serverPark;

        this.#serverInfoElement = document.getElementById(serverInfoId);
        this.#serverNameElement = document.getElementById(serverNameId);
        this.#serverUptimeElement = document.getElementById(serverUptimeId)
        this.#cpuElement = document.getElementById(cpuId);
        this.#memoryElement = document.getElementById(memoryId);
        this.#storageElement = document.getElementById(storageId);
        this.#toggleButtonElement = document.getElementById(toggleButtonId);

        this.#serverAddressView = new ServerAddressCopy(serverAddressId);
        this.#logView = new LogView(commandBoxId, writeCommandId);
        this.#playerView = new PlayerView(playersBoxId);
        this.#updateTimers = new UpdateTimers(this.#serverUptimeElement, this.#playerView.playersBoxElement);

        this.#setupListeners();

        this.loadMCServer(null);
    }

    /**
     * Sets up the listeners on the log view and the server park and the toggle button.
     */
    #setupListeners(){
        this.#serverPark.addListener("statusChange", ([serverName, status, onlineFrom, storage]) => this.#statusChange(serverName, status, onlineFrom, storage));
        this.#serverPark.addListener("pcUsage", ([serverName, cpu, memory]) => this.#pcUsageChange(serverName, cpu, memory));
        this.#serverPark.addListener("logReceived", ([serverName, messages]) => this.#logReceived(serverName, messages));
        this.#serverPark.addListener("playerLeft", ([serverName, username]) => this.#playerLeft(serverName, username));
        this.#serverPark.addListener("playerJoined", ([serverName, mcPlayer]) => this.#playerJoined(serverName, mcPlayer));
        this.#serverPark.addListener("serverNameChange", ([oldName, newName]) => this.#serverNameChange(oldName, newName));

        this.#logView.userCommandEntered = command => this.#selectedServer.writeCommand(command);
        this.#toggleButtonElement.addEventListener("click", () => this.#selectedServer.toggle());
    }


    // public events

    /**
     * Loads the information of a minecraft server to the GUI. If the name is null or the server cannot be found, a default server will be loaded.
     * @param serverName the name of the server to be loaded.
     */
    loadMCServer(serverName){

        let server = this.#serverPark.getServer(serverName);
        if(server == null)
            server = MinecraftServer.getNonFunctionalServer("No Server Selected");

        this.#selectedServer = server;

        this.setServerName(server.serverName);
        this.setStatus(server.status);
        this.setCpu(server.cpuUsage);
        this.setMemory(server.memoryUsage);
        this.setStorage(server.storage);
        this.setServerOnlineFrom(server.onlineFrom);

        this.#logView.clearLogs();
        this.#logView.loadLogsToView(server.logs);

        this.#playerView.clearAllPlayers();
        this.#playerView.addPlayersToView(server.players);

        this.#updateTimers.updateAllTime();
    }



    // ServerPark Listeners

    /**
     * Server name change listener.
     * @param oldName old name of the server.
     * @param newName new name of the server.
     */
    #serverNameChange(oldName, newName){
        if(this.#selectedServer.serverName !== oldName)
            return;

        this.setServerName(newName);
    }

    /**
     * Status change event listener.
     * @param serverName name of the server which invoked this event.
     * @param status new status of the server.
     * @param onlineFrom date object when the server went online (might be null).
     * @param storage the storage space the server takes on the disk.
     */
    #statusChange(serverName, status, onlineFrom, storage){
        if(this.#selectedServer.serverName === serverName)
            return;

        this.setStatus(status);
        this.setStorage(storage);
        this.setServerOnlineFrom(onlineFrom);
    }

    /**
     * PC usage event listener.
     * @param serverName name of the server which invoked this event.
     * @param cpu cpu usage
     * @param memory memory usage
     */
    #pcUsageChange(serverName, cpu, memory){
        if(this.#selectedServer.serverName === serverName)
            return;

        this.setCpu(cpu);
        this.setMemory(memory);
    }

    /**
     * Log received event listener.
     * @param serverName name of the server which invoked this event.
     * @param messages log messages.
     */
    #logReceived(serverName, messages){
        if(this.#selectedServer.serverName === serverName)
            return;

        this.#logView.loadLogsToView(messages);
    }

    /**
     * Player joined event listener
     * @param serverName name of the server which invoked this event.
     * @param mcPlayer player who joined the server.
     */
    #playerJoined(serverName, mcPlayer){
        if(this.#selectedServer.serverName === serverName)
            return;

        this.#playerView.addPlayerToView(mcPlayer);
    }

    /**
     * Player left event listener.
     * @param serverName name of the server which invoked this event.
     * @param username player who left the server.
     */
    #playerLeft(serverName, username){
        if(this.#selectedServer.serverName === serverName)
            return;

        this.#playerView.removePlayerFromView(username);
    }

    //GUI Handlers

    /**
     * Sets the server name html element to the value.
     * @param value the value.
     */
    setServerName(value){
        this.#setTextContent(this.#serverNameElement, value);
    }

    /**
     * Sets the cpu usage html element to the value.
     * @param value the value.
     */
    setCpu(value){
        this.#setTextContent(this.#cpuElement, value);
    }

    /**
     * Sets the memory usage html element to the value.
     * @param value the value.
     */
    setMemory(value){
        this.#setTextContent(this.#memoryElement, value);
    }

    /**
     * Sets the storage usage html element to the value.
     * @param value the value.
     */
    setStorage(value){
        this.#setTextContent(this.#storageElement, value);
    }

    /**
     * Sets the status classes for the Server-Info page.
     * @param value
     */
    setStatus(value){
        let cl = this.#serverInfoElement.classList;

        if(value === ServerStatus.Starting || value === ServerStatus.ShuttingDown){
            cl.add("loading-colors");
            cl.remove("online-colors");
        }
        else{
            cl.remove("loading-colors");

            if(value === ServerStatus.Online)
                cl.add("online-colors");
            else cl.remove("online-colors")
        }
    }

    /**
     * Sets the server online from value in the GUI
     * @param value the value to be set
     */
    setServerOnlineFrom(value){
        if(value == null)
            return;

        let milliseconds = value.getTime();

        this.#serverUptimeElement.setAttribute("data-online-from", milliseconds);
    }

    /**
     * Sets the text content of a html element
     * @param element the html element to set the text
     * @param text the text content to set-
     */
    #setTextContent(element, text){
        element.textContent = text;
    }

}