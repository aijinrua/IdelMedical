// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function selectThumbnail(currentEmelent, section) {
    var input = $(currentEmelent);
    var img = input.prev().find('img');
    var file = input.prop('files')[0];
    var url = URL.createObjectURL(file);

    alert(section);
    img.attr('src', url);

    var formData = new FormData();
    formData.append('id', id);
    formData.append('thumbnail', file);

    postDataAsync('/Notice/Detail', formData, function (data) {
        if (!data.isError) {
            //if (data.isfirst) {
            //    $('#thumbDefault').attr('src', url);
            //}

            location.reload();
        }
    });
}