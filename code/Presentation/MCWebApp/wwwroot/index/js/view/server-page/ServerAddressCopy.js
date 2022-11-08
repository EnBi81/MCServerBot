/**
 * Handles the server address copying event by user.
 */
class ServerAddressCopy{
    #serverAddressElement;
    #serverIpElement;
    #serverPortElement;

    constructor(serverAddressId) {
        this.#serverAddressElement = document.getElementById(serverAddressId);
        this.#serverIpElement = this.#serverAddressElement.querySelector("#ip-address");
        this.#serverPortElement = this.#serverAddressElement.querySelector("#port");

        this.#setupListeners();
    }

    /**
     * Sets up the GUI listeners for the address box.
     */
    #setupListeners(){
        this.#serverAddressElement.addEventListener("click", ()=>{
            this.copy()
        })
        this.#serverAddressElement.addEventListener("keypress", ev => {
            if(ev.key !== "Enter")
                return;

            this.copy();
        })
    }

    /**
     * Gets the server address from the GUI.
     * @returns {string}
     */
    #getServerAddress(){
        return this.#serverIpElement.textContent + ":" + this.#serverPortElement.textContent;
     }

    /**
     * Copies the server address to the clipboard and displays it in the GUI. Else it sends an error message.
     */
    copy(){
        this.#serverAddressElement.classList.remove("copied")
        this.#serverAddressElement.offsetHeight; //trigger reflow xdddd


        // check if we have permission to copy
        navigator.permissions.query({ name: "clipboard-read" }).then((result) => {
            if (result.state === "granted" || result.state === "prompt") {
                navigator.clipboard.writeText(this.#getServerAddress()).then()
                this.#serverAddressElement.classList.add("copied");
            }
            else {
                notificationView.addNotificationMessage('Cannot copy message to clipboard. Please use the https website for that!');
            }
        });
    }

}