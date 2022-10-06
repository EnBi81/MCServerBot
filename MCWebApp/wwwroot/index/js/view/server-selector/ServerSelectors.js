/**
 * Handles the events and functionalities of the server selectors.
 */
class ServerSelectors{
    #singleServerSelectors = {};

    #serverPark;
    #serverInfoPage;
    #selectedServer;

    #serverDropdownElement;
    #serverDropdownBgElement;
    #selectedServerNameElement;

    #serverAddWrapper;
    #serverAddTextbox;
    #serverAddButton;

    constructor(serverPark, serverInfoPage) {
        this.#serverPark = serverPark;
        this.#serverInfoPage = serverInfoPage;

        this.#serverDropdownElement = document.getElementById('servers');
        this.#serverDropdownBgElement = document.getElementById('servers-bg');
        this.#selectedServerNameElement = document.querySelector('.selected-server-name');
        this.#serverAddWrapper = document.querySelector('.new-server-wrapper');
        this.#serverAddTextbox = document.getElementById('new-server-box');
        this.#serverAddButton = document.getElementById('add-server-button');

        this.#setupListeners();
        this.#setupGui();

        this.selectServer(null);
    }

    /**
     * Sets up the required listeners.
     */
    #setupListeners(){
        this.#serverPark.addListener('serverAdded', ([server]) => this.#addServer(server));
        this.#serverPark.addListener('serverNameChange', ([o, n]) => this.#serverRename(o, n));
        this.#serverPark.addListener('serverDeleted', ([name]) => this.#serverDeleted(name));
        this.#serverAddWrapper.addEventListener('click', e => {
            e.stopPropagation();
        });
        this.#serverAddButton.addEventListener('click', () => {
            let text = this.#serverAddTextbox.value;
            if(text != null || text !== ""){
                this.#serverPark.addServer(text);
                this.#serverAddTextbox.value = "";
            }
        });
    }

    /**
     * Loads servers into the GUI.
     */
    #setupGui(){
        let allServers = this.#serverPark.getAllServers();
        for (const server of allServers) {
            this.#addServer(server);
        }
    }

    /**
     * Adds a server to the GUI.
     * @param server the server to add
     */
    #addServer(server){
        let singleServer = new SingleServerSelector(this.#serverPark, this.#serverInfoPage, server);
        singleServer.userSelected = name => this.selectServer(name);

        let element = singleServer.getServerElement();
        this.#serverDropdownElement.appendChild(element);

        this.#singleServerSelectors[server.serverName] = singleServer;


        if(this.#selectedServerNameElement.getAttribute('data-default-server') === '1'){
            singleServer.select();
        }

        // this is to keep the bg the same height
        this.#serverDropdownBgElement.innerHTML += '<div class="minecraft-server"></div>';
    }

    /**
     * Server renamed event.
     * @param oldName the old name of the server.
     * @param newName the new name of the server.
     */
    #serverRename(oldName, newName){
        let server = this.#singleServerSelectors[oldName];
        delete this.#singleServerSelectors[oldName];

        this.#singleServerSelectors[newName] = server;

        this.selectServer(newName);
    }

    /**
     * Server deleted event.
     * @param name name of the server that got deleted.
     */
    #serverDeleted(name){
        delete this.#singleServerSelectors[name];

        let keys = Object.keys(this.#singleServerSelectors);
        if(keys.length === 0)
            this.selectServer(null);
        else
            this.selectServer(keys.pop());

        // this is to keep the bg the same height
        try{
            this.#serverDropdownBgElement.children[0].remove();
        }catch (e){}
    }


    /**
     * Loads the server name to the selected server field in the server selector.
     * @param name the server name.
     */
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

        this.#serverInfoPage.loadMCServer(name);
    }
}