$(function () {
    var indexActionUrl = $('#userTableContainer').data('url');

    function FetchData(page) {
        var data = {
            searchTerm: $('#searchTerm').val() || '',
            roleId: $('#roleId').val() || '',
            isActive: $('#isActive').val() || '',
            page: page || 1,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        $.ajax({
            url: indexActionUrl,
            type: 'POST',
            data: data,
            success: function (result) {
                $('#userTableContainer').html(result);
            },
            error: function () {
                alert('Error loading users.');
            }
        });
    }

    $(document).on('click', '.page-link[data-page]', function (e) {
        e.preventDefault();
        FetchData($(this).data('page'));
    });

    $('#searchForm').on('submit', function (e) {
        e.preventDefault();
        FetchData(1);
    });

    $('#clearBtn').on('click', function () {
        $('#searchTerm').val('');
        $('#roleId').val('');
        $('#isActive').val('');
        FetchData(1);
    });
});
