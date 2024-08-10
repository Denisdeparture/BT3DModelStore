$('.remove').on('click', function (e) {
    e.preventDefault();
    var $this = $(this);
    var $input = $this.closest('div').find('input');
    var value = parseInt($input.val());
    if (value > 0) {
        value -= 1;
    }

    $input.val(value);

});
$('.add').on('click', function (e) {
    e.preventDefault();
    var $this = $(this);
    var $input = $this.closest('div').find('.op');
    var value = parseInt($input.val());
    value += 1;

    $input.val(value);
});