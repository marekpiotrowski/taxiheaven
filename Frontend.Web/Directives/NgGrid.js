taxiHeaven.directive('ngGrid', [function() {

    function link(scope, element, attrs) {
        var html = '<thead>';

        function renderGrid() {
            var data = scope[attrs['source']];
            if (data.length == 0) {
                return;
            }
            var html = '<thead><tr>';
            for (var property in data[0]) {
                if (property == 'Id')
                    property = '#';
                html += '<th>' + property + '</th>';
            }
            html += '</tr></thead><tbody>';
            data.forEach(function (row, i) {
                html += '<tr>';
                for (var property in row) {
                    if (property == 'Id')
                        html += '<th row="' + i + '">' + row[property] + '</th>';
                    else {
                        html += '<td>' + row[property] + '</td>';
                    }
                }
                html += '</tr>';
            });
            html += '</tbody>';
            element.html(html);
        }

        scope.$watchCollection(attrs['source'],
        function (newValue, oldValue) {
            element.html('');
            newValue.forEach(function (row) {
                renderGrid();
            });
        });
    }

    return {
        link: link
    };
}]);