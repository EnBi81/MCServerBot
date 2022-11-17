
function log(text) {
    console.log(text);
}

function startMutatorObserver(targetNode, addedListener, removedListener) {
    // Options for the observer (which mutations to observe)
    const config = { childList: true };

    // Callback function to execute when mutations are observed
    const callback = (mutationList, observer) => {
        for (const mutation of mutationList) {

            for (const mutationNode of mutation.addedNodes) {
                addedListener(mutationNode);
            }
            for (const removedNode of mutation.removedNodes) {
                removedListener(removedNode)
            }
        }
    };

    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    log("start observing elements for node")
    log(targetNode)

    // Start observing the target node for configured mutations
    observer.observe(targetNode, config);


    return observer
}

// get hubs
function getServerHubs() {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "/swaggergethubs", false); // false for synchronous request
    xmlHttp.send(null);
    let response = xmlHttp.responseText;
    return JSON.parse(response);
}

const hubs = getServerHubs();

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/serverpark")
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.on("Receive", (message) => {
    console.log("Message: " + message)
});

// Start the connection.
start();

// main
//const interval = setInterval(() => {
//    if (document.querySelector('div.swagger-ui') == null)
//        return;

//    log("swagger gui element found!");

//    try {
//        startObserve();
//    }
//    catch (e) {
//    }
    
//    clearInterval(interval);

//}, 10);

// general observer for checking if new sections are added or removed
function startObserve() {
    // Select the node that will be observed for mutations
    const targetNode = document.querySelector('div.swagger-ui');

    startMutatorObserver(
        targetNode,
        (mutationNode) => checkNewlyAddedSection(mutationNode),
        (mutationNode) => checkNewlyAddedSection(mutationNode));
}

// store the objects related to hubs (title, opblocks, mutationObserver)
const sectionObjects = {};

// check if a section is removed
function checkRemovedSection(sectionParent) {
    let sections = sectionParent.querySelectorAll('.opblock-tag-section');
    for (const section of sections) {
        let title = section.querySelector('h3 > a > span').textContent;
        if (sectionObjects[title] != null) {
            log("disconnecting observer for " + title)

            let obj = sectionObjects[title];
            obj.mutationObserver.disconnect();
        }
    }
}

// do work with newly added section
function checkNewlyAddedSection(sectionParent) {
    let sections = sectionParent.querySelectorAll('.opblock-tag-section');

    log("sections added: ")
    log(sections)

    for (const section of sections) {
        let title = section.querySelector('h3 > a > span').textContent;

        if (!hubs.includes(title))
            continue;

        let opblocks = section.querySelectorAll('div.opblock');
        log("hubs: ");
        log(opblocks)

        for (const opblock of opblocks) {
            opblock.setAttribute("data-opblock-hub", "");
            let methodSpan = opblock.querySelector('span.opblock-summary-method');
            let methodType = methodSpan.textContent;

            let pathSpan = opblock.querySelector('.opblock-summary-path');
            let path = pathSpan.getAttribute('data-path');

            let isListener = methodType == "GET";

            if (methodType == "POST") {
                log("Send endpoint:")
                log(opblock)

                methodSpan.textContent = "SEND";
                opblock.setAttribute("data-opblock-hub", "send")
            }
            else if (methodType == "GET") {
                log("Listener endpoint:")
                log(opblock)

                methodSpan.textContent = "LISTEN"
                opblock.setAttribute("data-opblock-hub", "listen")
            }
            else return;

            let observer = startMutatorObserver(
                opblock,
                (mutationNode) => {
                    if (mutationNode.tagName.toLowerCase() === "noscript") {
                        log("closed " + path)
                        hubPathRetracted(title, path);
                    }
                    else {
                        log("opblock: ");
                        log(opblock.outerHTML);
                        hubPathExtended(title, path, opblock, isListener);
                    }
                },
                () => { }
            )

           

            let sectionObj = {
                "title": title,
                "opblocks": opblocks,
                "mutationObserver": observer
            };

            sectionObjects[title] = sectionObj;
        }
    }
}

const hubPathConnections = {};

function closeHubConnection(path) {
    log("closing hub connection for " + path)
    if (hubPathConnections[path] != null) {
        hubPathConnections[path].close();
        delete hubPathConnections[path];
    }
}

function hubPathRetracted(title, path) {
    // TODO: the endpoint is closed, so close the hub connection
    closeHubConnection(path)
}

let opblockG;

function hubPathExtended(title, path, opblock, isListener) {
    // TODO: add event listeners

    // remove all listeners:
    //  box.replaceWith(box.cloneNode(true));
    

    let buttonWrapper = opblock.querySelector('.execute-wrapper');
    if (buttonWrapper == null)
        buttonWrapper = opblock.querySelector('.btn-group')

    let potentialButton = buttonWrapper.querySelector('.execute');

    if (potentialButton != null)
        addExecuteButtonListeners(opblock, buttonWrapper, potentialButton, isListener);

    startMutatorObserver(buttonWrapper,
        (mutationNode) => {
            if (mutationNode.classList.contains('execute')) {
                addExecuteButtonListeners(opblock, buttonWrapper, mutationNode, isListener)
            }
            else {

            }
        },
        (mutationNode) => {
            if (mutationNode.classList.contains('execute')) {
                buttonWrapper.innerHTML = '';
                closeHubConnection(path)
            }
        });
    
}

function addExecuteButtonListeners(opblock, buttonWrapper, button, isListener) {

    if (button.classList.contains("cloned"))
        return;

    let cloneButton = button.cloneNode(true);
    cloneButton.classList.add("cloned") // the normal button will be hidden
    buttonWrapper.appendChild(cloneButton);


    if (isListener)
        setupListener(cloneButton, opblock);
    else setupSender(cloneButton, opblock);
}

function setupListener(button, opblock) {
    button.onclick = () => {
        let buttonWrapper = opblock.querySelector(".execute-wrapper")

        if (buttonWrapper == null) {
            return;
        }

        let clearButton = document.createElement("button");
        clearButton.setAttribute("class", "btn btn-clear opblock-control__btn");
        clearButton.textContent = "Clear";

        buttonWrapper.appendChild(clearButton);
        buttonWrapper.classList.add("btn-group");
        buttonWrapper.classList.remove("execute-wrapper")

        let responseBlock = opblock.querySelector('.responses-wrapper');
        responseBlock.innerHTML = listenerHTML;

        let 
    }
}

function setupSender(button, responseWrapper) {

}


const listenerHTML = `<div class="opblock-section-header"> <h4>Responses</h4></div><div class="responses-inner"> <div> <div> <div> <div class="request-url"> <h4>Request URL</h4> <pre class="microlight"> https://localhost:7229/hubs/serverpark/Receive </pre> </div></div><h4>Server response</h4> <table class="responses-table live-responses-table"> <thead> <tr class="responses-header"> <td class="col_header response-col_status"> Code </td><td class="col_header response-col_description"> Details </td></tr></thead> <tbody> <tr class="response"> <td class="response-col_status"> 404 <div class="response-undocumented"> <i>Undocumented</i> </div></td><td class="response-col_description"> <div> <h5>Getting continous response... to stop, press 'Clear'</h5> <pre class="microlight"></pre> </div></td></tr></tbody> </table> </div></div></div>`;
/*
 * This should be when retracted
 * 
 * <div class="no-margin"> <div class="opblock-body"><div class="opblock-section"><div class="opblock-section-header"><div class="tab-header"><div class="tab-item active"><h4 class="opblock-title"><span>Parameters</span></h4></div></div><div class="try-out"><button class="btn try-out__btn cancel">Cancel</button></div></div><div class="parameters-container"><div class="opblock-description-wrapper"><p>No parameters</p></div></div></div><div class="execute-wrapper"><button class="btn execute opblock-control__btn">Execute</button></div><div class="responses-wrapper"><div class="opblock-section-header"><h4>Responses</h4></div><div class="responses-inner"></div></div></div> </div>
 * /

/*
 * This should be when program is running
 * 
 <div class="no-margin"> 
  <div class="opblock-body">
   <div class="opblock-section">
    <div class="opblock-section-header">
     <div class="tab-header">
      <div class="tab-item active">
       <h4 class="opblock-title">
        <span>Parameters </span>
       </h4>
      </div>
     </div>
     <div class="try-out">
      <button class="btn try-out__btn cancel">
       Cancel
      </button>
     </div>
    </div>
    <div class="parameters-container">
     <div class="opblock-description-wrapper">
      <p>No parameters</p>
     </div>
    </div>
   </div>
   <div class="btn-group">
    <button class="btn execute opblock-control__btn">
     Execute
    </button>
    <button class="btn btn-clear opblock-control__btn">
     Clear
    </button>
   </div>
   <div class="responses-wrapper">
    <div class="opblock-section-header">
     <h4>Responses</h4>
    </div>
    <div class="responses-inner">
     <div>
      <div>
       <div>
        <div class="request-url">
         <h4>Request URL</h4>
          <pre class="microlight">
           https://localhost:7229/hubs/serverpark/Receive
          </pre>
         </div>
        </div>
       <h4>Server response</h4>
       <table class="responses-table live-responses-table">
        <thead>
         <tr class="responses-header">
          <td class="col_header response-col_status">
           Code
          </td>
          <td class="col_header response-col_description">
           Details
          </td>
         </tr>
        </thead>
        <tbody>
         <tr class="response">
          <td class="response-col_status">
           404
           <div class="response-undocumented">
            <i>Undocumented</i>
           </div>
          </td>
          <td class="response-col_description">
           <div>
            <h5>Getting continous response... to stop, press 'Clear'</h5>
            <pre class="microlight">
             <span class="headerline">heelo</span>
            </pre>
           </div>
          </td>
         </tr>
        </tbody>
       </table>
      </div>
     </div>
    </div>
   </div>
  </div>
 </div> 
 */
