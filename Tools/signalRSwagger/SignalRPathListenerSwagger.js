class SignalRPathListenerSwagger {
    opblockBodyElement;
    path;

    executeWrapper;
    responseWrapper;
    mutationListener;

    signalrConnection;
    
    constructor(opblockBody, path) {
        // this will throw an exception if the executeWrapper is not a node
        let isNode = opblockBody.innerHTML;

        this.opblockBodyElement = opblockBody;
        this.path = path;

        this.#setup();
    }

    #setup() {
        this.executeWrapper = new SignalRBodyExecuteWrapper(this.opblockBodyElement);
        this.executeWrapper.cleared = () => this.cleared();
        this.executeWrapper.executed = () => this.execute();
        this.executeWrapper.closed = () => this.stopExecute();

        this.responseWrapper = new SignalRResponseWrapper(this.opblockBodyElement, {
            methodDecoration: 'Listening on',
            responseStatusCode: 'Listening...',
            description: 'Getting continous response... to stop, press \'Clear\'',
            headerLine: 'Listening...'
        });
    }

    close() {
        this.executeWrapper.close();
    }

    execute() {
        let loading = document.createElement('div');
        loading.classList.add('loading-container');
        loading.innerHTML = '<div class="loading"></div>';

        this.opblockBodyElement.insertBefore(loading, this.opblockBodyElement.children[2])

        let lastSlashIndex = this.path.lastIndexOf("/");
        let hubPath = this.path.substring(0, lastSlashIndex);
        let listener = this.path.substring(lastSlashIndex + 1)

        let fullUrl = location.origin + hubPath;

        if (this.signalrConnection != null)
            this.signalrConnection.stop();

        this.signalrConnection = new signalR.HubConnectionBuilder()
            .withUrl(fullUrl)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        

        try {
            this.signalrConnection.start();

            loading.remove();

            this.responseWrapper.resetToExecute(hubPath, listener, false);

            this.signalrConnection.on(listener,
                (...args) => this.responseWrapper.addMessageToResponse(args));
        } catch (err) {
            loading.remove();

            this.responseWrapper.resetToExecute(hubPath, listener, true);
            this.responseWrapper.addMessageToResponse([err]);
        }
    }

    stopExecute() {
        if (this.signalrConnection != null)
            this.signalrConnection.stop();
    }

    cleared() {
        this.responseWrapper.resetToDefault();
    }
}

// loading outer html
// <div class="loading-container"><div class="loading"></div></div>