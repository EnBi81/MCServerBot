/**
 * Handles the error message panel.
 */
class ErrorView{
    #errorContentElement;
    #serverPark;

    constructor(serverPark, errorContentId) {
        this.#errorContentElement = document.getElementById(errorContentId);
        this.#serverPark = serverPark;

        this.#setupListeners();
    }

    /**
     * Sets up the error listener to the ServerPark.
     */
    #setupListeners(){
        this.#serverPark.addListener("errorMessage", ([errorMess]) => this.addErrorMessage(errorMess));
    }

    /**
     * Creates an error message element and displays it in the view.
     * @param errorMessage
     */
    addErrorMessage(errorMessage){
        let errorMess = new ErrorMessageElement(errorMessage);
        let element = errorMess.createElement();
        this.#errorContentElement.appendChild(element);
        errorMess.startEventListening();
    }
}