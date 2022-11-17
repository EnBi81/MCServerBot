class SignalRPathSwagger {
    opblock;
    path;
    isListener;
    
    constructor(opblock) {
        this.opblock = opblock;
        this.#path = null;

        this.#setup();
    }

    #setup() {
        let summaryMethodElement = this.opblock.querySelector('.opblock-summary-method');
        let method = summaryMethodElement.textContent;

        if (method === 'GET') {
            this.isListener = true;
            opblock.setAttribute("data-opblock-hub", "listen");
            summaryMethodElement.textContent = 'LISTEN';
        }
        else {
            this.isListener = false;
            opblock.setAttribute("data-opblock-hub", "send");
            summaryMethodElement.textContent = 'SEND';
        }


        // start observer on closing/opening
    }

    close() {
        // close the observer 
    }

    getPath() {
        if (this.path == null) {
            let pathElement = this.opblock.querySelector('.opblock-summary-path');
            this.path = pathElement.getAttribute('data-path');
        }

        return this.path;
    }

    isExpanded() {
        return this.opblock.classList.contains('is-open');
    }
}