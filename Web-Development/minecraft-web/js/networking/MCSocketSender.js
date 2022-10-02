/**
 * Handles sending data to the server.
 */
class MCSocketSender {
    #socket;
    #socketReceiver;

    BASE_API_URL = "/api/v1/"

    constructor(socket, socketReceiver) {
        this.#socket = socket;
        this.#socketReceiver = socketReceiver;
    }

    #sendRequest(requestUrl, methodName, body, callback){
        try{
            let xmlHttp = new XMLHttpRequest();
            if(callback != null){
                xmlHttp.onreadystatechange = () => callback(xmlHttp);
            }
            xmlHttp.open(methodName, this.BASE_API_URL + requestUrl, true); // true for asynchronous
            xmlHttp.send(body);
        }
        catch (e){
            this.#socketReceiver.errorReceived("No connection with the server!");
        }

    }
    #post(requestUrl, data){
        this.#sendRequest(requestUrl, "POST", data);
    }

    #delete(requestUrl){
        this.#sendRequest(requestUrl, "DELETE", null);
    }

    #put(requestUrl, data){
        this.#sendRequest(requestUrl, "PUT", data);
    }

    #get(requestUrl, callback){
        this.#sendRequest(requestUrl, "GET", null, callback);
    }


    /**
     * Requests the setup data.
     * @param callback this method will be called with the parsed json data in the parameter.
     */
    getSetupData(callback){
        let networkCallback = request => {
            if (request.readyState === 4 && request.status === 200)
                callback(JSON.parse(request.textContent));
        }

        this.#get("serverpark", networkCallback);
    }

    /**
     * Sends a toggle request to the server.
     * @param serverName the server to turn on/off.
     */
    sendToggle(serverName){
        this.#post(`minecraftserver/${serverName}/toggle`);
    }

    /**
     * Add a new server with the specified name.
     * @param serverName the new server's name.
     */
    sendAddServer(serverName){
        let data = { "new-name": serverName };
        this.#post(`serverpark`, data);
    }

    /**
     * Remove the server with the specified name.
     * @param serverName the name of the server to be removed.
     */
    sendRemoveServer(serverName){
        this.#delete(`minecraftserver/${serverName}`);
    }

    /**
     * Rename a server.
     * @param oldName server to rename.
     * @param newName new name of the server-
     */
    sendRenameServer(oldName, newName){
        let data = { "new-name": newName };
        this.#put(`/api/v1/minecraftserver/${oldName}`, data);
    }

    /**
     * Write a command to the active server.
     * @param serverName name of the server.
     * @param command the command to write.
     */
    sendWriteCommand(serverName, command){
        let data = { "command-data": command };
        this.#post(`/api/v1/minecraftserver/${serverName}/commands`, data);
    }
}