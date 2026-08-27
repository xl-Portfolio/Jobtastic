$(function () {
    const $displayData = $('#displayData');
    const $editData = $('#editData');
    const $dataForm = $('#editDataForm');

    $('#editDataBtn').on('click', function () {
        $displayData.addClass('d-none');
        $editData.removeClass('d-none');
    });

    $('#cancelDataBtn').on('click', function () {
        $editData.addClass('d-none');
        $displayData.removeClass('d-none');
    });

    $dataForm.on('submit', function (e) {
        e.preventDefault();
        post(this, function (data) {
            $displayData.find('[data-role="email"]').text(data.email);
            $displayData.find('[data-role="phone"]').text(data.phoneNumber);
            $editData.addClass('d-none');
            $displayData.removeClass('d-none');
            notifySuccess('Änderungen gespeichert.');
        });
    });

    // The password section is only rendered on one's own account - an admin managing
    // someone else's cannot supply the current password. Everything below is therefore
    // conditional on those elements existing.
    const editPasswordBtn = document.getElementById('editPasswordBtn');
    const $editPassword = $('#editPassword');
    const $passwordForm = $('#editPasswordForm');

    if (editPasswordBtn && $editPassword.length) {
        $(editPasswordBtn).on('click', function () {
            $editPassword.removeClass('d-none');
            $(editPasswordBtn).addClass('d-none');
        });

        $('#cancelPasswordBtn').on('click', function () {
            $editPassword.addClass('d-none');
            $(editPasswordBtn).removeClass('d-none');
        });

        $passwordForm.on('submit', function (e) {
            e.preventDefault();
            post(this, function () {
                $passwordForm[0].reset();
                $editPassword.addClass('d-none');
                $(editPasswordBtn).removeClass('d-none');
                notifySuccess('Passwort geändert.');
            });
        });

        bindVisibilityToggle('passwordInput', 'visibilityIcon_Old');
        bindVisibilityToggle('newPasswordInput', 'visibilityIcon_New');
        bindVisibilityToggle('confirmPasswordInput', 'visibilityIcon_Confirm');
    }

    function post(formEl, onSuccess) {
        fetch(formEl.getAttribute('action') || window.location.href, {
            method: 'POST',
            body: new FormData(formEl),
            credentials: 'same-origin',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).then(async function (response) {
            // A non-JSON body means the request was redirected (e.g. to access denied),
            // which must not be mistaken for success.
            const data = await response.json().catch(function () { return null; });
            if (response.ok && data && data.success) {
                onSuccess(data);
                return;
            }
            notifyErrors(data && data.errors ? data.errors : ['Speichern fehlgeschlagen.']);
        }).catch(function () {
            notifyErrors(['Netzwerkfehler. Bitte erneut versuchen.']);
        });
    }

    function bindVisibilityToggle(inputId, iconId) {
        const input = document.getElementById(inputId);
        const icon = document.getElementById(iconId);

        icon.addEventListener('click', function () {
            if (icon.classList.contains('bi-eye-slash')) {
                icon.classList.replace('bi-eye-slash', 'bi-eye');
                input.type = 'text';
            } else {
                icon.classList.replace('bi-eye', 'bi-eye-slash');
                input.type = 'password';
            }
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
