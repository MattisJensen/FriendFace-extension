document.addEventListener('DOMContentLoaded', function () {
    var deleteFields = $('[id^="deleteField-"]');

    // Remove existing listeners to prevent multiple listeners being attached to one field
    deleteFields.off('click');

    // Attach listener to each delete field
    deleteFields.each(function () {
        $(this).on('click', function (event) {
            event.preventDefault();
            var postId = this.id.split('-')[1];
            postDelete($(this), postId);
        });
    });
});

function postDelete(deleteField, postId) {
    event.stopPropagation(); // Prevents dropdown from closing

    // Changes text and turn it red
    deleteField.text('Confirm');
    deleteField.css('color', 'rgb(255,255,255)');
    deleteField.css('backgroundColor', 'rgb(220,53,69)');

    // Add listener to execute delete request
    deleteField.on('click', function (e) {
        deleteRequest(postId)
    });

    // Add listener to reset the delete text when the dropdown is closed
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.dropdown-menu').length) {
            resetDeleteText(postId);
        }
    });
}

function deleteRequest(postId) {
    var post = $('#postContainer-' + postId);
    post.hide();

    $.ajax({
        type: 'PUT',
        url: '/Home/DeletePost',
        contentType: 'application/json',
        data: JSON.stringify(postId),
        dataType: 'json',
        success: function (success) {
            post.remove();
        },
        error: function (error) {
            post.show();
            resetDeleteText(postId);
        }
    });
}

function resetDeleteText(postId) {
    var deleteField = $('#deleteField-' + postId)
    
    deleteField.text('Delete');
    // reset color to default
    deleteField.css('color', '');
    deleteField.css('backgroundColor', '');

    deleteField.off('click', deleteRequest);
}

