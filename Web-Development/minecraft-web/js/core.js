let userCode = document.body.getAttribute("data-user-code");
document.body.removeAttribute("data-user-code");

const serverPark = new ServerPark(
    `wss://${location.host}/ws?minecraft-web-login=${userCode}`
);

let notificationView = new NotificationView(serverPark, "notification-content");

let serverOptionsSetupData = [
    {
        singleServerOptionId: "rename-server-option",
        listeners: [
            { "keypress": e => e.key === "Enter" }
        ],
        onProceed: inputElement => serverInfoPage.getSelectedServer().renameServer(inputElement.value),
    },
    {
        singleServerOptionId: "delete-server-option",
        listeners: [
            { "click": () => true }
        ],
        onProceed: () => serverInfoPage.getSelectedServer().deleteServer(),
    }
]

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
    "command-input",
    'server-name-wrapper',
    'server-options',
    serverOptionsSetupData
);

const serverSelector = new ServerSelectors(serverPark, serverInfoPage);