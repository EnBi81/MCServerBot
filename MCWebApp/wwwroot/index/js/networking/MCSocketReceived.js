/**
 * These methods are called when data is received from the server. Override the methods to use them.
 */
class MCSocketReceived{
    errorReceived(errorMessage){
        this.#handleData("error", errorMessage);
    }

    logoutReceived(){
        this.#handleData("logout");
    }

    serverNameChangeReceived(oldName, newName){
        this.#handleData("name change", oldName, newName);
    }

    serverDeletedReceived(serverName){
        this.#handleData("server delete", serverName);
    }

    serverAddedReceived(serverName, storage){
        this.#handleData("server added", serverName, storage);
    }

    activeServerChangeReceived(serverName){
        this.#handleData("active server change", serverName);
    }

    logReceived(serverName, messages){
        this.#handleData("log received", serverName, messages);
    }

    playerJoinedReceived(serverName, username, onlineFrom, pastUptime){
        this.#handleData("player joined", serverName, username, onlineFrom, pastUptime);
    }

    playerLeftReceived(serverName, username){
        this.#handleData("player left", serverName, username);
    }

    pcUsageReceived(serverName, cpu, memory){
        this.#handleData("pc usage", serverName, cpu, memory);
    }

    statusChangeReceived(serverName, status, onlineFrom, storage){
        this.#handleData("status change", serverName, status, onlineFrom, storage);
    }

    #handleData(){
        const args = Array.from(arguments);
        console.log("---received: " + args.join("-"));
    }
}