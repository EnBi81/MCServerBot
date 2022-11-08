/**
 * Handles events and other tasks related to the log and command window.
 */
class LogView{
    logMessages;
    commandInputElement;

    constructor(logBoxId, commandInputId) {
        this.logMessages = document.getElementById(logBoxId);
        this.commandInputElement = document.getElementById(commandInputId);

        this.#setupInputListeners();
    }

    /**
     * Sets up the listener for the command input text input.
     */
    #setupInputListeners(){
        this.commandInputElement.addEventListener("keypress", e => {
            if(e.key !== "Enter")
                return;

            let command = this.commandInputElement.value.trim();
            if(command.length === 0)
                return;

            this.clearCommandInput();

            this.userCommandEntered(command);
        })
    }

    /**
     * This method is replaced by the object created this instance.
     * @param command command which was entered.
     */
    userCommandEntered(command){
        console.log("User entered command: " + command)
    }

    /**
     * clears the log message container.
     */
    clearLogs(){
        this.logMessages.innerHTML = "";
    }

    /**
     * Loads the log messages into the view.
     * @param logs logs to load.
     */
    loadLogsToView(logs) {
       
        for (const log of logs) {
            let div = this.#createLogDiv(log);
            this.#appendLogMessage(div);
        }
    }

    /**
     * Clears the command input field.
     */
    clearCommandInput(){
        this.setCommandInput("");
    }

    /**
     * Sets the command input field to a value.
     * @param value the value to set the input field to.
     */
    setCommandInput(value){
        this.commandInputElement.value = value;
    }

    /**
     * Creates an html element for the log message.
     * @param log the log data
     * @returns {HTMLDivElement}
     */
    #createLogDiv(log){
        let message = log.message;
        let type = log.logType;

        let div = document.createElement("div");
        div.textContent = message;

        if(type === 2) {
            div.classList.add("user-message")
        }
        else{
            div.classList.add("system-message");
            if(type === 1)
                div.classList.add("error")
        }

        return div;
    }

    /**
     * Append a message to the end of the log container. Also scrolls down to the loaded log message if the GUI was scrolled to the bottom.
     * @param htmlElement the html element to append to the log container.
     */
    #appendLogMessage(htmlElement){
        let scrolledToBottom = this.logMessages.scrollTop === (this.logMessages.scrollHeight - this.logMessages.offsetHeight)
        this.logMessages.appendChild(htmlElement);

        if(scrolledToBottom)
            this.logMessages.scrollTo(0, this.logMessages.scrollHeight);
    }
}