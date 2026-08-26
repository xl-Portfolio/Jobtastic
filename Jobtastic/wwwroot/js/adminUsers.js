$(function () {
    const $table = $('#allUsers');

    function antiforgeryToken() {
        return $('#adminActionForm input[name="__RequestVerificationToken"]').val();
    }

    function post(url, fields) {
        const body = new FormData();
        body.append('__RequestVerificationToken', antiforgeryToken());
        Object.keys(fields).forEach(function (key) { body.append(key, fields[key]); });

        return fetch(url, {
            method: 'POST',
            body: body,
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            const data = await response.json().catch(function () { return null; });
            if (response.ok && data && data.success) return data;
            notifyErrors(data && data.errors ? data.errors : ['Aktion fehlgeschlagen.']);
            return null;
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
            return null;
        });
    }

    $table.on('click', '.lockToggleBtn', function () {
        const $btn = $(this);
        const $row = $btn.closest('tr');
        const locked = $btn.data('locked') === true || $btn.data('locked') === 'true';

        Swal.fire({
            icon: 'warning',
            title: locked ? 'Konto entsperren?' : 'Konto sperren?',
            text: (locked ? 'Zugang wieder freigeben für "' : 'Anmeldung sperren für "') + $row.data('email') + '"',
            showCancelButton: true,
            confirmButtonText: locked ? 'Entsperren' : 'Sperren',
            cancelButtonText: 'Abbrechen',
            confirmButtonColor: locked ? '#198754' : '#dc3545'
        }).then(function (result) {
            if (!result.isConfirmed) return;

            post('/Admin/SetLocked', { userId: $row.data('userId'), locked: !locked })
                .then(function (data) {
                    if (!data) return;
                    $btn.data('locked', data.locked);
                    $row.find('[data-role="status"]').html(data.locked
                        ? '<span class="badge text-bg-danger">gesperrt</span>'
                        : '<span class="badge text-bg-success">aktiv</span>');
                    notifySuccess(data.locked ? 'Konto gesperrt.' : 'Konto entsperrt.');
                });
        });
    });

    $table.on('click', '.roleToggleBtn', function () {
        const $btn = $(this);
        const $row = $btn.closest('tr');
        const isAdmin = $btn.data('isAdmin') === true || $btn.data('isAdmin') === 'true';

        Swal.fire({
            icon: 'question',
            title: isAdmin ? 'Admin-Rolle entziehen?' : 'Zum Admin ernennen?',
            text: $row.data('email'),
            showCancelButton: true,
            confirmButtonText: isAdmin ? 'Entziehen' : 'Ernennen',
            cancelButtonText: 'Abbrechen'
        }).then(function (result) {
            if (!result.isConfirmed) return;

            post('/Admin/SetAdminRole', { userId: $row.data('userId'), isAdmin: !isAdmin })
                .then(function (data) {
                    if (!data) return;
                    $btn.data('isAdmin', data.isAdmin);
                    // One badge per account, mirroring EffectiveRole on the server.
                    // Owners have no toggle, so only Admin <-> User happens here.
                    $row.find('[data-role="roles"]').html(data.isAdmin
                        ? '<span class="badge text-bg-primary">Admin</span>'
                        : '<span class="badge text-bg-secondary">User</span>');
                    notifySuccess(data.isAdmin ? 'Admin-Rolle vergeben.' : 'Admin-Rolle entzogen.');
                });
        });
    });

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
            title: 'Aktion nicht möglich',
            html: '<ul class="text-start mb-0">' +
                errors.map(function (msg) {
                    return '<li>' + $('<div>').text(msg).html() + '</li>';
                }).join('') +
                '</ul>'
        });
    }
});
