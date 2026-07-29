(function () {
    var ticketId = $('#commentsContainer').length ? $('input[name="__RequestVerificationToken"]').first().closest('.card-footer').length : 0;

    $('#addCommentBtn').click(function () {
        var text = $('#commentText').val().trim();
        if (!text) { Swal.fire('Validation', 'Please enter a comment.', 'warning'); return; }
        $.ajax({
            url: '/Ticket/AddComment',
            type: 'POST',
            data: {
                ticketId: ticketId,
                commentText: text,
                isInternalNote: $('#isInternalNote').is(':checked'),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function () {
                $('#commentText').val('');
                $('#isInternalNote').prop('checked', false);
                Swal.fire({ icon: 'success', title: 'Comment added!', showConfirmButton: false, timer: 1500 });
                location.reload();
            },
            error: function () { Swal.fire('Error', 'Could not add comment.', 'error'); }
        });
    });

    $('.change-status').click(function (e) {
        e.preventDefault();
        var statusId = $(this).data('statusid');
        var tId = $(this).data('ticketid');
        Swal.fire({
            title: 'Change Status',
            input: 'text',
            inputLabel: 'Reason (optional)',
            showCancelButton: true,
            confirmButtonText: 'Change',
            confirmButtonColor: '#2b3e50'
        }).then(function (result) {
            if (result.isConfirmed) {
                $('#statusTicketId').val(tId);
                $('#statusNewStatusId').val(statusId);
                $('#statusChangeReason').val(result.value || '');
                $('#statusForm').submit();
            }
        });
    });

    $('#statusForm').on('submit', function (e) {
        e.preventDefault();
        $.ajax({
            url: '/Ticket/ChangeStatus',
            type: 'POST',
            data: $(this).serialize(),
            success: function () {
                Swal.fire({ icon: 'success', title: 'Status updated!', showConfirmButton: false, timer: 1500 });
                location.reload();
            },
            error: function () { Swal.fire('Error', 'Could not change status.', 'error'); }
        });
    });

    $('#uploadBtn').click(function () {
        var formData = new FormData();
        formData.append('ticketId', ticketId);
        formData.append('file', $('#uploadForm input[type="file"]')[0].files[0]);
        formData.append('__RequestVerificationToken', $('input[name="__RequestVerificationToken"]').val());
        $.ajax({
            url: '/Ticket/UploadAttachment',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function () {
                Swal.fire({ icon: 'success', title: 'File uploaded!', showConfirmButton: false, timer: 1500 });
                location.reload();
            },
            error: function () { Swal.fire('Error', 'Could not upload file.', 'error'); }
        });
    });
})();
