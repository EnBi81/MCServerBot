class CustomMutationObserver {

    mutationListener;

    targetNode;
    addedListener;
    removedListener;

    subtree = false;

    constructor(targetNode, addedListener, removedListener) {
        this.targetNode = targetNode;
        this.addedListener = addedListener;
        this.removedListener = removedListener;
    }

    withSubtree() {
        this.subtree = true;
    }

    start() {
        if (this.mutationListener != null)
            return;

        const callback = (mutationList, observer) => {
            for (const mutation of mutationList) {

                if (typeof this.addedListener === 'function') {
                    for (const mutationNode of mutation.addedNodes) {
                        this.addedListener(mutationNode);
                    }
                }

                if (typeof this.removedListener === 'function') {
                    for (const removedNode of mutation.removedNodes) {
                        this.removedListener(removedNode)
                    }
                }
            }
        };

        this.mutationListener = new MutationObserver(callback);


        this.mutationListener.observe(this.targetNode, { childList: true, subtree: this.subtree });
    }


    close() {
        this.mutationListener.disconnect();
    }
}

class SignalRBodyExecuteWrapper {
    #clonedButtonClass = 'cloned';
    #btnGroupClass = 'btn-group';
    #executeWrapperClass = 'execute-wrapper';
    #executeButtonClass = 'execute';
    #btnClearClass = 'btn-clear';


    executeWrapper;

    mutationListener;

    constructor(opblockBody) {
        // this will throw an exception if the executeWrapper is not a node
        let isNode = opblockBody.innerHTML;

        this.executeWrapper = opblockBody.querySelector(`.${this.#executeWrapperClass}`);
        if (this.executeWrapper == null)
            this.executeWrapper = opblockBody.querySelector(`.${this.#btnGroupClass}`);


        this.#setup();
    }

    #setup() {
        this.mutationListener = new CustomMutationObserver(
            this.executeWrapper,
            b => this.buttonAdded(b),
            b => this.buttonRemoved(b)
        );
        this.mutationListener.start();

        let potentialButton = this.executeWrapper.querySelector(`.${this.#executeButtonClass}`);
        if (potentialButton != null)
            this.addCloneButton(potentialButton);
    }

    close() {
        this.closed();
        this.mutationListener.close();
        this.removeClearButton();
        this.removeCloneButton();
    }

    buttonAdded(buttonElement) {
        if (buttonElement.classList.contains(this.#clonedButtonClass))
            return;
        if (!buttonElement.classList.contains(this.#executeButtonClass))
            return;

        this.addCloneButton(buttonElement);
    }

    buttonRemoved(buttonElement) {
        if (buttonElement.classList.contains(this.#clonedButtonClass))
            return;
        if (!buttonElement.classList.contains(this.#executeButtonClass))
            return;

        this.cleared();

        this.removeCloneButton();
        this.removeClearButton();
    }

    addCloneButton() {
        let cloneButtonExisting = this.executeWrapper.querySelector(`.${this.#clonedButtonClass}`);
        if (cloneButtonExisting != null)
            return;

        let cloneButton = document.createElement('button');
        cloneButton.setAttribute('class', 'btn execute opblock-control__btn');
        cloneButton.classList.add(this.#clonedButtonClass)
        cloneButton.textContent = 'Execute';
        cloneButton.onclick = () => {
            this.addClearButton();
            this.executed();
        }

        this.executeWrapper.appendChild(cloneButton);
    }

    removeCloneButton() {
        let cloneButton = this.executeWrapper.querySelector(`.${this.#clonedButtonClass}`);
        if (cloneButton == null)
            return;

        this.executeWrapper.removeChild(cloneButton);
    }

    checkClearButtonExist() {
        let clearButtonExisting = this.executeWrapper.querySelector(`.${this.#btnClearClass}`);
        return clearButtonExisting != null;
    }

    addClearButton() {
        if (this.checkClearButtonExist())
            return;

        let clearButton = document.createElement("button");
        clearButton.setAttribute("class", "btn btn-clear opblock-control__btn");
        clearButton.textContent = "Clear";
        clearButton.onclick = () => {
            this.cleared();
            this.removeClearButton(true);
        }

        this.executeWrapper.appendChild(clearButton)

        this.executeWrapper.classList.add(this.#btnGroupClass);
        this.executeWrapper.classList.remove(this.#executeWrapperClass)
    }

    removeClearButton() {
        let clearButton = this.executeWrapper.querySelector(`.${this.#btnClearClass}`);
        if (clearButton == null)
            return;

        this.executeWrapper.removeChild(clearButton);

        this.executeWrapper.classList.remove(this.#btnGroupClass);
        this.executeWrapper.classList.add(this.#executeWrapperClass)

        this.closed();
    }

    isExpanded() {
        // else its 'execute-wrapper'
        return this.executeWrapper.classList.contains(this.#btnGroupClass);
    }


    closed() { }
    cleared() { }
    executed() { }
}

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

class SignalRParameterSwagger {

    parameterType;
    subStype;
    valueTd;

    constructor(parameterTr) {
        this.valueTd = parameterTr.querySelector('.parameters-col_description');
        let typeElement = parameterTr.querySelector('.parameter__type');
        let parameterTypeTemp = typeElement.textContent;

        if (parameterTypeTemp.indexOf('(') >= 0) {
            this.parameterType = parameterTypeTemp.substring(0, parameterTypeTemp.indexOf('('));
            this.subStype = parameterTypeTemp.substring(parameterTypeTemp.indexOf('(') + 2);
            this.subStype = this.subStype.substring(0, this.subStype.indexOf(')'))
        }
        else {
            this.parameterType = parameterTypeTemp;
            this.subStype = 'double';
        }
    }

    #getCorrectValue(paramType, obj) {
        if (paramType === 'string') {
            return obj.querySelector('input').value;
        }
        else if (paramType === 'integer') {
            let textValue = obj.querySelector('input').value;
            return isNaN(textValue) ? -1 : parseInt(textValue, 10);
        }
        else if (paramType === 'number') {
            if (this.subStype === 'double') {
                let textValue = obj.querySelector('input').value;
                return isNaN(textValue) ? -1 : parseFloat(textValue);
            }
            else if (this.subStype === 'float') {
                let textValue = obj.querySelector('input').value;
                if (isNaN(textValue))
                    return -1;

                let float64 = parseFloat(textValue);
                let float32View = new DataView(new ArrayBuffer(4));
                float32View.setFloat32(0, float64);

                let float32 = float32View.getFloat32();
                return float32;
            }
            else
                return 0;
        }
        else if (paramType === 'boolean') {
            let value = obj.querySelector('select').value;
            return value === 'true';
        }
        else if (paramType === 'object') {
            let value = obj.querySelector('textarea').value;
            try {
                let parsed = JSON.parse(value);
                return parsed;
            }
            catch (e) {
                return null;
            }


        }
        else if (paramType.startsWith('array')) {
            let arrayType = paramType.substring(paramType.indexOf('[') + 1);
            arrayType = arrayType.substring(0, arrayType.indexOf(']'));

            if (arrayType === 'array') {
                return 0;
            }

            let valueObjects = obj.querySelectorAll('.json-schema-form-item');
            let resultArr = [];

            for (const valueObj of valueObjects) {
                let value = this.#getCorrectValue(arrayType, valueObj);
                resultArr.push(value);
            }

            return resultArr;
        }

        return null;
    }

    getValue() {
        return this.#getCorrectValue(this.parameterType, this.valueTd);
    }
}

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

class SignalRResponseWrapper {
    #responsesClass = 'responses-wrapper';

    #baseResponseDiv = `<div> <div> <div> <div class="request-url"> <h4>Request URL</h4> <pre class="microlight"> https://localhost:7229/hubs/serverpark/Receive </pre> </div></div><h4>Server response</h4> <table class="responses-table live-responses-table"> <thead> <tr class="responses-header"> <td class="col_header response-col_status"> Code </td><td class="col_header response-col_description"> Details </td></tr></thead> <tbody> <tr class="response"> <td class="response-col_status"> 404 </td><td class="response-col_description"> <div> <h5>Getting continous response... to stop, press 'Clear'</h5> <pre class="microlight"> <span class="headerline">heelo</span> </pre> </div></td></tr></tbody> </table> </div></div>`;

    responsesElement;
    responsesInnerElement;
    responseTextContainer;

    mutationListener;

    config

    constructor(opblockBody, config) {
        this.config = config;
        let responsesElement = opblockBody.querySelector(`.${this.#responsesClass}`);

        if (responsesElement != null) {
            this.#setup(responsesElement);
            return;
        }

        this.mutationListener = new CustomMutationObserver(opblockBody,
            (node) => {
                if (node.classList.contains(this.#responsesClass))
                    this.#setup(node);
            });
        this.mutationListener.start();

    }

    #setup(node) {
        this.responsesElement = node;
        this.responsesInnerElement = this.responsesElement.querySelector('.responses-inner');
        this.responsesElement.querySelector('.responses-table').remove();

        if (this.mutationListener != null)
            this.mutationListener.close();
    }

    resetToDefault() {
        this.responsesInnerElement.innerHTML = '';
    }

    resetToExecute(hubPath, hubMethod, isFail) {
        this.responsesInnerElement.innerHTML = this.#baseResponseDiv;
        let urlElement = this.responsesInnerElement.querySelector('.request-url .microlight');
        urlElement.textContent = `Hub: '${location.origin}${hubPath}'  |  ${this.config.methodDecoration}: '${hubMethod}'`; // Listening on

        let responseRow = this.responsesInnerElement.querySelector('.response');
        let responseStatusCode = responseRow.querySelector('.response-col_status');
        let description = responseRow.querySelector('.response-col_description h5');
        this.responseTextContainer = responseRow.querySelector('.response-col_description .microlight');

        if (isFail) {
            responseStatusCode.textContent = 'FAILED';
            description.textContent = 'This request has failed, please try again!';
            this.responseTextContainer.innerHTML = '';
        }
        else {
            responseStatusCode.textContent = this.config.responseStatusCode;//'Listening...';
            description.textContent = this.config.description; //'	Getting continous response... to stop, press \'Clear\'';
            this.responseTextContainer.innerHTML = `<span class="headerline">${this.config.headerLine}</span>`;//Listening...
        }
    }

    addMessageToResponse(array) {
        if (this.responseTextContainer == null)
            return;

        let span = document.createElement('span');
        span.classList.add('headerline');
        let text = array.join(' | ');
        span.textContent = text;

        this.responseTextContainer.appendChild(span);
    }
}



class SignalRSwaggerSetup {

    swaggerUINode;


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

            if (!name.endsWith('Hub'))
                continue;

            this.swaggerHubs.push(hub);
            hub.startSetup();
        }
    }
}

(function () {
    let setup = null;

    let swaggerUIId = document.querySelector('#swagger-ui');
    let observer = new CustomMutationObserver(swaggerUIId, (node) => {
        if (node.classList.contains('wrapper')) {
            if (setup == null)
                setup = new SignalRSwaggerSetup();
            observer.close();
        }
    });
    observer.withSubtree();
    observer.start();
})()

