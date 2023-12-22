document.addEventListener('DOMContentLoaded', function () {
    var editFields = $('[id^="editField-"]');

    // Remove existing listeners to prevent multiple listeners being attached to one field
    editFields.off('click');

    // Attach listener to each delete field
    editFields.each(function () {
        $(this).on('click', function (event) {
            event.preventDefault();
            var postId = this.id.split('-')[1];
            addEditForm(postId);
        });
    });
});

function addEditForm(postId) {
    var postContentField = $('#postContent-' + postId);
    postContentField.addClass('d-none');

    var originalPostContent = postContentField.text();
    var charLimit = postCharLimit;

    var formHtml = `
            <form id="editContent-form-${postId}">
                <input class="d-none" id="editContent-${postId}" name="PostId" value="${postId}">
                <input class="form-control-plaintext shadow-none input-field" type="text" id="editContent-editField-${postId}" name="Content" value="${originalPostContent}">

                <div id="editContent-charcounter-container-${postId}" class="form-text">
                    <p><span id="editContent-chars-${postId}">${originalPostContent.length}</span>/${charLimit}</p>
                </div>

                <button class="btn btn-success btn-sm mb-4 me-2" id="editContent-button-save-${postId}" type="submit">Save</button>
                <button id="editContent-button-cancel-${postId}" class="btn btn-secondary btn-sm mb-4" type="button" >Cancel</button>
            </form>
        `;
    postContentField.after(formHtml);
    
    addEditFunctionality(postId);
}

function addEditFunctionality(postId) {
    var form = $('#editContent-form-' + postId);
    form.attr('action', "/Home/EditPost");
    form.attr('method', "post");
    
    var menuButton = $('#postMenuButton-' + postId);
    menuButton.addClass('d-none');
    
    var saveButton = $('#editContent-button-save-' + postId);
    var cancelButton = $('#editContent-button-cancel-' + postId);

    var charCountText = $('#editContent-chars-' + postId);
    var charLimit = postCharLimit;

    var editField = $('#editContent-editField-' + postId);
    editField.focus();

    cancelButton.on('click', function (event) {
        leaveEditMode(postId);
    });

    editField.on('input', function () {
        charCountText.text(editField.val().length); // Update character count text
        saveButton.prop('disabled', editField.val().length > charLimit); // Disable save button if content length exceeds char limit
    });
}

function leaveEditMode(postId) {
    var menuButton = $('#postMenuButton-' + postId);
    var form = $('#editContent-form-' + postId);
    var postContentField = $('#postContent-' + postId);

    menuButton.removeClass('d-none');
    form.remove();
    postContentField.removeClass('d-none');
}
