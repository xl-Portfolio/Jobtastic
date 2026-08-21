$(function () {
    const $createBtn = $('#createMandateBtn');
    const $addPanel = $('#addMandate');
    const $addForm = $('#addMandateForm');
    const $list = $('#mandatesList');
    const createBtnDefaultHtml = $createBtn.html();

    $createBtn.on('click', function () {
        const isOpening = $addPanel.hasClass('d-none');
        $addPanel.toggleClass('d-none');
        $createBtn.html(isOpening
            ? '<i class="bi bi-x-lg"></i> Abbrechen'
            : createBtnDefaultHtml);
    });

    $addForm.on('submit', function (e) {
        e.preventDefault();
        postMandate(this, window.location.pathname + '?handler=AddMandate', function (html) {
            $('#noMandatesLabel').remove();
            $list.prepend(html);
            $addForm[0].reset();
            $addPanel.addClass('d-none');
            $createBtn.html(createBtnDefaultHtml);
            notifySuccess('Mandat hinzugefügt.');
        });
    });

    $list.on('click', '.editMandateBtn', function () {
        const id = $(this).data('id');
        $('#displayMandate_' + id).addClass('d-none');
        $('#editMandateForm_' + id).removeClass('d-none');
    });

    $list.on('click', '.cancelEditBtn', function () {
        const id = $(this).data('id');
        $('#editMandateForm_' + id).addClass('d-none');
        $('#displayMandate_' + id).removeClass('d-none');
    });

    $list.on('submit', '.editMandateForm', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        postMandate(this, window.location.pathname + '?handler=EditMandate', function (html) {
            $('#mandateItem_' + id).replaceWith(html);
            notifySuccess('Änderungen gespeichert.');
        });
    });

    function postMandate(formEl, url, onSuccess) {
        fetch(url, {
            method: 'POST',
            body: new FormData(formEl),
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            if (response.ok) {
                onSuccess(await response.text());
            } else {
                const data = await response.json().catch(function () { return null; });
                notifyErrors(data && data.errors ? data.errors : ['Unbekannter Fehler beim Speichern.']);
            }
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
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