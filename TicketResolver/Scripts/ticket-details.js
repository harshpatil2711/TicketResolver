(function () {
    var ticketId = $('#ticketIdVal').val();

    $('#commentFile').on('change', function () {
        $('#clearCommentFile').toggle(!!this.files.length);
    });

    $('#clearCommentFile').click(function () {
        $('#commentFile').val('');
        $(this).hide();
    });

    $('#addCommentBtn').click(function () {
        var text = $('#commentText').val().trim();
        if (!text) { Swal.fire('Validation', 'Please enter a comment.', 'warning'); return; }
        var isInternal = $('#isInternalNote').is(':checked');
        var fileInput = $('#commentFile')[0];
        var file = fileInput && fileInput.files[0];

        var fd = new FormData();
        fd.append('ticketId', ticketId);
        fd.append('commentText', text);
        fd.append('isInternalNote', isInternal);
        fd.append('__RequestVerificationToken', $('input[name="__RequestVerificationToken"]').val());
        if (file) fd.append('file', file);

        $.ajax({
            url: '/Ticket/AddComment',
            type: 'POST',
            data: fd,
            processData: false,
            contentType: false,
            success: function () {
                $('#commentText').val('');
                $('#isInternalNote').prop('checked', false);
                $('#commentFile').val('');
                $('#clearCommentFile').hide();
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

})();