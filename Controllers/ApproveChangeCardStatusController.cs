using ISSUING_APP.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThuHoTaiQuay.Authencation;

namespace ISSUING_APP.Controllers
{
    public class ApproveChangeCardStatusController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: ApproveLockCard
        //[CustomAuthorize(Roles = Roles.BASE_ROLE)]
        public ActionResult Index(string id)
        {

            return View();
        }
        [CustomAuthorize(Roles = Roles.BASE_ROLE)]
        public ActionResult SearchDataApproveLockCard(string OP_LOCKCARD_ID, string OP_LOCK_TYPE, string OP_LOCK_CODE, string OP_LOCK_STATUS,
    string OP_MAKER, string OP_BRANCH_NO, string OP_CLIENT_NUMBER, string OP_CLIENT_NAME, string OP_CLIENT_TEXT, string OP_CARD_NUMBER,
    string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_LOCK_FROM, string OP_LOCK_TO)
        {
            UtilsCard _util = new UtilsCard();
            List<LockCard> data = _util.LoadDataApproveLockCard(OP_LOCKCARD_ID, OP_LOCK_TYPE, OP_LOCK_CODE, OP_LOCK_STATUS, OP_MAKER,
             OP_BRANCH_NO, OP_CLIENT_NUMBER, OP_CLIENT_NAME, OP_CLIENT_TEXT, OP_CARD_NUMBER, OP_CARD_NUMBER_A, OP_CARD_NUMBER_B, OP_LOCK_FROM, OP_LOCK_TO);
            ViewData["lstBranch"] = Utils.LoadBranchs();

            //data.RemoveAll(x => x.CLIENT_NUMBER.Contains("FINX"));

            return (ActionResult)PartialView("ApproveLockCard_PartialPage", data);
        }
        [CustomAuthorize(Roles = Roles.BASE_ROLE)]
        public ActionResult doExportExcel(string OP_LOCKCARD_ID, string OP_LOCK_TYPE, string OP_LOCK_CODE, string OP_LOCK_STATUS,
          string OP_MAKER, string OP_BRANCH_NO, string OP_CLIENT_NUMBER, string OP_CLIENT_NAME
          , string OP_CLIENT_TEXT, string OP_CARD_NUMBER, string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_LOCK_FROM, string OP_LOCK_TO)
        {
            UtilsCard _util = new UtilsCard();
            List<LockCard> data = _util.LoadDataApproveLockCard(OP_LOCKCARD_ID, OP_LOCK_TYPE, OP_LOCK_CODE, OP_LOCK_STATUS, OP_MAKER,
             OP_BRANCH_NO, OP_CLIENT_NUMBER, OP_CLIENT_NAME, OP_CLIENT_TEXT, OP_CARD_NUMBER, OP_CARD_NUMBER_A, OP_CARD_NUMBER_B, OP_LOCK_FROM, OP_LOCK_TO);

            string path = Server.MapPath("~/Upload/LockCardTemplate.xls");
            excelTools _excelTools = new excelTools();

            DataTable table = _excelTools.ConvertToDataTable<LockCard>(data,
                new string[] {"LOCK_TYPE",
                                "LOCK_CODE",
                                "LOCK_STATUS",
                                "CLIENT_SHORT_NAME",
                                "CLIENT_NUMBER",
                                "CONTRACT_NUMBER",
                                "CONTRACT_NAME",
                                "CONTR_TYPE_NAME",
                                "LOCK_FROM",
                                "LOCK_TO",
                                "BLD_NAME",
                                "MAKER",
                                "MAKE_DATE",
                                "CHECKER",
                                "CHECK_DATE",
                                "NOTE",
                                "MAKER_BRANCH",
                                "CLIENT_DATE_OPEN",
                                "CLIENT_BIRTH_DATE",
                                "CLIENT_PHONE",
                                "CLIENT_PHONE_H",
                                "CLIENT_PHONE_M",
                                "CLIENT_E_MAIL",
                                "CLIENT_ADDRESS_LINE",
                                "INTERVAL_TYPE",
                                "CONTR_STATUS",
                                "DATE_OPEN",
                                "CARD_EXPIRE",
                                "PRODUCT",
                                "PRODUCTION_STATUS",
                                "PRODUCT_NAME",
                                "CARD_BRANCH_NO",
                                "CARD_BRANCH_NAME",
                                "CLIENT_BRANCH_NO",
                                "RESULT_CODE",
                                "RESULT_MESSAGE"
                });
            foreach (DataRow row in table.Rows)
            {
                row["LOCK_TYPE"] = row["LOCK_TYPE"].ToString().Equals("1") ? "Khóa thẻ" : "Mở khóa thẻ";
                row["LOCK_CODE"] = row["LOCK_CODE"].ToString().Equals("1") ? "Tạm khóa" : "Khóa vĩnh viễn";
                string LOCK_STATUS = "";
                switch (row["LOCK_STATUS"].ToString())
                {
                    case "0": LOCK_STATUS = "Tạo mới"; break;
                    case "1": LOCK_STATUS = "Chờ duyệt"; break;
                    case "2": LOCK_STATUS = "Đã duyệt"; break;
                    case "3": LOCK_STATUS = "Đã hủy"; break;
                }
                row["LOCK_STATUS"] = LOCK_STATUS;
            }
            MemoryStream ms = _excelTools.ExportFileExcel(table, path, 2);
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "LockCard.xls");

        }
        [CustomAuthorize(Roles = ThuHoTaiQuay.Authencation.Roles.BASE_ROLE)]
        public ActionResult SearchDataApproveLockCardByID(string OP_LOCKCARD_ID)
        {
            UtilsCard _util = new UtilsCard();
            List<LockCard> data = _util.LoadDataApproveLockCard(OP_LOCKCARD_ID, null, null, null, null, null, null, null, null, null, null, null, null, null);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CustomAuthorize(Roles = Roles.BASE_ROLE)]
		public ActionResult SaveApproveLockCard(string P_CARD_NUMBER, string P_LOCKCARD_ID, string P_NOTE, string P_LOCK_STATUS)
		{
			SESSION_PARA oPra = CShared.getSession();
			UtilsCard _util = new UtilsCard();

			List<LockCard> infoPortal = _util.LoadDataApproveLockCard(P_LOCKCARD_ID, null, null, null, null, null, null, null, null, null, null, null, null, null);
			Result rs = new Result();
			if (oPra.oAccount == null)
			{
				rs.code = "99";
				rs.message = "Vui lòng đăng nhập lại";
				return Json(rs, JsonRequestBehavior.AllowGet);
			}
			string OP_MAKER_BRANCH = oPra.oAccount.Branch;
			string OP_CHECKER = oPra.oAccount.UserName;
			if (infoPortal[0].LOCK_TYPE_OLD == "C055" && infoPortal[0].LOCK_TYPE == "C00" && infoPortal[0].CARD_PRODUCT_CODE.Contains("_CR_") && infoPortal[0].CARD_PRODUCT_CODE.Contains("_M"))
			{
				string result = _util.CheckResultIsuingCardLos(infoPortal[0].CONTRACT_NUMBER, infoPortal[0].CARD_PRODUCT_CODE);
				{
					if (result != "00")
					{
						rs.code = "001";
						rs.message = "Thẻ chưa được nhập liệu hoàn tất trên Card LOS";
					}
				}
			}
			if (P_LOCK_STATUS == "2" && infoPortal[0].LOCK_TYPE == infoPortal[0].LOCK_TYPE_OLD && infoPortal[0].LOCK_TYPE == "C053")
			{
				rs = _util.InsertTableLockCardECRM(infoPortal[0]);
				if (rs.code == "00")
				{
					rs.message = "Duyệt cập nhật thời gian khóa thẻ thành công";
					_util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, "0", "Success");
				}
				else
				{
					rs.message = "Duyệt cập nhật thời gian khóa thẻ thất bại khi cập nhật dư liệu ECRM";
					_util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, "01", "Update ECRM Error");
				}
				return Json(rs, JsonRequestBehavior.AllowGet);
			}
			else if (P_LOCK_STATUS == "2")
			{
				if ((infoPortal[0].CARD_PRODUCT_CODE.Contains("_CR_") || infoPortal[0].PRODUCT_NAME.ToLower().Contains("corporate")) && infoPortal[0].LOCK_TYPE == "C054")
				{
					// check last main card TD + DN

					CustomerServices CS = new CustomerServices();
					List<CardInfo> listCard = CS.LoadCards(infoPortal[0].LOC_ACCT);
					bool notLastCard = listCard.Exists(x => x.CONTRACT_NUMBER != infoPortal[0].CONTRACT_NUMBER && x.CONTR_STATUS != "Cancelled");
					bool existSubCard = listCard.Exists(x => x.CARD_PRODUCT_CODE.EndsWith("_S") && x.CONTR_STATUS != "Cancelled");
					bool isMain = infoPortal[0].CARD_PRODUCT_CODE.EndsWith("_M");

					if (isMain && existSubCard)
					{
						rs.code = "E001";
						rs.message = "Vui lòng hủy tất cả thẻ phụ/thẻ đã thay thế trước khi hủy thẻ chính và đóng hợp đồng!";

						_util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, rs.code, rs.message);
						return Json(rs, JsonRequestBehavior.AllowGet);
					}

					// true meant have another card not Cancelled
					if (notLastCard && infoPortal[0].IS_CLOSE_CONTRACT == "Y")
					{
						rs.code = "E001";
						rs.message = "Vui lòng hủy tất cả thẻ phụ/thẻ đã thay thế trước khi hủy thẻ chính và đóng hợp đồng!";

						_util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, rs.code, rs.message);
						return Json(rs, JsonRequestBehavior.AllowGet);
					}
					if (!notLastCard && infoPortal[0].IS_CLOSE_CONTRACT == "N")
					{
						rs.code = "E002";
						rs.message = "Yêu cầu hủy thông tin contract!";

						_util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, rs.code, rs.message);
						return Json(rs, JsonRequestBehavior.AllowGet);
					}
				}
				//4. Build Message API
				ReqMessageLockCard request = new ReqMessageLockCard
				{
					Channel = CShared.Channel,// "PORTAL_ISSUING",
					PartnerId = CShared.PartnerId,// "PORTAL",
					ServiceCode = "28",
					RequestId = infoPortal[0].LOCKCARD_ID,
					RequestTime = DateTime.Now.Ticks.ToString()
				};
				request.Extras = new Dictionary<string, object>();
				request.Extras.Add("cardNumber", infoPortal[0].CONTRACT_NUMBER);

				if (infoPortal[0].LOCK_TYPE == "C14")
				{
					request.Extras.Add("statusCode", "C053");
				}
				else
				{
					request.Extras.Add("statusCode", infoPortal[0].LOCK_TYPE);
				}

				string reason = "PORTAL" + "-" + infoPortal[0].MAKER_BRANCH + "-" + infoPortal[0].MAKER + "-" + OP_CHECKER + "-" + infoPortal[0].REASON;
				int length = reason.Length;
				if (length > 255)
				{
					length = 255;
				}
				request.Extras.Add("reason", reason.Substring(0, length));

				//added by leln2 09/10/2019 closed contract
				if (infoPortal[0].IS_CLOSE_CONTRACT == "Y" && infoPortal[0].LOCK_TYPE == "C054")
				{
					request.Extras.Add("contractNumber", infoPortal[0].LOC_ACCT);
					request.Extras.Add("contractStatus", "A14");
				}
				// end;

				log.InfoFormat("[ApproveLockCard][SaveApproveLockCard] ===>>>>>> request = {0}", Newtonsoft.Json.JsonConvert.SerializeObject(request));
				AuditLog.Log(CShared.GetIPAddress(), CShared.getSession().oAccount.Branch, CShared.getSession().oAccount.Tellerid, "ApproveChangeCardStatusController", "SaveApproveLockCard", "Send Request Change Status Card", infoPortal[0].LOCKCARD_ID, request);
				GWSocket_Way4 socket = new GWSocket_Way4();
				var response = socket.SendAndReceive(request);
				AuditLog.Log(CShared.GetIPAddress(), CShared.getSession().oAccount.Branch, CShared.getSession().oAccount.Tellerid, "ApproveChangeCardStatusController", "SaveApproveLockCard", "Send Request Change Status Card", infoPortal[0].LOCKCARD_ID, response);
				log.InfoFormat("[ApproveLockCard][SaveApproveLockCard] ===<<<<<< response = {0}", Newtonsoft.Json.JsonConvert.SerializeObject(response));

				if (response == null)
				{
					rs.code = "99";
					rs.message = "Không nhận được phản hồi từ Way4";
					rs.para2 = Newtonsoft.Json.JsonConvert.SerializeObject(request);
					return Json(rs, JsonRequestBehavior.AllowGet);
				}
				if (response.ResponseCode == "0")
				{
					if (infoPortal[0].IS_CLOSE_CONTRACT == "Y" && infoPortal[0].CARD_PRODUCT_CODE.Contains("_CR_"))
					{
						// LELN2 ADDED 31/12/2019 COMMENT COLLAT_REF IN HDB_ISSUING_CARDS
						_util.CommentCollatRef(infoPortal[0].ACNT_CONTRACT_ID);
					}
					rs = _util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, response.ResponseCode, response.ResponseMessage);
					if (rs.code == "00")
					{
						if (infoPortal[0].LOCK_TYPE == "C053")
						{
							rs = _util.InsertTableLockCardECRM(infoPortal[0]);
							if (rs.code == "00")
							{
								rs.message = "Duyệt thay đổi trạng thái thẻ thành công";
							}
							else
							{
								rs.message = "Duyệt thay đổi trạng thái thẻ thành công trên Way4, Portal nhưng thất bại khi cập nhật dư liệu ECRM";
							}
						}
						else if (infoPortal[0].LOCK_TYPE == "C00")
						{
							rs = _util.UpdateTableLockCardECRM(infoPortal[0]);
							if (rs.code == "00")
							{
								rs.message = "Duyệt thay đổi trạng thái thẻ thành công";
							}
							else
							{
								rs.message = "Duyệt thay đổi trạng thái thẻ thành công trên Way4, Portal nhưng lỗi khi cập nhật dữ liệu ECRM";
							}
						}
						else
						{
							rs.message = "Duyệt thay đổi trạng thái thẻ thành công";
						}

					}
					else
					{
						rs.message = "Duyệt thay đổi trạng thái thẻ thành công trên Way4, nhưng lỗi khi lưu dữ liệu Portal";
					}
					return Json(rs, JsonRequestBehavior.AllowGet);
				}
				else
				{
					rs = _util.SaveApproveLockCardStatus(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS, response.ResponseCode, response.ResponseMessage);
					if (rs.code == "00")
					{
						rs.message = "Duyệt thay đổi trạng thái thẻ thất bại khi gọi WAY4";
					}
					else
					{
						rs.message = " Duyệt thay đổi trạng thái thẻ thất bại khi gọi Way4 và lỗi khi lưu dữ liệu Portal";
					}
					rs.para2 = Newtonsoft.Json.JsonConvert.SerializeObject(request);
					rs.para3 = Newtonsoft.Json.JsonConvert.SerializeObject(response);
					return Json(rs, JsonRequestBehavior.AllowGet);
				}
			}
			else
			{
				rs = _util.SaveApproveLockCard(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS);
				if (rs.code == "00")
				{
					rs.message = "Hủy thay đổi trạng thái thành công";
				}
				return Json(rs, JsonRequestBehavior.AllowGet);
			}
		}

		[CustomAuthorize(Roles = ThuHoTaiQuay.Authencation.Roles.BASE_ROLE)]
        public ActionResult GetDataCardInfoByCardID(string P_LOCKCARD_ID, string OP_CARD_TYPE)
        {
            UtilsCard _util = new UtilsCard();
            List<LockCard> data = _util.LoadDataApproveLockCard(P_LOCKCARD_ID, null, null, null, null, null, null, null, null, null, null, null, null, null);
            return (ActionResult)PartialView("ApproveCardDialog_PartialPage", data);
        }
    }

}