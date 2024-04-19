using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DBManager;
using ISSUING_APP.Models;
using log4net;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace ISSUING_APP.Service
{
    public class CancelCardsBulkService
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string schemaWAY4 = CShared.schemaWay4;
        private readonly string package = "OPT_HDB_CM_CARD_BULK_SERVICE";

        public int InsertBatchDetails(List<CancelCardsBulk> lstData)
        {
            var size = lstData.Count;
            ResultMessage result = new ResultMessage();

            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "INSERT_CANCEL_CARD_BULK_DETAIL");

            log.InfoFormat("[CancelCardsBulkService][InsertBatchDetails] storeName={0}", storeName);
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.UUID).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCH_ID", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.BATCH_ID).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_NAME", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.FILE_NAME).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CREATE_USER", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CREATE_USER).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CREATE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CREATE_BRANCH).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_CODE", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.RESULT_CODE).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.RESULT_MESSAGE).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CLIENT_NAME", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CLIENT_NAME).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CLIENT_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CLIENT_NUMBER).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CONTRACT_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CONTRACT_NUMBER).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CARD_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CARD_NUMBER).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CARD_STATUS", OracleDbType = OracleDbType.Varchar2, Value = lstData.Select(x => x.CARD_STATUS).ToArray(), Direction = ParameterDirection.Input });
               

                connect.InsertImportBatches(storeName, size, listParam.ToArray());
                log.Info("[CancelCardsBulkService][InsertBatchDetails] Import success: " + size);
            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][InsertBatchDetails] Exception: " + ex.Message, ex);
                return 0;
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return size;
        }

        public ResultMessage InsertFileMasterCancelCardsBulk(FileMasterCancelCardsBulk data)
        {
            ResultMessage result = new ResultMessage();

            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "INSERT_HDB_CM_BULK_FILE_MASTER");

            log.InfoFormat("[CancelCardsBulkService][InsertFileMasterCancelCardsBulk] storeName={0}", storeName);
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = data.UUID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CHANNEL_TYPE", OracleDbType = OracleDbType.Varchar2, Value = "PORTAL_CM", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_NAME", OracleDbType = OracleDbType.Varchar2, Value = data.FILE_NAME, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_SERVICE_ID", OracleDbType = OracleDbType.Varchar2, Value = "CANCEL_BULK", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_SERVICE_TYPE", OracleDbType = OracleDbType.Varchar2, Value = "CANCEL_BULK", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_PRODUCT", OracleDbType = OracleDbType.Varchar2, Value = "CARD", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_PROCESS_TYPE", OracleDbType = OracleDbType.Varchar2, Value = "BATCH", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_TOTAL_AMOUNT", OracleDbType = OracleDbType.Int32, Value = data.TOTAL_AMOUNT, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_TOTAL_ROW", OracleDbType = OracleDbType.Int32, Value = data.TOTAL_ROW, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_TOTAL_ROW_SUCCESS", OracleDbType = OracleDbType.Int32, Value = 0, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCH_ID", OracleDbType = OracleDbType.Varchar2, Value = data.BATCH_ID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_CODE", OracleDbType = OracleDbType.Varchar2, Value = "N", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Value = "New record", Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CREATE_USER", OracleDbType = OracleDbType.Varchar2, Value = data.CREATE_USER, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CREATE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = data.CREATE_BRANCH, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_USER", OracleDbType = OracleDbType.Varchar2, Value = data.APPROVE_USER, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = data.APPROVE_BRANCH, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_DATE", OracleDbType = OracleDbType.Varchar2, Value = data.APPROVE_DATE, Direction = ParameterDirection.Input });

                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_CODE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });


                connect.ExecuteProc(storeName, listParam.ToArray());

                log.Info("[CancelCardsBulkService][InsertFileMasterCancelCardsBulk] Insert file result : " + listParam[listParam.Count - 2].Value.ToString());
                log.Info("[CancelCardsBulkService][InsertFileMasterCancelCardsBulk] Insert file result : " + listParam[listParam.Count - 1].Value.ToString());
                result.code = listParam[listParam.Count - 2].Value.ToString();
                result.message = listParam[listParam.Count - 1].Value.ToString();
            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][InsertFileMasterCancelCardsBulk] Exception: " + ex.Message, ex);
                return result;
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public List<CancelCardsBulk> ValidateFileCancelCardsBulk(string fileUUID, string batchId)
        {

            List<CancelCardsBulk> listData = new List<CancelCardsBulk>();


            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "VALIDATE_FILE_CANCEL_BULK");

            log.InfoFormat("[CancelCardsBulkService][ValidateFileDetailC2C] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][ValidateFileDetailC2C] fileUUID={0}", fileUUID);
            log.InfoFormat("[CancelCardsBulkService][ValidateFileDetailC2C] batchId={0}", batchId);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_UUID", OracleDbType = OracleDbType.Varchar2, Value = fileUUID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCHID", OracleDbType = OracleDbType.Varchar2, Value = batchId, Direction = ParameterDirection.Input });

                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_DATA", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });


                connect.FillProc(storeName, listParam.ToArray(), ref ds);

                if (ds != null && ds.Tables.Count != 0)
                {
                    listData = ds.Tables[0].ToList<CancelCardsBulk>();
                }
                log.Info("[CancelCardsBulkService][ValidateFileCancelCardsBulkService] Result list count: " + listData.Count);
            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][ValidateFileCancelCardsBulkService] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return listData;
        }

        public bool doUpdateResultFileMaster(FileMasterCancelCardsBulk file)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "UPDATE_RESULT_FILE_BULK");
            bool result = true;
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultFileMaster] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultFileMaster] fileUUID={0}", file.UUID);
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultFileMaster] batchId={0}", file.BATCH_ID);
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = file.UUID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCH_ID", OracleDbType = OracleDbType.Varchar2, Value = file.BATCH_ID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_STATUS", OracleDbType = OracleDbType.Varchar2, Value = file.STATUS.ToString(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_USER", OracleDbType = OracleDbType.Varchar2, Value = file.APPROVE_USER, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = file.APPROVE_BRANCH, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_DATE", OracleDbType = OracleDbType.Varchar2, Value = file.APPROVE_DATE, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_CODE", OracleDbType = OracleDbType.Varchar2, Value = file.RESULT_CODE, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Value = file.RESULT_MESSAGE, Direction = ParameterDirection.Input });

                connect.ExecuteProc(storeName, listParam.ToArray());

                log.Info("[CancelCardsBulkService][doUpdateResultFileMaster] Update success file: " + file.UUID);

            }
            catch (Exception ex)
            {
                result = false;
                log.Error("[CancelCardsBulkService][doUpdateResultFileMaster]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public bool doUpdateAttachment(FileMasterCancelCardsBulk file)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "UPDATE_ATTACHMENT_FILE_BULK");
            bool result = true;
            log.InfoFormat("[CancelCardsBulkService][doUpdateAttachment] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][doUpdateAttachment] fileUUID={0}", file.UUID);
            log.InfoFormat("[CancelCardsBulkService][doUpdateAttachment] batchId={0}", file.BATCH_ID);
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = file.UUID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCHID", OracleDbType = OracleDbType.Varchar2, Value = file.BATCH_ID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_REMARK", OracleDbType = OracleDbType.Varchar2, Value = file.REMARK, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_ATTACHMENT", OracleDbType = OracleDbType.Varchar2, Value = file.ATTACHMENT, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_ATTACHMENT_PATH", OracleDbType = OracleDbType.Varchar2, Value = file.ATTACHMENT_PATH, Direction = ParameterDirection.Input });

                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_CODE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                connect.ExecuteProc(storeName, listParam.ToArray());

                log.Info("[CancelCardsBulkService][doUpdateAttachment] Result code: " + listParam[5]);
                log.Info("[CancelCardsBulkService][doUpdateAttachment] Result message: " + listParam[6]);

            }
            catch (Exception ex)
            {
                result = false;
                log.Error("[CancelCardsBulkService][doUpdateAttachment]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public List<FileMasterCancelCardsBulk> getListFileCancelBulk(string branch, string status, string fromDate, string toDate)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "GET_LIST_FILE_CANCEL_BULK");
            List<FileMasterCancelCardsBulk> result = new List<FileMasterCancelCardsBulk>();

            log.InfoFormat("[CancelCardsBulkService][getListFileCancelBulk] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][getListFileCancelBulk] branch={0}", branch);
            log.InfoFormat("[CancelCardsBulkService][getListFileCancelBulk] status={0}", status);
            log.InfoFormat("[CancelCardsBulkService][getListFileCancelBulk] fromDate={0}", fromDate);
            log.InfoFormat("[CancelCardsBulkService][getListFileCancelBulk] toDate={0}", toDate);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = branch, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_STATUS", OracleDbType = OracleDbType.Varchar2, Value = status, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FROM_DATE", OracleDbType = OracleDbType.Varchar2, Value = fromDate, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_TO_DATE", OracleDbType = OracleDbType.Varchar2, Value = toDate, Direction = ParameterDirection.Input });


                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_DATA", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

                connect.FillProc(storeName, listParam.ToArray(), ref ds);

                log.Info("[CancelCardsBulkService][getListFileCancelBulk] Result message: " + listParam[listParam.Count - 2].Value.ToString());
                if (ds != null && ds.Tables.Count != 0)
                {
                    result = ds.Tables[0].ToList<FileMasterCancelCardsBulk>();
                    if(result.Count > 0)
                    {
                        result.ForEach(x => x.LIST_ATTACHMENT_NAME = x.ATTACHMENT.Split(',').ToList());
                    }
                }
                log.Info("[CancelCardsBulkService][getListFileCancelBulk] Result list count: " + result.Count);

            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][getListFileCancelBulk]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public List<CancelCardsBulk> getDetailsFileCancelBulk(string fileUUID)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "GET_DETAIL_FILE_CANCEL_BULK");
            List<CancelCardsBulk> result = new List<CancelCardsBulk>();

            log.InfoFormat("[CancelCardsBulkService][getDetailsFileCancelBulk] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][getDetailsFileCancelBulk] fileUUID={0}", fileUUID);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = fileUUID, Direction = ParameterDirection.Input });


                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_DATA", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

                connect.FillProc(storeName, listParam.ToArray(), ref ds);

                log.Info("[CancelCardsBulkService][getDetailsFileCancelBulk] Result message: " + listParam[listParam.Count - 2].Value.ToString());
                if (ds != null && ds.Tables.Count != 0)
                {
                    result = ds.Tables[0].ToList<CancelCardsBulk>();
                }
                log.Info("[CancelCardsBulkService][getDetailsFileCancelBulk] Result list count: " + result.Count);

            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][getDetailsFileCancelBulk]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public FileMasterCancelCardsBulk getFileCancelBulk(string fileUUID)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "GET_FILE_CANCEL_BULK");
            List<FileMasterCancelCardsBulk> result = new List<FileMasterCancelCardsBulk>();

            log.InfoFormat("[CancelCardsBulkService][getFileCancelBulk] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][getFileCancelBulk] fileUUID={0}", fileUUID);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_UUID", OracleDbType = OracleDbType.Varchar2, Value = fileUUID, Direction = ParameterDirection.Input });


                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_DATA", OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.Output });

                connect.FillProc(storeName, listParam.ToArray(), ref ds);

                log.Info("[CancelCardsBulkService][getFileCancelBulk] Result message: " + listParam[listParam.Count - 2].Value.ToString());
                if (ds != null && ds.Tables.Count != 0)
                {
                    result = ds.Tables[0].ToList<FileMasterCancelCardsBulk>();
                    if (result.Count > 0)
                    {
                        result.ForEach(x => x.LIST_ATTACHMENT_NAME = x.ATTACHMENT.Split(',').ToList());
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][getFileCancelBulk]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result.Count > 0 ? result[0] : null;
        }

        public string checkFileExist(string fileName)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "GET_FILE_ID");
            string result = "";

            log.InfoFormat("[CancelCardsBulkService][checkFileExist] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][checkFileExist] fileName={0}", fileName);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_NAME", OracleDbType = OracleDbType.Varchar2, Value = fileName, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_UUID", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                connect.FillProc(storeName, listParam.ToArray(), ref ds);

                OracleString p2val = (OracleString)listParam[1].Value;
                if (!p2val.IsNull)
                {
                    result = p2val.Value;
                }
                log.InfoFormat("[CancelCardsBulkService][checkFileExist] result={0}", result);
            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][checkFileExist]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public string updateResultApproveFileCancel(FileMasterCancelCardsBulk file)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "UPDATE_RESULT_APPROVE_FILE_CANCEL");
            string result = "";

            log.InfoFormat("[CancelCardsBulkService][updateResultApproveFileCancel] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][updateResultApproveFileCancel] UUID={0}", file.UUID);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = file.UUID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCH_ID", OracleDbType = OracleDbType.Varchar2, Value = file.BATCH_ID, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_STATUS", OracleDbType = OracleDbType.Varchar2, Value = file.STATUS.ToString(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_USER", OracleDbType = OracleDbType.Varchar2, Value = file.APPROVE_USER, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = file.APPROVE_BRANCH, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_DATE", OracleDbType = OracleDbType.Varchar2, Value = file.APPROVE_DATE, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_ROW_SUCCESS", OracleDbType = OracleDbType.Varchar2, Value = file.TOTAL_ROW_SUCCESS, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_ROW_FAIL", OracleDbType = OracleDbType.Varchar2, Value = file.TOTAL_ROW_FAIL, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_REJECT_REASON", OracleDbType = OracleDbType.Varchar2, Value = file.REJECT_REASON, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_CODE", OracleDbType = OracleDbType.Varchar2, Value = file.RESULT_CODE, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Value = file.RESULT_MESSAGE, Direction = ParameterDirection.Input });

                listParam.Add(new OracleParameter() { ParameterName = "P_OUT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                
                connect.FillProc(storeName, listParam.ToArray(), ref ds);

                OracleString p2val = (OracleString)listParam[listParam.Count - 1].Value;
                if (!p2val.IsNull)
                {
                    result = p2val.Value;
                }
                log.InfoFormat("[CancelCardsBulkService][updateResultApproveFileCancel] result={0}", result);
            }
            catch (Exception ex)
            {
                log.Error("[CancelCardsBulkService][updateResultApproveFileCancel]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }

        public bool doUpdateResultBatch(List<CancelCardsBulk> listData, string fileUUID, string batchId)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = string.Format("{0}.{1}.{2}", schemaWAY4, package, "UPDATE_RESULT_APPROVE_CANCEL_DETAIL");
            bool result = true;
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultBatch] storeName={0}", storeName);
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultBatch] fileUUID={0}", fileUUID);
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultBatch] batchId={0}", batchId);
            log.InfoFormat("[CancelCardsBulkService][doUpdateResultBatch] Total row update={0}", listData.Count);
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.UUID).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_BATCH_ID", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.BATCH_ID).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_USER", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.APPROVE_USER).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.APPROVE_BRANCH).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_APPROVE_BRANCH", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.APPROVE_DATE).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_CODE", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.RESULT_CODE).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT_MESSAGE", OracleDbType = OracleDbType.Varchar2, Value = listData.Select(x => x.RESULT_MESSAGE).ToArray(), Direction = ParameterDirection.Input });


                connect.InsertImportBatches(storeName, listData.Count, listParam.ToArray());
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("[CancelCardsBulkService][doUpdateResultBatch]  Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return result;
        }
    }
}