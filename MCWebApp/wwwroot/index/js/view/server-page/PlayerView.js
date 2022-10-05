/**
 * Responsible for handling the players in the GUI.
 */
class PlayerView{
    playersBoxElement;

    constructor(playerBoxId) {
        this.playersBoxElement = document.getElementById(playerBoxId);
    }


    /**
     * clears the players box's content.
     */
    clearAllPlayers(){
        this.playersBoxElement.innerHTML = "";
    }


    /**
     * Adds multiple players to the view.
     * @param minecraftPlayers players to add
     */
    addPlayersToView(minecraftPlayers){
        for (const minecraftPlayer of minecraftPlayers) {
            this.addPlayerToView(minecraftPlayer);
        }
    }

    /**
     * Adds a single player to the view.
     * @param minecraftPlayer the player to add
     */
    addPlayerToView(minecraftPlayer){
        let playerElement = this.#createPlayerHtml(minecraftPlayer);
        this.#appendPlayerBox(playerElement);
    }

    /**
     * Removes a player from the view.
     * @param username the player's username to remove.
     */
    removePlayerFromView(username){
        for (const playersBox of this.playersBoxElement.children) {
            let usernameAttr = playersBox.getAttribute("data-username");
            if(username === usernameAttr) {
                playersBox.delete();
                break;
            }
        }
    }

    /**
     * Creates the html text for a player.
     * @param player the player object.
     * @returns {string}
     */
    #createPlayerHtml(player){
        let username = player.username;
        let onlineFrom = player.onlineFrom;
        let pastUptime = player.pastUptime;

        return `<div class="player" data-username="${username}" data-online-from="${onlineFrom.getTime()}" data-past-uptime="${SimpleTimeObject.toSeconds(pastUptime)}">\n` +
            `<pre>${username} - <span class=\"time\"></span></pre>\n` +
            "<pre class=\"total-time\">Total uptime: <span class='total-time-span'></span></pre>\n" +
            "</div>";
    }

    /**
     * Appends the html content to the end of the playersBox.
     * @param element
     */
    #appendPlayerBox(element){
        this.playersBoxElement.innerHTML += element;
    }
}