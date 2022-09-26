/**
 * Store information of all the setting constants.
 * @type {{"primary-color": {eventsToListen: string[], valueInputId: string, defaultValue: string, resetButtonId: string, cssProperty: string}, "logout-color": {eventsToListen: string[], valueInputId: string, defaultValue: string, resetButtonId: string, cssProperty: string}, "secondary-color": {eventsToListen: string[], valueInputId: string, defaultValue: string, resetButtonId: string, cssProperty: string}, "base-hover-opacity": {eventsToListen: string[], valueInputId: string, defaultValue: number, resetButtonId: string, cssProperty: string}, "font-scale": {eventsToListen: string[], valueInputId: string, defaultValue: number, resetButtonId: string, cssProperty: string}, "base-opacity": {eventsToListen: string[], valueInputId: string, defaultValue: number, resetButtonId: string, cssProperty: string}, "font-type": {eventsToListen: string[], valueInputId: string, defaultValue: string, resetButtonId: string, cssProperty: string}}}
 */
const settingSetup = {
    "font-type": {
        defaultValue: 'Arial',
        cssProperty: '--base-font-family',
        valueInputId: 'fonts',
        resetButtonId: 'reset-font',
        eventsToListen: ['change'],
    },
    "font-scale": {
        defaultValue: 1,
        cssProperty: '--base-font-scale',
        valueInputId: 'font-size-input',
        resetButtonId: 'reset-font-size',
        eventsToListen: ['change'],
    },
    "primary-color": {
        defaultValue: '#121921',
        cssProperty: '--base-primary-color',
        valueInputId: 'primary-color-picker',
        resetButtonId: 'reset-primary',
        eventsToListen: ['change'],
    },
    "secondary-color": {
        defaultValue: '#311a3c',
        cssProperty: '--base-secondary-color',
        valueInputId: 'secondary-color-picker',
        resetButtonId: 'reset-secondary',
        eventsToListen: ['change'],
    },
    "logout-color": {
        defaultValue: '#d97c7c',
        cssProperty: '--base-logout-button-color',
        valueInputId: 'button-color-picker',
        resetButtonId: 'reset-button-color',
        eventsToListen: ['change'],
    },
    "base-opacity": {
        defaultValue: 1,
        cssProperty: '--base-opacity',
        valueInputId: 'content-op',
        resetButtonId: 'reset-base-op',
        eventsToListen: ['change', 'mousemove'],
    },
    "base-hover-opacity": {
        defaultValue: 0.3,
        cssProperty: '--base-hover-opacity',
        valueInputId: 'content-hover-op',
        resetButtonId: 'reset-hover-op',
        eventsToListen: ['change'],
    }
};

// load fonts
(function () {
    const fonts = [
        'Andale Mono',
        'Arial',
        'Arial Bold',
        'Arial Italic',
        'Arial Bold Italic',
        'Arial Black',
        'Comic Sans MS',
        'Comic Sans MS Bold',
        'Courier New',
        'Courier New Bold',
        'Courier New Italic',
        'Courier New Bold Italic',
        'Georgia',
        'Georgia Bold',
        'Georgia Italic',
        'Georgia Bold Italic',
        'Impact',
        'Lucida Console',
        'Lucida Sans Unicode',
        'Marlett',
        'Minecraft Mono',
        'Minion Web',
        'Symbol',
        'Times New Roman',
        'Times New Roman Bold',
        'Times New Roman Italic',
        'Times New Roman Bold Italic',
        'Tahoma',
        'Trebuchet MS',
        'Trebuchet MS Bold',
        'Trebuchet MS Italic',
        'Trebuchet MS Bold Italic',
        'Verdana',
        'Verdana Bold',
        'Verdana Italic',
        'Verdana Bold Italic',
        'Webdings'
    ];

    let fontDropdown = document.getElementById('fonts');

    //load every font to the font html box
    for (const font of fonts) {
        let option = document.createElement("option")
        option.textContent = font;
        option.setAttribute("value", font)

        fontDropdown.appendChild(option)
    }
})();

// create an empty SettingsCollection instance.
let settingsCollection = new SettingsCollection('settings-wrapper', 'settings-title');
// load the settings data into the SettingsCollection
settingsCollection.load(settingSetup);

/**
 * Toggles the visibility of the settings panel.
 */
function toggleSettings(){
    settingsCollection.toggle();
}