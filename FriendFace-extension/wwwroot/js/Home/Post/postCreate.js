document.addEventListener('DOMContentLoaded', function () {
    var postButton = $('#post-btn');
    
    // Remove existing listeners to prevent multiple listeners being attached to one field
    postButton.off('click');

    postButton.on('click', function (event) {
        postAddInputView();
    });
});

function postAddInputView() {
    var form = $('#postContent-form');
    form.attr('action', "/Home/CreatePost");
    form.attr('method', "post");
    
    $('#postCreateContainer').show();
    $('#post-btn').attr('disabled', true);
    
    let publishButton = $('#postContent-button-publish');
    publishButton.removeAttr('disabled')

    let postContentPublishField = $('#postContent-publishField');
    postContentPublishField.focus();
    postContentPublishField.get(0).readOnly = false;

    let charsInPublishFieldPlaceholder = $('#postContent-chars');
    let charLimit = postCharLimit;

    postContentPublishField.on('input', function () {
        // Update character count text
        charsInPublishFieldPlaceholder.text(postContentPublishField.val().length);

        // Disable publish button if content length exceeds char limit
        publishButton.attr('disabled', postContentPublishField.val().length > charLimit);
    });

    // only add listener once to publish button
    if (!publishButton.data('listener')) {
        publishButton.data('listener', true);
        publishButton.on('click', function () {
            event.preventDefault();
            postCreate();
        });
    }
}

function postCreate() {
    var feedContainer = $("#feed");
    var loading = `
        <div class="d-flex justify-content-center align-items-center mt-4 mb-4 small" id="loading">
            <span class="spinner-border text-primary" role="status"></span>
        </div>
    `;
    
    feedContainer.prepend(loading);
    
    var postContentPublishField = $('#postContent-publishField');
    var content = postContentPublishField.val();
    
    postCancel();
    
    $.ajax({
        type: 'POST',
        url: '/Home/CreatePost',
        contentType: 'application/json',
        data: JSON.stringify(content),
        dataType: 'json',
        success: function (success) {
            var loadingIcon = $('#loading');
            loadingIcon.remove();
            
            // only add post if profile feed button is checked
            var profileFeedButton = $('#profile-feed');
            if (profileFeedButton.prop('checked')) {
                var htmlString = success.value;
                feedContainer.prepend(htmlString)
            }
        },
        error: function (error) {
            var loadingIcon = $('#loading');
            loadingIcon.remove();
        }
    });
}


