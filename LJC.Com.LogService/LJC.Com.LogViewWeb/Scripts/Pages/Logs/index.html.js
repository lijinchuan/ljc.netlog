(function () {
    var pos = $("#pos").val();

    $('#cc').combo({
        required: true,
        editable: false
    });
    $('#sp').appendTo($('#cc').combo('panel'));
    $('#sp input').click(function () {
        var v = $(this).val();
        var s = $(this).next('span').text();
        $('#cc').combo('setValue', v).combo('setText', s).combo('hidePanel');
    });

    $('#sp input:first').trigger('click');

    $.get("ReadLog.ashx", { loglevel: $(":input[name='loglevel']").val(), pos: $("#pos").val() },
        function (json) {


    });
})();