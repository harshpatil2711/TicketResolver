var pieChart;
var currentGroupBy = 'Status';

function updateButtonState() {
    $('#btnChartStatus').removeClass('btn-dark').addClass('btn-outline-secondary');
    $('#btnChartPriority').removeClass('btn-dark').addClass('btn-outline-secondary');
    if (currentGroupBy === 'Priority') {
        $('#btnChartPriority').removeClass('btn-outline-secondary').addClass('btn-dark');
    } else {
        $('#btnChartStatus').removeClass('btn-outline-secondary').addClass('btn-dark');
    }
}

function loadChart() {
    $.getJSON('/Home/GetChartData', { groupBy: currentGroupBy }, function (data) {
        if (pieChart) pieChart.destroy();
        var container = $('#ticketPieChart').parent();
        $('#chartLegend').empty();
        if (!data || data.error || data.length === 0) {
            container.html('<p class="text-muted pt-5">No data to display.</p>');
            return;
        }
        container.html('<canvas id="ticketPieChart" height="180"></canvas>');
        var colors = ['#dc3545','#ffc107','#28a745','#6c757d','#17a2b8','#fd7e14','#2b3e50'];
        var labels = data.map(function (d) { return d.label; });
        var values = data.map(function (d) { return d.value; });
        var usedColors = data.map(function (d, i) { return colors[i % colors.length]; });

        pieChart = new Chart(document.getElementById('ticketPieChart'), {
            type: 'pie',
            data: { labels: labels, datasets: [{ data: values, backgroundColor: usedColors }] },
            options: {
                plugins: { legend: { display: false } },
                responsive: true, maintainAspectRatio: false
            }
        });

        var legendHtml = '';
        labels.forEach(function (l, i) {
            legendHtml += '<span class="legend-item"><span class="legend-dot" style="background:' + usedColors[i] + ';"></span> ' + l + '</span>';
        });
        $('#chartLegend').html(legendHtml);
    });
}

$(document).ready(function () {
    updateButtonState();
    loadChart();

    $('#btnChartStatus').on('click', function () {
        currentGroupBy = 'Status';
        updateButtonState();
        loadChart();
    });

    $('#btnChartPriority').on('click', function () {
        currentGroupBy = 'Priority';
        updateButtonState();
        loadChart();
    });
});
