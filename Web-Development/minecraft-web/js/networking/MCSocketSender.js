/**
 * Handles sending data to the server.
 */
class MCSocketSender {
    #socket;
    #socketReceiver;

    constructor(socket, socketReceiver) {
        this.#socket = socket;
        this.#socketReceiver = socketReceiver;
    }

    #sendToServer(requestName, data = null){
        if(this.#socket.readyState === 3){
            this.#socketReceiver.errorReceived("No connection with the server! Please refresh the page.");
            return;
        }

        let request = {request: requestName, data: data};
        let json = JSON.stringify(request);

        try{
            this.#socket.send(json);
        } catch (e){
            this.#socketReceiver.errorReceived("No connection with the server! Please refresh the page.");
        }

    }

    /**
     * Sends a toggle request to the server.
     * @param serverName the server to turn on/off.
     */
    sendToggle(serverName){
        let data = { "server-name": serverName };
        this.#sendToServer('toggle', data);
    }

    /**
     * Add a new server with the specified name.
     * @param serverName the new server's name.
     */
    sendAddServer(serverName){
        let data = { "server-name": serverName };
        this.#sendToServer('add-server', data);
    }

    /**
     * Remove the server with the specified name.
     * @param serverName the name of the server to be removed.
     */
    sendRemoveServer(serverName){
        let data = { "server-name": serverName };
        this.#sendToServer('remove-server', data);
    }

    /**
     * Rename a server.
     * @param oldName server to rename.
     * @param newName new name of the server-
     */
    sendRenameServer(oldName, newName){
        let data = { "old-name": oldName, "new-name": newName };
        this.#sendToServer('rename-server', data);
    }

    /**
     * Write a command to the active server.
     * @param serverName name of the server.
     * @param command the command to write.
     */
    sendWriteCommand(serverName, command){
        let data = { "server-name": serverName, "command": command };
        this.#sendToServer('write-command', data);
    }

    /**
     * Logs out from the page.
     */
    sendLogout(){
        this.#sendToServer('logout', null);
    }
}