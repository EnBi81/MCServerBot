//update all the times present on the page
(function(){
    const updateTimeMainBox = document.getElementById("server-uptime");
    function updateTime(){
        function subtractDates(dateFrom, dateTo) {
            function formatNum(num){
                let s = num + "";
                if(s.includes("."))
                    s = s.substr(0, s.indexOf("."))

                return s.length === 1 ? "0" + s : s;
            }

            // res = (h * 3600 + m * 60 + s) * 1000 + ms
            let res = dateTo.getTime() - dateFrom.getTime();

            res /= 1000;
            let s = res % 60;
            res /= 60;
            let m = res % 60;
            let h = res / 60;

            return {
                h: h,
                m: m,
                s: s,
                asString(){
                    return formatNum(this.h) + ":" + formatNum(this.m) + ":" + formatNum(this.s);
                }
            };
        }

        let now = new Date();

        updateTimeMainBox.textContent = subtractDates(serverOnline, now).asString();



        for (const activePlayerKey of Object.keys(playersBox)) {
            let activePlayer = playersBox[activePlayerKey];

            let baseTime = subtractDates(activePlayer.onlineFrom, now);
            activePlayer.timeSpan.textContent = baseTime.asString();

            let playerPast = activePlayer.pastUptime;
            baseTime.h += playerPast.h; 
            baseTime.m += playerPast.m;
            baseTime.s += playerPast.s;

            if (baseTime.s >= 60) {
                baseTime.s %= 60;
                baseTime.m += 1;
            }

            if (baseTime.m >= 60) {
                baseTime.m %= 60;
                baseTime.h += 1;
            }
               

            activePlayer.totalTimeSpan.textContent = baseTime.asString();
        }
    }

    updateTime();
    setInterval(updateTime, 1000)
})();