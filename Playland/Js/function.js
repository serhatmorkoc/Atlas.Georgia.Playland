
function openDatePicker(date) {
    $("#datePicker1").datepicker({
        autoclose: true,
        todayHighlight: true,
        format: 'dd.mm.yyyy',
        language: 'tr',
        locale: 'tr',

    }).datepicker('update', date);

    $("#datePicker2").datepicker({
        autoclose: true,
        todayHighlight: true,
        format: 'dd.mm.yyyy',
        language: 'tr',
        locale: 'tr',
        maxDate: 0,
    }).datepicker('update', date);
}

$(function () {
    openDatePicker(new Date());
});


function getValue(id) {
    if (!id.startsWith("#")) {
        return $("#" + id).val();
    }
    else {
        return $(id).val();
    }
}

function formatDecimalToMoney(value) {
    return value.toFixed(2).replace('.', ',').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1.");
}

$(document).ajaxStart(function () {
    startLoader();
});


$(document).ajaxComplete(function () {
    endLoader();
});

var folderPath = "";

function startLoader() {
    $("#ajaxLoadingDiv").css('display', 'block');
    $("#closerDiv").css('display', 'block');
}

function endLoader() {
    $("#ajaxLoadingDiv").css('display', 'none');
    $("#closerDiv").css('display', 'none');
}


function executeAjax(actionURL, request, onSuccess, onFail) {
    var data = JSON.stringify(request);

    if (onFail == null || onFail == undefined) {
        onFail = function (jqXHR, textStatus, errorThrown) {
            var errorMessage = jqXHR.responseText + "\r" + textStatus + "\r" + errorThrown;
            alert(errorMessage);
        };
    }

    $.ajax
    ({
        type: 'POST',
        url: actionURL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        success: onSuccess,
        error: onFail
    });
}

function executeAjaxDone(executionUrl, requestData, onDoneCallBack, onFailCallBack) {
    if (onFailCallBack == undefined) {
        onFailCallBack = function (jqXHR, textStatus, errorThrown) {
            var errorMessage = jqXHR.responseText + "\r" + textStatus + "\r" + errorThrown;
            alert(errorMessage);
        };
    }

    $.ajax({
        url: executionUrl,
        type: "get",
        data: requestData,
        dataType: 'html'
    }).done(onDoneCallBack).fail(onFailCallBack);
}



function OnMvcAjaxBegin() {

}

function OnMvcAjaxFailure() {

}

function OnMvcAjaxCompleted() {

}



