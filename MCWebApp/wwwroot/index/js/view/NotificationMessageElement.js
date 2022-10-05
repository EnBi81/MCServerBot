/**
 * Handles an error message element and its events.
 */
class NotificationMessageElement {
    #htmlElement;
    #htmlInnerElement;
    #errorMessage;

    constructor(message) {
        this.#errorMessage = message;
    }

    /**
     * Creates an error message element.
     * @param primaryColor optional primary color
     * @param secondaryColor optional secondary color
     * @returns {HTMLDivElement}
     */
    createElement(primaryColor, secondaryColor){
        let innerElement = document.createElement("div");
        innerElement.classList.add("notification-text-content");
        innerElement.textContent = this.#errorMessage;

        let element = document.createElement("div");
        element.classList.add("single-notification-message");
        element.appendChild(innerElement);

        if(primaryColor != null)
            element.style.setProperty('--notification-primary-color', primaryColor);
        if(secondaryColor != null)
            element.style.setProperty('--notification-secondary-color', secondaryColor);

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