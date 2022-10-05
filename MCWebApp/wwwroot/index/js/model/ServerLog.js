/**
 * Represents a single server log message.
 */
class ServerLog{
    static SYSTEM_MESSAGE = 0;
    static SYSTEM_ERROR_MESSAGE = 1;
    static USER_MESSAGE = 2;

    message; // string
    logType; // number {0, 1, 2}

    constructor(message, logType) {
        this.message = message;
        this.logType = logType;
    }

}