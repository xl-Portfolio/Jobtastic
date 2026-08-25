document.querySelectorAll('.password-field').forEach(function (wrapper) {
    var input = wrapper.querySelector('input');
    var icon = wrapper.querySelector('.eyeIcon');
    if (!input || !icon) return;

    icon.addEventListener('click', function () {
        if (icon.classList.contains('bi-eye-slash')) {
            icon.classList.replace('bi-eye-slash', 'bi-eye');
            input.type = 'text';
        } else {
            icon.classList.replace('bi-eye', 'bi-eye-slash');
            input.type = 'password';
        }
    });
});
