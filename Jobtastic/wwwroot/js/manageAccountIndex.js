var displayData = document.getElementById("displayData");
var editDataBtn = document.getElementById("editDataBtn");
var editData = document.getElementById("editData");
editDataBtn.addEventListener("click", function () {
    displayData.classList.add("d-none");
    editData.classList.remove("d-none");
});

var editPasswordBtn = document.getElementById("editPasswordBtn");
var editPassword = document.getElementById("editPassword");
editPasswordBtn.addEventListener("click", function () {
    editPassword.classList.remove("d-none");
    editPasswordBtn.classList.add("d-none");
});

var passwordInput = document.getElementById("passwordInput");
var visibilityIconOld = document.getElementById("visibilityIcon_Old");

visibilityIconOld.addEventListener("click", function () {
    if (visibilityIconOld.classList.contains("bi-eye-slash")) {
        visibilityIconOld.classList.replace("bi-eye-slash", "bi-eye");
        passwordInput.type = "text";
    } else {
        visibilityIconOld.classList.replace("bi-eye", "bi-eye-slash");
        passwordInput.type = "password";
    }
    
})

var newPasswordInput = document.getElementById("newPasswordInput");
var visibilityIconNew = document.getElementById("visibilityIcon_New");

visibilityIconNew.addEventListener("click", function () {
    if (visibilityIconNew.classList.contains("bi-eye-slash")) {
        visibilityIconNew.classList.replace("bi-eye-slash", "bi-eye");
        newPasswordInput.type = "text";
    } else {
        visibilityIconNew.classList.replace("bi-eye", "bi-eye-slash");
        newPasswordInput.type = "password";
    }

})

var confirmPasswordInput = document.getElementById("confirmPasswordInput");
var visibilityIconConfirm = document.getElementById("visibilityIcon_Confirm");

visibilityIconConfirm.addEventListener("click", function () {
    if (visibilityIconConfirm.classList.contains("bi-eye-slash")) {
        visibilityIconConfirm.classList.replace("bi-eye-slash", "bi-eye");
        confirmPasswordInput.type = "text";
    } else {
        visibilityIconConfirm.classList.replace("bi-eye", "bi-eye-slash");
        confirmPasswordInput.type = "password";
    }

})