
function registerPrevNextButtonClick(backButton, backUrl, nextButton, nextUrl, getCustomDataScreen) {
    registerButtonClick(backButton, backUrl, getCustomDataScreen);
    registerButtonClick(nextButton, nextUrl, getCustomDataScreen);
}

function registerButtonClick(button, url, getCustomDataScreen, callback) {

    if (button != null && url != null) {
        button.off('click ');
        button.click(function () {

            if (callback && typeof (callback) === "function") {

                var valid = callback();

                if (valid) {
                    loadPartialViewIntoWizardContentDiv(url, getCustomDataScreen);
                }
            } else {

                loadPartialViewIntoWizardContentDiv(url, getCustomDataScreen);
            }

            return false;
        });
    }
}

function highlightMenuItem(active) {
    $('.nav').find('a[class*=selected]').each(function () {

        $(this).removeClass('selected');
    });

    active.addClass('selected');
}



function loadPartialViewIntoWizardContentDiv(actionUrl, getCustomDataScreen) {
    displayProgressBar(true);
    var paramObj = getDataScreen(getCustomDataScreen);

    $.ajax({
        url: actionUrl,
        data: JSON.stringify(paramObj),
        datatype: 'json',
        contentType: 'application/json',
        type: 'post',
        success: function (data) {
            $('#wizardContentDiv').html(data);
            setInputMaxLength();
            displayProgressBar(false);
        },
        error: function (e) {
            displayProgressBar(false);
            if (e.status == 401){
                window.location.href = "/Login/Index";
            }

        }
    });
}

function getDataScreen(getCustomDataScreen) {

    var values = {};
    $("#wizardContentDiv :input[type='text'],input[type='hidden'], #wizardContentDiv Select, #wizardContentDiv Area").each(function () {
        getElementValue(values, this);
    });
    $("#wizardContentDiv :input[type='checkbox'], #wizardContentDiv :input[type='radio']").each(function () {
        if ($(this).is(":checked")) {
            getElementValue(values, this);
        }
    });
    if (typeof getCustomDataScreen == 'function')
        values = getCustomDataScreen(values);
    return values;
}

function getElementValue(values, element) {
    if (values.extras == null || values.extras == "undefined") {
        values.extras = {};
    }
    if (element.name.search(/\[\]/) > 0) //search for [] in name
    {
       
        var newName = element.name.replace(/\[\]/,'');
        if (typeof values[newName] != "undefined") {
           
            values[newName] = values[newName].concat([$(element).val()]);
        }
        else {

            values[newName] = [$(element).val()];
        }
    }
    else {

      
        if (element.name != "") {
            if (element.name.indexOf('ext_')==0) {

                values.extras[element.name] = $(element).val();
            } else {
                values[element.name] = $(element).val();
            }
        }


    }
}

function displayProgressBar(show) {
    var divLoadingIndicator = $("#divLoadingIndicator");
    //var progressBar = $('#imgProgressbar');
    if (show) {
        /*divLoadingIndicator.height($(document).height() - 20);
        divLoadingIndicator.width($(document).width() - 22);*/
        divLoadingIndicator.show();

        /*var x = ($(window).width() - progressBar.width()) / 2;
        var y = ($(window).height() - progressBar.height()) / 2;
        progressBar.css({ top: y, left: x });
        progressBar.show();*/
    }
    else {
        divLoadingIndicator.hide();
        /*progressBar.hide();*/
    }
}

function toogleButtonDisableState(button, isDisable) {
    if (button != null) {
        button.prop('disabled', isDisable);
    }
}

function setInputMaxLength() {
    $('input[data-val-length-max]').each(function (index) {
        $(this).attr('maxlength', $(this).attr('data-val-length-max'));
    });
}

function downloadFile(url) {
    var hiddenIframeId = 'hiddenDownloader';
    var iframe = $('#' + hiddenIframeId);
    if (iframe.length <= 0) {
        $('body').append('<iframe style="position:absolute; left:-10000px; visible:hidden;" src="' + url + '" id="' + hiddenIframeId + '" />');
    } else {
        iframe.attr('src', url);
    }
};

function isEmpty(val) {
    return val == '' || typeof val == 'undefined';
}

function SetFinishPrecent(currStep) {
    var listStep = {
        'HolderName': 1,
        'HolderAddress': 2,
        'HConfirmDemutualization': 3,
        'HDemutualization': 4,
        'HReportContact': 5,
        'HConfirmClainContact': 6,
        'HClainContact': 7,
        'HColoradoDeduction': 8,
        'HStateID': 9,
        'HEnterStateID': 10,
        'HolderCreatedSeccussfull': 11
    };
    var totalStepGeneralInformation = 4;
    var totalStepGeneralContact = 3;
    var totalStepStateSpecific = 3;
    var numCurrStep = listStep[currStep];
    $("#step_nav a").removeClass("selected");
    switch (numCurrStep) {
        case listStep.HolderName:
            $("#GeneralFinished").html($.string.Format(" {0} % completed", 0));
        case listStep.HolderAddress:
        case listStep.HConfirmDemutualization:
        case listStep.HDemutualization:
            $(".information").addClass("selected");
            var percentGenarateInfo = parseInt((numCurrStep - 1) * 100 / totalStepGeneralInformation);
            $("#GeneralInformation").html($.string.Format(" {0} % completed", percentGenarateInfo));
            $("#GeneralContact").html($.string.Format(" {0} % completed", 0));
            $("#StateSpecific").html($.string.Format(" {0} % completed", 0));
            break;

        case listStep.HReportContact:
            var percentGenarateInfo = parseInt((numCurrStep - 1) * 100 / totalStepGeneralInformation);
            $("#GeneralInformation").html($.string.Format(" {0} % completed", percentGenarateInfo));
            $("#StateSpecific").html($.string.Format(" {0} % completed", 0));
        case listStep.HConfirmClainContact:
        case listStep.HClainContact:
            $(".contact").addClass("selected");
            var percentGeneralContact = parseInt((numCurrStep - totalStepGeneralInformation - 1) * 100 / totalStepGeneralContact);
            $("#GeneralContact").html($.string.Format(" {0} % completed", percentGeneralContact));
            break;

        case listStep.HColoradoDeduction:
            var percentGeneralContact = parseInt((numCurrStep - totalStepGeneralInformation - 1) * 100 / totalStepGeneralContact);
            $("#GeneralContact").html($.string.Format(" {0} % completed", percentGeneralContact));
        case listStep.HStateID:
        case listStep.HEnterStateID:
            $(".copy").addClass("selected");
            var percentGeneralContact = parseInt((numCurrStep - totalStepGeneralInformation - totalStepGeneralContact - 1) * 100 / totalStepStateSpecific);
            $("#StateSpecific").html($.string.Format(" {0} % completed", percentGeneralContact));
            break;
        case listStep.HolderCreatedSeccussfull:
            $("#StateSpecific").html($.string.Format(" {0} % completed", 100));
            $("#GeneralFinished").html($.string.Format(" {0} % completed", 100));
            $(".finish").addClass("selected");
            numCurrStep++;
            break;
    }
    totalStepHolder = Object.keys(listStep).length;
    var percentStepHolder = parseInt((numCurrStep - 1) * 100 / totalStepHolder);
    $("#GeneralFinished").html($.string.Format(" {0} % completed", percentStepHolder));


}

//SetFinishPrecent('HolderCreatedSeccussfull')