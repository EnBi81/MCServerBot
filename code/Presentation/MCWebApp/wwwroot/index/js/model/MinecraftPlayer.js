/**
 * Represents a single minecraft player on one server.
 */
class MinecraftPlayer{
    username;  // string
    onlineFrom; // Date object
    pastUptime; // SimpleDateObject

    constructor(username, onlineFrom, pastUptime) {
        this.username = username;
        this.onlineFrom = onlineFrom;
        this.pastUptime = pastUptime;
    }


    /**
     * Gets the current session time.
     * @returns {SimpleTimeObject}
     */
    getCurrentSessionTime(){
        if(this.onlineFrom == null || !(this.onlineFrom instanceof Date))
            return new SimpleTimeObject(0, 0, 0);

        let millis = DateHelper.subtractDatesByMilliseconds(this.onlineFrom, DateHelper.now());
        return new SimpleTimeObject(0, 0, millis / 1000);
    }

    /**
     * Gets the full uptime for the user.
     * @returns {SimpleTimeObject}
     */
    getAllSessionTime(){
        let sessionTime = this.getCurrentSessionTime();
        return DateHelper.addSimpleTimeObject(sessionTime, this.pastUptime);
    }
}