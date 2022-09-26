/**
 * This class is responsible for updating the time values in the webpage.
 */
class UpdateTimers{
    #serverUptimeBox;
    #playersBox;

    constructor(serverUptimeBox, playersBox) {
        this.#serverUptimeBox = serverUptimeBox;
        this.#playersBox = playersBox;

        setInterval(() => this.updateAllTime(), 1000);
    }

    /**
     * Updates all the timer objects.
     */
    updateAllTime(){
        // this updates the server uptime
        let onlineFrom = this.#serverUptimeBox.getAttribute("data-online-from");
        if(onlineFrom != null){
            let date = new Date(parseInt(onlineFrom));
            let difference = DateHelper.subtractDatesByMilliseconds(date, DateHelper.now());
            let simpleTime = SimpleTimeObject.fromSeconds(difference / 1000);
            this.#serverUptimeBox.textContent = simpleTime.asString();
        }

        // updates the players uptime
        for (const playerBox of this.#playersBox.children) {
            let playerOnlineFrom = playerBox.getAttribute("data-online-from");
            let date = new Date(parseInt(playerOnlineFrom));
            let difference = DateHelper.subtractDatesByMilliseconds(date, DateHelper.now());
            let simpleTime = SimpleTimeObject.fromSeconds(difference / 1000);

            let pastUptime = playerBox.getAttribute("data-past-uptime");
            let simpleTimePast = SimpleTimeObject.fromSeconds(parseInt(pastUptime));
            simpleTimePast = DateHelper.addSimpleTimeObject(simpleTime, simpleTimePast);

            let onlineTimeBox = playerBox.querySelector(".time");
            let fullOnlineTime = playerBox.querySelector(".total-time-span");

            onlineTimeBox.textContent = simpleTime.asString();
            fullOnlineTime.textContent = simpleTimePast.asString();
        }
    }
}