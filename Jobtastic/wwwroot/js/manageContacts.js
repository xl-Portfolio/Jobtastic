$(function () {
    const $createBtn = $('#createContactBtn');
    const $addPanel = $('#addContact');
    const $addForm = $('#addContactForm');
    const $list = $('#contactsList');
    const createBtnDefaultHtml = $createBtn.html();

    // Preserves the page's userId (admin editing another account) across handler calls.
    function handlerUrl(handler) {
        const params = new URLSearchParams();
        params.set('handler', handler);
        const userId = new URLSearchParams(window.location.search).get('userId');
        if (userId) params.set('userId', userId);
        return window.location.pathname + '?' + params.toString();
    }

    $createBtn.on('click', function () {
        const isOpening = $addPanel.hasClass('d-none');
        $addPanel.toggleClass('d-none');
        $createBtn.html(isOpening
            ? '<i class="bi bi-x-lg"></i> Abbrechen'
            : createBtnDefaultHtml);
    });

    $addForm.on('submit', function (e) {
        e.preventDefault();
        postContact(this, handlerUrl('AddContact'), function (html) {
            $('#noContactsLabel').remove();
            $list.prepend(html);
            $addForm[0].reset();
            $addPanel.addClass('d-none');
            $createBtn.html(createBtnDefaultHtml);
            notifySuccess('Kontakt hinzugefügt.');
        });
    });

    $list.on('submit', '.editContactForm', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        postContact(this, handlerUrl('EditContact'), function (html) {
            $('#contactItem_' + id).replaceWith(html);
            notifySuccess('Änderungen gespeichert.');
        });
    });

    $list.on('click', '.editContactBtn', function () {
        const id = $(this).data('id');
        $('#displayContact_' + id).addClass('d-none');
        $('#editContactForm_' + id).removeClass('d-none');
    });

    $list.on('click', '.cancelEditBtn', function () {
        const id = $(this).data('id');
        $('#editContactForm_' + id).addClass('d-none');
        $('#displayContact_' + id).removeClass('d-none');
    });

    $list.on('click', '.deleteContactBtn', function () {
        const id = $(this).data('id');
        const contactName = $('#contactItem_' + id).find('[data-role="contact-name"]').first().text().trim();

        Swal.fire({
            icon: 'warning',
            title: 'Kontakt löschen?',
            text: 'Möchtest du "' + contactName + '" wirklich entfernen?',
            showCancelButton: true,
            confirmButtonText: 'Löschen',
            cancelButtonText: 'Abbrechen',
            confirmButtonColor: '#dc3545'
        }).then(function (result) {
            if (!result.isConfirmed) return;
            deleteContact(id);
        });
    });

    function deleteContact(id) {
        const formEl = document.getElementById('deleteContactForm_' + id);
        fetch(handlerUrl('DeleteContact'), {
            method: 'POST',
            body: new FormData(formEl),
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            const data = await response.json().catch(function () { return null; });
            if (response.ok && data && data.success) {
                $('#contactItem_' + id).remove();
                if (!$list.children('.contact-item').length) {
                    $list.append('<label id="noContactsLabel">kein Kontakt hinterlegt</label>');
                }
                notifySuccess('Kontakt gelöscht.');
                return;
            }
            notifyErrors(data && data.errors ? data.errors : ['Löschen fehlgeschlagen.']);
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
    }

    function postContact(formEl, url, onSuccess) {
        fetch(url, {
            method: 'POST',
            body: new FormData(formEl),
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            if (response.ok) {
                onSuccess(await response.text());
                return;
            }
            const data = await response.json().catch(function () { return null; });
            notifyErrors(data && data.errors ? data.errors : ['Unbekannter Fehler beim Speichern.']);
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
    }

    function notifySuccess(title) {
        Swal.fire({ icon: 'success', title: title, toast: true, position: 'top-end', showConfirmButton: false, timer: 2000 });
    }

    function notifyErrors(errors) {
        Swal.fire({
            icon: 'error',
            title: 'Bitte prüfen',
            html: '<ul class="text-start mb-0">' +
                errors.map(function (msg) { return '<li>' + $('<div>').text(msg).html() + '</li>'; }).join('') +
                '</ul>'
        });
    }
});