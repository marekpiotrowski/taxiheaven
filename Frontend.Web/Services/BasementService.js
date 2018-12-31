taxiHeaven.service('basementService', function () {
    this.get = function() {
        return $.ajax({
            type: 'GET',
            url: 'http://localhost:51529/api/Basement',
            contentType: "application/json"
        });
    };

    this.add = function(model) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/Basement',
            contentType: "application/json",
            data: JSON.stringify(model),
            headers: {
                'Authorization': 'Basic ' + btoa(sessionStorage.getItem('login') + ':' + sessionStorage.getItem('password'))
            }
        });
    }
});