/**
 * Handles an error message element and its events.
 */
class ErrorMessageElement{
    #htmlElement;
    #htmlInnerElement;
    #errorMessage;

    constructor(message) {
        this.#errorMessage = message;
    }

    /**
     * Creates an error message element.
     * @returns {HTMLDivElement}
     */
    createElement(){
        let innerElement = document.createElement("div");
        innerElement.classList.add("error-text-content");
        innerElement.textContent = this.#errorMessage;

        let element = document.createElement("div");
        element.classList.add("single-error-message");
        element.appendChild(innerElement);

        this.#htmlInnerElement = innerElement;
        this.#htmlElement = element;
        return element;
    }

    /**
     * Starts listening to events on the element.
     */
    startEventListening(){
        this.#htmlInnerElement.addEventListener("mouseout", () => {
            this.#htmlElement.classList.add("fade-out");
            setTimeout(() => this.#htmlElement.remove(), 1500);
        });
    }

}