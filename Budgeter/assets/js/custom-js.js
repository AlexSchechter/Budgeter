$(".edit-profile-button").click(function () {
    $('html, body').animate({
        scrollTop: $("#edit-profile-section").offset().top - 60
    }, 200);
})

$(function () {
    $('#container').highcharts({
        data: {
            table: 'datatable'
        },
        chart: {
            type: 'column'
        },
        title: {
           text: null
        },
        yAxis: {
            allowDecimals: false,
            title: {
                text: '$'
            }
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.series.name + '</b><br/>' +
                    this.point.y + ' ' + this.point.name.toLowerCase();
            }
        }
    });
});