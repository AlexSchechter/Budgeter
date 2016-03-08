$(".edit-profile-button").click(function (event) {
    event.preventDefault();
    $("#edit-profile-section").show();
    $('html, body').animate({
        scrollTop: $("#edit-profile-section").offset().top - 60
    }, 200);
})


$(".btn-demo").click(function (event) {
    var user = $(this).data('assigned-id');
    if (user == "john@snow.com") {
        event.preventDefault();
        $(".demo-text").show();
    }
});

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