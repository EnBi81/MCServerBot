class ServerSelectors{
    #singleServerSelectors = {};

    #serverPark;
    #serverInfoPage;
    #selectedServer;

    #serverDropdownElement;
    #selectedServerNameElement;

    constructor(serverPark, serverInfoPage) {
        this.#serverPark = serverPark;
        this.#serverInfoPage = serverInfoPage;

        this.#serverDropdownElement = document.getElementById('servers');
        this.#selectedServerNameElement = document.querySelector('.selected-server-name');
        this.#setupListeners();
        this.#setupGui();

        this.selectServer(null);
    }

    #setupListeners(){
        this.#serverPark.addListener('serverAdded', ([server]) => this.#addServer(server));
        this.#serverPark.addListener('serverNameChange', ([o, n]) => this.#serverRename(o, n));
        this.#serverPark.addListener('serverDeleted', ([name]) => this.#serverDeleted(name));
    }

    #setupGui(){
        let allServers = this.#serverPark.getAllServers();
        for (const server of allServers) {
            this.#addServer(server);
        }
    }

    #addServer(server){
        let singleServer = new SingleServerSelector(this.#serverPark, this.#serverInfoPage, server);
        singleServer.userSelected = name => this.selectServer(name);

        let element = singleServer.getServerElement();
        this.#serverDropdownElement.appendChild(element);

        this.#singleServerSelectors[server.serverName] = singleServer;


        if(this.#selectedServerNameElement.getAttribute('data-default-server') === '1'){
            singleServer.select();
        }
    }

    #serverRename(oldName, newName){
        let server = this.#singleServerSelectors[oldName];
        delete this.#singleServerSelectors[oldName];

        this.#singleServerSelectors[newName] = server;
    }

    #serverDeleted(name){
        delete this.#singleServerSelectors[name];
    }


    selectServer(name){
        this.#selectedServer = this.#serverPark.getServer(name);

        if(this.#selectedServer == null){
            this.#selectedServer = MinecraftServer.getNonFunctionalServer()
            this.#selectedServerNameElement.setAttribute('data-default-server', '1');
        }
        else{
            this.#selectedServerNameElement.setAttribute('data-default-server', '0');
        }


        this.#selectedServerNameElement.textContent = this.#selectedServer.serverName;
    }
}