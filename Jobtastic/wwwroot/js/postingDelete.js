$(function () {
    const $table = $('#allPostings');
    if (!$table.length) return;

    $table.on('click', '.deletePostingBtn', function () {
        const $row = $(this).closest('tr');
        const id = $row.data('id');
        const title = $row.data('title');

        Swal.fire({
            icon: 'warning',
            title: 'Inserat löschen?',
            text: 'Möchtest du "' + title + '" wirklich endgültig entfernen?',
            showCancelButton: true,
            confirmButtonText: 'Löschen',
            cancelButtonText: 'Abbrechen',
            confirmButtonColor: '#dc3545'
        }).then(function (result) {
            if (!result.isConfirmed) return;
            deletePosting($row, id);
        });
    });

    function deletePosting($row, id) {
        const body = new FormData();
        body.append('__RequestVerificationToken', $('#postingActionForm input[name="__RequestVerificationToken"]').val());
        body.append('id', id);

        fetch('/Posting/DeleteJob', {
            method: 'POST',
            body: body,
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            const data = await response.json().catch(function () { return null; });
            if (response.ok && data && data.success) {
                removeRow($row);
                notifySuccess('Inserat gelöscht.');
                return;
            }
            notifyErrors(data && data.errors ? data.errors : ['Löschen fehlgeschlagen.']);
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
    }

    function removeRow($row) {
        const table = $table.DataTable();
        table.row($row).remove().draw(false);

        // The "no postings yet" message is rendered server-side instead of the table,
        // so the last deletion needs a reload rather than a DOM tweak.
        if (table.rows().count() === 0) {
            window.location.reload();
        }
    }

    function notifySuccess(title) {
        Swal.fire({
            icon: 'success',
            title: title,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 2000
        });
    }

    function notifyErrors(errors) {
        Swal.fire({
            icon: 'error',
            title: 'Bitte prüfen',
            html: '<ul class="text-start mb-0">' +
                errors.map(function (msg) {
                    return '<li>' + $('<div>').text(msg).html() + '</li>';
                }).join('') +
                '</ul>'
        });
    }
});
