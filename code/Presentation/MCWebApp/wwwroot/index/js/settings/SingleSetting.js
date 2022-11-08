/**
 * Represents a single setting instance.
 */
class SingleSetting {
    defaultValue;
    currentValue;
    resetButton; // reset button element
    valueInputElement; // input element
    cssProperty;

    constructor(defaultValue, cssProperty, valueInputId, resetButtonId, eventsToListen) {
        this.defaultValue = defaultValue;
        this.resetButton = document.getElementById(resetButtonId);
        this.valueInputElement = document.getElementById(valueInputId);
        this.cssProperty = cssProperty;

        this.#setSavedOrDefault();
        this.#setupListeners(eventsToListen);
    }

    /**
     * Sets up the listeners for the input.
     * @param eventsToListen
     */
    #setupListeners(eventsToListen){
        for (const event of eventsToListen) {
            this.valueInputElement.addEventListener(event, () => {
                this.set(this.valueInputElement.value);
            })
        }

        this.resetButton.addEventListener('dblclick', () => {
            this.#setInputToValue(this.defaultValue);
            this.set(this.defaultValue);
        });
    }

    /**
     * Resets the setting to the default value.
     */
    reset(){
        this.#setInputToValue(this.defaultValue);
        this.set(this.defaultValue);
    }

    /**
     * Sets the setting to the given value, and saves it.
     * @param value
     */
    set(value){
        if(value == null || this.currentValue === value)
            return;


        this.#setCssProperty(value);
        this.currentValue = value;
        this.#save();
    }

    #setCssProperty(value){
        document.body.style.setProperty(this.cssProperty, value);
    }

    /**
     * Saves the current value of the setting to the localstorage
     */
    #save(){
        localStorage.setItem(this.cssProperty, this.currentValue);
    }

    /**
     * Gets the saved value from the localstorage if any is saved, else the default value,
     * and sets the setting to that.
     */
    #setSavedOrDefault(){
        let saved = localStorage.getItem(this.cssProperty);

        if(saved === undefined)
            saved = this.defaultValue;

        this.#setInputToValue(saved);
        this.set(saved);
    }

    /**
     * Sets the input value to the value given in the parameter.
     * @param value the value to set the input field.
     */
    #setInputToValue(value){
        this.valueInputElement.value = value;
    }
}