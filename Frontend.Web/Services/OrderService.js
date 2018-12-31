taxiHeaven.service('orderService', function () {
    this.placeAnOrder = function(order) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/Order',
            data: JSON.stringify(order),
            contentType: "application/json"
        });
    };

    this.get = function(id) {
        return $.ajax({
            type: 'GET',
            url: 'http://localhost:51529/api/Order/' + id,
            contentType: "application/json"
        });
    }

    this.confirm = function(id) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/Order/Confirm?orderId=' + id,
            contentType: "application/json"
        });
    }

    this.cancel = function(id) {
        return $.ajax({
            type: 'POST',
            url: 'http://localhost:51529/api/Order/Cancel?orderId=' + id,
            contentType: "application/json"
        });
    }
});