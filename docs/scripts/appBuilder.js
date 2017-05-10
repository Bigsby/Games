module.exports = {
    ProcessData: function (data) { },
    RegisterComponents: function (app) {
        app.component("home", {
            templateUrl: templatePath("home"),
            controller: function (data) {
                this.games = data.games;
            }
        });

        app.component("flowGame", {
            templateUrl: templatePath("flowGame"),
            controller: function ($http, $stateParams) {
                var vm = this;
                vm.$params = $stateParams;
                $http.get("data/" + vm.$params.id + ".json").then(function (response) {
                    angular.extend(vm, response.data);
                });
            }
        });

        app.component("flowPack", {
            templateUrl: templatePath("flowPack"),
            controller: function ($http, $stateParams) {
                var vm = this;
                vm.$params = $stateParams;
                $http.get("data/" + vm.$params.id + ".json").then(function (response) {
                    angular.extend(vm, response.data);
                    vm.pack = response.data.packs.find(function (pack) {
                        return pack.name == vm.$params.pack;
                    });
                });
            }
        });

        var pad = "000";

        function padLevel(level) {
            return pad.substring(0, pad.length - level.length) + level;
        };

        function imagePath(vm, level, tag) {
            return "images/"
                + vm.$params.id
                + "/"
                + vm.pack.name
                + "/"
                + vm.section.name
                + "/"
                + padLevel(level.number.toString())
                + tag
                + ".jpg";
        };

        app.component("flowSection", {
            templateUrl: templatePath("flowSection"),
            controller: function ($http, $stateParams) {
                var vm = this;
                vm.$params = $stateParams;

                vm.levelImagePath = function (level) {
                    return imagePath(vm, level, "");
                };

                vm.levelImageInitialPath = function (level) {
                    return imagePath(vm, level, "i");
                };

                $http.get("data/" + vm.$params.id + ".json").then(function (response) {
                    angular.extend(vm, response.data);
                    vm.pack = response.data.packs.find(function (pack) {
                        return pack.name == vm.$params.pack;
                    });

                    if (vm.pack)
                        vm.section = vm.pack.sections.find(function (section) {
                            return section.name == vm.$params.section;
                        });
                });
            }
        });

        app.component("flowLevel", {
            templateUrl: templatePath("flowLevel"),
            controller: function ($http, $stateParams) {
                var vm = this;
                vm.$params = $stateParams;

                vm.levelImagePath = function (level) {
                    return imagePath(vm, level, "");
                };

                vm.levelImageInitialPath = function (level) {
                    return imagePath(vm, level, "i");
                };

                $http.get("data/" + vm.$params.id + ".json").then(function (response) {
                    angular.extend(vm, response.data);
                    vm.pack = response.data.packs.find(function (pack) {
                        return pack.name == vm.$params.pack;
                    });

                    if (vm.pack) {
                        vm.section = vm.pack.sections.find(function (section) {
                            return section.name == vm.$params.section;
                        });

                        if (vm.section) {
                            vm.level = vm.section.levels.find(function (level) {
                                return level.number == vm.$params.level;
                            });

                            if (vm.level){
                                vm.initialImage = imagePath(vm, vm.level, "i");
                                vm.solutionImage = imagePath(vm, vm.level, "");
                            }
                        }
                    }

                });
            }
        });
    },
    RegisterStates: function (stateProvider) {
        stateProvider.state({
            name: "home",
            url: "/",
            component: "home"
        });

        stateProvider.state({
            name: "flowGame",
            url: "/flow/:id",
            component: "flowGame"
        });

        stateProvider.state({
            name: "flowPack",
            url: "/flow/:id/:pack",
            component: "flowPack"
        });

        stateProvider.state({
            name: "flowSection",
            url: "/flow/:id/:pack/:section",
            component: "flowSection"
        });

        stateProvider.state({
            name: "flowLevel",
            url: "/flow/:id/:pack/:section/:level",
            component: "flowLevel"
        });
    }
}