/**
 * Handles the profile pic view. This class is responsible for refreshing the profile pick and name of the logged in user.
 */
class ProfilePicView{
    #welcomeMessageElement;
    #profPicElement;
    #nameElement;

    #serverPark;

    constructor(serverPark) {
        this.#welcomeMessageElement = document.querySelector(".welcome-message");
        this.#profPicElement = this.#welcomeMessageElement.querySelector(".prof-pic");
        this.#nameElement = this.#welcomeMessageElement.querySelector("#player-name");

        this.#serverPark = serverPark;

        this.#setupListener();
    }

    #setupListener(){
        this.#welcomeMessageElement.addEventListener('dblclick', () => {
            let id = this.#nameElement.getAttribute('data-user-id')
            serverPark.getRefreshedProfile(id, user => this.setupData(user));
        })
    }


    setupData(discordUser){
        this.#nameElement.setAttribute('data-user-id', discordUser.discordId);
        this.#nameElement.textContent = discordUser.discordName;
        this.#profPicElement.setAttribute('src', discordUser.profilePicUrl);
    }
}