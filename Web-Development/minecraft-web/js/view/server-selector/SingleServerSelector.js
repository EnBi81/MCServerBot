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

    #setupListeners(){
        this.#serverPark.addListener('serverNameChange', ([o, n]) => this.#serverNameChangeListener(o, n));
        this.#serverPark.addListener('serverDeleted', ([name]) => this.#serverDeletedListener(name));
    }

    #serverNameChangeListener(oldName, newName){
        if(this.getName() === newName){
            this.rename(newName);
        }
    }

    #serverDeletedListener(name){
        if(this.getName() === name){
            this.delete();
        }
    }

    userSelected(servername){

    }


    getName(){
        return this.#mcServer.serverName;
    }

    select(){
        serverInfoPage.loadMCServer(this.#mcServer.serverName);
        this.userSelected(this.#mcServer.serverName);
    }

    delete(){
        this.#htmlElement.remove();
    }

    rename(newName){
        this.#htmlElement.textContent = newName;
    }

    getServerElement(){
        //<div class="minecraft-server">Server1</div>
        this.#htmlElement = document.createElement("div");
        this.#htmlElement.textContent = this.#mcServer.serverName;

        this.#htmlElement.addEventListener('click', () => {
            this.select();
        })

        return this.#htmlElement;
    }
}