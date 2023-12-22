document.addEventListener('DOMContentLoaded', function () {
    attachEditFieldListeners();
});

function attachEditFieldListeners() {
    var followButtons = $('[id^="followButton-"]');
    var unfollowButtons = $('[id^="unfollowButton-"]');
    var userSearchContainers = $('[id^="userSearchContainer-"]');

    // Remove existing listeners
    followButtons.off('click');
    unfollowButtons.off('click');
    userSearchContainers.off('click');

    // Attach listener to each follow button
    followButtons.each(function () {
        $(this).on('click', function (event) {
            event.stopPropagation(); // Prevent event from bubbling up
            var userId = this.id.split('-')[1];
            toggleFollow("follow", userId);
        });
    });

    // Attach listener to each unfollow button
    unfollowButtons.each(function () {
        $(this).on('click', function (event) {
            event.stopPropagation(); // Prevent event from bubbling up
            var userId = this.id.split('-')[1];
            toggleFollow("unfollow", userId);
        });
    });

    // Attach listener to each user search container
    userSearchContainers.each(function () {
        $(this).on('click', function (event) {
            var userId = this.id.split('-')[1];
            openProfile(userId);
        });
    });
}

function toggleFollow(action, userId) {
    var url = '';
    if (action === "follow") {
        url = '/User/FollowUser?userIdToFollow=' + userId;
    } else if (action === "unfollow") {
        url = '/User/UnfollowUser?userIdToUnfollow=' + userId;
    }
    window.location.href = url;
}

function openProfile(userId) {
    window.location.href = '/User/Profile/' + userId;
}
