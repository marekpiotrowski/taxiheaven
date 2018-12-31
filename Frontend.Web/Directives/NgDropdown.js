taxiHeaven.directive('ngDropdown', [function() {

    function link(scope, element, attrs) {
        var source = scope[attrs['source']];
        var scope = scope;

        scope.$watchCollection(attrs['source'],
            function (newValue, oldValue) {
                element.html('');
                newValue.forEach(function(row) {
                    element.append('<option data-value="' + row[attrs['valueProperty']] + '">' + row[attrs['nameProperty']] + '</option>')
                });
            });


        element.change(function () {
            var properties = attrs['ngDropdown'].split('.');
            var valueObject = scope;
            properties.forEach(function (property, i) {
                if (!valueObject[property] && i < properties.length - 1) {
                    valueObject[property] = {};
                }
                if (i < properties.length - 1)
                    valueObject = valueObject[property];
            });
            valueObject[properties[properties.length - 1]] = $(this).find(":selected").attr('data-value');
        });
    }

    return {
        link: link
    };
}]);