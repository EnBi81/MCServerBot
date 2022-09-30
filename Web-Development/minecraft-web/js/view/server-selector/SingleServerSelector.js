/**
 * Represents a single server selector.
 */
class SingleServerSelector{
    #serverPark;
    #serverInfoPage;

    #mcServer;

    #htmlElement;

    constructor(serverPark, serverInfoPage, mcServer) {
        this.#serverPark = serverPark;
        this.#serverInfoPage = serverInfoPage;
        this.#mcServer = mcServer;

        this.#setupListeners();
    }

    /**
     * Sets up the required listeners.
     */
    #setupListeners(){
        this.#serverPark.addListener('serverNameChange', ([o, n]) => this.#serverNameChangeListener(o, n));
        this.#serverPark.addListener('serverDeleted', ([name]) => this.#serverDeletedListener(name));
    }

    /**
     * Server name change event handler.
     * @param oldName old name of the server.
     * @param newName new name of the server.
     */
    #serverNameChangeListener(oldName, newName){
        if(this.getName() === newName){
            this.rename(newName);
        }
    }

    /**
     * Server deleted event handler.
     * @param name name of the server that got deleted.
     */
    #serverDeletedListener(name){
        if(this.getName() === name){
            this.delete();
        }
    }

    /**
     * This method has to be overridden. It is invoked when the current server is selected.
     * @param servername name of the current server.
     */
    userSelected(servername){

    }


    /**
     * Gets the name of the current server.
     * @returns {*}
     */
    getName(){
        return this.#mcServer.serverName;
    }

    /**
     * Selects the current server.
     */
    select(){
        serverInfoPage.loadMCServer(this.#mcServer.serverName);
        this.userSelected(this.#mcServer.serverName);
    }

    /**
     * Deletes the current server selector.
     */
    delete(){
        this.#htmlElement.remove();
    }

    /**
     * Renames the current server selector.
     * @param newName
     */
    rename(newName){
        this.#htmlElement.textContent = newName;
    }

    /**
     * Gets the server element.
     * @returns {*}
     */
    getServerElement(){
        if(this.#htmlElement != null)
            return this.#htmlElement;

        //<div class="minecraft-server">Server1</div>
        this.#htmlElement = document.createElement("div");
        this.#htmlElement.textContent = this.#mcServer.serverName;

        this.#htmlElement.addEventListener('click', () => {
            this.select();
        })

        return this.#htmlElement;
    }
}