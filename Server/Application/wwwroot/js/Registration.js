const pass = document.getElementById("password");
const confpass = document.getElementById("confirm-password");
let message = "";
pass.addEventListener("input", function (event)
{
    if (pass.value != confpass.value) {
        message = "Пароли не совпадают";
    }
    else {
        message = "";
    }
    pass.setCustomValidity(message);
});