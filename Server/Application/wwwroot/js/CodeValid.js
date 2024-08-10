const pass = document.getElementById("env");
const confpass = document.getElementById("code");

code.addEventListener("input", function (event) {
    if (pass.value != confpass.value) {
        pass.setCustomValidity("Неверный код");
        event.preventDefault();
    }

});