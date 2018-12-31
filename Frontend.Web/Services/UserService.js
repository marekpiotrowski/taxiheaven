taxiHeaven.service('userService', function () {
    this.signIn = function(login, password) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/User/SignIn?login=' + encodeURIComponent(login) +
                '&password=' + encodeURIComponent(password),
            contentType: "application/json"
        });
    };

    this.signUp = function (model) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/User',
            data: JSON.stringify(model),
            contentType: "application/json"
        });
    };

    this.addEmployee = function(asAdmin, asEmployee, guid) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/User/AddEmployee?admin=' + asAdmin + '&employee=' + asEmployee +
                '&guid=' + guid,
            contentType: "application/json",
            headers: {
                'Authorization': 'Basic ' + btoa(sessionStorage.getItem('login') + ':' + sessionStorage.getItem('password'))
            }
        });
    }

    this.addDriver = function (model) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/Driver',
            contentType:  'application/json; charset=utf-8',
            headers: {
                'Authorization': 'Basic ' + btoa(sessionStorage.getItem('login') + ':' + sessionStorage.getItem('password'))
            },
            data: JSON.stringify(model)
        });
    }

    this.get = function() {
        return $.ajax({
            type: 'GET',
            url: 'http://localhost:51529/api/Driver',
            contentType: 'application/json; charset=utf-8'
        });
    }
});