module.exports = {
    ProcessData: function (data) { },
    RegisterComponents: function (app) {
        app.component("home", {
            templateUrl: templatePath("home"),
            controller: function (data) {
                this.games = data.games;
            }
        });
    },
    RegisterStates: function (stateProvider) {
        stateProvider.state({
            name: "home",
            url: "/",
            component: "home"
        });
    }
}