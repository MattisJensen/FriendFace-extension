document.addEventListener('DOMContentLoaded', function () {
    var postButton = $('#post-btn');

    postButton.on('click', function (event) {
        postCreate();
    });
});

function postCreate() {
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
}

