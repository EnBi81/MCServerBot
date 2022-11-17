class SignalRHubSwagger {
    opblockTagSection;
    name;
    opblockSwaggers;

    constructor(opblockTagSection) {
        this.opblockTagSection = opblockTagSection;
        this.name = null;
        this.opblockSwaggers = [];

        this.getName();

        // Mutation observer on closing/opening
    }

    close() {
        // close the observer
    }

    setupOpblocks() {
        this.opblockSwaggers = [];
        
        let opblocks = this.opblockTagSection.querySelectorAll('.opblock');
        for (const opblock of opblocks) {
            let opblockSwagger = new SignalRHubOpblockSwagger(opblock);
            this.opblockSwaggers.push(opblockSwagger);
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