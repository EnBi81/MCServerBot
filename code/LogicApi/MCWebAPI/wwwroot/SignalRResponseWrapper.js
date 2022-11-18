class SignalRResponseWrapper {
    #responsesClass = 'responses-wrapper';

    #baseResponseDiv = `<div> <div> <div> <div class="request-url"> <h4>Request URL</h4> <pre class="microlight"> https://localhost:7229/hubs/serverpark/Receive </pre> </div></div><h4>Server response</h4> <table class="responses-table live-responses-table"> <thead> <tr class="responses-header"> <td class="col_header response-col_status"> Code </td><td class="col_header response-col_description"> Details </td></tr></thead> <tbody> <tr class="response"> <td class="response-col_status"> 404 </td><td class="response-col_description"> <div> <h5>Getting continous response... to stop, press 'Clear'</h5> <pre class="microlight"> <span class="headerline">heelo</span> </pre> </div></td></tr></tbody> </table> </div></div>`;
    
    responsesElement;
    responsesInnerElement;
    responseTextContainer;

    mutationListener;

    constructor(opblockBody) {
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
        urlElement.textContent = `Hub: '${location.origin}${hubPath}'  |  Listening on: '${hubMethod}'`;

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
            responseStatusCode.textContent = 'Listening...';
            description.textContent = '	Getting continous response... to stop, press \'Clear\'';
            this.responseTextContainer.innerHTML = '<span class="headerline">Listening...</span>';
        }
    }

    addMessageToResponse(array) {
        if (this.responseTextContainer == null)
            return;

        console.log(array);

        let span = document.createElement('span');
        span.classList.add('headerline');
        let text = array.join(', ');
        span.textContent = text;

        this.responseTextContainer.appendChild(span);
    }
}