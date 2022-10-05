/**
 * Represents a single Minecraft server instance
 */
class MinecraftServer{
    static getNonFunctionalServer(serverName){
        if(serverName == null)
            serverName = "No Server";

        let mcServer = new MinecraftServer(new MCNetworking(), serverName, ServerStatus.Offline, [], [], "- %", "- B", "- B", new Date(0));

        mcServer.renameServer = () => {};
        mcServer.deleteServer = () => {};
        mcServer.writeCommand = () => {};
        mcServer.toggle = () => {};

        return mcServer;
    }


    #mcNetworking;

    serverName; // string
    players = []; // MinecraftPlayer[]
    logs = []; // ServerLog[]
    cpuUsage; // string
    memoryUsage; // string
    storage; // string
    status; // string: ServerStatus
    onlineFrom; // Date | null object

    constructor(mcNetworking, serverName, status,
                players, logs, cpu, memory,
                storage, onlineFrom) {
        this.#mcNetworking = mcNetworking;
        this.serverName = serverName;
        this.players = [...players];
        this.logs = [...logs];
        this.cpuUsage = cpu;
        this.memoryUsage = memory;
        this.storage = storage;
        this.status = status;
        this.onlineFrom = onlineFrom;
    }

    /**
     * Gets if the server is running.
     * @returns {boolean}
     */
    isRunning(){
        return this.status !== ServerStatus.Offline;
    }

    /**
     * Sends a server rename request to the server.
     * @param newName the new name of the server.
     */
    renameServer(newName){
        this.#mcNetworking.sendHandler.sendRenameServer(this.serverName, newName);
    }

    /**
     * Sends a server delete request to the server.
     */
    deleteServer(){
        this.#mcNetworking.sendHandler.sendRemoveServer(this.serverName);
    }


    /**
     * Sends a toggle request to the server
     */
    toggle(){
        this.#mcNetworking.sendHandler.sendToggle(this.serverName);
    }

    /**
     * Sends a command to execute to the server.
     * @param command the command to send.
     */
    writeCommand(command){
        this.#mcNetworking.sendHandler.sendWriteCommand(this.serverName, command);
    }
}
