document.addEventListener('DOMContentLoaded', function() {
    var cancelButton = $('#postContent-button-cancel');

    // Remove existing listeners to prevent multiple listeners being attached to one field
    cancelButton.off('click');
    
    cancelButton.on('click', function (event) {
        postCancel();
    });
});

function postCancel() {
    $('#postCreateContainer').hide();
    $('#postContent-publishField').val('');
    $('#postContent-publishField').get(0).readOnly = true;
    $('#postContent-chars').text('0');
    $('#postContent-button-publish').attr('disabled', true);
    $('#post-btn').removeAttr('disabled');
}