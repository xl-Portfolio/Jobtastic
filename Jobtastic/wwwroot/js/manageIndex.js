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
    visibilityIconOld.classList.replace("bi-eye-slash", "bi-eye");
    passwordInput.type = "text";
})