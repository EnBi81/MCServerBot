/**
 * Handles the error message panel.
 */
class NotificationView {
    #notificationContentElement;
    #serverPark;

    constructor(serverPark, notificationContentId) {
        this.#notificationContentElement = document.getElementById(notificationContentId);
        this.#serverPark = serverPark;

        this.#setupListeners();
    }

    /**
     * Sets up the notification listener to the ServerPark.
     */
    #setupListeners(){
        this.#serverPark.addListener("errorMessage", ([errorMess]) => this.addNotificationMessage(errorMess, 'red', 'orange'));
        this.#serverPark.addListener("serverDeleted", ([name]) => this.addNotificationMessage(`Server ${name} has been deleted`, 'blue', 'green'));
        this.#serverPark.addListener("serverAdded", ([server, isSetup]) => {
            if(!isSetup)
                this.addNotificationMessage(`Server ${server.serverName} has been added`, 'yellow', 'purple');
        });
    }

    /**
     * Creates a notification message element and displays it in the view.
     * @param notificationMessage message
     * @param primaryColor optional primary color
     * @param secondaryColor optional secondary color
     */
    addNotificationMessage(notificationMessage, primaryColor, secondaryColor){
        let errorMess = new NotificationMessageElement(notificationMessage);
        let element = errorMess.createElement(primaryColor, secondaryColor);
        this.#notificationContentElement.appendChild(element);
        errorMess.startEventListening();
    }
}