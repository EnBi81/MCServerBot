class SignalRPathSenderSwagger {
    opblockBodyElement;
    path;

    executeWrapper;
    responseWrapper;
    mutationListener;


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

        this.responseWrapper = new SignalRResponseWrapper(this.opblockBodyElement, {
            methodDecoration: 'Sending to',
            responseStatusCode: 'Sending...',
            description: 'Sending message to the hub',
            headerLine: 'Sending...'
        });
    }

    close() {
        this.executeWrapper.close();
    }

    async execute() {
        let loading = document.createElement('div');
        loading.classList.add('loading-container');
        loading.innerHTML = '<div class="loading"></div>';

        this.opblockBodyElement.insertBefore(loading, this.opblockBodyElement.children[2])

        let lastSlashIndex = this.path.lastIndexOf("/");
        let hubPath = this.path.substring(0, lastSlashIndex);
        let hubmethod = this.path.substring(lastSlashIndex + 1).trim()

        let fullUrl = location.origin + hubPath;

        let signalrConnection = new signalR.HubConnectionBuilder()
            .withUrl(fullUrl)
            .configureLogging(signalR.LogLevel.Information)
            .build();



        try {
            await signalrConnection.start();
            loading.remove();

            this.responseWrapper.resetToExecute(hubPath, hubmethod, false);

            let parameterValues = this.#getParameters();
            let sentString = parameterValues.join(', ');
            parameterValues.unshift(hubmethod);


            // this is to add array as parameters
            let prom = signalrConnection.invoke.apply(signalrConnection, parameterValues);

            this.responseWrapper.addMessageToResponse([`Sent to ${hubmethod}!`]);
            this.responseWrapper.addMessageToResponse([`Values ${sentString}`]);
        } catch (err) {
            loading.remove();

            this.responseWrapper.resetToExecute(hubPath, hubmethod, true);
            this.responseWrapper.addMessageToResponse([err]);
        }

        setTimeout(() => {
            if (signalrConnection != null)
                signalrConnection.stop();
        }, 100);
    }
    

    cleared() {
        this.responseWrapper.resetToDefault();
    }


    #getParameters() {
        let parameterValues = [];

        let tableRows = this.opblockBodyElement.querySelectorAll('table.parameters tbody tr');

        for (const row of tableRows) {
            let paramSwagger = new SignalRParameterSwagger(row);
            let value = paramSwagger.getValue();
            parameterValues.push(value);
        }

        return parameterValues;
    }
}