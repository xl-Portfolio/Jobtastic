document.getElementById('postJobBtn').addEventListener('click', function (e) {
    if (this.dataset.hasMandate === 'false') {
        e.preventDefault();
        const mandateUrl = this.dataset.mandateUrl;
        Swal.fire({
            icon: 'warning',
            title: 'Kein Firmenmandat hinterlegt',
            text: 'Legen Sie ein Firmenmandat an, um einen Job inserieren zu können.',
            confirmButtonText: 'Mandat anlegen',
            showCancelButton: true,
            cancelButtonText: 'Abbrechen'
        }).then(function (result) {
            if (result.isConfirmed) {
                window.location.href = mandateUrl;
            }
        });
    }
});