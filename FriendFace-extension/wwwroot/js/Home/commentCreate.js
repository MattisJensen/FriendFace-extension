document.addEventListener('DOMContentLoaded', function () {
    var commentButtons = $('[id^="commentButton-"]');

    // Remove existing listeners to prevent multiple listeners being attached to one field
    commentButtons.off('click');

    // Attach listener to each delete field
    commentButtons.each(function () {
        $(this).on('click', function (event) {
            var postId = this.id.split('-')[1];
            addCommentForm(postId);
        });
    });
});

function addCommentForm(postId) {
    var charLimit = postCharLimit;

    var formHtml = `
            <form id="comment-form-${postId}">
                <input class="d-none" id="comment-${postId}" name="PostId" value="${postId}">
                <input class="form-control-plaintext shadow-none input-field mt-3" id="comment-field-${postId}" type="text" name="Content" placeholder="write your comment...">

                <div class="form-text" id="comment-charcounter-container-${postId}">
                    <p><span id="comment-chars-${postId}">0</span>/${charLimit}</p>
                </div>

                <button class="btn btn-success btn-sm mb-4 me-2" id="comment-button-save-${postId}" type="submit">
                    <i class="fas fa-arrow-up"></i> Publish Post
                </button>
                <button class="btn btn-secondary btn-sm mb-4" id="comment-button-cancel-${postId}" type="button">Cancel</button>
            </form>
        `;
    
   $('#postFooter-' + postId).after(formHtml);

    addCreateFunctionality(postId);
}

function addCreateFunctionality(postId) {
    var form = $('#comment-form-' + postId);
    form.attr('action', "/Home/CreateComment");
    form.attr('method', "post");
    
    var saveButton = $('#comment-button-save-' + postId);
    var cancelButton = $('#comment-button-cancel-' + postId);

    var charCountText = $('#comment-chars-' + postId);
    var charLimit = postCharLimit;

    var field = $('#comment-field-' + postId);
    field.focus();
    
    cancelButton.on('click', function (event) {
        leaveEditMode(postId);
    });

    field.on('input', function () {
        charCountText.text(field.val().length); // Update character count text
        saveButton.prop('disabled', field.val().length > charLimit); // Disable save button if content length exceeds char limit
    });
}

function leaveEditMode(postId) {
    var form = $('#comment-form-' + postId);
    var postContentField = $('#postContent-' + postId);
    
    form.remove();
    postContentField.removeClass('d-none');
}

