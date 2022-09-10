let serverOnline = new Date();



// logs
const SYSTEM_MESSAGE = 0;
const SYSTEM_ERROR_MESSAGE = 1;
const USER_MESSAGE = 2;

const logBox = document.getElementById("log-messages");
function addLog(message, type){
    let div = document.createElement("div");
    div.textContent = message;

    if(type === 2) {
        div.classList.add("user-message")
    }
    else{
        div.classList.add("system-message");
        if(type === 1)
            div.classList.add("error")
    }

    let scrolledToBottom = logBox.scrollTop === (logBox.scrollHeight - logBox.offsetHeight)
    logBox.appendChild(div);

    if(scrolledToBottom)
        logBox.scrollTo(0, logBox.scrollHeight);
}



//player management
const activePlayers = document.getElementById("active-players")
const playersBox = {} //playername : {onlinefrom, pastuptime, timespan, totaltimespan, fullplayerdiv}
function addPlayer(username, onlineFrom, pastUptime) {
    //let text = "<div class=\"player\" data-username = \"" + username + "\">\n" +
    //    "<pre>" + username + " - <span class=\"time\"></span></pre>\n" +
    //    "<pre class=\"total-time\">Total uptime: <span class='total-time-span'></span></pre>\n" +
    //    "</div>"

    let timeSpan = document.createElement("span");
    timeSpan.classList.add("time");

    let usernamePre = document.createElement("pre");
    usernamePre.textContent = username + " - ";
    usernamePre.appendChild(timeSpan);

    let totalTimeSpan = document.createElement("span");
    totalTimeSpan.classList.add("total-time-span");

    let totalPre = document.createElement("pre");
    totalPre.classList.add("total-time");
    totalPre.textContent = "Total uptime: ";
    totalPre.appendChild(totalTimeSpan);

    let playerDiv = document.createElement("div");
    playerDiv.classList.add("player");
    playerDiv.setAttribute("data-username", username);
    playerDiv.appendChild(usernamePre);
    playerDiv.appendChild(totalPre);



    activePlayers.appendChild(playerDiv);

    //let playerDiv = activePlayers.querySelector(".player[data-username=\"" + username + "\"]");
    //let timeSpan = playerDiv.querySelector(".time");
    //let totalTimeSpan = playerDiv.querySelector(".total-time-span");

    playersBox[username] = {
        onlineFrom: onlineFrom,
        pastUptime: pastUptime,
        timeSpan: timeSpan,
        totalTimeSpan: totalTimeSpan,
        playerDiv: playerDiv
    };
}
function removePlayer(username){
    playersBox[username].playerDiv.remove();
    delete playersBox[username];
}



//set status of the server
//can be: "online", "offline", "starting", "shutting-down"
const serverInfo = document.getElementById("server-info")
function setStatus(status){
    let cl = serverInfo.classList;

    if(status === "starting" || status === "shutting-down"){
        cl.add("loading-colors");
        cl.remove("online-colors");
    }
    else{
        cl.remove("loading-colors");

        if(status === "online")
            cl.add("online-colors");
        else cl.remove("online-colors")
    }
}


//pc data
const cpu = document.getElementById("cpu");
const memory = document.getElementById("memory");
const storage = document.getElementById("storage");

function setCpuAndMemory(cpuData, memoryData){
    cpu.textContent = cpuData;
    memory.textContent = memoryData;
}

function setStorage(storageData) {
    storage.textContent = storageData;
}


//logout
function logoutReceived() {
    $.removeCookie('minecraft-web-login')
    //document.cookie = "minecraft-web-login= ; expires = Thu, 01 Jan 1970 00:00:00 GMT"
    document.documentElement.innerHTML =
        "<head>\n" +
        "    <meta charset=\"UTF-8\">\n" +
        "    <title>Logged out</title>\n" +
        "</head>\n" +
        "<body style=\"background: black; color: white; font-size: 4rem; display: flex;justify-content: center; align-items: center; height: 100vh;\">\n" +
        "You are logged out\n" +
        "</body>\n"
}



