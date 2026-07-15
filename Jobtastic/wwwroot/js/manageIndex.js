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