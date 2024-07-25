const pass = document.getElementById("password");
const confpass = document.getElementById("confirm-password");
const sub = document.getElementById("sub");
sub.addEventListener("input", function (event)
{
    if (pass.value != confpass.value) {
        pass.setCustomValidity("Пароли не совпадают");
        event.preventDefault();
    }
   
});