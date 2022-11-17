
function log(text) {
    console.log(text);
}



function getServerHubs() {
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "/swaggergethubs", false); // false for synchronous request
    xmlHttp.send(null);
    let response = xmlHttp.responseText;
    return JSON.parse(response);
}


const interval = setInterval(() => {
    if (document.querySelector('div.swagger-ui') == null)
        return;

    log("swagger gui element found!");

    try {
        let hubs = getServerHubs();
        startObserve(hubs);
    }
    catch (e) {
    }
    
    clearInterval(interval);

}, 10);

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
            for (const removedNode of mutation.removedNodes) {
                checkRemovedSection(removedNode)
            }
        }
    };

    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    log("start observing elements")

    // Start observing the target node for configured mutations
    observer.observe(targetNode, config);
}

const sectionObjects = {}

function checkNewlyAddedSection(sectionParent, hubs) {
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
            opblock.setAttribute("data-opblock-hub", "")
            let methodSpan = opblock.querySelector('span.opblock-summary-method');
            let methodType = methodSpan.textContent;

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


            const config = { childList: true };

            const callback = (mutationList, observer) => {
                for (const mutation of mutationList) {

                    for (const mutationNode of mutation.addedNodes) {

                    }
                }
            };

            const observer = new MutationObserver(callback);

            observer.observe(opblock, config);

            log("creating observer for " + title)

            let sectionObj = {
                "title": title,
                "opblocks": opblocks,
                "mutationObserver": observer
            };

            sectionObjects[title] = sectionObj;
        }
        

        // set up mutatorchecker on the opblock to get the button
        //let executeButton = opblock.querySelector('.btn.execute.opblock-control__btn');
        //executeButton.click()
    }
}

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
