/**
 * Keeps track of the settings instances.
 */
class SettingsCollection{

    settings = {} // all the settings by "settingsName": SingleSetting

    #settingWrapperElement; // to toggle hide
    #settingsAllResetElement; // add event listener to reset all settings
    #logoutButton;

    constructor(settingWrapperId, settingsAllResetId, logoutButtonId) {
        this.#settingWrapperElement = document.getElementById(settingWrapperId);
        this.#settingsAllResetElement = document.getElementById(settingsAllResetId);
        this.#logoutButton = document.getElementById(logoutButtonId);

        this.#setupListener();
    }

    /**
     * Sets up the double click listener for the all-reset button.
     */
    #setupListener(){
        this.#settingsAllResetElement.addEventListener('dblclick', () => this.reset());
        this.#logoutButton.addEventListener('click', () => {
            let result = confirm('Do you really want to log out?');
            if(result === true){
                Logout.logout();
            }
        })
    }

    /**
     * Toggles the settings panel's visibility.
     */
    toggle(){
        this.#settingWrapperElement.classList.toggle('hidden');
    }

    /**
     * Loads the settings from the settingsSetup object.
     * @param settingsSetup to load the settings from.
     */
    load(settingsSetup){
        for (const [key, value] of Object.entries(settingsSetup)) {
            settings[key] = new SingleSetting(
                value.defaultValue,
                value.cssProperty,
                value.valueInputId,
                value.resetButtonId,
                value.eventsToListen,
            );
        }
    }

    /**
     * Resets all the settings to their default value.
     */
    reset(){
        for (const setting of Object.values(settings)) {
            setting.reset();
        }
    }
}
