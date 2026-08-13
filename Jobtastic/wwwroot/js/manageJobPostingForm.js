const VolumeHours = /** @type {HTMLInputElement} */ (document.getElementById("VolumeHours"));
var FulltimeYes = document.getElementById("FulltimeYes");
var FulltimeNo = document.getElementById("FulltimeNo");

FulltimeYes.addEventListener("click", function () {
    VolumeHours.value = 40;
})
FulltimeNo.addEventListener("click", function () {
    VolumeHours.value = null;
})