(function () {
    $('.edit-category').click(function () {
        $('#editCategoryId').val($(this).data('id'));
        $('#editCategoryName').val($(this).data('name'));
        new bootstrap.Modal($('#editCategoryModal')).show();
    });
    $('.edit-priority').click(function () {
        $('#editPriorityId').val($(this).data('id'));
        $('#editPriorityName').val($(this).data('name'));
        $('#editPrioritySequence').val($(this).data('sequence'));
        new bootstrap.Modal($('#editPriorityModal')).show();
    });
    $('.edit-status').click(function () {
        $('#editStatusId').val($(this).data('id'));
        $('#editStatusName').val($(this).data('name'));
        $('#editIsTerminal').prop('checked', $(this).data('terminal') === 'true');
        new bootstrap.Modal($('#editStatusModal')).show();
    });

    $('.delete-category, .delete-priority, .delete-status').click(function () {
        var btn = $(this);
        var isCategory = btn.hasClass('delete-category');
        var isPriority = btn.hasClass('delete-priority');
        var url = isCategory ? '/MasterData/DeleteCategory' : isPriority ? '/MasterData/DeletePriority' : '/MasterData/DeleteStatus';
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
            $.post(url, { id: id, __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() })
                .done(function () { location.reload(); })
                .fail(function () { Swal.fire('Error', 'Delete failed.', 'error'); });
        });
    });
})();
