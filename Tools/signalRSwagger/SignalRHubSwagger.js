class SignalRHubSwagger {
    opblockTagSection;
    name;
    pathSwaggers = [];

    mutationObserver

    constructor(opblockTagSection) {
        this.opblockTagSection = opblockTagSection;
        this.name = null;
        
    }

    #loadFromNode(node) {
        let opblocks = node.querySelectorAll('.opblock');
        for (const opblock of opblocks) {
            let path = new SignalRPathSwagger(opblock);
            this.pathSwaggers.push(path);
        }
    }

    // before invoking this method, please check if this is actually a hub, or just a normal http controller
    startSetup() {

        this.#loadFromNode(this.opblockTagSection);
            
        
        this.mutationObserver = new CustomMutationObserver(
            this.opblockTagSection,
            (addedNode) => this.#loadFromNode(addedNode),
            () => this.#closePathSwaggers(),
        );
        this.mutationObserver.start();
    }

    close() {
        this.mutationObserver.close();
        this.#closePathSwaggers();
    }

    #closePathSwaggers() {
        for (const opblock of this.pathSwaggers) {
            opblock.close();
        }
        this.pathSwaggers = [];
    }

    setupOpblocks() {
        this.#closePathSwaggers();
        
        let opblocks = this.opblockTagSection.querySelectorAll('.opblock');
        for (const opblock of opblocks) {
            let opblockSwagger = new SignalRHubOpblockSwagger(opblock);
            this.pathSwaggers.push(opblockSwagger);
        }
    }

    getName() {
        if (this.name == null) {
            let nameSpan = this.opblockTagSection.querySelector('h3 > a > span');
            this.name = nameSpan.textContent;
        }

        return this.name;
    }

    isOpen() {
        return this.opblockTagSection.classList.contains('is-open');
    }
}