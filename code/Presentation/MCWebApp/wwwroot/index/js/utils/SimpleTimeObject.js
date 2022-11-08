/**
 * Represents a simple Timespan object.
 */
class SimpleTimeObject {
    /**
     * Creates a SimpleTimeObject from the seconds given in the parameter.
     * @param seconds the seconds to convert into SimpleTimeObject
     * @returns {SimpleTimeObject}
     */
    static fromSeconds(seconds){
        return new SimpleTimeObject(0, 0, seconds);
    }

    /**
     * Gets the sum of the seconds from a SimpleTimeObject
     * @param simpleTime the SimpleTimeObject to convert to second.
     * @returns {Number}
     */
    static toSeconds(simpleTime){
        return simpleTime.hour() * 3600 + simpleTime.minute() * 60 + simpleTime.second()
    }


    static fromText(text){
        let hour = text.slice(0, 2);
        let minute = text.slice(2, 4);
        let second = text.slice(4, 6);

        return new SimpleTimeObject(parseInt(hour), parseInt(minute), parseInt(second));
    }


    #hour;
    #minute;
    #second;

    constructor(hour, minute, second) {
        if(isNaN(second) || second < 0)
            second = 0;
        if(isNaN(minute) || minute < 0)
            minute = 0;
        if(isNaN(hour) || hour < 0)
            hour = 0;


        if(second >= 60){
            minute += Math.floor(second / 60);
            second %= 60;
        }

        if(minute >= 60){
            hour += Math.floor(minute / 60);
            minute %= 60;
        }

        this.#hour = hour;
        this.#minute = minute;
        this.#second = second;
    }


    /**
     * Converts this object to a string.
     * @returns {string}
     */
    asString(){
        return this.#formatNum(this.hour()) + ":" +
            this.#formatNum(this.minute()) + ":" +
            this.#formatNum(this.second());
    }

    /**
     * Returns the hour.
     * @returns {Number}
     */
    hour(){
        return this.#hour;
    }

    /**
     * Returns the minute.
     * @returns {Number}
     */
    minute(){
        return this.#minute;
    }

    /**
     * Returns the second.
     * @returns {Number}
     */
    second(){
        return this.#second;
    }

    /**
     * Formats a number to 00 string format.
     * @param num the number to format.
     * @returns {string}
     */
    #formatNum(num){
        let s = num + "";
        if(s.includes("."))
            s = s.substring(0, s.indexOf("."))

        return s.length === 1 ? "0" + s : s;
    }
}