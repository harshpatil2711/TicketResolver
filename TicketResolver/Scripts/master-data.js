(function () {
    $('.edit-category').click(function () {
        $('#editCategoryId').val($(this).data('id'));
        $('#editCategoryName').val($(this).data('name'));
        new bootstrap.Modal($('#editCategoryModal')).show();
    });

    $('.delete-category').click(function () {
        var btn = $(this);
        var id = btn.data('id');
        Swal.fire({
            title: 'Confirm Delete',
            text: 'Are you sure?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: 'Delete'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            $.post('/MasterData/DeleteCategory', { id: id, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() })
                .done(function () { location.reload(); })
                .fail(function () { Swal.fire('Error', 'Delete failed.', 'error'); });
        });
    });
})();
