
(function () {
    var method;
    var noop = function () { };
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());

function UpperCase(textBox) {
    //var selectionStart = textBox.selectionStart;
    textBox.value = textBox.value.toUpperCase();
    //textBox.selectionStart = selectionStart;
}
function convertaccents(input) {
    if (input.value == '')
        return;
    var selectionStart = input.selectionStart;
    text = input.value;
    text = text.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    text = text.replace(/đ/g, "d");
    text = text.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    text = text.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    text = text.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    text = text.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    text = text.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    text = text.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    text = text.replace(/Đ/g, "D");
    text = text.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    text = text.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    text = text.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    text = text.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    text = text.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");

    str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ()-_+?:}{/.,' ";
    for (i = 0; i < text.length; i++) {
        if (text.length >= 1 && str.indexOf(text.substr(i, 1)) == -1) {
            text = text.replace(text.substr(i, 1), '');
            i = 0;
        }
    }

    input.value = text;
    input.selectionStart = selectionStart;
    return;
}
function convertaccents2(input, event) {
    if (event.keyCode != 37 && event.keyCode != 39 && event.keyCode != 8)
        convertaccents(input);
}


function formatCurrency(num) {
    var str = num.toString();
    var isNegative = false;
    if (str.charAt(0) == '-') {
        isNegative = true;
    }

    var str = str.replace(/[^\d]/g, ''), parts = false, output = [], i = 1, formatted = null;

    if (str != "" && str != null) {
        str = parseFloat(str).toString();
    }

    if (str.indexOf(".") > 0) {
        parts = str.split(".");
        str = parts[0];
    }
    str = str.split("").reverse();
    for (var j = 0, len = str.length; j < len; j++) {
        if (str[j] != ",") {
            output.push(str[j]);
            if (i % 3 == 0 && j < (len - 1)) {
                output.push(",");
            }

            i++;
        }
    }
    formatted = output.reverse().join("");
    var out = formatted + ((parts) ? "." + parts[1].substr(0, 2) : "");
    if (isNegative) {
        out = "-" + out;
    }
    return out;
};

function validAccountRegex(acct) {
    var hdBankAccountRegex = /\w{14}[0-9]$/g;
    return hdBankAccountRegex.test(acct.trim());
};

function toNumber(str) {
    return parseFloat(str.toString().replace(/,/g, ''));
}

String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, "");
}
String.prototype.ltrim = function () {
    return this.replace(/^\s+/, "");
}
String.prototype.rtrim = function () {
    return this.replace(/\s+$/, "");
}

function validRegexDate(dateStr) {
    var format = /^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/g;
    return format.test(dateStr);
}

function validIDNo(idNo) {
    var format = /^[0-9]|[A-Z0-9]/g;
    return format.test(idNo);
}


function validPhoneNo(phone) {
    var format = /^[0-9]{8,14}$/g;
    return format.test(phone);
}


function customAjax(type, data, contentType, url, success, error) {
    displayProgressBar(true);
    $.ajax({
        type: type,
        data: data,
        contentType: contentType,
        url: url,
        success: function (result) {
            displayProgressBar(false);
            if (success && typeof (success) === "function") {
                success(result);
            }

        },
        error: function (e) {
            displayProgressBar(false);
            if (error && typeof (error) === "function") {
                error(result);
            }


        }
    });
}