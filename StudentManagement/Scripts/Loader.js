$(".btn").click(function () {
    $("#divLoader").show();
    $.ajax
        ({
            url: '/Test/Submit',
            dataType: "json",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            data: {},
            //async: true,
            //processData: false,
            // cache: false,
            success: function (data) {
                $("#divLoader").hide();
                alert(data);
            }
            //error: function (xhr) {
            //    $("#divLoader").hide();
            //    alert('error');
            //}
        })
});