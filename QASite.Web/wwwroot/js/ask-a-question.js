$(() => {
    const counter = 1
    $('#tags').keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            const newTag = $(this).val();
            $(this).append(`<span class="badge badge-primary">${newTag}</span>`)
        }
})
