var displayData = document.getElementById("displayData");
var editDataBtn = document.getElementById("editDataBtn");
var editData = document.getElementById("editData");
editDataBtn.addEventListener("click", function () {
    displayData.classList.add("d-none");
    editData.classList.remove("d-none");
});

var cancelDataBtn = document.getElementById("cancelDataBtn");
cancelDataBtn.addEventListener("click", function () {
    editData.classList.add("d-none");
    displayData.classList.remove("d-none");
});

// The password section is only rendered on one's own account - an admin managing
// someone else's cannot supply the current password. Everything below is therefore
// conditional on those elements existing.
var editPasswordBtn = document.getElementById("editPasswordBtn");
var editPassword = document.getElementById("editPassword");

if (editPasswordBtn && editPassword) {
    editPasswordBtn.addEventListener("click", function () {
        editPassword.classList.remove("d-none");
        editPasswordBtn.classList.add("d-none");
    });

    var cancelPasswordBtn = document.getElementById("cancelPasswordBtn");
    cancelPasswordBtn.addEventListener("click", function () {
        editPassword.classList.add("d-none");
        editPasswordBtn.classList.remove("d-none");
    });

    bindVisibilityToggle("passwordInput", "visibilityIcon_Old");
    bindVisibilityToggle("newPasswordInput", "visibilityIcon_New");
    bindVisibilityToggle("confirmPasswordInput", "visibilityIcon_Confirm");
}

function bindVisibilityToggle(inputId, iconId) {
    var input = document.getElementById(inputId);
    var icon = document.getElementById(iconId);

    icon.addEventListener("click", function () {
        if (icon.classList.contains("bi-eye-slash")) {
            icon.classList.replace("bi-eye-slash", "bi-eye");
            input.type = "text";
        } else {
            icon.classList.replace("bi-eye", "bi-eye-slash");
            input.type = "password";
        }
    });
}
