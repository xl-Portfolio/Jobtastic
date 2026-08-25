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

const companySelect = document.getElementById("CompanySelect");
const contactSelect = document.getElementById("ContactSelect");

if (companySelect && contactSelect) {
    function filterContactsByCompany() {
        const companyId = companySelect.value;
        let selectedStillVisible = false;
        Array.from(contactSelect.options).forEach(function (option) {
            if (!option.value) return;
            const matches = option.dataset.company === companyId;
            option.hidden = !matches;
            if (matches && option.selected) selectedStillVisible = true;
        });
        if (!selectedStillVisible) contactSelect.value = "";
    }

    companySelect.addEventListener("change", filterContactsByCompany);
    filterContactsByCompany();
}