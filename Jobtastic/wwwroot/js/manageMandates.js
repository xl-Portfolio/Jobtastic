var createMandateBtn = document.getElementById("createMandateBtn");
var addMandate = document.getElementById("addMandate");
createMandateBtn.addEventListener("click", function () {
    addMandate.classList.remove("d-none");
    createMandateBtn.classList.add("d-none");
});

var allDisplays = document.querySelectorAll(".mandateDisplay")
var allFormDivs = document.querySelectorAll(".mandateForm")
var allEditMandateBtns = document.querySelectorAll(".editMandateBtn")

allEditMandateBtns.forEach(function (btn) {
    btn.addEventListener("click", function () {
        var form = btn.closest("form");
        var mandateForm = form.querySelector(".mandateForm")
        var mandateDisplay = form.querySelector(".mandateDisplay")
        allFormDivs.forEach(function (div) {
            div.classList.add("d-none")
        });
        allDisplays.forEach(function (display) {
            display.classList.remove("d-none")
        });
        mandateDisplay.classList.add("d-none")
        mandateForm.classList.remove("d-none")
    });
});