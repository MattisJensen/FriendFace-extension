document.addEventListener('DOMContentLoaded', function () {
    var followingFeedButton = $('#following-feed-btn');
    followingFeedButton.on('click', function (event) {
        followingFeed();
    });

    var profileFeedButton = $('#profile-feed-btn');
    profileFeedButton.on('click', function (event) {
        profileFeed();
    });
});

function followingFeed() {
    window.location.href = '/Home/Index';
}

function profileFeed() {
    window.location.href = '/Home/IndexWithProfileFeed';
}