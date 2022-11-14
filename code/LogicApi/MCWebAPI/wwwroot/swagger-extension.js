
function getServerHubs() {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "/swaggergethubs", false); // false for synchronous request
    xmlHttp.send(null);
    let response = xmlHttp.responseText;
    return JSON.parse(response);
}


let interval = setInterval(() => {
    if (document.querySelector('div.swagger-ui') == null)
        return;


    let hubs = getServerHubs();

    startObserve(hubs);
    clearInterval(interval);

}, 10);


function checkNewlyAddedSection(sectionParent, hubs) {
    let sections = sectionParent.querySelectorAll('.opblock-tag-section');

    for (const section of sections) {
        let title = section.querySelector('h3 > a > span').textContent;

        if (!hubs.includes(title))
            continue;

        let opblock = section.querySelector('div.opblock');
        opblock.setAttribute("data-opblock-hub", "")
        let methodSpan = opblock.querySelector('span.opblock-summary-method');
        methodSpan.textContent = "HUB";

        // set up mutatorchecker on the opblock to get the button
        //let executeButton = opblock.querySelector('.btn.execute.opblock-control__btn');
        //executeButton.click()
    }
}

function startObserve(hubs) {
    // Select the node that will be observed for mutations
    const targetNode = document.querySelector('div.swagger-ui');

    // Options for the observer (which mutations to observe)
    const config = { childList: true };

    // Callback function to execute when mutations are observed
    const callback = (mutationList, observer) => {
        for (const mutation of mutationList) {

            for (const mutationNode of mutation.addedNodes) {
                checkNewlyAddedSection(mutationNode, hubs);
            }
        }
    };

    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    // Start observing the target node for configured mutations
    observer.observe(targetNode, config);
}
