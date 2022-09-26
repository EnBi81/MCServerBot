/**
 * Date util methods.
 */
class DateHelper{
    /**
     * Gets the current date
     * @returns {Date}
     */
    static now(){
        return new Date();
    }

    /**
     * Subtracts two dates and returns the elapsed milliseconds.
     * @param dateFrom from
     * @param dateTo to
     * @returns {number}
     */
    static subtractDatesByMilliseconds(dateFrom, dateTo){
        return dateTo.getTime() - dateFrom.getTime();
    }


    /**
     * Adds two SimpleTimeObjects
     * @param date1
     * @param date2
     * @returns {SimpleTimeObject}
     */
    static addSimpleTimeObject(date1, date2){
        let temp = {
            h: date1.hour() + date2.hour(),
            m: date1.minute() + date2.minute(),
            s: date1.second() + date2.second()
        };

        return new SimpleTimeObject(temp.h, temp.m, temp.s);
    }
}