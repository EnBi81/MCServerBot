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