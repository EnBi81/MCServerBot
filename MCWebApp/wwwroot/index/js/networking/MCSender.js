/**
 * Handles sending data to the server.
 */
class MCSender {
    #socketReceiver;

    BASE_API_URL = "/api/v1/"

    constructor(socketReceiver) {
        this.#socketReceiver = socketReceiver;
    }

    #sendRequest(requestUrl, methodName, data, callback) {

        let body = data !== null ? JSON.stringify(data) : null;


        try{
            let xmlHttp = new XMLHttpRequest();
            if (callback != null) {
                xmlHttp.onload = () => callback(xmlHttp);
            }
            
            xmlHttp.open(methodName, requestUrl, true); // true for asynchronous
            xmlHttp.setRequestHeader("Content-Type", "application/json");
            xmlHttp.send(body);

        }
        catch (e){
            this.#socketReceiver.errorReceived("No connection with the server!" + e);
        }

    }
    #post(requestUrl, data){
        this.#sendRequest(requestUrl, "POST", data, this.#createNetworkcallback());
    }

    #delete(requestUrl){
        this.#sendRequest(requestUrl, "DELETE", null, this.#createNetworkcallback());
    }

    #put(requestUrl, data) {
        this.#sendRequest(requestUrl, "PUT", data, this.#createNetworkcallback());
    }

    #get(requestUrl, callback){
        this.#sendRequest(requestUrl, "GET", null, callback);
    }

    #createNetworkcallback(callbackFunction) {
        return request => {
            if (request.status <= 300 && callbackFunction != null)
                callbackFunction(JSON.parse(request.responseText));

            else if (request.status >= 400 && request.status < 500) {
                try {
                    let json = JSON.parse(request.responseText);
                    this.#socketReceiver.errorReceived(json['errorMessage']);
                }
                catch (e) {
                    console.error(e);
                }
                
            }
        }
    }


    /**
     * Requests the setup data.
     * @param callback this method will be called with the parsed json data in the parameter.
     */
    getSetupData(callback) {
        let networkCallback = this.#createNetworkcallback(callback);

        this.#get(this.BASE_API_URL + "serverpark", networkCallback);
    }

    /**
     * Sends a toggle request to the server.
     * @param serverName the server to turn on/off.
     */
    sendToggle(serverName){
        this.#post(this.BASE_API_URL + `minecraftserver/${serverName}/toggle`);
    }

    /**
     * Add a new server with the specified name.
     * @param serverName the new server's name.
     */
    sendAddServer(serverName){
        let data = { "new-name": serverName };
        this.#post(this.BASE_API_URL + `serverpark`, data);
    }

    /**
     * Remove the server with the specified name.
     * @param serverName the name of the server to be removed.
     */
    sendRemoveServer(serverName){
        this.#delete(this.BASE_API_URL + `minecraftserver/${serverName}`);
    }

    /**
     * Rename a server.
     * @param oldName server to rename.
     * @param newName new name of the server-
     */
    sendRenameServer(oldName, newName){
        let data = { "new-name": newName };
        this.#put(this.BASE_API_URL + `minecraftserver/${oldName}`, data);
    }

    /**
     * Write a command to the active server.
     * @param serverName name of the server.
     * @param command the command to write.
     */
    sendWriteCommand(serverName, command){
        let data = { "command-data": command };
        this.#post(this.BASE_API_URL + `minecraftserver/${serverName}/commands`, data);
    }
}