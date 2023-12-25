document.addEventListener('DOMContentLoaded', function () {
    var commentButtons = $('[id^="commentButton-"]');

    // Remove existing listeners to prevent multiple listeners being attached to one field
    commentButtons.off('click');

    // Attach listener to each delete field
    commentButtons.each(function () {
        $(this).on('click', function (event) {
            var postId = this.id.split('-')[1];
            addCommentSection(postId);
        });
    });
});

function addCommentSection(postId) {
    // Comment Button hides comments for this post
    var commentButton = $('#commentButton-' + postId);
    commentButton.off('click');
    commentButton.on('click', function (event) {
        removeCommentsContainer(postId, commentButton);
    });
    
    // add comment input field and buttons
    var charLimit = postCharLimit;
    var formHtml = `
        <form id="comment-form-${postId}">
            <input class="form-control-plaintext shadow-none input-field mt-3" id="comment-field-${postId}" type="text" name="Content" placeholder="write your comment...">

            <div class="form-text" id="comment-charcounter-container-${postId}">
                <p><span id="comment-chars-${postId}">0</span>/${charLimit}</p>
            </div>

            <button class="btn btn-success btn-sm mb-4 me-2" id="comment-button-save-${postId}" type="submit" disabled>
                <i class="fas fa-arrow-up"></i> Publish Comment
            </button>
            <button class="btn btn-secondary btn-sm mb-4" id="comment-button-cancel-${postId}" type="button" disabled>Cancel</button>
        </form>
    `;
    
    var commentContainer = `
        <div id="comments-${postId}"
        </div>
    `;

    $('#postFooter-' + postId).after(formHtml, commentContainer);

    addPublishCommentFunctionality(postId);
    addCommentsFromPost(postId);
}

function addPublishCommentFunctionality(postId) {
    var cancelButton = $('#comment-button-cancel-' + postId);
    var publishButton = $('#comment-button-save-' + postId);

    cancelButton.on('click', function (event) {
        emptyCommentField(postId);
        cancelButton.prop('disabled', true);
        publishButton.prop('disabled', true);
    });

    publishButton.on('click', function () {
        event.preventDefault(); 
        publishComment(postId);
        cancelButton.prop('disabled', true);
        publishButton.prop('disabled', true);
    });

    var inputField = $('#comment-field-' + postId);
    inputField.focus();

    var charCountText = $('#comment-chars-' + postId);
    var charLimit = postCharLimit;

    inputField.on('input', function () {
        charCountText.text(inputField.val().length); // Update character count text
        cancelButton.prop('disabled', inputField.val().length == 0); // Disable cancel button if content length is 0
        publishButton.prop('disabled', inputField.val().length > charLimit || inputField.val().length == 0); // Disable save button if content length exceeds char limit
    });
}

function addCommentsFromPost(postId) {
    var loading = `
        <div class="d-flex justify-content-center align-items-center mt-4 mb-4 small" id="loading-comments-${postId}">
            <span class="spinner-border text-primary" role="status"></span>
        </div>
    `;

    var commentsContainer = $('#comments-' + postId);
    commentsContainer.append(loading);

    $.ajax({
        type: 'GET',
        url: '/Home/GetCommentsFromPost?postId=' + postId,
        dataType: 'json',
        success: function (success) {
            // remove loading icon
            var loadingIcon = $('#loading-comments-' + postId);
            loadingIcon.remove();

            // add comments
            var htmlString = success.value;
            commentsContainer.append(htmlString);
        },
        error: function (error) {
            // remove loading icon
            var loadingIcon = $('#loading-comments-' + postId);
            loadingIcon.remove();
        }
    });
}

function publishComment(postId, content) {
    var commentsContainer = $('#comments-' + postId);
    var loading = `
        <div class="d-flex justify-content-center align-items-center mt-4 mb-4 small" id="loading-comment-${postId}">
            <span class="spinner-border text-primary" role="status"></span>
        </div>
    `;

    commentsContainer.after(loading);

    var field = $('#comment-field-' + postId);
    var content = field.val();
    emptyCommentField(postId);

    var comment = {
        PostId: postId,
        Content: content
    }
    
    $.ajax({
        type: 'POST',
        url: '/Home/CreateComment',
        contentType: 'application/json',
        data: JSON.stringify(comment),
        dataType: 'json',
        success: function (success) {
            var loadingIcon = $('#loading-comment-' + postId);
            loadingIcon.remove();

            var htmlString = success.value;
            commentsContainer.prepend(htmlString);
            
            var commentCount = $('#commentCount-' + postId);
            var commentCountNumber = parseInt(commentCount.text());
            commentCountNumber += 1;
            commentCount.text(commentCountNumber);
        },
        error: function (error) {
            var loadingIcon = $('#loading-comment-' + postId);
            loadingIcon.remove();
        }
    });
}

function emptyCommentField(postId) {
    $('#comment-field-' + postId).val('');
    $('#comment-chars-' + postId).text('0');
}

function removeCommentsContainer(postId, commentButton) {
    $('#comments-' + postId).remove();
    $('#comment-form-' + postId).remove();

    commentButton.on('click', function (event) {
        addCommentSection(postId);
    });
}