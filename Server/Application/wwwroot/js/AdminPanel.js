$(document).ready(function () {
    $(".view-info div").hide();

    $(".buttons li:first").attr("id", "active");

    $(".view-info div:first").fadeIn();

    $('.button_opt button').click(function (e) {
        e.preventDefault();
        if ($(this).closest("li").attr("id") == "active") {
            return
        } else {
            $(".view-info div").hide();

            $(".buttons li").attr("id", "");

            $(this).parent().attr("id", "active");
            // active le parent du li selectionner

            $('#' + $(this).attr('name')).fadeIn();
            // Montre le texte
        }


    });

});