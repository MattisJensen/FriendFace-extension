document.addEventListener('DOMContentLoaded', function() {
    var cancelButton = $('#postContent-button-cancel');

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