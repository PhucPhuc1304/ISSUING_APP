//==========================================
// Check max row, max chars length of one row
//==========================================
function CheckValidString(el, _maxRow, _maxLengthChars) {
    var maxRow = _maxRow; //50;
    var maxLengthChars = _maxLengthChars; //35;
    var s = '';
    var oMessage = checkTextArea(el, maxRow, maxLengthChars);
    if (!oMessage.isValid) s = '<span class="Error">' + oMessage.Mesage + ' ( tối đa '+ maxRow +' dòng - mỗi một dòng tối đa ' + maxLengthChars + ' ký tự )' + '</span><br/>';
    for (var i = 0; i < oMessage.Rows.length; i++) s += '<span>' + oMessage.Rows[i].message + '</span><br/>';
    $('#boxWarningMessageArea').html(s);

    if (!oMessage.isValid) $(el).attr('has-err', 1);
    else $(el).attr('has-err', 0);
}
function checkTextArea(oEl, v_allowNumRow, v_allowCharLength) {
    var sContent = $(oEl).val();
    var allowNumRow = v_allowNumRow;
    var allowCharLength = v_allowCharLength;
    var oResult = { isValid: true, Mesage: '.', Rows: [] };
    var arrTmp = sContent.split(/\r*\n/);
    var iRows = arrTmp.length;

    $('.tipsWarning').remove();
    var oldRows = parseInt($(oEl).attr('rl-height'));
    var oldHeight = parseInt($(oEl).css('height'));
    if (iRows > oldRows) {
        $(oEl).attr('rl-height', iRows);
        $(oEl).css('height', oldHeight + 20);
    } else if (iRows < oldRows) {
        $(oEl).attr('rl-height', iRows);
        $(oEl).css('height', oldHeight - 20);
    }

    if (iRows > allowNumRow) {
        arrTmp.pop();
        var sOld = '';
        for (var i = 0; i < arrTmp.length; i++) {
            if (i == 0) sOld = arrTmp[i];
            else sOld += "\n" + arrTmp[i];
        }

        $(oEl).val(sOld);
        oResult.isValid = false;
        oResult.Mesage = "Kiểm tra trường Nội dung !";
    }
    var offsetTop = 20;
    for (var i = 0; i < arrTmp.length; i++) {
        if ($('#tips_' + (i + 1)).length == 0)
            $('.tipContentShort').append('<span class="tipsWarning" id="tips_' + (i + 1) + '" style="position: absolute;top: ' + (i) * offsetTop + 'px;right: 17px;">(' + v_allowCharLength + ')</span>');

        if (arrTmp[i].length > allowCharLength) {
            oResult.isValid = false;
            oResult.Mesage = "Kiểm tra trường Nội dung !";
            $('#tips_' + (i + 1)).html('(!)').css('color', 'red');
            //oResult.Rows.push({ r: (i + 1), message: 'Dòng ' + (i + 1).toString() + ' lớn hơn 50 ký tự!' });
        } else {
            //oResult.Rows.push({ r: (i + 1), message: 'Dòng ' + (i + 1).toString() + ' được nhập thêm ' + (50 - arrTmp[i].length) + ' ký tự!' });
            $('#tips_' + (i + 1)).html('(' + (v_allowCharLength - arrTmp[i].length) + ')');
        }
    }

    return oResult;
}

//==========================================
// Show message dialog and comfirm dialog
//==========================================
function shwMsg(newoptions) {
    var options = { title: 'Thông báo', message: 'Thông báo người dùng!', width: 350, callback: null };
    options = $.extend({}, options, newoptions);

    $('#dlgMsg').dialog({
        title: options.title,
        modal: true,
        width: options.width,
        height: 'auto',
        resizable: true,
        open: function () {
            $("#MsgContext").html(options.message);
        },
        position: { my: "center", at: "center", of: window },
        buttons: [
            {
                text: "Close",
                click: function () {
                    $(this).dialog("close");

                    if (typeof (options.callback) == "function")
                        options.callback();
                }
            }]
    })
};
function shwConfirm(newoptions) {
    var options = { title: 'Thông báo', message: 'Thông báo người dùng!', width: 350, callback: null };
    options = $.extend({}, options, newoptions);

    $('#dlgMsg').dialog({
        title: options.title,
        modal: true,
        width: options.width,
        height: 'auto',
        resizable: true,
        open: function () {
            $("#MsgContext").html(options.message);
        },
        position: { my: "center", at: "center", of: window },
        buttons: [
            {
                text: "Yes",
                click: function () {
                    $(this).dialog("close");

                    if (typeof (options.callback) == "function")
                        options.callback();
                }
            }, {
                text: "No",
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    })
};

//==========================================
// NumberFormat 2 decimal
//==========================================
function numberFormatCyy(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1))
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    return x1 + x2;
}
function stripNonNumericCyy(str) {
    str += '';
    var rgx = /^\d/;
    var rgx2dot = /^\./;
    var out = '';
    while (str.length > 0 && str.charAt(0) == '0')
        str = str.substring(1);
    for (var i = 0; i < str.length; i++) {
        if (rgx.test(str.charAt(i)) || rgx2dot.test(str.charAt(i))) {
            if (!((str.charAt(i) == '.' && out.indexOf('.') != -1) ||
                   (str.charAt(i) == '-' && out.length != 0))) {
                out += str.charAt(i);
            }
        }
    }
    return out;
}
function convertCyy(input, noformat) {
    //noformat.value = input.value.replace(/,/g, '');
    $(noformat).val(input.value.replace(/,/g, ''));
    input.value = numberFormatCyy(stripNonNumericCyy(input.value));
    if (input.maxLength > 0)
        input.value = input.value.substring(0, input.maxLength);
    return;
}

function shwLoading(bShow) {
    var sLoading = '<div class="loading_wrapper"><div><img src="/Content/Images/loading.gif" /> Vui lòng đợi...</div></div>';
    if ($('.loading_wrapper').length == 0) $('body').append(sLoading);

    if (bShow) $('.loading_wrapper').show(500);
    else $('.loading_wrapper').hide(500);
};
function showAlert(o) {
    //var s = '<strong>' + o.title + '! </strong>';
    //s += o.message;
    //$('#AlertMessage').html(s);
    //$("#success-alert").fadeTo(5000, 500).slideUp(500, function () {
    //    $("#success-alert").slideUp(5000);
    //});


    //show dialog
    var s = '<div id="dlgInfo" title="" style="display:none;">' +
            '<p id="contentholderInfo" style="font-size:12px;"> ... </p>' +
            '</div>';
    if ($('#dlgInfo').length == 0) {
        $('body').append(s);
    }

    var objDlg = $("#dlgInfo").dialog({
        title: o.title,
        modal: true,
        width: '30%',
        height: 'auto',
        resizable: false,
        open: function () {
            $("#contentholderInfo").html(o.message);
        },
        position: { my: "center", at: "center", of: window },
        buttons: [{
                text: "Thoát",
                click: function () {
                    $(this).dialog("close");
                }
            }]
    });
};
function parseDate(str) {
    var t = str.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
    if (t !== null) {
        var d = +t[1], m = +t[2], y = +t[3];
        var date = new Date(y, m - 1, d);
        if (date.getFullYear() === y && date.getMonth() === m - 1) {
            return date;
        }
    }

    return null;
};
function printDiv(el) {
    var divToPrint = el;//$('.page')[0];
    var newWin = window.open('', 'Print-Window');
    newWin.document.open();
    newWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</body></html>');
    newWin.document.close();
    setTimeout(function () { newWin.close(); }, 10);
}
function OpenWindow(url) {
    var win = window.open(url, '_blank');
    if (win) {
        //Browser has allowed it to be opened
        win.focus();
    } else {
        //Browser has blocked it
        alert('Please allow popups for this website');
    }
};