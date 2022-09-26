let userCode = document.body.getAttribute("data-user-code");
document.body.removeAttribute("data-user-code");

const serverPark = new ServerPark(
    `wss://${location.host}/ws?minecraft-web-login=${userCode}`
);

let errorView = new ErrorView(serverPark, "error-content");
const serverInfoPage = new ServerInfoPage(
    serverPark,
    "server-info",
    "server-name",
    "server-address",
    "server-uptime",
    "active-players",
    "cpu",
    "memory",
    "storage",
    "start-button",
    "log-messages",
    "command-input"
)