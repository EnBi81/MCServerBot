const scriptTagsToLoad = [
    "https://code.jquery.com/jquery-3.6.1.min.js",
    "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.js",
    "/swagger/extensions/SignalRSwaggerExtension",
]

for (const scriptLink of scriptTagsToLoad) {
    let scriptTag = document.createElement('script');
    scriptTag.defer = true;
    scriptTag.src = scriptLink;

    document.head.appendChild(scriptTag);
}

let css = ".opblock[data-opblock-hub=send]{--strong-color:#80007e;--transparent-color:rgba(254,97,248,.1)}.opblock[data-opblock-hub=listen]{--strong-color:#ffd600;--transparent-color:rgba(255,240,0,.1)}.opblock[data-opblock-hub]{border-color:var(--strong-color)!important;background:var(--transparent-color)!important}.opblock[data-opblock-hub] .opblock-summary-method,.opblock[data-opblock-hub] span::after{background:var(--strong-color)!important}.opblock[data-opblock-hub] .execute:not(.cloned){display:none}";
let cssElement = document.createElement('style')
cssElement.innerHTML = css;

document.head.appendChild(cssElement);