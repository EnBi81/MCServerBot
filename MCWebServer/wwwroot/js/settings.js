const defaultSettings = {
    font: 'Arial',
    primary:  '#121921',
    secondary: '#311a3c',
    button: '#d97c7c',
    baseOp: 1,
    baseHoverOp: 0.3
};


// open/close view
const settings_wrapper = document.getElementById("settings-wrapper");
const settings = document.getElementById("settings");

function toggleSettings(){
    settings_wrapper.classList.toggle("hidden");
    settings.classList.toggle("hidden");
}

let fontType = defaultSettings.font;
let primaryColor = defaultSettings.primary;
let secondaryColor = defaultSettings.secondary;
let buttonColor = defaultSettings.button;
let baseOpacity = defaultSettings.baseOp;
let baseHoverOpacity = defaultSettings.baseHoverOp;

function editSetting(){
    document.body.style.setProperty('--base-font-family', fontType);
    document.body.style.setProperty('--base-primary-color', primaryColor);
    document.body.style.setProperty('--base-secondary-color', secondaryColor);
    document.body.style.setProperty('--base-logout-button-color', buttonColor);
    document.body.style.setProperty('--base-opacity', baseOpacity);
    document.body.style.setProperty('--base-hover-opacity', baseHoverOpacity);

    saveSettings()
}



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

const fontStyleDropdown = document.getElementById('fonts');
for (const font of fonts) {
    let option = document.createElement("option")
    option.textContent = font;
    option.setAttribute("value", font)

    fontStyleDropdown.appendChild(option)
}

const primaryColorPicker = document.getElementById("primary-color-picker");
const secondaryColorPicker = document.getElementById("secondary-color-picker");
const buttonColorPicker = document.getElementById("button-color-picker");
const baseOpSlide = document.getElementById("content-op");
const baseOpHoverSlide = document.getElementById("content-hover-op");
const events = ['change', 'mousemove'];


fontStyleDropdown.addEventListener('mousemove', () => {
    fontType = fontStyleDropdown.value;
    editSetting();
})

primaryColorPicker.addEventListener('change', ()=>{
    primaryColor = primaryColorPicker.value;
    editSetting();
})

secondaryColorPicker.addEventListener('change', ()=>{
    secondaryColor = secondaryColorPicker.value;
    editSetting();
})

buttonColorPicker.addEventListener('change', ()=>{
    buttonColor = buttonColorPicker.value;
    editSetting();
})

events.forEach(evt => {
    baseOpSlide.addEventListener(evt, () => {
        baseOpacity = baseOpSlide.value / 100;
        editSetting();
    })
})

baseOpHoverSlide.addEventListener('change', () => {
    baseHoverOpacity = baseOpHoverSlide.value / 100;
    editSetting();
})



function saveSettings(){
    let settings = [
        fontType,
        primaryColor,
        secondaryColor,
        buttonColor,
        baseOpacity,
        baseHoverOpacity
    ];

    let text = settings.join(',');
    localStorage.setItem("settings", text);
}

(function(){
    function getSettings(){
        let settings = localStorage.getItem("settings")

        if(settings == null)
            return defaultSettings;

        try{
            let settingsArray = settings.split(',');
            return {
                font: settingsArray[0],
                primary: settingsArray[1],
                secondary: settingsArray[2],
                button: settingsArray[3],
                baseOp: settingsArray[4],
                baseHoverOp: settingsArray[5]
            }
        }
        catch (e){}

        return defaultSettings;
    }

    let settings = getSettings();
    fontType = settings.font
    primaryColor = settings.primary;
    secondaryColor = settings.secondary;
    buttonColor = settings.button;
    baseOpacity = settings.baseOp;
    baseHoverOpacity = settings.baseHoverOp;

    fontStyleDropdown.value = fontType;
    primaryColorPicker.value = primaryColor;
    secondaryColorPicker.value = secondaryColor;
    buttonColorPicker.value = buttonColor;
    baseOpSlide.value = baseOpacity * 100;
    baseOpHoverSlide.value = baseHoverOpacity * 100;

    editSetting();
})();


// reset buttons
(function(){
    const fullReset = document.getElementById('settings-title');
    const fontReset = document.getElementById('reset-font');
    const primaryReset = document.getElementById('reset-primary');
    const secondaryReset = document.getElementById('reset-secondary');
    const buttonColorReset = document.getElementById('reset-button-color');
    const opacityReset = document.getElementById('reset-base-op');
    const opacityHoverReset = document.getElementById('reset-hover-op');

    fullReset.addEventListener('dblclick', () =>{
        fontType = defaultSettings.font;
        primaryColor = defaultSettings.primary;
        secondaryColor = defaultSettings.secondary;
        buttonColor = defaultSettings.button;
        baseOpacity = defaultSettings.baseOp;
        baseHoverOpacity = defaultSettings.baseHoverOp;

        fontStyleDropdown.value = fontType;
        primaryColorPicker.value = primaryColor;
        secondaryColorPicker.value = secondaryColor;
        buttonColorPicker.value = buttonColor;
        baseOpSlide.value = baseOpacity * 100;
        baseOpHoverSlide.value = baseHoverOpacity * 100;

        editSetting();
    })
    fontReset.addEventListener('dblclick', ()=>{
        fontType = defaultSettings.font;
        fontStyleDropdown.value = fontType;
        editSetting();
    })
    primaryReset.addEventListener('dblclick', ()=>{
        primaryColor = defaultSettings.primary;
        primaryColorPicker.value = primaryColor;
        editSetting();
    })
    secondaryReset.addEventListener('dblclick', ()=>{
        secondaryColor = defaultSettings.secondary;
        secondaryColorPicker.value = secondaryColor;
        editSetting();
    })
    buttonColorReset.addEventListener('dblclick', ()=>{
        buttonColor = defaultSettings.button;
        buttonColorPicker.value = buttonColor;
        editSetting();
    })
    opacityReset.addEventListener('dblclick', ()=>{
        baseOpacity = defaultSettings.baseOp;
        baseOpSlide.value = baseOpacity * 100;
        editSetting();
    })
    opacityHoverReset.addEventListener('dblclick', ()=>{
        baseHoverOpacity = defaultSettings.baseHoverOp;
        baseOpHoverSlide.value = baseHoverOpacity * 100;
        editSetting();
    })
})();
