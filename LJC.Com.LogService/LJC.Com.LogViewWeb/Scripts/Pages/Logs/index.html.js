(function () {
    ($.live == undefined) &&
         $.fn.extend({
             live: function (event, callback) {
                 if (this.selector) {
                     $(document).on(event, this.selector, callback);
                 }
             }
         });

    var pos = $("#pos").val();
    var count = 0;

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

    var tbody=$("tbody:eq(1)")

    function load()
    {
        tbody.find("tr:last").remove();
        $.get("ReadLog.ashx", { loglevel: $(":input[name='loglevel']").val(), pos: pos },
            function (json) {
                if (json.result == 0)
                {
                    return;
                }
                pos = json.lastpos;
                for (var i = 0; i < json.data.length; i++) {
                    var item=json.data[i];
                    tbody.append("<tr><td>" + (++count) + "</td><td>" + item.time + "</td><td>" + item.logfrom + "</td><td>" + item.title + "</td><td>" + item.content + "</td></tr>");
                }
                if(json.lastpos==-1)
                {
                    tbody.append("<tr><td colspan='5' style='text-align:center;'>数据加载完毕......</td></tr>");
                } else {
                    tbody.append("<tr><td colspan='5' style='text-align:center;' class='btncontinue'>点击继续......</td></tr>");
                }

            });
    }

    $(".btncontinue").live('click',function () {
        load();
    });

    load();
})();