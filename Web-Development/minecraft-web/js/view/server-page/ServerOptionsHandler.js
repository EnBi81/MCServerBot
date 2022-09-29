/**
 * Handles the server option panel functionality.
 */
class ServerOptionsHandler{
    #serverOptionsElement;
    #serverNameWrapper;
    singleServerOptions = [];

    constructor(serverNameWrapperId, serverOptionsWrapperId, setupData) {
        this.#serverNameWrapper = document.getElementById(serverNameWrapperId);
        this.#serverOptionsElement = document.getElementById(serverOptionsWrapperId);

        this.#setupData(setupData);
        this.#setupListeners();
    }

    #setupListeners(){
        this.#serverNameWrapper.addEventListener('click', () => {
            this.#serverNameWrapper.classList.toggle('server-option-active');
            if(this.#serverNameWrapper.classList.contains('server-option-active'))
            {
                this.closeAllProceeds();
                this.deleteTextFromFields();
            }
        });
    }

    #setupData(data){
        for (const option of data) {
            let singleServerOption = new SingleServerOption(
                option.singleServerOptionId,
                option.listeners,
                option.onProceed
            );

            this.singleServerOptions.push(singleServerOption);
        }
    }

    /**
     * Clear all the text boxes from the server option panel
     */
    deleteTextFromFields(){
        let textFields = this.#serverOptionsElement.querySelectorAll('input[type="text"]');
        for (const textField of textFields) {
            textField.value = '';
        }
    }

    /**
     * Cancels all proceed windows.
     */
    closeAllProceeds(){
        for (const singleServerOption of this.singleServerOptions) {
            singleServerOption.cancel();
        }
    }
}