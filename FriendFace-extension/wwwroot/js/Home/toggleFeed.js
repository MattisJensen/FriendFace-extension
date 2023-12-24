document.addEventListener('DOMContentLoaded', function () {
    var followingFeedButton = $('#following-feed-btn');
    var profileFeedButton = $('#profile-feed-btn');
    
    // remove existing listeners to prevent multiple listeners being attached to one field
    followingFeedButton.off('click');
    profileFeedButton.off('click');
    
    followingFeedButton.on('click', function (event) {
        toggleFeed('Following');
    });

    profileFeedButton.on('click', function (event) {
        toggleFeed('Profile');
    });
});

function toggleFeed(feedType) {
    var feedContainer = $("#feed");
    feedContainer.empty();
    
    // add loading icon to feed container
    var loading = `
        <div class="d-flex justify-content-center align-items-center mt-4" id="loading">
            <span class="spinner-border text-primary" role="status"></span>
        </div>
    `;
    
    feedContainer.append(loading);
    
    $.ajax({
        type: 'GET',
        url: '/Home/GetFeedPosts?feedType=' + feedType,
        dataType: 'json',
        success: function (success) {
            var loadingIcon = $('#loading');
            loadingIcon.remove();
            
            var htmlString = success.value;
            feedContainer.append(htmlString);

            // Trigger DOMContentLoaded event in all javascript files to attach event listeners to the new elements 
            var event = document.createEvent('Event');
            event.initEvent('DOMContentLoaded', true, true);
            document.dispatchEvent(event);
        },
        error: function (error) {
            var loadingIcon = $('#loading');
            loadingIcon.remove();
        }
    });
}