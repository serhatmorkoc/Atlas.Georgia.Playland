var DashboardOperations = function () {
    return {
        getSummaries: function (_startDate, _endDate, _typeId) {
            return fnGetSummaries(_startDate, _endDate, _typeId);
        },
        getUploadDetail: function (_startDate, _endDate, _typeId) {
            return fnGetUploadDetail(_startDate, _endDate, _typeId);
        },
        OnPostLoginSucceed: function (_response) {
            return fnOnPostLoginSucceed(_response);
        }
    }


    function fnGetSummaries(_startDate, _endDate, typeId) {
        var _ajaxUrl = "/Home/GetSummaries/";
        var _request = {
            startDate: _startDate,
            endDate: _endDate,
            typeId: typeId
        };

        executeAjax(_ajaxUrl, _request, function (response) {
            if (response.IsSucceed) {
                var summary = response.Result;
                $("#totalTransaction").text(formatDecimalToMoney(summary.TotalTransaction));
                //$("#freePlay").text(parseFloat(summary.TotalFreeGame).toFixed(2));
                //$("#gameCount").text(parseFloat(summary.TotalGameCount).toFixed(2));


                $("#freePlay").text(formatDecimalToMoney(summary.TotalFreeGame).replace(",00", ""));
                $("#gameCount").text(formatDecimalToMoney(summary.TotalGameCount).replace(",00", ""));
                $("#totalGameAmount").text(formatDecimalToMoney(summary.TotalGameAmount));
                $("#totalCash").text(formatDecimalToMoney(summary.TotalCashAmount));
                $("#totalCreditCard").text(formatDecimalToMoney(summary.TotalCreditCard));
                $("#totalBonus").text(formatDecimalToMoney(summary.TotalBonus));
                $("#totalUploadQuantity").text(summary.TotalUploadQuantity);
                $("#totalVisitor").text(summary.TotalVisitor);

                var HtmlDet = "<table class='table table-bordered'>";
                HtmlDet += "<thead>";
                HtmlDet += "<tr>";
                HtmlDet += "<td><strong>Card</strong></td>";
                HtmlDet += "<td><strong>Adet</strong></td>";
                HtmlDet += "</tr>";
                HtmlDet += "</thead>";
                HtmlDet += "<tbody>";


                for (var i = 0; i < summary.CardPlays.length; i++) {
                    var Row = summary.CardPlays[i];
                    HtmlDet += "<tr>";
                    HtmlDet += "<td>" + Row.CardNo + "</td>";
                    HtmlDet += "<td>" + Row.PlayCount + "</td>";
                    HtmlDet += "</tr>";

                }
                HtmlDet += "</tbody>";
                HtmlDet += "</table>";
                document.getElementById("divModalContent").innerHTML = HtmlDet;

            }
            else {
                alert(response.Error.ErrorMessage);
            }
        })
    }

    function fnOnPostLoginSucceed(_response) {
        if (!_response.IsSucceed) {
            alert(_response.Error.ErrorMessage);
            return;
        }
        else {
            window.location.href = "/Home/Dashboard";
        }
    }

    function fnGetUploadDetail(startDate, endDate, typeId) {
        var url = "/Home/_UploadDetail";
        var request = {
            startDate: startDate,
            endDate: endDate,
            typeId: typeId,
        };

        executeAjaxDone(url, request, function (response) {
            _modalFormId = "#uploadDialog";
            _modalBodyId = "#uploadBody";
            $(_modalFormId).modal({
                show: true,
            });
            $(".modal-dialog").css("width", 600);
            $(".modal-footer").css("border-top", "1px solid #ddd");
            $(".modal-footer").css("background", "#f5f5f5");

            $(_modalBodyId).css("height", 400);
            $(_modalBodyId).css("overflow-y", "auto");
            $(_modalBodyId).css("font-size", "13px");
            $(_modalBodyId).html(response);
        })
    }
};