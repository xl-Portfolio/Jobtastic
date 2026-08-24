$(function () {
    const $createBtn = $('#createMandateBtn');
    const $addPanel = $('#addMandate');
    const $addForm = $('#addMandateForm');
    const $list = $('#mandatesList');
    const createBtnDefaultHtml = $createBtn.html();

    const $nameInput = $('#Input_Name');
    const $suggestions = $('#companySuggestions');
    const $existingId = $('#existingCompanyId');
    const $notice = $('#existingCompanyNotice');

    $createBtn.on('click', function () {
        const isOpening = $addPanel.hasClass('d-none');
        $addPanel.toggleClass('d-none');
        $createBtn.html(isOpening
            ? '<i class="bi bi-x-lg"></i> Abbrechen'
            : createBtnDefaultHtml);
    });
    $list.on('submit', '.editMandateForm', function (e) {
        e.preventDefault();
        const id = $(this).data('id');
        postMandate(this, window.location.pathname + '?handler=EditMandate', function (html) {
            $('#mandateItem_' + id).replaceWith(html);
            notifySuccess('Änderungen gespeichert.');
        }, handleEditCompanyConflict);
    });

    $list.on('click', '.editMandateBtn', function () {
        const id = $(this).data('id');
        $('#editMandateForm_' + id).find('[name="ForceCreate"]').val('false');
        $('#displayMandate_' + id).addClass('d-none');
        $('#editMandateForm_' + id).removeClass('d-none');
    });

    $list.on('click', '.cancelEditBtn', function () {
        const id = $(this).data('id');
        $('#editMandateForm_' + id).addClass('d-none');
        $('#displayMandate_' + id).removeClass('d-none');
    });
    $list.on('click', '.deleteMandateBtn', function () {
        const id = $(this).data('id');
        const companyName = $('#mandateItem_' + id).find('label').first().text().trim();

        Swal.fire({
            icon: 'warning',
            title: 'Mandat löschen?',
            text: 'Möchtest du "' + companyName + '" wirklich entfernen?',
            showCancelButton: true,
            confirmButtonText: 'Löschen',
            cancelButtonText: 'Abbrechen',
            confirmButtonColor: '#dc3545'
        }).then(function (result) {
            if (!result.isConfirmed) return;
            deleteMandate(id);
        });
    });
    function deleteMandate(id) {
        const formEl = document.getElementById('deleteMandateForm_' + id);
        fetch(window.location.pathname + '?handler=DeleteMandate', {
            method: 'POST',
            body: new FormData(formEl),
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            const data = await response.json().catch(function () { return null; });
            if (response.ok && data && data.success) {
                $('#mandateItem_' + id).remove();
                if (!$list.children('.mandate-item').length) {
                    $list.append('<label id="noMandatesLabel">kein Mandat hinterlegt</label>');
                }
                notifySuccess('Mandat gelöscht.');
                return;
            }
            notifyErrors(data && data.errors ? data.errors : ['Löschen fehlgeschlagen.']);
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
    }

    $addForm.on('submit', function (e) {
        e.preventDefault();
        postMandate(this, window.location.pathname + '?handler=AddMandate', function (html) {
            $('#noMandatesLabel').remove();
            $list.prepend(html);
            $addForm[0].reset();
            clearExistingSelection(false);
            $('#forceCreate').val('false');
            $addPanel.addClass('d-none');
            $createBtn.html(createBtnDefaultHtml);
            notifySuccess('Mandat hinzugefügt.');
        });
    });

    function postMandate(formEl, url, onSuccess, onConflict) {
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
            if (response.status === 409) {
                const data = await response.json().catch(function () { return null; });
                if (data && data.conflict && data.candidates && data.candidates.length) {
                    (onConflict || handleAddCompanyConflict)(formEl, url, data.candidates, onSuccess);
                    return;
                }
            }
            const data = await response.json().catch(function () { return null; });
            notifyErrors(data && data.errors ? data.errors : ['Unbekannter Fehler beim Speichern.']);
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
    }

    function handleAddCompanyConflict(formEl, url, candidates, onSuccess) {
        const buttonsHtml = candidates.map(function (c) {
            return '<button type="button" class="btn btn-outline-primary btn-sm d-block w-100 mb-2 companyCandidateBtn" data-id="' + c.id + '">' +
                $('<div>').text(c.name).html() + '</button>';
        }).join('');

        Swal.fire({
            icon: 'question',
            title: 'Meinst du eine dieser Firmen?',
            html: '<div class="text-start">' + buttonsHtml + '</div>',
            showCancelButton: true,
            showConfirmButton: false,
            cancelButtonText: 'Nein, neue Firma anlegen',
            didOpen: function () {
                Swal.getHtmlContainer().querySelectorAll('.companyCandidateBtn').forEach(function (btn) {
                    btn.addEventListener('click', function () {
                        $(formEl).find('#existingCompanyId').val(btn.getAttribute('data-id'));
                        Swal.close();
                        postMandate(formEl, url, onSuccess);
                    });
                });
            }
        }).then(function (result) {
            if (result.dismiss === Swal.DismissReason.cancel) {
                $(formEl).find('#forceCreate').val('true');
                postMandate(formEl, url, onSuccess);
            }
        });
    }
    function handleEditCompanyConflict(formEl, url, candidates, onSuccess) {
        const namesHtml = candidates.map(function (c) {
            return '<li>' + $('<div>').text(c.name).html() + '</li>';
        }).join('');

        Swal.fire({
            icon: 'question',
            title: 'Ähnliche Firma vorhanden',
            html: 'Es existiert bereits mindestens eine ähnlich benannte Firma:' +
                '<ul class="text-start">' + namesHtml + '</ul>' +
                'Trotzdem unter diesem Namen speichern?',
            showCancelButton: true,
            confirmButtonText: 'Trotzdem speichern',
            cancelButtonText: 'Abbrechen'
        }).then(function (result) {
            if (!result.isConfirmed) return;
            $(formEl).find('[name="ForceCreate"]').val('true');
            postMandate(formEl, url, onSuccess, handleEditCompanyConflict);
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
    let searchTimeout;
    $nameInput.on('input', function () {
        clearExistingSelection(false);
        const term = $(this).val().trim();
        clearTimeout(searchTimeout);
        if (term.length < 2) {
            $suggestions.addClass('d-none').empty();
            return;
        }
        searchTimeout = setTimeout(function () {
            fetch(window.location.pathname + '?handler=SearchCompanies&term=' + encodeURIComponent(term), {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).then(function (r) { return r.json(); })
                .then(function (data) { renderSuggestions(data.companies || []); })
                .catch(function () { /* Vorschläge sind optional, still scheitern */ });
        }, 300);
    });

    function renderSuggestions(companies) {
        if (!companies.length) {
            $suggestions.addClass('d-none').empty();
            return;
        }
        $suggestions.html(companies.map(function (c) {
            const name = $('<div>').text(c.name).html();
            return '<button type="button" class="list-group-item list-group-item-action companySuggestionItem" ' +
                'data-id="' + c.id + '" data-name="' + name + '" ' +
                'data-logo="' + $('<div>').text(c.logoImageSource || '').html() + '" ' +
                'data-website="' + $('<div>').text(c.websiteURL || '').html() + '" ' +
                'data-description="' + $('<div>').text(c.description || '').html() + '">' +
                name + '</button>';
        }).join(''));
        $suggestions.removeClass('d-none');
    }

    $suggestions.on('click', '.companySuggestionItem', function () {
        const $btn = $(this);
        selectExistingCompany({
            id: $btn.data('id'), name: $btn.data('name'),
            logo: $btn.data('logo'), website: $btn.data('website'),
            description: $btn.data('description')
        });
    });

    function selectExistingCompany(c) {
        $existingId.val(c.id);
        $nameInput.val(c.name).prop('readonly', true);
        $addForm.find('[name="Input.LogoImageSource"]').val(c.logo).prop('readonly', true);
        $addForm.find('[name="Input.WebsiteURL"]').val(c.website).prop('readonly', true);
        $addForm.find('[name="Input.Description"]').val(c.description).prop('readonly', true);
        $suggestions.addClass('d-none').empty();
        $notice.removeClass('d-none');
    }

    function clearExistingSelection(clearFields) {
        $existingId.val('');
        $notice.addClass('d-none');
        $nameInput.prop('readonly', false);
        $addForm.find('[name="Input.LogoImageSource"], [name="Input.WebsiteURL"], [name="Input.Description"]').prop('readonly', false);
        if (clearFields) {
            $nameInput.val('');
            $addForm.find('[name="Input.LogoImageSource"], [name="Input.WebsiteURL"], [name="Input.Description"]').val('');
        }
    }

    $notice.on('click', '#resetExistingCompany', function (e) {
        e.preventDefault();
        clearExistingSelection(true);
        $nameInput.trigger('focus');
    });
});