
// get hubs
//function getServerHubs() {
//    var xmlHttp = new XMLHttpRequest();
//    xmlHttp.open("GET", "/swaggergethubs", false); // false for synchronous request
//    xmlHttp.send(null);
//    let response = xmlHttp.responseText;
//    return JSON.parse(response);
//}


//const targetNode = document.querySelector('div.swagger-ui');
//let sections = sectionParent.querySelectorAll('.opblock-tag-section');



const scriptTagsToLoad = [
    "https://code.jquery.com/jquery-3.6.1.min.js",
    "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.js",
    "/SignalRSwaggerExtension.js",
]

const cssToLoad = ["/swagger-hubs.css"]

for (const scriptLink of scriptTagsToLoad) {
    let scriptTag = document.createElement('script');
    scriptTag.defer = true;
    scriptTag.src = scriptLink;

    document.head.appendChild(scriptTag);
}

for (const cssLink of cssToLoad) {
    let linkTag = document.createElement('link');
    linkTag.rel = "stylesheet";
    linkTag.href = cssLink;

    document.head.appendChild(linkTag);
}