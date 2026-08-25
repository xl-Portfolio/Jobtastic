const idInput = document.getElementById("ID");
if (idInput && idInput.value === "0") {
    const experienceSelect = document.getElementById("Experience");
    const modeSelect = document.getElementById("Mode");
    if (experienceSelect) experienceSelect.value = "";
    if (modeSelect) modeSelect.value = "";
}

const VolumeHours = /** @type {HTMLInputElement} */ (document.getElementById("VolumeHours"));
var FulltimeYes = document.getElementById("FulltimeYes");
var FulltimeNo = document.getElementById("FulltimeNo");

FulltimeYes.addEventListener("click", function () {
    VolumeHours.value = 40;
})
FulltimeNo.addEventListener("click", function () {
    VolumeHours.value = null;
})

const companySelect = document.getElementById("CompanyID");
const contactSelect = document.getElementById("ContactSelect");

if (companySelect && contactSelect) {
    function filterContactsByCompany() {
        const companyId = companySelect.value;
        Array.from(contactSelect.options).forEach(function (option) {
            if (!option.value) return;
            option.hidden = companyId !== "" && option.dataset.company !== companyId;
        });
        const selectedOption = contactSelect.selectedOptions[0];
        if (companyId !== "" && selectedOption && selectedOption.hidden) {
            contactSelect.value = "";
        }
    }

    function fillCompanyFromContact() {
        if (companySelect.value) return;
        const selectedOption = contactSelect.selectedOptions[0];
        const companyId = selectedOption ? selectedOption.dataset.company : "";
        if (companyId) companySelect.value = companyId;
    }

    companySelect.addEventListener("change", filterContactsByCompany);
    contactSelect.addEventListener("change", function () {
        fillCompanyFromContact();
        filterContactsByCompany();
    });

    fillCompanyFromContact();
    filterContactsByCompany();
}