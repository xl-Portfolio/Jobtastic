var createMandateBtn = document.getElementById("createMandateBtn");
var addMandate = document.getElementById("addMandate");
createMandateBtn.addEventListener("click", function () {
    addMandate.classList.remove("d-none");
    createMandateBtn.classList.add("d-none");
});
