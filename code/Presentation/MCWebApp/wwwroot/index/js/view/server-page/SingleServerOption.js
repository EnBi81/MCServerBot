/**
 * Represents a single server option.
 */
class SingleServerOption{
    #serverSingleOptionElement;
    #inputElement;
    #cancelButton;
    #proceedButton;

    #onProceed;

    constructor(singleServerOptionId, listeners, onProceed) {
        this.#serverSingleOptionElement = document.getElementById(singleServerOptionId);
        this.#inputElement = this.#serverSingleOptionElement.querySelector("label input");
        this.#cancelButton = this.#serverSingleOptionElement.querySelector(".cancel-button");
        this.#proceedButton = this.#serverSingleOptionElement.querySelector(".proceed-button");
        this.#onProceed = onProceed;

        this.#setupListeners(listeners);
    }

    /**
     * Sets up the required listeners.
     * @param listeners the listener parameters.
     */
    #setupListeners(listeners){
        for (const listenerObject of listeners) {
            for (const [eventName, condition] of Object.entries(listenerObject)) {
                this.#inputElement.addEventListener(eventName, e => {
                    if(condition(e))
                        this.goToProceedOptions();
                });
            }
        }

        this.#cancelButton.addEventListener("click", () => this.cancel());
        this.#proceedButton.addEventListener("click", () => this.proceed());
    }

    /**
     * Open proceed window on the current server option.
     */
    goToProceedOptions(){
        this.#serverSingleOptionElement.classList.add("proceed-active")
    }

    /**
     * Cancel the proceed window.
     */
    cancel(){
        this.#serverSingleOptionElement.classList.remove("proceed-active")
    }

    /**
     * Execute the function.
     */
    proceed(){
        this.#onProceed(this.#inputElement);
    }
}