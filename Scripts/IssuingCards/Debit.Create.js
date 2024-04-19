var isuingcards;
var listImages = [];

function chkSameAddr_Click(cb) {
	if (cb.checked) {
		$('#addr_lh_city').val($('#addr_hk_city').val());
		$('#addr_lh_district').val($('#addr_hk_district').val());
		$('#addr_lh_ward').val($('#addr_hk_ward').val());
		$('#addr_lh_addr').val($('#addr_hk_addr').val());
	} else {

	}
};

function cbSMS_OPTION_onChange(el) {
    switch ($(el).val()) {
        case "A01":
            $('#txtAmount_SMS_display').prop('disabled', false);
            $('#txtAmount_SMS_display').val(0);
            $('#txtAmount_SMS').val(0);
            break;
        default:
            $('#txtAmount_SMS_display').prop('disabled', true);
            $('#txtAmount_SMS_display').val(0);
            $('#txtAmount_SMS').val(0);
            break;
    }
};

function chkUSE_SMS_Checked(cb) {
	if (cb.checked) {
        $('#SMS_PHONE').prop('disabled', false);
        $('#cbSMS_OPTION').prop('disabled', false);
    } else {
        $('#SMS_PHONE').prop('disabled', true);
        $('#cbSMS_OPTION').prop('disabled', true);
        $('#cbSMS_OPTION').val('');
        $('#txtAmount_SMS_display').val(0);
        $('#txtAmount_SMS').val(0);
        $('#SMS_PHONE').val();
    }
};

function search_o_customer() {
	var sCode = $("#debit_CONTRACTPRODUCTCODE").children("option:selected").val();
	if (sCode == '') {
		showAlert({
			title: 'Cảnh báo', message: 'Vui lòng chọn sản phẩm thẻ!'
		});
		return false;
	}

	shwLoading(true);
	$.post('/MainIssuingCard/SearchOCustomer', {
		contractproductcode: $("#debit_CONTRACTPRODUCTCODE").children("option:selected").val(),
		contractproductcode_name: $("#debit_CONTRACTPRODUCTCODE option:selected").text()
	}, function (data, status) {
		if (data.code === undefined) {
			objDlg = $("#dlgReview").dialog({
				title: "Tìm kiếm Classifier",
				modal: true,
				width: '35%',
				height: '300',//'auto',
				resizable: false,
				open: function () {
					$('#dlgReviewWrapper').html(data);
				},
				position: { my: "center", at: "center", of: window },
				buttons: [
                    {
                    	text: "Thoát",
                    	click: function () {
                    		$(objDlg).dialog("close");
                    	}
                    }
				]
			});
		} else {
			showAlert({
				title: 'Cảnh báo', message: data.message
			});
		}
	})
    .done(function () { })
    .fail(function (e) {
    	showAlert({
    		title: 'Cảnh báo', message: 'Có lỗi trong quá trình thực hiện truy vấn!'
    	});
    })
    .always(function () {
    	shwLoading(false);
    });
};


function get_classifier(sCode, sText) {
	$('#debit_ISSUING_FOR_CUSTOMER').val(sCode);
	$(objDlg).dialog("close");
};

$(document).ready(function () {
    //setup SMS
    if (!$('#debit_trans_internet').prop('checked')) {
        $('#SMS_PHONE').prop('disabled', true);
        $('#cbSMS_OPTION').prop('disabled', true);
        $('#txtAmount_SMS_display').prop('disabled', true);
        $('#txtAmount_SMS_display').prop('disabled', true);
    }

	//Events input, checkbox
	$('#phone_home, #phone_mobile, #debit_INCENTIVE_POLICY_VALUE').keyup(function () {
		this.value = this.value.replace(/[^0-9\,]/g, '');
	});
	
	//date
	$("#dt_of_issuance").datepicker();
	$("#dt_of_issuance").datepicker("setDate");
	$("#dt_of_issuance").datepicker("option", "dateFormat", 'dd/mm/yy');

	$("#birth_date").datepicker();
	$("#birth_date").datepicker("setDate");
	$("#birth_date").datepicker("option", "dateFormat", 'dd/mm/yy');
	//
	$("#debit_INCENTIVE_POLICY_FROM_DATE").datepicker();
	$("#debit_INCENTIVE_POLICY_FROM_DATE").datepicker("setDate");
	$("#debit_INCENTIVE_POLICY_FROM_DATE").datepicker("option", "dateFormat", 'dd/mm/yy');
	//
	$("#debit_INCENTIVE_POLICY_TO_DATE").datepicker();
	$("#debit_INCENTIVE_POLICY_TO_DATE").datepicker("setDate");
	$("#debit_INCENTIVE_POLICY_TO_DATE").datepicker("option", "dateFormat", 'dd/mm/yy');


	$("#debit_CONTRACTPRODUCTCODE").change(function () {
		shwLoading(true);
		var clientNo = $("#client_no").val();
		var CONTRACTPRODUCTCODE = $('#debit_CONTRACTPRODUCTCODE').children("option:selected").val();
		if (clientNo == null || clientNo == undefined || clientNo == "") {
			shwLoading(false);
			return;
		}
		$.post('/MainIssuingCard/QueryAccount', {
			clientNo: clientNo,
			CONTRACTPRODUCTCODE: CONTRACTPRODUCTCODE
		}, function (data, status) {
			if (data != null && data.length > 0) {
				$('.TKTGTT, .TKTGTN').each(function () {
					if ($(this).val() == 2) {
						$(this).prop("checked", true);
						$('#debit_crd_debit_tktt').attr("disabled", false);
						$('#debit_crd_debit_tktn').attr("disabled", false);
						$('#debit_crd_debit_tktt').html('<option value="">chọn tài khoản</option>');
						$('#debit_crd_debit_tktn').html('<option value="">chọn tài khoản</option>');
						for (var i = 0; i < data.length; i++) {
							$('#debit_crd_debit_tktt').append('<option value="' + data[i].ACCT_NO + '">' + data[i].ACCT_NO + '</option>');
							$('#debit_crd_debit_tktn').append('<option value="' + data[i].ACCT_NO + '">' + data[i].ACCT_NO + '</option>');
						}
					}
				});
			} else {
				$('.TKTGTT, .TKTGTN').each(function () {
					if ($(this).val() == 1) {
						$(this).prop("checked", true);
						$('#debit_crd_debit_tktt').attr("disabled", true);
						$('#debit_crd_debit_tktn').attr("disabled", true);
						$('#debit_crd_debit_tktt').html('<option value="">chọn tài khoản</option>');
						$('#debit_crd_debit_tktn').html('<option value="">chọn tài khoản</option>');
					}
				});
			}
		}).done(function () { })
	.fail(function (e) { showAlert({ title: 'Cảnh báo', message: 'Có lỗi trong quá trình thực hiện truy vấn!' }); })
	.always(function () { shwLoading(false); });
	});

	$('#fileupload').fileupload({
		add: function (e, data) {
			var uploadErrors = [];
			console.log(data.originalFiles[0]['type']);
			//var acceptFileTypes = /^pdf\/(pdf)$/i;
			var acceptFileTypes = /^image\/(jpeg|jpg|tif)$/i;
			if (data.originalFiles[0]['type'].length && !acceptFileTypes.test(data.originalFiles[0]['type'])) {
				uploadErrors.push('Chỉ được phép tải lên tập tin image: JPG hoac TIF!!!');
				showAlert({ title: 'Cảnh báo', message: 'Chỉ được phép tải lên tập tin JPG hoac TIF!!!' });
			}
			if (data.originalFiles[0].size && data.originalFiles[0].size > 2 * 1024 * 1024) {
				uploadErrors.push('Bạn đã tải lên tập tin quá lớn hơn 2MB!!!');
				showAlert({ title: 'Cảnh báo', message: 'Bạn đã tải lên tập tin lớn hơn 2MB!!!' });
			}
			if (uploadErrors.length > 0) {
				//alert(uploadErrors.join("\n"));
			} else {
				data.submit();
			}
		},
		url: '/ViewIssuingCard/Upload',
		dataType: 'json',
		done: function (e, data) {
			console.log(data.result);
			if (data.result.code != '00') {
				showAlert({ title: 'Cảnh báo', message: data.message });
			} else {
				$('#lblOldFileNameUploaded').html(data.result.oldFileName);
				$('#txtOldFileNameUploaded').val(data.result.oldFileName);
				$('#txtNewFileNameUploaded').val(data.result.newFileName);
				$("#imgUpload").attr("src", data.result.path);
				$("#imgUpload").css("display", "");
				$("#fileName").val(data.result.newFileName);
				$("#fileOldName").val(data.result.oldFileName);
				$("#fileSize").val(data.result.size);
				$("#fileExt").val(data.result.ext);
				$("#filePath").val(data.result.path);

				listImages.push({
					UUID: '',
					FILE_ORG: data.result.oldFileName,
					FILE_NAME: data.result.newFileName,
					FILE_EXT: data.result.ext,
					FILE_PATH: data.result.path,
					FILE_SIZE: data.result.size
				});
			}
		},
		progressall: function (e, data) { }
	});

	$('#reload').click(function () {
		location.reload(true);
	})
	/// Gọi rest query
	$("#query").click(function () {
		shwLoading(true);
		$.post('/MainIssuingCard/query', {
			id: $("#search").val()
		}, function (data, status) {
			if (data.code === undefined || data.code == null || data.code <= 0) {
				window.isuingcards = data[0];
				console.log(window.isuingcards);

				$('#client_name').val(data[0].CLIENT_NAME);
				$('#client_name').attr("disabled", true);

				$('#card_client_name').val(data[0].CARD_CLIENT_NAME);
				$('#card_client_name').attr("disabled", true);

				$('#global_id').val(data[0].GLOBAL_ID);
				$('#global_id').attr("disabled", true);

				$('#dt_of_issuance').val(data[0].sDT_OF_ISSUANCE);
				$('#dt_of_issuance').attr("disabled", true);

				$('#place_of_issuance').val(data[0].PLACE_OF_ISSUANCE);
				$('#place_of_issuance').attr("disabled", true);

				$('#birth_date').val(data[0].sBIRTH_DATE);
				$('#birth_date').attr("disabled", true);
				/// Chọn loại ID
				$('#global_id_type').find('option').each(function () {
					if ($(this).val() == data[0].GLOBAL_ID_TYPE) {
						$(this).attr('selected', 'selected');
					}
				});
				$('#global_id_type').attr("disabled", true);

				$('#phone_home').val(data[0].PHONE_HOME);
				$('#phone_home').attr("disabled", true);

				$('#phone_mobile').val(data[0].PHONE_MOBILE);
				$('#phone_mobile').attr("disabled", true);

				$('#email').val(data[0].EMAIL);
				$('#email').attr("disabled", true);

				$('#addr_hk_city').attr("disabled", true);

				$('#addr_hk_district').attr("disabled", true);
				$('#addr_hk_ward').attr("disabled", true);

				$('#addr_hk_addr').val(data[0].ADDR_HK_ADDR);
				$('#addr_hk_addr').attr("disabled", true);

				$('#addr_lh_city').attr("disabled", true);
				$('#addr_lh_district').attr("disabled", true);

				$('#addr_lh_ward').attr("disabled", true);
				$('#addr_lh_addr').val(data[0].ADDR_HK_ADDR);

				$('#addr_lh_addr').attr("disabled", true);
				$('#client_no').val(data[0].CLIENT_NO);

				$('#maker').attr("disabled", true);
				$('#title_info').html("");
				$('.section-07').css("display", "none");

			    //added 10/04/19
				$('#sex').find('option').each(function () {
				    if ($(this).val() == data[0].SEX) {
						$(this).attr('selected', 'selected');
			        }
			    });
				$('#sex').attr("disabled", true);

				$('#client_type').find('option').each(function () {
				    if ($(this).val() == data[0].CLIENT_TYPE) {
						$(this).attr('selected', 'selected');
			}
			});
				$('#client_type').attr("disabled", true);

				$('#industry').find('option').each(function() {
				    if($(this).val() == data[0].INDUSTRY) {
						$(this).attr('selected', 'selected');
			}
			});
				$('#industry').attr("disabled", true);

				$('#business').find('option').each(function () {
				    if($(this).val() == data[0].BUSINESS) {
						$(this).attr('selected', 'selected');
			}
			});
				$('#business').attr("disabled", true);

				$('#class_4').find('option').each(function() {
				    if($(this).val() == data[0].CLASS_4) {
						$(this).attr('selected', 'selected');
			}
			});
				$('#class_4').attr("disabled", true);

				$('#class_5').find('option').each(function () {
				    if($(this).val() == data[0].CLASS_5) {
						$(this).attr('selected', 'selected');
			}
			});
				$('#class_5').attr("disabled", true);


				$('#resident_status').find('option').each(function () {
				    if($(this).val() == data[0].RESIDENT_STATUS) {
						$(this).attr('selected', 'selected');
			}
			});
				$('#resident_status').attr("disabled", true);

				//$('.TKTGTT, .TKTGTN').each(function () {
				//    debugger;
				//    if ($(this).val() == 2) {
				//        $(this).attr("checked", true);
				//        $('#debit_crd_debit_tktt').attr("disabled", false);
				//        $('#debit_crd_debit_tktn').attr("disabled", false);
				//        $.post('/MainIssuingCard/QueryAccount', {
				//            clientNo: window.isuingcards.CLIENT_NO
				//        }, function (data, status) {
				//            $('#debit_crd_debit_tktt').html('<option value="">Chọn tài khoản</option>');
				//            $('#debit_crd_debit_tktn').html('<option value="">Chọn tài khoản</option>');
				//            data.forEach(function (element) {
				//                $('#debit_crd_debit_tktt').append('<option value="' + element.ACCT_NO + '">' + element.ACCT_NO + '</option>');
				//                $('#debit_crd_debit_tktn').append('<option value="' + element.ACCT_NO + '">' + element.ACCT_NO + '</option>');
				//            });
				//        });
				//    }
				//});

			} else {
				shwMsg({
					title: 'Thông báo', message: data.message, width: 350, callback: function () {
						location.reload();
					}
				});
			}
		}).done(function () { })
	.fail(function (e) { showAlert({ title: 'Cảnh báo', message: 'Có lỗi trong quá trình thực hiện truy vấn!' }); })
	.always(function () { shwLoading(false); });
	});
	//$(".debit_release_time_type").on("click", function () {
	//    enableDisableClickRadio("debit_release_time_type", "debit_release_time_amount");
	//});
});
/// Click và disable click
//function enableDisableClickRadio(radio, impact) {
//    if ($("." + radio + ":checked").val() == "FTS") {
//        $("#" + impact).removeAttr("disabled");
//    } else {
//        $("#" + impact).attr('disabled', 'disabled');
//        $("#" + impact).val("");
//    }
//};
function check_input(name, text, dodai) {
	if (checkNullorEmpty(text)) {
		alert(name + " rỗng!"); // Cần thay đổi alert;
		return true;
	}
	else if (text.length > dodai) {
		alert(name + " quá dài!"); // Cần thay đổi alert;
		return true;
	}
	return false;
}
function checkNullorEmpty(value) {
	return !value;
}
$(".dTime").datepicker();
$(".dTime").datepicker("setDate");
$(".dTime").datepicker("option", "dateFormat", 'dd/mm/yy');
// Kiểm tra ngày tháng năm
function checkdatetime(name, text) {
	if (checkNullorEmpty(text)) {
		alert(name + " rỗng!"); // Cần thay đổi alert;
		return true;
	} else if (!isValidDate(text)) {
		alert(name + " sai định dạng");
		return true;
	} else if (lessDate(text)) {
		alert(name + "không được lớn hơn ngày hiện tại!")
		return true;
	}
	return false;
}
function isValidDate(s) {
	var bits = s.split('/');
	var d = new Date(bits[2], bits[1] - 1, bits[0]);
	return d && (d.getMonth() + 1) == bits[1];
}
function lessDate(s) {
	var bits = s.split('/');
	var d = new Date(bits[2], bits[1] - 1, bits[0]);
	var tmp = new Date();
	if (d > tmp) {
		return true;
	}
	return false;
}
/// Gọi rest
function send(isuingcards) {
	isuingcards.ATTACHMENTS = listImages;
	shwLoading(true);
	$.post('/MainIssuingCard/save', {
		data: isuingcards
	}, function (data, status) {
		if (data.code == "00") {
			shwMsg({
				title: 'Thông báo', message: data.message, width: 350, callback: function () {
					location.href = "/IssuingCard/viewIssuingCard/index?uuid=" + data.para1;
				}
			});
		}
		else {
			shwMsg({
				title: 'Thông báo', message: data.message, width: 350, callback: null 
			});
		}
	}).done(function () { })
	.fail(function (e) { showAlert({ title: 'Cảnh báo', message: 'Có lỗi trong quá trình thực hiện truy vấn!' }); })
	.always(function () { shwLoading(false); });
}
function validate_numberPhone(mobile) {
	var vnf_regex = /((09|03|07|08|05)+([0-9]{8})\b)/g;
	if (mobile !== '') {
		if (vnf_regex.test(mobile) == false) {
			alert('Số điện thoại của bạn không đúng định dạng!');
			return true;
		}
	}
	return false;
}
//Validateemail
function validate_email(email) {
	var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	return re.test(email);
}
//Validate select -> option

function validate_selected(name, selected) {
	if (checkNullorEmpty(selected)) {
		alert("Vui lòng chọn " + name);
		return true;
	}
}
function validate_qa(security_question) {
	if (security_question.length <= 0) {
		alert("Vui lòng chọn câu hỏi bảo mật");
		return true;
	} else {
		security_question.forEach(function (element) {
			if (check_input("Câu hỏi bảo mật", element.QST_CODE)) {
				return true;
			} else if (check_input("Câu hỏi bảo mật", element.QST_NAME)) {
				return true;
			} else if (check_input("Câu trả lời", element.ASW_NAME)) {
				return true;
			}
		});
	}
	return false;
}
function save(status) {
	var security_question = [];
	$('.debit_sercurity_question:checked').each(function () {
		var item = {};
		item["QST_CODE"] = $(this).val();
		item["QST_NAME"] = $(this).attr('desc');
		item["ASW_NAME"] = $("#debit_security_answer_" + $(this).val()).val();
		security_question.push(item);
	});
	if (validate_qa(security_question)) {
		return;
	}
	if (window.isuingcards === undefined || window.isuingcards == null || window.isuingcards.length <= 0) {
		var isuingcards = {
			"ISSUING_CARDS_STATUS": status,
			"CLIENT_NO": $('#client_no').val(),
			"CLIENT_NAME": $('#client_name').val().trim(),
			"CARD_CLIENT_NAME": $('#card_client_name').val().trim(),
			"MAKER": $('#maker').val(), // Coi sua
			"GLOBAL_ID_TYPE": $("#global_id_type").children("option:selected").val(),
			"GLOBAL_ID": $('#global_id').val(),
			"sDT_OF_ISSUANCE": $('#dt_of_issuance').val(),
			"PLACE_OF_ISSUANCE": $('#place_of_issuance').val(),
			"sBIRTH_DATE": $('#birth_date').val(),
			"PHONE_HOME": $('#phone_home').val(),
			"PHONE_MOBILE": $('#phone_mobile').val(),
			"EMAIL": $('#email').val(),
			"ADDR_HK_CITY": $('#addr_hk_city').val(),
			"ADDR_HK_DISTRICT": $('#addr_hk_district').val(),
			"ADDR_HK_WARD": $('#addr_hk_ward').val(),
			"ADDR_HK_ADDR": $('#addr_hk_addr').val(),
			"ADDR_LH_CITY": $('#addr_lh_city').val(),
			"ADDR_LH_DISTRICT": $('#addr_lh_district').val(),
			"ADDR_LH_WARD": $('#addr_lh_ward').val(),
			"ADDR_LH_ADDR": $('#addr_lh_addr').val(),
			"CARD_TYPE": $('.card_type:checked').val(),
			"SECURITY_QUESTION": security_question,
			ATTACHMENTS: [{
				FILE_ORG: $("#fileOldName").val(),
				FILE_NAME: $("#fileName").val(),
				FILE_EXT: $("#fileExt").val(),
				FILE_SIZE: $("#fileSize").val(),
				FILE_PATH: $("#filePath").val(),
			}]
		};
		isuingcards["BRANCH"] = $('#debit_branch').val();
		isuingcards["SELLER"] = $('#debit_seller').val();
		isuingcards["ISSUING_CARD_PLACE"] = $('#debit_ISSUING_CARD_PLACE').children("option:selected").val();
		isuingcards["CONTRACTPRODUCTCODE"] = $('#debit_CONTRACTPRODUCTCODE').children("option:selected").val();
	    //isuingcards["ISSUING_FOR_CUSTOMER"] = $('#debit_ISSUING_FOR_CUSTOMER').children("option:selected").val();
		isuingcards["ISSUING_FOR_CUSTOMER"] = $('#debit_ISSUING_FOR_CUSTOMER').val();

		isuingcards["INCENTIVE_POLICY"] = $('#debit_INCENTIVE_POLICY').children("option:selected").val();
		isuingcards["sINCENTIVE_POLICY_FROM_DATE"] = $('#debit_INCENTIVE_POLICY_FROM_DATE').val();
		isuingcards["sINCENTIVE_POLICY_TO_DATE"] = $('#debit_INCENTIVE_POLICY_TO_DATE').val();
		isuingcards["INCENTIVE_POLICY_VALUE"] = $('#debit_INCENTIVE_POLICY_VALUE').val();

		isuingcards["RELEASE_TIME_TYPE"] = $('#debit_release_time_type').children("option:selected").val();
		//isuingcards["RELEASE_TIME_AMOUNT"] = $('#debit_release_time_amount').val();
		isuingcards["CRD_DEBIT_TKTT_TYPE"] = $('#debit_crd_debit_tktt_type:checked').val();
		//isuingcards["CRD_DEBIT_TKTT"] = $('#debit_crd_debit_tktt').children("option:selected").val();
		//isuingcards["CRD_DEBIT_TKTN"] = $('#debit_crd_debit_tktn').children("option:selected").val();
		//isuingcards["SECURITY_QUESTION"] = $('#debit_sercurity_question').val();
		//isuingcards["SECURITY_ANSWER"] = $('#debit_security_answer').val();
		isuingcards["TRANS_INTERNET"] = $('#debit_trans_internet:checked').val();
		isuingcards["CRD_DEBIT_RCV_NV"] = $('#debit_crd_debit_rcv_nv').val();

		isuingcards["SEX"] = $('#sex').children("option:selected").val();
		isuingcards["CLIENT_TYPE"] = $('#client_type').children("option:selected").val();
		isuingcards["INDUSTRY"] = $('#industry').children("option:selected").val();
		isuingcards["BUSINESS"] = $('#business').children("option:selected").val();
		isuingcards["CLASS_4"] = $('#class_4').children("option:selected").val();
		isuingcards["CLASS_5"] = $('#class_5').children("option:selected").val();
		isuingcards["RESIDENT_STATUS"] = $('#resident_status').children("option:selected").val();

        //use SMS
		isuingcards["SMS_USE"] = $('#USE_SMS').prop('checked') ? "Y" : "N";
		isuingcards["SMS_PHONE"] = $('#SMS_PHONE').val();
		isuingcards["SMS_OPTION"] = $('#cbSMS_OPTION').val();
		isuingcards["SMS_FEE"] = $('#txtAmount_SMS').val();
		isuingcards["SMS_TARIFF"] = $('#cbSMS_OPTION').val();

		if (isuingcards.SMS_USE == 'Y') {
		    if (isuingcards["SMS_PHONE"] == '') {
		        shwMsg({ title: 'Thông báo', message: 'Vui lòng nhập số điện thoại!' });
		        return false;
		    }

		    if (isuingcards["SMS_PHONE"] == '') {
		        shwMsg({ title: 'Thông báo', message: 'Vui lòng nhập số điện thoại!' });
		        return false;
		    }

		    if (validate_numberPhone(isuingcards["SMS_PHONE"])) {
		        return false;
		    }

		    if (isuingcards["SMS_OPTION"] == '') {
		        shwMsg({ title: 'Thông báo', message: 'Vui lòng chọn Tariff và số tiền!' });
		        return false;
		    }
		}
		

		/// comment lát mở ra
		isuingcards["CONTRACTPRODUCTCODE_NAME"] = $("#debit_CONTRACTPRODUCTCODE option:selected").text();
	    //checkEmail
		if (isuingcards.CLIENT_NO == '') {
		    if (check_input("email", isuingcards.EMAIL, 50)) {
		        return false;
		    }

		    if (check_input("số điện thoại di động", isuingcards.PHONE_MOBILE, 12)) {
		        return false;
		    }
		}

		if (check_input("họ tên chủ thẻ", isuingcards.CLIENT_NAME, 50) ||
			check_input("họ tên chủ thẻ (dập nổi/in chìm)", isuingcards.CARD_CLIENT_NAME, 21) ||
			check_input("mã nhân viên", isuingcards.MAKER, 20) ||
			check_input("số id", isuingcards.GLOBAL_ID, 16) ||
			check_input("nơp cấp id", isuingcards.PLACE_OF_ISSUANCE, 30) ||
			check_input("số điện thoại cố định", isuingcards.PHONE_HOME, 12) ||
			//check_input("số điện thoại di động", isuingcards.PHONE_MOBILE, 12) ||
			//check_input("email", isuingcards.EMAIL, 50) ||
			check_input("tỉnh/tp hộ khẩu", isuingcards.ADDR_HK_CITY, 140) ||
			check_input("quận/huyện hộ khẩu", isuingcards.ADDR_HK_DISTRICT, 140) ||
			check_input("phường/xã hộ khẩu", isuingcards.ADDR_HK_WARD, 140) ||
			check_input("số nhà & tên đường hộ khẩu", isuingcards.ADDR_HK_ADDR, 140) ||
			check_input("tỉnh/tp liên lạc", isuingcards.ADDR_LH_CITY, 140) ||
			check_input("quận/huyện liên lạc", isuingcards.ADDR_LH_DISTRICT, 140) ||
			check_input("phường/xã liên lạc", isuingcards.ADDR_LH_WARD, 140) ||
			check_input("số nhà & tên đường liên lạc", isuingcards.ADDR_LH_ADDR, 140) ||
			check_input("Họ tên nhân viên ", isuingcards.CRD_DEBIT_RCV_NV, 20)) {
			return;
		}
		if (checkdatetime("ngày sinh ", isuingcards.sBIRTH_DATE) || checkdatetime("ngày cấp ", isuingcards.sDT_OF_ISSUANCE)
			) {
			return;
		}
		//if (validate_attachments(isuingcards.ATTACHMENTS)) {
		//	return;
		//}
		if (checkNullorEmpty(isuingcards.sINCENTIVE_POLICY_FROM_DATE)) {
			alert("Từ ngày ưu đãi đang bỏ trống"); // Cần thay đổi alert;
			return true;
		}
		if (checkNullorEmpty(isuingcards.sINCENTIVE_POLICY_TO_DATE)) {
			alert("Ô đến ngày ưu đãi đang bỏ trống!"); // Cần thay đổi alert;
			return true;
		}
		if (!isValidDate(isuingcards.sINCENTIVE_POLICY_FROM_DATE)) {
			alert("Sai định dạng ô từ ngày ưu đãi"); // Cần thay đổi alert;
			return true;
		}
		if (!isValidDate(isuingcards.sINCENTIVE_POLICY_TO_DATE)) {
			alert("Sai định dạng ô đến ngày ưu đãi"); // Cần thay đổi alert;
			return true;
		}

		if (validate_numberPhone(isuingcards.PHONE_MOBILE) || validate_numberPhone(isuingcards.PHONE_HOME)) {
			return;
		}
		if (!validate_email(isuingcards.EMAIL)) {
			alert("email sai định dạng!!!");
			return;
		}
		if (
			validate_selected("Mã nhân viên bán hàng", isuingcards.SELLER) ||
			validate_selected("Nơi dập thẻ", isuingcards.ISSUING_CARD_PLACE) ||
			validate_selected("Sản phẩm thẻ", isuingcards.CONTRACTPRODUCTCODE) ||
			validate_selected("Đối tượng khách hàng: ", isuingcards.ISSUING_FOR_CUSTOMER) ||
			validate_selected("Chính sách ưu đãi", isuingcards.INCENTIVE_POLICY) ||
			validate_selected("Loại ID", isuingcards.GLOBAL_ID_TYPE) ||
			validate_selected("Thời gian phát hành", isuingcards.RELEASE_TIME_TYPE) ||
			check_input("Giá trị ưu đãi ", isuingcards.INCENTIVE_POLICY_VALUE, 20) ||
			//added 10/04/19
			validate_selected("SEX", isuingcards.SEX) ||
            validate_selected("CLIENT_TYPE", isuingcards.CLIENT_TYPE) ||
            validate_selected("INDUSTRY", isuingcards.INDUSTRY) ||
            validate_selected("BUSINESS", isuingcards.BUSINESS) ||
            validate_selected("CLASS_4", isuingcards.CLASS_4) ||
            validate_selected("CLASS_5", isuingcards.CLASS_5) ||
            validate_selected("RESIDENT_STATUS", isuingcards.RESIDENT_STATUS)
			) {
			return;
		}
		if (checkNullorEmpty(isuingcards.trans_internet)) {
			isuingcards.trans_internet = "";
		}
		//if ($('.TKTGTT:checked').val() == 2 && checkNullorEmpty(isuingcards.CRD_DEBIT_TKTT)) {
		//    alert("Vui lòng nhập tài khoản trung gian!");
		//    return;
		//}
		//if ($('.TKTGTN:checked').val() == 2 && checkNullorEmpty(isuingcards.CRD_DEBIT_TKTN)) {
		//    alert("Vui lòng nhập tài khoản trung gian!");
		//    return;
		//}
		//if (isuingcards.RELEASE_TIME_TYPE == 2 && checkNullorEmpty(isuingcards.RELEASE_TIME_AMOUNT)) {
		//    alert("Vui lòng nhập phí!");
		//    return;
		//}
		if (!validate_email(isuingcards.EMAIL)) {
			alert("email sai định dạng!!!");
			return;
		}
		send(isuingcards);
	}
	else {
		window.isuingcards.ISSUING_CARDS_STATUS = status;
		window.isuingcards["CARD_TYPE"] = $('.card_type:checked').val();
		window.isuingcards["BRANCH"] = $('#debit_branch').val();
		window.isuingcards["SELLER"] = $('#debit_seller').val();
		window.isuingcards["ISSUING_CARD_PLACE"] = $('#debit_ISSUING_CARD_PLACE').children("option:selected").val();
		//window.isuingcards["CARDPRODUCTCODE"] = $('#debit_CARDPRODUCTCODE').children("option:selected").val();
	    //window.isuingcards["ISSUING_FOR_CUSTOMER"] = $('#debit_ISSUING_FOR_CUSTOMER').children("option:selected").val();
		window.isuingcards["ISSUING_FOR_CUSTOMER"] = $('#debit_ISSUING_FOR_CUSTOMER').val();

		window.isuingcards["INCENTIVE_POLICY"] = $('#debit_INCENTIVE_POLICY').children("option:selected").val();
		window.isuingcards["sINCENTIVE_POLICY_FROM_DATE"] = $('#debit_INCENTIVE_POLICY_FROM_DATE').val();
		window.isuingcards["sINCENTIVE_POLICY_TO_DATE"] = $('#debit_INCENTIVE_POLICY_TO_DATE').val();
		window.isuingcards["INCENTIVE_POLICY_VALUE"] = $('#debit_INCENTIVE_POLICY_VALUE').val();

		window.isuingcards["RELEASE_TIME_TYPE"] = $('#debit_release_time_type').children("option:selected").val();
		window.isuingcards["RELEASE_TIME_AMOUNT"] = $('#debit_release_time_amount').val();
		window.isuingcards["CRD_DEBIT_TKTT_TYPE"] = $('#debit_crd_debit_tktt_type:checked').val();
		window.isuingcards["CRD_DEBIT_TKTT"] = $('#debit_crd_debit_tktt').children("option:selected").val();
		window.isuingcards["CRD_DEBIT_TKTN"] = $('#debit_crd_debit_tktn').children("option:selected").val();
		window.isuingcards["SECURITY_QUESTION"] = security_question;
		//window.isuingcards["SECURITY_ANSWER"] = $('#debit_security_answer').val();
		window.isuingcards["TRANS_INTERNET"] = $('#debit_trans_internet:checked').val();
		window.isuingcards["CRD_DEBIT_RCV_NV"] = $('#debit_crd_debit_rcv_nv').val();
		window.isuingcards["CONTRACTPRODUCTCODE"] = $('#debit_CONTRACTPRODUCTCODE').children("option:selected").val();
		window.isuingcards["CONTRACTPRODUCTCODE_NAME"] = $("#debit_CONTRACTPRODUCTCODE option:selected").text();
		window.isuingcards["ATTCHMENT"] = $('#imgUpload').attr('src');

	    //use SMS
		window.isuingcards["SMS_SMS"] = $('#USE_SMS').prop('checked') ? "Y" : "N";
		window.isuingcards["SMS_PHONE"] = $('#SMS_PHONE').val();
		window.isuingcards["SMS_OPTION"] = $('#cbSMS_OPTION').val();
		window.isuingcards["SMS_FEE"] = $('#txtAmount_SMS').val();
		window.isuingcards["SMS_TARIFF"] = $('#cbSMS_OPTION').val();

		if (window.isuingcards.SMS_SMS == 'Y') {
		    if (window.isuingcards["SMS_PHONE"] == '') {
		        shwMsg({ title: 'Thông báo', message: 'Vui lòng nhập số điện thoại!' });
		        return false;
		    }

		    if (window.isuingcards["SMS_PHONE"] == '') {
		        shwMsg({ title: 'Thông báo', message: 'Vui lòng nhập số điện thoại!' });
		        return false;
		    }

		    if (validate_numberPhone(window.isuingcards["SMS_PHONE"])) {
		        return false;
		    }

		    if (window.isuingcards["SMS_OPTION"] == '') {
		        shwMsg({ title: 'Thông báo', message: 'Vui lòng chọn Tariff và số tiền!' });
		        return false;
		    }
		}

		if (isuingcards.CLIENT_NO == '') {
		    if (check_input("email", isuingcards.EMAIL, 50)) {
		        return false;
		    }

		    if (check_input("số điện thoại di động", isuingcards.PHONE_MOBILE, 12)) {
		        return false;
		    }
		}

		if (
            //check_input("email", window.isuingcards.EMAIL, 50) ||
            //check_input("số điện thoại di động", window.isuingcards.PHONE_MOBILE, 12) ||
			check_input("số nhà & tên đường hộ khẩu", window.isuingcards.ADDR_HK_ADDR, 140) || check_input("số nhà & tên đường liên lạc", window.isuingcards.ADDR_LH_ADDR, 140) ||
			 check_input("Họ tên nhân viên ", window.isuingcards.CRD_DEBIT_RCV_NV, 20)) {
			return;
		}
		if (
			validate_selected("Mã nhân viên bán hàng", window.isuingcards.SELLER) || validate_selected("Nơi dập thẻ", window.isuingcards.ISSUING_CARD_PLACE) ||
			validate_selected("Sản phẩm thẻ", window.isuingcards.CONTRACTPRODUCTCODE) || validate_selected("Đối tượng khách hàng: ", window.isuingcards.ISSUING_FOR_CUSTOMER) ||
			validate_selected("Chính sách ưu đãi", window.isuingcards.INCENTIVE_POLICY) || validate_selected("Thời gian phát hành", window.isuingcards.RELEASE_TIME_TYPE) ||
			check_input("Giá trị ưu đãi ", window.isuingcards.INCENTIVE_POLICY_VALUE, 20)
			) {
			return;
		}
		if (checkNullorEmpty(window.isuingcards.sINCENTIVE_POLICY_FROM_DATE)) {
			alert("Từ ngày ưu đãi đang bỏ trống"); // Cần thay đổi alert;
			return true;
		}
		if (checkNullorEmpty(window.isuingcards.sINCENTIVE_POLICY_TO_DATE)) {
			alert("Ô đến ngày ưu đãi đang bỏ trống!"); // Cần thay đổi alert;
			return true;
		}
		if (!isValidDate(window.isuingcards.sINCENTIVE_POLICY_FROM_DATE)) {
			alert("Sai định dạng ô từ ngày ưu đãi"); // Cần thay đổi alert;
			return true;
		}
		if (!isValidDate(window.isuingcards.sINCENTIVE_POLICY_TO_DATE)) {
			alert("Sai định dạng ô đến ngày ưu đãi"); // Cần thay đổi alert;
			return true;
		}

		if (checkNullorEmpty(window.isuingcards.TRANS_INTERNET)) {
			window.isuingcards.TRANS_INTERNET = "";
		}
		if ($('.TKTGTT:checked').val() == 2 && checkNullorEmpty(window.isuingcards.CRD_DEBIT_TKTT)) {
			alert("Vui lòng chọn tài khoản trung gian!");
			return;
		}
		if ($('.TKTGTN:checked').val() == 2 && checkNullorEmpty(window.isuingcards.CRD_DEBIT_TKTN)) {
			alert("Vui lòng chọn tài khoản trích nợ!");
			return;
		}
		//if (window.isuingcards.RELEASE_TIME_TYPE == 'FTS' && checkNullorEmpty(window.isuingcards.RELEASE_TIME_AMOUNT)) {
		//    alert("Vui lòng nhập phí!");
		//    return;
		//} else if (window.isuingcards.RELEASE_TIME_TYPE == 'NRM') {
		//    window.isuingcards.RELEASE_TIME_TYPE == "0";
		//}
		if (!validate_email(window.isuingcards.EMAIL)) {
			alert("email sai định dạng!!!");
			return;
		}
		send(window.isuingcards);
	}
}
//Load seller
var objDlg;
function search_seller() {
	shwLoading(true);
	$.post('/MainIssuingCard/searchSeller', function (data, status) {
		objDlg = $("#dlgReview").dialog({
			title: "Tìm kiếm thông tin nhân viên bán hàng",
			modal: true,
			width: '50%',
			height: '400',//'auto',
			resizable: true,
			open: function () {
				$('#dlgReviewWrapper').html(data);
				//InitDataTableDLG();
			},
			position: { my: "center", at: "center", of: window },
			buttons: [
				{
					text: "Thoát",
					click: function () {
						$(objDlg).dialog("close");
					}
				}
			]
		});
	}).done(function () { })
	.fail(function (e) { showAlert({ title: 'Cảnh báo', message: 'Có lỗi trong quá trình thực hiện truy vấn!' }); })
	.always(function () { shwLoading(false); });
};
function query_seller() {
	var q = $("#emp_code_seller").val();
	$.post('/MainIssuingCard/querySeller', {
		q: q
	}, function (data, status) {
		console.log(data);
		debugger;
		if (data.code !== undefined || data.code != null)
			$('tbody').html("<tr><td colspan=2 style='text-align:center'>" + data.message + "</td></tr>");
		else {
			var str = "";
			data.forEach(function (item) {
				str = str + "<tr><td><a class='seller_code' style='cursor: pointer; color: #3c8dbc;' onclick=" + "\"" + "saler_onclick(\'" + item.CODE + "\')" + "\"" + ">" + item.CODE + "</a></td><td>" + item.NAME + "</td></tr>";
			});
			$('tbody').html(str);
		}
	});
}

function saler_onclick(q) {
	$('#debit_seller').val(q);
	$("#dlgReview").dialog("close");
}

// Kiểm tra file
function validate_attachments(attachments) {
	for (var i = 0; i < attachments.length; i++) {
		var element = attachments[i];
		if (checkNullorEmpty(element.FILE_ORG) || checkNullorEmpty(element.FILE_NAME) ||
						checkNullorEmpty(element.FILE_EXT) || checkNullorEmpty(element.FILE_SIZE) || checkNullorEmpty(element.FILE_PATH)) {
			alert(name + " Chưa upload file!"); // Cần thay đổi alert;
			return true;
		}
	}
	return false;
}
