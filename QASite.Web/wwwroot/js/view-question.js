$(() => {

    $("#like-button").on('click', function () {
        const questionId = $("#question-id").val();
        console.log("hi")
        $.post(`addLike`, { questionId }, function () {
            updateLikes();
            $("#like-button").prop('disabled', true);
        });          
    });

    function updateLikes() {
        const questionId = $("#question-id").val();
        $.get(`getLikes`, { questionId }, function (result) {
            console.log(result);
            $("#likes-count").text(result);
        });
    };

    setInterval(() => {
        updateLikes();
    }, 1000);

});