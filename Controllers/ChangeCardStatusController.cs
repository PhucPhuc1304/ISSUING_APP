using ISSUING_APP.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThuHoTaiQuay.Authencation;

namespace ISSUING_APP.Controllers
{
    public class ChangeCardStatusController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //[CustomAuthorize(Roles = Roles.BASE_ROLE)]
        public ActionResult Index()
        {
            ViewBag.Menu = CShared.GenerateMenu(Request, (String)RouteData.Values["id"]);
            return View();
        }
        //[CustomAuthorize(Roles = Roles.BASE_ROLE)]
        public ActionResult SearchData(string OP_CARD_BRANCH, string OP_CLIENT_TEXT, string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_CARD_DATE_FROM, string OP_CARD_DATE_TO, string OP_CARD_NUMBER)
        {

            return (ActionResult)PartialView("ViewAndChangeStatus");

        }

        [HttpPost]
        [CustomAuthorize(Roles = Roles.BASE_ROLE)]
		public ActionResult SaveAction(string OP_ACNT_CONTRACT_ID, string P_LOCKCARD_ID, string P_LOCK_TYPE, string P_LOCK_CODE, string P_LOCK_FROM, string P_LOCK_TO, string P_NOTE
		 , string P_BLD_CODE, string P_YC_FILENAME, string P_YC_FILEPATH, string P_REASON, string P_REG_NUMBER, string P_CONTR_STATUS_NAME,
		string LOC_ACCT, string CONTR_STATUS_ACCT_CODE, string LOC_ACCT_STATUS, string CARD_PRODUCT_CODE, string IS_CLOSE_CONTRACT, string LOCK_TYPE_OLD)
		{
			Result rs = new Result();
			SESSION_PARA oPra = CShared.getSession();
			UtilsCard _util = new UtilsCard();
			string OP_MAKER_BRANCH = oPra.oAccount.Branch;
			string OP_MAKER = oPra.oAccount.UserName;
			if (string.IsNullOrEmpty(P_LOCK_TYPE))
			{
				rs.code = "01";
				rs.message = "Vui lòng chọn trạng thái muốn thay đổi!";
				return Json(rs, JsonRequestBehavior.AllowGet);
			}
			if (string.IsNullOrEmpty(P_REASON))
			{
				rs.code = "01";
				rs.message = "Vui lòng nhập lý do thay đổi!";
				return Json(rs, JsonRequestBehavior.AllowGet);
			}
			if (P_REASON.Length >= 200)
			{
				rs.code = "01";
				rs.message = "Vui lòng nhập lý do dưới 200 ký tự!";
				return Json(rs, JsonRequestBehavior.AllowGet);
			}

			if (LOCK_TYPE_OLD == "C055" && P_LOCK_CODE == "C00" && CARD_PRODUCT_CODE.Contains("_CR_") && CARD_PRODUCT_CODE.Contains("_M"))
			{
				string result = _util.CheckResultIsuingCardLos(LOC_ACCT, CARD_PRODUCT_CODE);
				if (result != "00")
				{
					rs.code = "001";
					rs.message = "Thẻ chưa được nhập liệu hoàn tất trên Card LOS";
					return Json(rs, JsonRequestBehavior.AllowGet);
				}
			}


			rs = _util.SaveLockCard(OP_MAKER_BRANCH, OP_MAKER, OP_ACNT_CONTRACT_ID, ref P_LOCKCARD_ID, P_LOCK_TYPE, P_LOCK_CODE, P_LOCK_FROM, P_LOCK_TO, P_NOTE
				, P_BLD_CODE, P_YC_FILENAME, P_YC_FILEPATH, P_REASON, P_REG_NUMBER, P_CONTR_STATUS_NAME, LOC_ACCT, CONTR_STATUS_ACCT_CODE, LOC_ACCT_STATUS, CARD_PRODUCT_CODE, IS_CLOSE_CONTRACT, LOCK_TYPE_OLD);

			if (rs.code != "00")
			{
				return Json(rs, JsonRequestBehavior.AllowGet);
			}
			else
			{
				rs.message = "Chuyển duyệt thành công";
			}
			if ((oPra.oAccount.Roles.Contains("ttt-input") || oPra.oAccount.Roles.Contains("ttt-review") || oPra.oAccount.Roles.Contains("dvkh")) && rs.code == "00")
			{
				string P_LOCK_STATUS = "2";
				string OP_CHECKER = OP_MAKER;
				List<LockCard> infoPortal = _util.LoadDataApproveLockCard(P_LOCKCARD_ID, null, null, null, null, null, null, null, null, null, null, null, null, null);
				if (infoPortal[0].LOCK_TYPE == infoPortal[0].LOCK_TYPE_OLD && infoPortal[0].LOCK_TYPE == "C053")
				{
					rs = _util.SaveApproveLockCard(OP_MAKER_BRANCH, OP_CHECKER, P_LOCKCARD_ID, P_NOTE, P_LOCK_STATUS);
					if (rs.code == "00")
					{
						rs = _util.InsertTableLockCardECRM(infoPortal[0]);
						if (rs.code == "00")
						{
							rs.message = "Duyệt cập nhật thời gian khóa thẻ thành công";
						}
						else
						{
							rs.message = "Duyệt cập nhật thời gian khóa thẻ đã lưu thành công trên Portal, Nhưng thất bại khi cập nhật dư liệu ECRM";
						}
					}
					else
					{
						rs.message = "Duyệt cập nhật thời gian khóa thẻ lỗi khi lưu dữ liệu Portal";
					}
					return Json(rs, JsonRequestBehavior.AllowGet);
				}
				else if (P_LOCK_STATUS == "2")
				{
					//4. Build Message API
					ReqMessageLockCard request = new ReqMessageLockCard
					{
						Channel = CShared.Channel,
						PartnerId = CShared.PartnerId,
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
					AuditLog.Log(CShared.GetIPAddress(), CShared.getSession().oAccount.Branch, CShared.getSession().oAccount.Tellerid, "ApproveChangeCardStatusController", "SaveApproveLockCard",
	"Send Request Change Status Card", infoPortal[0].LOCKCARD_ID, request);
					GWSocket_Way4 socket = new GWSocket_Way4();
					var response = socket.SendAndReceive(request);
					AuditLog.Log(CShared.GetIPAddress(), CShared.getSession().oAccount.Branch, CShared.getSession().oAccount.Tellerid, "ApproveChangeCardStatusController", "SaveApproveLockCard",
	"Send Request Change Status Card", infoPortal[0].LOCKCARD_ID, response);
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
			}
			return Json(rs, JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
        //[CustomAuthorize(Roles = Roles.BASE_ROLE)]
        public ActionResult UploadFiles()
        {
            try
            {
                SESSION_PARA oPra = CShared.getSession();
                string OP_MAKER_BRANCH = oPra.oAccount.Branch;
                string OP_MAKER = oPra.oAccount.UserName;
                string path = Server.MapPath(string.Format("~/Upload/CHANGE_STATUS/{0}/", DateTime.Now.ToString("MMyyyy")));
                string folderRoot = string.Format("/Upload/CHANGE_STATUS/{0}/", DateTime.Now.ToString("MMyyyy"));
                CreateFolder(folderRoot);
                HttpPostedFileBase file = Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                var _ext = Path.GetExtension(file.FileName);

                string lFileName = OP_MAKER_BRANCH + "_" + OP_MAKER + "_" + DateTime.Now.Ticks + _ext;
                path = path + lFileName;

                ViewBag.MsgFileName = lFileName;
                file.SaveAs(path);
                return Json(lFileName + ";" + folderRoot + lFileName);
            }
            catch (Exception ex)
            {
                log.InfoFormat("[ChangeCardStatus][UploadFiles] ===<<<<<< error = {0}", ex);
                return Json("");
            }
        }
        public void CreateFolder(string Folder)
        {
            if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(Folder)))
            {
                System.IO.Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(Folder));
            }
        }
    }
}