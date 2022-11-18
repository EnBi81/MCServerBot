﻿class SignalRSwaggerSetup {

    swaggerUINode;

    hubNames = ["ServerParkHub"];

    swaggerHubs = []

    mutationObserver;
    
    constructor() {
        this.swaggerUINode = document.querySelector('div.swagger-ui');

        let sections = this.swaggerUINode.querySelectorAll('.opblock-tag-section');

        if (sections != null && sections.length > 0) {
            this.#setupSection(sections);
        }

        this.mutationObserver = new CustomMutationObserver(
            this.swaggerUINode,
            (node) => {
                let sectionsUI = node.querySelectorAll('.opblock-tag-section');

                if (sectionsUI != null && sectionsUI.length > 0) {
                    this.#setupSection(sectionsUI);
                }
            }
        );

        this.mutationObserver.start();
    }

    close() {
        this.mutationObserver.close();
        this.#closeHubs();
    }


    #closeHubs() {
        for (const hub of this.swaggerHubs)
            hub.close();

        this.swaggerHubs = [];
    }

    #setupSection(sections) {
        this.#closeHubs();

        for (const section of sections) {
            let hub = new SignalRHubSwagger(section);
            let name = hub.getName();

            if (!this.hubNames.includes(name))
                continue;

            this.swaggerHubs.push(hub);
            hub.startSetup();
        }
    }
}


const targetNode = document.querySelector('div.swagger-ui');
let sections = sectionParent.querySelectorAll('.opblock-tag-section');