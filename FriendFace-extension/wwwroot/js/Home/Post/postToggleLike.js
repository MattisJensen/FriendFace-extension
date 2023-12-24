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
    toggleLikeIcon(postId);
    updateLikeCount(postId);
    
    $.ajax({
        type: 'POST',
        url: '/Home/ToggleLikePost',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(postId),
        success: function (success) {
        },
        error: function (error) {
            toggleLikeIcon(postId);
            updateLikeCount(postId);
        }
    });
}

function toggleLikeIcon(postId) {
    var likeButton = $('#likeButton-' + postId);
    likeButton.toggleClass('far fas');
    likeButton.toggleClass('color-inherit color-red');
}

function updateLikeCount(postId) {
    // increase or decrease like count
    var likeButton = $('#likeButton-' + postId);
    var likeCount = $('#likeCount-' + postId);
    var likeCountNumber = parseInt(likeCount.text());
    likeCountNumber = likeButton.hasClass('fas') ? likeCountNumber + 1 : likeCountNumber - 1;
    likeCount.text(likeCountNumber);
}

