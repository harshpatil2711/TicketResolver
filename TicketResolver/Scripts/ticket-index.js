// Tickets page - Search, Pagination, Sort
$(function () {
    var indexActionUrl = $('#ticketTableContainer').data('url');

    var currentSortColumn    = $('#sortColumn').val() || 'Created';
    var currentSortDirection = $('#sortDirection').val() || 'DESC';
    var windowsize = 5;
    var totalcount = 1;

    // ── Sort Arrow Rendering ──────────────────────────────────────────
    function updateSortArrows() {
        $("th[data-sort]").each(function () {
            $(this).find(".sort-icon").html("&#8597;").css("opacity", "0.35");
        });
        var $active = $("th[data-sort='" + currentSortColumn + "'] .sort-icon");
        if ($active.length) {
            $active
                .html(currentSortDirection === "ASC" ? "&#8593;" : "&#8595;")
                .css("opacity", "1");
        }
    }

    // ── Windowed Page Buttons ─────────────────────────────────────────
    function buttonlist() {
        var $buttons = $("#buttonlist");
        $buttons.empty();
        var pagesize    = parseInt($("#size").val()) || 10;
        var pagescount  = Math.ceil(totalcount / pagesize);
        var currentpage = parseInt($("#page").val()) || 1;
        var start       = Math.floor((currentpage - 1) / windowsize) * windowsize + 1;
        var end         = start + windowsize - 1;
        if (end > pagescount) end = pagescount;

        for (var i = start; i <= end; i++) {
            var isActive = currentpage === i;
            $buttons.append(
                '<button type="button" class="btn btn-sm page-number-btn pageno ' +
                (isActive ? 'btn-dark' : 'btn-outline-secondary') + '" data-page="' + i + '">' + i + '</button>'
            );
        }
        $("#prevbtn").prop("disabled", currentpage <= 1);
        $("#nextbtn").prop("disabled", currentpage >= pagescount || pagescount === 0);
    }

    function updateDatashown() {
        var page  = parseInt($("#page").val()) || 1;
        var size  = parseInt($("#size").val()) || 10;
        var start = ((page - 1) * size) + 1;
        var end   = Number(start) + Number(size) - 1;
        if (end > totalcount) { end = totalcount; }
        if (totalcount === 0) {
            $("#datashown").html("Showing 0 to 0 of 0 entries");
        } else {
            $("#datashown").html('Showing <span class="text-dark fw-bold">' + start + '</span> to <span class="text-dark fw-bold">' + end + '</span> of <span class="text-dark fw-bold">' + totalcount + '</span> entries');
        }
    }

    // ── Main AJAX Fetch ───────────────────────────────────────────────
    function FetchData() {
        var data = {
            searchTerm: $('#searchTerm').val() || '',
            categoryId: $('#categoryId').val() || '',
            priorityId: $('#priorityId').val() || '',
            statusId: $('#statusId').val() || '',
            PageNumber: $('#page').val() || 1,
            PageSize: $('#size').val() || 10,
            sortColumn: currentSortColumn,
            sortDirection: currentSortDirection,
            isUnassigned: $('#isUnassigned').val() || '',
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        };

        $.ajax({
            url: indexActionUrl,
            type: 'POST',
            data: data,
            success: function (result) {
                $('#ticketTableContainer').html(result);

                var tc = $('#ticketcount').val();
                totalcount = tc ? parseInt(tc) || 0 : 0;

                // Sync sort state from hidden fields echoed by the partial view
                var sc = $('#sortColumn').val();
                var sd = $('#sortDirection').val();
                if (sc) currentSortColumn = sc;
                if (sd) currentSortDirection = sd;

                updateSortArrows();
                buttonlist();
                updateDatashown();
            },
            error: function () {
                alert('Error loading tickets.');
            }
        });
    }

    // ── Initial Load ──────────────────────────────────────────────────
    updateSortArrows();
    buttonlist();
    updateDatashown();

    // ── Sort Column Click ─────────────────────────────────────────────
    $(document).on('click', 'th[data-sort]', function () {
        var col = $(this).data('sort');
        if (currentSortColumn === col) {
            currentSortDirection = currentSortDirection === 'ASC' ? 'DESC' : 'ASC';
        } else {
            currentSortColumn    = col;
            currentSortDirection = 'ASC';
        }
        $('#page').val(1);
        FetchData();
    });

    // ── Search Submit ─────────────────────────────────────────────────
    $('#searchForm').on('submit', function (e) {
        e.preventDefault();
        $('#page').val(1);
        FetchData();
    });

    // ── Clear Filters ─────────────────────────────────────────────────
    $('#clearBtn').on('click', function () {
        $('#searchTerm').val('');
        $('#categoryId').val('');
        $('#priorityId').val('');
        $('#statusId').val('');
        $('#isUnassigned').val('');
        $('#page').val(1);
        FetchData();
    });

    // ── Prev / Next ───────────────────────────────────────────────────
    $('#prevbtn').on('click', function () {
        var currentpage = parseInt($('#page').val()) || 1;
        if (currentpage > 1) {
            $('#page').val(currentpage - 1);
            FetchData();
        }
    });

    $('#nextbtn').on('click', function () {
        var currentpage = parseInt($('#page').val()) || 1;
        $('#page').val(currentpage + 1);
        FetchData();
    });

    // ── Page Number Buttons ───────────────────────────────────────────
    $(document).on('click', '.pageno', function () {
        $('#page').val($(this).data('page'));
        FetchData();
    });

    // ── Page Size Change ──────────────────────────────────────────────
    $('#size').on('change', function () {
        $('#page').val(1);
        FetchData();
    });

    // ── Delete Ticket (admin only) ────────────────────────────────────
    $(document).on('click', '.delete-ticket', function () {
        var id = $(this).data('id');
        Swal.fire({
            title: 'Confirm Delete',
            text: 'Are you sure? This will permanently delete the ticket.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            confirmButtonText: 'Delete'
        }).then(function (r) {
            if (!r.isConfirmed) return;
            $.post('/Ticket/Delete', {
                id: id,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            }).done(function (result) {
                if (result && result.success) {
                    FetchData();
                } else {
                    Swal.fire('Error', (result && result.error) || 'Delete failed.', 'error');
                }
            }).fail(function () {
                Swal.fire('Error', 'Delete failed.', 'error');
            });
        });
    });
});
