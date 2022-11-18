class SignalRPathSwagger {
    opblock;
    #path;
    isListener;

    pathListener;
    pathSender;

    mutationListener;

    
    constructor(opblock) {
        this.opblock = opblock;
        this.#path = null;

        this.#setup();
    }

    #setup() {
        let summaryMethodElement = this.opblock.querySelector('.opblock-summary-method');
        let method = summaryMethodElement.textContent;
        let opblock = this.opblock;

        if (method === 'GET') {
            this.isListener = true;
            opblock.setAttribute("data-opblock-hub", "listen");
            summaryMethodElement.textContent = 'LISTEN';

            this.#createListenerIfAvailable();
        }
        else {
            this.isListener = false;
            opblock.setAttribute("data-opblock-hub", "send");
            summaryMethodElement.textContent = 'SEND';

            this.#createSenderIfAvailable();
        }
        
        this.mutationListener = new CustomMutationObserver(
            this.opblock,
            (node) => {
                if (node.tagName.toLowerCase() === 'noscript')
                    return;

                if (this.isListener)
                    this.#createListenerIfAvailable();
                else
                    this.#createSenderIfAvailable();
            },
            (node) => {
                if (node.tagName.toLowerCase() === 'noscript')
                    return;
                

                if (this.isListener)
                    this.pathListener.close();
                else this.pathSender.close();
            }
        );

        this.mutationListener.start();
        // start observer on closing/opening
    }

    #createListenerIfAvailable() {
        let opblockBody = this.opblock.querySelector('.opblock-body');
        if (opblockBody != null) {
            this.pathListener = new SignalRPathListenerSwagger(opblockBody, this.getPath());
        }
    }

    #createSenderIfAvailable() {
        let opblockBody = this.opblock.querySelector('.opblock-body');
        if (opblockBody != null) {
            this.pathSender = new SignalRPathSenderSwagger(opblockBody, this.getPath());
        }
    }

    close() {
        this.mutationListener.close();
        
        if (this.pathListener != null)
            this.pathListener.close();
        if (this.pathSender != null)
            this.pathSender.close();
    }

    getPath() {
        if (this.#path == null) {
            let pathElement = this.opblock.querySelector('.opblock-summary-path');
            this.#path = pathElement.getAttribute('data-path');
        }

        return this.#path;
    }

    isExpanded() {
        return this.opblock.classList.contains('is-open');
    }
}