document.addEventListener('DOMContentLoaded', function () {
    var likeButtons = $('[id^="likeButton-"]');

    // Remove existing listeners to prevent multiple listeners being attached to one field
    likeButtons.off('click');

    // listener for each like field
    likeButtons.each(function () {
        $(this).on('click', function (event) {
            var postId = this.id.split('-')[1];
            toggleLike(postId);
        });
    });
});

function toggleLike(postId) {
    window.location.href = '/Home/ToggleLikePost?postId=' + postId;
}