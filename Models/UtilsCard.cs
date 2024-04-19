using DBManager;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using ISSUING_APP.Models;
using Oracle.DataAccess.Client;
using System.Globalization;
using Newtonsoft.Json;

namespace ISSUING_APP.Models
{
    public class ConfigFee
    {
        public string CONTRACT_PRODUCT_CODE { get; set; }
        public string CONTRACT_PRODUCT_NAME { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string TYPE { get; set; }
        public string AMOUNT { get; set; }
        public string CHARGE_ENOUGH { get; set; }
        public string CODE { get; set; }
    }
    public class AccNoFee
    {
        public string ACCT_NO { get; set; }
        public string ACCT_TYPE { get; set; }
    }

    public class AcctInfo
    {
        public string ACCT_NO { get; set; }
        public string ACCT_TYPE { get; set; }
        public string INTERNAL_KEY { get; set; }
        public string CCY { get; set; }
        public string CLIENT_SHORT { get; set; }
    }

    public class UtilsCard
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string schemaPortal { get; set; }
        private string packagePortal { get; set; }
        private string schemaECRM { get; set; }
        private string packageECRM { get; set; }
        private string schemaWAY4 { get; set; }
        private string packageWAY4 { get; set; }
        private string schemaEOC { get; set; }
        private string packageEOC { get; set; }
        public UtilsCard()
        {
            //this.schemaECRM = System.Configuration.ConfigurationManager.AppSettings["ECRM_SCHEMA"];
            schemaEOC = System.Configuration.ConfigurationManager.AppSettings["EOC_SCHEMA"];
            packageEOC = "HDB_WEB_ISSUING_EOC";
            schemaPortal = CShared.schemaPortal;
            packagePortal = "HDB_ISSUING";

            schemaWAY4 = "WAY4";
            packageWAY4 = "HDB.OPT_HDB_CARD_MANAGERMANT";
            schemaECRM = "ECRM";
            packageECRM = "HDB_ISSUING";
        }

        //        PROCEDURE GET_CONTR_STATUS
        //  (
        //        OP_RESULT_CUR OUT SYS_REFCURSOR
        //  )

        //PROCEDURE GET_CONTR_TYPE
        //  (
        //        OP_RESULT_CUR OUT SYS_REFCURSOR
        //  )

        public List<ListGeneral> LoadListTypeProduct(string P_CODE)
        {
            List<ListGeneral> lsResult = new List<ListGeneral>();
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_TYPE_PRODUCT";
            log.InfoFormat("[UtilsCard][LoadListTypeProduct] storeName={0}", storeName);
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[2];
                (parameters[0] = connect.Parameter("P_CODE", OracleDbType.Varchar2)).Value = P_CODE;
                (parameters[1] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<ListGeneral>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadListTypeProduct] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return lsResult;
        }

        public List<LyDoKhoa> LoadListLyDoKhoa()
        {
            List<LyDoKhoa> lsResult = new List<LyDoKhoa>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_LIST_BOLYDO";
            log.InfoFormat("[UtilsCard][LoadListLyDoKhoa] storeName={0}", storeName);
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[1];
                (parameters[0] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<LyDoKhoa>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadListLyDoKhoa] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return lsResult;
        }
        public List<CBoxData> LoadGlobals_ByGroupType(string sGroupType)
        {
            List<CBoxData> lsResult = new List<CBoxData>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_GLOBALS_BYGROUPTYPE";
            log.InfoFormat("[Utils][LoadGlobals_ByGroupType] storeName={0}", storeName);
            log.InfoFormat("[Utils][LoadGlobals_ByGroupType] GroupType={0}", sGroupType);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[2];
                (parameters[0] = connect.Parameter("P_GROUPTYPE", OracleDbType.Varchar2)).Value = sGroupType;
                (parameters[1] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<CBoxData>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][LoadGlobals_ByGroupType] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }
        public List<CBoxData> LoadGlobals_ByGroupType(string sID, string sName, string sGroupType, string sPara1, string sPara2, string sPara3)
        {
            List<CBoxData> lsResult = new List<CBoxData>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_GLOBALS_FILTER";
            log.InfoFormat("[Utils][LoadGlobals_ByGroupType] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[7];
                parameters[0] = connect.Parameter("P_ID", OracleDbType.Varchar2, sID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("P_NAME", OracleDbType.Varchar2, sName);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("P_GROUP_TYPE", OracleDbType.Varchar2, sGroupType);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("P_PARA1", OracleDbType.Varchar2, sPara1);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("P_PARA2", OracleDbType.Varchar2, sPara2);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("P_PARA3", OracleDbType.Varchar2, sPara3);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;
                (parameters[6] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<CBoxData>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][LoadGlobals_ByGroupType] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }
        public List<CBoxData> LoadGlobals_ADJCode(string ADJContract, string ADJ_NOCO)
        {
            List<CBoxData> lsResult = new List<CBoxData>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_GLOBALS_ADJCode";
            log.InfoFormat("[Utils][LoadGlobals_ByGroupType] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = connect.Parameter("P_PARA1", OracleDbType.Varchar2, ADJContract);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("P_PARA2", OracleDbType.Varchar2, ADJ_NOCO);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                (parameters[2] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<CBoxData>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][LoadGlobals_ByGroupType] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }
        public List<CBoxData> LoadGlobals_ADJContract(string ADJContract, string ADJ_NOCO)
        {
            List<CBoxData> lsResult = new List<CBoxData>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_GLOBALS_ADJCode";
            log.InfoFormat("[Utils][LoadGlobals_ByGroupType] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = connect.Parameter("P_PARA1", OracleDbType.Varchar2, ADJContract);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("P_PARA2", OracleDbType.Varchar2, ADJ_NOCO);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                (parameters[2] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<CBoxData>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][LoadGlobals_ByGroupType] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }

        public List<ListGeneral> LoadListCONTR_TYPE()
        {
            List<ListGeneral> lsResult = new List<ListGeneral>();
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_CONTR_TYPE";
            log.InfoFormat("[UtilsCard][LoadListCONTR_STATUS] storeName={0}", storeName);
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[1];
                (parameters[0] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<ListGeneral>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadListCONTR_STATUS] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return lsResult;
        }
        public List<ListGeneral> LoadListCONTR_STATUS()
        {
            List<ListGeneral> lsResult = new List<ListGeneral>();
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_CONTR_STATUS";
            log.InfoFormat("[UtilsCard][LoadListCONTR_STATUS] storeName={0}", storeName);
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[1];
                (parameters[0] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<ListGeneral>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadListCONTR_STATUS] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return lsResult;
        }
        public static List<CBoxData> getCardProductName(string ProductName)
        {
            List<CBoxData> lsResult = new List<CBoxData>();
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = "HDB.OPT_HDB_CARD_MANAGERMANT.GET_CARD_PRODUCTCODE";

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = connect.Parameter("P_PRODUCT_NAME", OracleDbType.Varchar2, ProductName);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;
                (parameters[1] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<CBoxData>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][getCardProductCode] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return lsResult;
        }
        public List<ConfigFee> getConfigFee(string P_CONTRACT_PRODUCT_CODE, string P_CONTRACT_PRODUCT_NAME, string P_CHARGE_TYPE, string P_TYPE)
        {
            List<ConfigFee> lsResult = new List<ConfigFee>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_ISSUING_FEE";
            log.InfoFormat("[Utils][getConfigFee] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[5];
                parameters[0] = connect.Parameter("P_CONTRACT_PRODUCT_CODE", OracleDbType.Varchar2, P_CONTRACT_PRODUCT_CODE);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;


                parameters[1] = connect.Parameter("P_CONTRACT_PRODUCT_NAME", OracleDbType.Varchar2, P_CONTRACT_PRODUCT_NAME);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("P_CHARGE_TYPE", OracleDbType.Varchar2, P_CHARGE_TYPE);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("P_TYPE", OracleDbType.Varchar2, P_TYPE);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                (parameters[4] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<ConfigFee>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][getConfigFee] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }
        /// <summary>
        /// Chuyen vao bang thu phi
        /// </summary>
        ///P_FEE_CODE IN VARCHAR2,-- input: fee issuing
        ///P_CARDNUMBER IN VARCHAR2,-- số thẻ: 6 đầu 4 cuối
        ///P_TKTT  IN VARCHAR2, -- số tk thu phí
        ///P_AMOUNT IN NUMBER, -- số tiền thu phí
        ///P_CHARGE_ENOUGH IN VARCHAR2-- loại tận thu: N or thu full: Y
        /// <returns></returns>
        public bool SetupFee(string P_FEE_CODE, string P_CARDNUMBER, string P_TKTT, int P_AMOUNT, string P_CHARGE_ENOUGH)
        {
            SqlUtil connect = new SqlUtil("EOC");
            string storeName = schemaPortal + "." + "HDB_ISSUING" + ".SetupFee";
            log.InfoFormat("[Utils][SetupFee] storeName={0}", storeName);
            bool isOk = true;
            DataSet ds = new DataSet();
            try
            {

                OracleParameter[] parameters = new OracleParameter[5];
                parameters[0] = connect.Parameter("P_FEE_CODE", OracleDbType.Varchar2, P_FEE_CODE);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;


                parameters[1] = connect.Parameter("P_CARDNUMBER", OracleDbType.Varchar2, P_CARDNUMBER);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("P_TKTT", OracleDbType.Varchar2, P_TKTT);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("P_AMOUNT", OracleDbType.Int32, P_AMOUNT);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("P_CHARGE_ENOUGH", OracleDbType.Varchar2, P_CHARGE_ENOUGH);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                connect.FillProc(storeName, parameters, ref ds);
            }
            catch (Exception ex)
            {
                isOk = false;
                log.Error("[Utils][SetupFee] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return isOk;
        }


        /// <summary>
        /// Chuyen vao bang thu phi của hoannht
        /// </summary> 
        /// <returns></returns>
        public bool SetupFeeRegister(string P_FEE_CODE, string P_CARDNUMBER, string P_PRODUCT_CODE,
            string P_TKTT, long P_AMOUNT, string P_LOC_ACCT, string P_USER_ID, string P_BRANCH,
            string P_CHARGE_FULL = "Y")
        {
            SqlUtil connect = new SqlUtil("EOC");
            string storeName = schemaEOC + "." + "HDB_SERV_CHARGE" + ".PROC_CARD_REGISTER_FEE";
            log.InfoFormat("[Utils][SetupFeeRegister] storeName={0}", storeName);
            bool isOk = false;
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[12];
                parameters[0] = connect.Parameter("P_ERR_CODE", OracleDbType.Varchar2);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Output;
                parameters[0].Size = 4000;

                parameters[1] = connect.Parameter("P_ERR_MSG", OracleDbType.Varchar2);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[1].Size = 4000;

                parameters[2] = connect.Parameter("P_PRODUCT_CODE", OracleDbType.Varchar2, P_PRODUCT_CODE);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("P_FEE_TYPE", OracleDbType.Varchar2, P_FEE_CODE);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("P_CHARGE_AMOUNT", OracleDbType.Long, P_AMOUNT);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("P_CHARGE_CCY", OracleDbType.Varchar2, "VND");
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("P_CHARGE_FULL", OracleDbType.Varchar2, P_CHARGE_FULL);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("P_CHARGE_ACCT_NO", OracleDbType.Varchar2, P_TKTT);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("P_PAN", OracleDbType.Varchar2, P_CARDNUMBER.Replace("******", "***"));
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                parameters[9] = connect.Parameter("P_LOC_ACCT", OracleDbType.Varchar2, P_LOC_ACCT);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;

                parameters[10] = connect.Parameter("P_USERID", OracleDbType.Varchar2, P_USER_ID);
                parameters[10].IsNullable = true;
                parameters[10].Direction = ParameterDirection.Input;

                parameters[11] = connect.Parameter("P_BRANCH", OracleDbType.Varchar2, P_BRANCH);
                parameters[11].IsNullable = true;
                parameters[11].Direction = ParameterDirection.Input;


                connect.FillProc(storeName, parameters, ref ds);


                log.InfoFormat("[Utils][SetupFeeRegister] P_ERR_CODE: {0}", parameters[0].Value.ToString());
                log.InfoFormat("[Utils][SetupFeeRegister] P_ERR_MSG: {0}", parameters[0].Value.ToString());

                if (parameters[0].Value.ToString() == "000000")
                    return true;


            }
            catch (Exception ex)
            {
                isOk = false;
                log.Error("[Utils][SetupFeeRegister] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return isOk;
        }

        public bool SetupFeeRegister2(string P_FEE_CODE, string P_CARDNUMBER, string P_PRODUCT_CODE,
            string P_TKTT, long P_AMOUNT, string P_LOC_ACCT, string P_USER_ID, string P_BRANCH,
            string ip, string controller, string action, string P_CHARGE_FULL = "Y")
        {
            SqlUtil connect = new SqlUtil("EOC");
            string storeName = schemaEOC + "." + "HDB_SERV_CHARGE" + ".PROC_CARD_REGISTER_FEE";
            log.InfoFormat("[Utils][SetupFeeRegister] storeName={0}", storeName);
            bool isOk = false;
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[12];
                parameters[0] = connect.Parameter("P_ERR_CODE", OracleDbType.Varchar2);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Output;
                parameters[0].Size = 4000;

                parameters[1] = connect.Parameter("P_ERR_MSG", OracleDbType.Varchar2);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[1].Size = 4000;

                parameters[2] = connect.Parameter("P_PRODUCT_CODE", OracleDbType.Varchar2, P_PRODUCT_CODE);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("P_FEE_TYPE", OracleDbType.Varchar2, P_FEE_CODE);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("P_CHARGE_AMOUNT", OracleDbType.Long, P_AMOUNT);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("P_CHARGE_CCY", OracleDbType.Varchar2, "VND");
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("P_CHARGE_FULL", OracleDbType.Varchar2, P_CHARGE_FULL);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("P_CHARGE_ACCT_NO", OracleDbType.Varchar2, P_TKTT);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("P_PAN", OracleDbType.Varchar2, P_CARDNUMBER.Replace("******", "***"));
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                parameters[9] = connect.Parameter("P_LOC_ACCT", OracleDbType.Varchar2, P_LOC_ACCT);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;

                parameters[10] = connect.Parameter("P_USERID", OracleDbType.Varchar2, P_USER_ID);
                parameters[10].IsNullable = true;
                parameters[10].Direction = ParameterDirection.Input;

                parameters[11] = connect.Parameter("P_BRANCH", OracleDbType.Varchar2, P_BRANCH);
                parameters[11].IsNullable = true;
                parameters[11].Direction = ParameterDirection.Input;

                AuditLog.Log("CM", ip, P_BRANCH, P_USER_ID, controller, action,
                    System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "." + System.Reflection.MethodBase.GetCurrentMethod().Name, storeName, parameters);

                connect.FillProc(storeName, parameters, ref ds);

                log.Error("[Utils][SetupFeeRegister] RESULT: " + parameters[0].Value.ToString());

                if (parameters[0].Value.ToString() == "000000")
                    return true;


            }
            catch (Exception ex)
            {
                isOk = false;
                log.Error("[Utils][SetupFeeRegister] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return isOk;
        }

        /// <summary>
        /// Chuyen vao bang thu phi của hoannht
        /// </summary> 
        /// <returns></returns>
        public bool SetupFeeAnual(string P_FEE_CODE, string P_CARDNUMBER, string P_PRODUCT_CODE,
            string P_TKTT, long P_AMOUNT, string P_LOC_ACCT, string P_USER_ID, string P_BRANCH, string P_FROM_DATE,
            string P_TO_DATE, string P_SETUP_FEE, string P_CCY)
        {
            SqlUtil connect = new SqlUtil("EOC");
            string storeName = schemaEOC + "." + "HDB_SERV_CHARGE" + ".PROC_CARD_SETUP_ANUAL_FEE";
            log.InfoFormat("[Utils][SetupFeeAnual] storeName={0}", storeName);
            bool isOk = false;
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[15];
                parameters[0] = connect.Parameter("P_ERR_CODE", OracleDbType.Varchar2);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Output;
                parameters[0].Size = 4000;

                parameters[1] = connect.Parameter("P_ERR_MSG", OracleDbType.Varchar2);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[1].Size = 4000;

                parameters[2] = connect.Parameter("P_PRODUCT_CODE", OracleDbType.Varchar2, P_PRODUCT_CODE);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("P_FEE_TYPE", OracleDbType.Varchar2, P_FEE_CODE);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("P_CHARGE_AMOUNT", OracleDbType.Long, P_AMOUNT);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("P_CHARGE_ACCT_NO", OracleDbType.Varchar2, P_TKTT);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("P_FROM_DATE", OracleDbType.Varchar2, P_FROM_DATE);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("P_TO_DATE", OracleDbType.Varchar2, P_TO_DATE);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("P_PAN", OracleDbType.Varchar2, P_CARDNUMBER.Replace("******", "***"));
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                parameters[9] = connect.Parameter("P_LOC_ACCT", OracleDbType.Varchar2, P_LOC_ACCT);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;

                parameters[10] = connect.Parameter("P_ACCT_NO", OracleDbType.Varchar2, P_TKTT);
                parameters[10].IsNullable = true;
                parameters[10].Direction = ParameterDirection.Input;

                parameters[11] = connect.Parameter("P_SETUP_FEE", OracleDbType.Varchar2, P_SETUP_FEE);
                parameters[11].IsNullable = true;
                parameters[11].Direction = ParameterDirection.Input;

                parameters[12] = connect.Parameter("P_USERID", OracleDbType.Varchar2, P_USER_ID);
                parameters[12].IsNullable = true;
                parameters[12].Direction = ParameterDirection.Input;

                parameters[13] = connect.Parameter("P_BRANCH", OracleDbType.Varchar2, P_BRANCH);
                parameters[13].IsNullable = true;
                parameters[13].Direction = ParameterDirection.Input;

                parameters[14] = connect.Parameter("P_CHARGE_CCY", OracleDbType.Varchar2, P_CCY);
                parameters[14].IsNullable = true;
                parameters[14].Direction = ParameterDirection.Input;


                connect.FillProc(storeName, parameters, ref ds);

                log.Error("[Utils][SetupFeeAnual] RESULT: " + parameters[0].Value.ToString());

                if (parameters[0].Value.ToString() == "000000")
                    return true;


            }
            catch (Exception ex)
            {
                isOk = false;
                log.Error("[Utils][SetupFeeAnual] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return isOk;
        }




        /// <summary>
        /// Chuyen vao bang thu phi của hoannht
        /// </summary> 
        /// <returns></returns>
        public bool SetupRegisterSms(string cif, string fullname, string phone, string address, string acctNo, string internalKey, string ccy)
        {

            if (phone.Trim().StartsWith("0"))
                phone = "84" + phone.Substring(1);
            SqlUtil connect = new SqlUtil("ECRM");
            string storeName = "ECRM" + "." + "HDB_ISSUING" + ".insert_sms_client_reg";
            log.InfoFormat("[Utils][SetupRegisterSms] storeName={0}", storeName);
            bool isOk = false;
            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[29];
                parameters[0] = connect.Parameter("p_tran_date", OracleDbType.Date, DateTime.Now);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;


                parameters[1] = connect.Parameter("p_client_no", OracleDbType.Varchar2, cif);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;


                parameters[2] = connect.Parameter("p_client_short", OracleDbType.Varchar2, fullname);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;


                parameters[3] = connect.Parameter("p_phone", OracleDbType.Varchar2, phone);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;


                parameters[4] = connect.Parameter("p_pwd", OracleDbType.Varchar2, "1234");
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;


                parameters[5] = connect.Parameter("p_active", OracleDbType.Long, 1);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;


                parameters[6] = connect.Parameter("p_user_id", OracleDbType.Varchar2, "PORTALWAY4");
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;


                parameters[7] = connect.Parameter("p_ws_id", OracleDbType.Varchar2, null);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;


                parameters[8] = connect.Parameter("p_auth_id", OracleDbType.Varchar2, null);
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;


                parameters[9] = connect.Parameter("p_status", OracleDbType.Long, 1);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;


                parameters[10] = connect.Parameter("p_rate", OracleDbType.Varchar2, "1");
                parameters[10].IsNullable = true;
                parameters[10].Direction = ParameterDirection.Input;


                parameters[11] = connect.Parameter("p_adver", OracleDbType.Varchar2, "1");
                parameters[11].IsNullable = true;
                parameters[11].Direction = ParameterDirection.Input;


                parameters[12] = connect.Parameter("p_msg", OracleDbType.Varchar2, "1");
                parameters[12].IsNullable = true;
                parameters[12].Direction = ParameterDirection.Input;


                parameters[13] = connect.Parameter("p_client_address", OracleDbType.Varchar2, address);
                parameters[13].IsNullable = true;
                parameters[13].Direction = ParameterDirection.Input;


                parameters[14] = connect.Parameter("p_bill_address", OracleDbType.Varchar2, null);
                parameters[14].IsNullable = true;
                parameters[14].Direction = ParameterDirection.Input;


                parameters[15] = connect.Parameter("p_next_charge_date", OracleDbType.Varchar2, null);
                parameters[15].IsNullable = true;
                parameters[15].Direction = ParameterDirection.Input;


                parameters[16] = connect.Parameter("p_acct_no", OracleDbType.Varchar2, acctNo);
                parameters[16].IsNullable = true;
                parameters[16].Direction = ParameterDirection.Input;


                parameters[17] = connect.Parameter("p_internal_key", OracleDbType.Varchar2, internalKey);
                parameters[17].IsNullable = true;
                parameters[17].Direction = ParameterDirection.Input;


                parameters[18] = connect.Parameter("p_balance", OracleDbType.Long, 0);
                parameters[18].IsNullable = true;
                parameters[18].Direction = ParameterDirection.Input;


                parameters[19] = connect.Parameter("p_tran_status", OracleDbType.Varchar2, "0");
                parameters[19].IsNullable = true;
                parameters[19].Direction = ParameterDirection.Input;


                parameters[20] = connect.Parameter("p_ccy", OracleDbType.Varchar2, ccy);
                parameters[20].IsNullable = true;
                parameters[20].Direction = ParameterDirection.Input;


                parameters[21] = connect.Parameter("p_pre_balance", OracleDbType.Long, 0);
                parameters[21].IsNullable = true;
                parameters[21].Direction = ParameterDirection.Input;


                parameters[22] = connect.Parameter("p_is_bankstaff", OracleDbType.Varchar2, "0");
                parameters[22].IsNullable = true;
                parameters[22].Direction = ParameterDirection.Input;


                parameters[23] = connect.Parameter("p_create_user", OracleDbType.Varchar2, "PORTALWAY4");
                parameters[23].IsNullable = true;
                parameters[23].Direction = ParameterDirection.Input;


                parameters[24] = connect.Parameter("p_create_date", OracleDbType.Date, DateTime.Now);
                parameters[24].IsNullable = true;
                parameters[24].Direction = ParameterDirection.Input;


                parameters[25] = connect.Parameter("p_loc_acct", OracleDbType.Varchar2, acctNo);
                parameters[25].IsNullable = true;
                parameters[25].Direction = ParameterDirection.Input;


                parameters[26] = connect.Parameter("p_approve_by", OracleDbType.Varchar2, "PORTALWAY4");
                parameters[26].IsNullable = true;
                parameters[26].Direction = ParameterDirection.Input;


                parameters[27] = connect.Parameter("p_approve_ws_id", OracleDbType.Varchar2, null);
                parameters[27].IsNullable = true;
                parameters[27].Direction = ParameterDirection.Input;


                parameters[28] = connect.Parameter("p_status_result", OracleDbType.Varchar2);
                parameters[28].IsNullable = true;
                parameters[28].Direction = ParameterDirection.Output;
                parameters[28].Size = 4000;


                connect.FillProc(storeName, parameters, ref ds);
                log.InfoFormat("[Utils][SetupRegisterSms] PHONE: " + phone + " acctNo:  " + acctNo + "result: " + parameters[28].Value.ToString());
                log.Error("[Utils][SetupRegisterSms] RESULT: " + parameters[28].Value.ToString());

                if (parameters[28].Value.ToString() == "1" || parameters[28].Value.ToString() == "2")
                    return true;


            }
            catch (Exception ex)
            {
                isOk = false;
                log.Error("[Utils][SetupRegisterSms] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return isOk;
        }



        public List<AccNoFee> getAccNoFee1(string P_CLIENT_NO, string P_GLOBAL_ID)
        {
            List<AccNoFee> lsResult = new List<AccNoFee>();
            SqlUtil connect = new SqlUtil("EOC");
            string storeName = schemaEOC + "." + packageEOC + ".GET_ACCNO";
            log.InfoFormat("[Utils][getAccNoFee1] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = connect.Parameter("P_CLIENT_NO", OracleDbType.Varchar2, P_CLIENT_NO);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("P_GLOBAL_ID", OracleDbType.Varchar2, P_GLOBAL_ID);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                (parameters[2] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<AccNoFee>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][getAccNoFee1] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }
        /// <summary>
        /// Đọc danh sách ClientInfo từ ECRM
        /// </summary>
        /// <returns></returns>
        public List<ClientInfo> LoadListClientInfo(string OP_ACNT_CONTRACT_ID, string OP_CLIENT_NUMBER, string OP_CLIENT_NAME, string OP_CLIENT_TEXT,
            string OP_CARD_NUMBER, string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_CARD_ACCOUNT, string OP_CARD_STATUS, string OP_CARD_TYPE)
        {

            List<ClientInfo> lsResult = new List<ClientInfo>();
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_LIST_CLIENTS";
            log.InfoFormat("[UtilsCard][LoadListClientInfo] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[11];

                parameters[0] = connect.Parameter("OP_ACNT_CONTRACT_ID", OracleDbType.Varchar2, OP_ACNT_CONTRACT_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;


                parameters[1] = connect.Parameter("OP_CLIENT_NUMBER", OracleDbType.Varchar2, OP_CLIENT_NUMBER);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_CLIENT_NAME", OracleDbType.Varchar2, OP_CLIENT_NAME);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CARD_NUMBER", OracleDbType.Varchar2, OP_CARD_NUMBER);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_CARD_NUMBER_A", OracleDbType.Varchar2, OP_CARD_NUMBER_A);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_CARD_NUMBER_B", OracleDbType.Varchar2, OP_CARD_NUMBER_B);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("OP_CARD_ACCOUNT", OracleDbType.Varchar2, OP_CARD_ACCOUNT);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("OP_CARD_STATUS", OracleDbType.Varchar2, OP_CARD_STATUS);
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                parameters[9] = connect.Parameter("OP_CARD_TYPE", OracleDbType.Varchar2, OP_CARD_TYPE);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;

                (parameters[10] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<ClientInfo>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadListClientInfo] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }


            return lsResult;
        }


        /// <summary>
        /// Đọc danh sách CardInfo từ ECRM
        /// </summary>
        /// <returns></returns>
        public List<CardInfo> LoadListCardInfo(string OP_ACNT_CONTRACT_ID, string OP_CLIENT_NUMBER, string OP_CLIENT_NAME, string OP_CLIENT_TEXT,
            string OP_CARD_NUMBER, string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_CARD_ACCOUNT, string OP_CARD_STATUS, string OP_CARD_TYPE)
        {

            List<CardInfo> lsResult = new List<CardInfo>();

            DataTable dt = LoadDataCardInfo(OP_ACNT_CONTRACT_ID, OP_CLIENT_NUMBER, OP_CLIENT_NAME, OP_CLIENT_TEXT,
                OP_CARD_NUMBER, OP_CARD_NUMBER_A, OP_CARD_NUMBER_B, OP_CARD_ACCOUNT, OP_CARD_STATUS, OP_CARD_TYPE);
            if (dt != null)
                lsResult = dt.ToList<CardInfo>();

            return lsResult;
        }
        public DataTable LoadDataCardInfo(string OP_ACNT_CONTRACT_ID, string OP_CLIENT_NUMBER, string OP_CLIENT_NAME, string OP_CLIENT_TEXT,
            string OP_CARD_NUMBER, string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_CARD_ACCOUNT, string OP_CARD_STATUS, string OP_CARD_TYPE)
        {

            DataTable dt = null;
            List<CardInfo> lsResult = new List<CardInfo>();
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_LIST_CARDS_V2";
            log.InfoFormat("[UtilsCard][LoadDataListCardInfo] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[11];

                parameters[0] = connect.Parameter("OP_ACNT_CONTRACT_ID", OracleDbType.Varchar2, OP_ACNT_CONTRACT_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;


                parameters[1] = connect.Parameter("OP_CLIENT_NUMBER", OracleDbType.Varchar2, OP_CLIENT_NUMBER);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_CLIENT_NAME", OracleDbType.Varchar2, OP_CLIENT_NAME);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CARD_NUMBER", OracleDbType.Varchar2, OP_CARD_NUMBER);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_CARD_NUMBER_A", OracleDbType.Varchar2, OP_CARD_NUMBER_A);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_CARD_NUMBER_B", OracleDbType.Varchar2, OP_CARD_NUMBER_B);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("OP_CARD_ACCOUNT", OracleDbType.Varchar2, OP_CARD_ACCOUNT);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("OP_CARD_STATUS", OracleDbType.Varchar2, OP_CARD_STATUS);
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                parameters[9] = connect.Parameter("OP_CARD_TYPE", OracleDbType.Varchar2, OP_CARD_TYPE);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;

                (parameters[10] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataListCardInfo] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return dt;
        }
        public List<CardInfo> LoadDataParentCardInfo(string OP_PARENTID)
        {

            List<CardInfo> lsResult = new List<CardInfo>();
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_ACNT_CONTRACT_BY_PARENT";
            log.InfoFormat("[UtilsCard][LoadDataParentCardInfo] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[2];

                parameters[0] = connect.Parameter("OP_PARENTID", OracleDbType.Int64, OP_PARENTID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                (parameters[1] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<CardInfo>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataParentCardInfo] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return lsResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OP_CARD_CODE"></param>
        /// <returns></returns>
        public List<LockCard> LoadDataListLockCard(string OP_CARD_CODE)
        {
            return LoadDataListLockCard(OP_CARD_CODE, null, null, null, null, null, null, null, null, null);
        }


        /// <summary>
        /// Đọc danh sách ListCard từ ECRM
        /// </summary>
        /// <returns></returns>
        public List<LockCard> LoadDataListLockCard(string OP_CARD_CODE, string OP_BRANCH_NO, string OP_CLIENT_NUMBER,
            string OP_CLIENT_NAME, string OP_CLIENT_TEXT, string OP_CARD_NUMBER, string OP_CARD_ACCOUNT
            , string OP_CARD_STATUS, string OP_CARD_TYPE, string OP_RESULT_CUR)
        {
            List<LockCard> lsResult = new List<LockCard>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_LIST_LOCKCARDS";
            log.InfoFormat("[UtilsCard][LoadDataListLockCard] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[10];
                parameters[0] = connect.Parameter("OP_CARD_CODE", OracleDbType.Varchar2, OP_CARD_CODE);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("OP_BRANCH_NO", OracleDbType.Varchar2, OP_BRANCH_NO);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_CLIENT_NUMBER", OracleDbType.Varchar2, OP_CLIENT_NUMBER);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_CLIENT_NAME", OracleDbType.Varchar2, OP_CLIENT_NAME);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_CARD_NUMBER", OracleDbType.Varchar2, OP_CARD_NUMBER);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_CARD_ACCOUNT", OracleDbType.Varchar2, OP_CARD_ACCOUNT);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;


                parameters[7] = connect.Parameter("OP_CARD_STATUS", OracleDbType.Varchar2, OP_CARD_STATUS);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("OP_CARD_TYPE", OracleDbType.Varchar2, OP_CARD_TYPE);
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                (parameters[9] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<LockCard>();

            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataListLockCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }



        public Result SaveLockCard(string OP_MAKER_BRANCH, string OP_MAKER, string OP_ACNT_CONTRACT_ID, ref string P_LOCKCARD_ID, string P_LOCK_TYPE, string P_LOCK_CODE, string P_LOCK_FROM, string P_LOCK_TO, string txtP_NOTE,
           string P_BLD_CODE, string P_YC_FILENAME, string P_YC_FILEPATH, string P_REASON = null, string P_REG_NUMBER = null, string P_CONTR_STATUS_NAME = null,
          string LOC_ACCT = null, string CONTR_STATUS_ACCT_CODE = null, string LOC_ACCT_STATUS = null, string CARD_PRODUCT_CODE = null, string IS_CLOSE_CONTRACT = null, string LOCK_TYPE_OLD = null)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".INS_LOCKCARDS";
            log.InfoFormat("[UtilsCard][SaveLockCard] storeName={0}", storeName);
            List<CardInfo> Listinfo = LoadListCardInfo(OP_ACNT_CONTRACT_ID, null, null, null, null, null, null, null, null, null);
            List<CBoxData> listCodeDebitDN = Utils.LoadAllProductDN(CShared.DEBIT);
            CardInfo info = Listinfo[0];
            DataSet ds = new DataSet();
            Result rs = new Result();
            rs.code = "";
            rs.message = "";
            /// Đóng hợp đồng leln2 08/10/2019
            if (string.IsNullOrEmpty(CARD_PRODUCT_CODE))
            {
                rs.code = "001";
                rs.message = "Không tìm thấy mã sản phẩm!";
                return rs;
            }
			if (LOCK_TYPE_OLD == "C055" && P_LOCK_CODE == "C00")
			{
				if (CARD_PRODUCT_CODE.Contains("_CR_") && CARD_PRODUCT_CODE.Contains("_M"))
				{
					string result = CheckResultIsuingCardLos(info.CARD_PRODUCT_CODE, info.CONTRACT_NUMBER);
					if (result != "00")
					{
						rs.code = "001";
						rs.message = "Thẻ chưa được nhập liệu hoàn tất trên Card LOS";
						return rs;
					}
				}

			}

			if (IS_CLOSE_CONTRACT == "Y" &&  P_LOCK_TYPE == "C054")
            {
                //// Check đóng tất cả hợp đồng trước khi thanh toán chưa
                if (!checkContractAllowClose(info.ACNT_CONTRACT__OID, info.CONTRACT_NUMBER))
                {
                    rs.code = "001";
                    rs.message = "Vui lòng hủy tất cả thẻ phụ/thẻ đã thay thế trước khi hủy thẻ chính và đóng hợp đồng!";
                    return rs;
                }
                if(CARD_PRODUCT_CODE.Contains("_CR_"))
                {
                    //// Check dư nợ trước khi tạo lệnh hủy  
                    APIGetContractWay4 api = new APIGetContractWay4();
                    ResMessage response = api.TakeInfoCreditDebt(LOC_ACCT);
                    if (response == null)
                    {
                        rs.code = "002";
                        rs.message = "Lỗi tìm kiếm thông tin dư nợ thẻ!";
                        log.InfoFormat("[UtilsCard][SaveLockCard][Query API get totalPaymentDue]: {0}", "response is null");
                    }
                    if (response.ResponseCode != "0")
                    {
                        rs.code = "002";
                        rs.message = "Lỗi tìm kiếm thông tin dư nợ thẻ!";
                        log.InfoFormat("[UtilsCard][SaveLockCard][Query API get totalPaymentDue]: {0}", "response.ResponseCode != 0");

                    }
                    string json = JsonConvert.SerializeObject(response.Extras);
                    if (String.IsNullOrEmpty(json))
                    {
                        rs.code = "002";
                        rs.message = "Lỗi tìm kiếm thông tin dư nợ thẻ!";
                        log.InfoFormat("[UtilsCard][SaveLockCard][Query API get totalPaymentDue]: {0}", "Error convert Json");
                    }
                    InfoCreditDebt icb = JsonConvert.DeserializeObject<InfoCreditDebt>(json);
                    if (icb == null || string.IsNullOrEmpty(icb.totalPaymentDue))
                    {
                        rs.code = "002";
                        rs.message = "Lỗi tìm kiếm thông tin dư nợ thẻ!";
                        log.InfoFormat("[UtilsCard][SaveLockCard][Query API get totalPaymentDue]: {0}", "Error convert Json 2");
                    }
                    Decimal blocked, closingBalance, currentInterest;
                    Decimal.TryParse(icb.blocked, out blocked);
                    Decimal.TryParse(icb.closingBalance, out closingBalance);
                    Decimal.TryParse(icb.currentInterest, out currentInterest);
                    if ((blocked + closingBalance + currentInterest) > 0)
                    {
                        rs.code = "003";
                        rs.message = "Vui lòng thanh toán dư nợ thẻ tín dụng";
                        log.InfoFormat("[UtilsCard][SaveLockCard][Query API get totalPaymentDue]: {0}", "totalPaymentDue = " + (blocked + closingBalance + currentInterest));
                    }
                } 
            }
            if (rs.code == "")
            {
                try
                {
                    P_LOCKCARD_ID = Guid.NewGuid().ToString();
                    DateTime? LOCK_FROM = null, LOCK_TO = null;
                    if (P_LOCK_TYPE == "C053")
                    {
                        if (string.IsNullOrEmpty(P_LOCK_TO))
                        {
                            rs.code = "001";
                            rs.message = "Thiếu ngày tháng khóa thẻ!";
                            return rs;
                        }
                        DateTime date2 = new DateTime();
                        if (!DateTime.TryParseExact(P_LOCK_TO, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date2))
                        {
                            rs.code = "001";
                            rs.message = "Trường đến ngày sai định dạng, Vui lòng chọn (dd/MM/yyyy)!";
                            return rs;
                        }
                        LOCK_TO = date2;
                    }

                    /// Ghi audilog
                    #region audilog
                    AudilogChangeStatus audilog = new AudilogChangeStatus();

                    audilog.REG_NUMBER = info.REG_NUMBER;
                    audilog.CLIENT_SHORT_NAME = info.CLIENT_SHORT_NAME;
                    audilog.CLIENT_NUMBER = info.CLIENT_NUMBER;
                    audilog.CLIENT_OPEN_DATE = info.CLIENT_OPEN_DATE;
                    audilog.CLIENT_BIRTH_DATE = info.CLIENT_BIRTH_DATE;
                    audilog.CLIENT_PHONE = info.CLIENT_PHONE;
                    audilog.CLIENT_PHONE_H = info.CLIENT_PHONE_H;
                    audilog.CLIENT_PHONE_M = info.CLIENT_PHONE_M;
                    audilog.CLIENT_EMAIL = info.CLIENT_EMAIL;
                    audilog.ADDRESS_LINE_1 = info.ADDRESS_LINE_1;
                    audilog.ADDRESS_LINE_2 = info.ADDRESS_LINE_2;
                    audilog.ADDRESS_LINE_3 = info.ADDRESS_LINE_3;
                    audilog.ADDRESS_LINE_4 = info.ADDRESS_LINE_4;
                    audilog.CLIENT_ADDRESS_LINE = info.CLIENT_ADDRESS_LINE;
                    audilog.REG_ADDR = info.REG_ADDR;
                    audilog.REG_ADDR_CITY = info.REG_ADDR_CITY;
                    audilog.REG_ADDR_DISTRICT = info.REG_ADDR_DISTRICT;
                    audilog.REG_ADDR_WARD = info.REG_ADDR_WARD;
                    audilog.REG_ADDR_LINE = info.REG_ADDR_LINE;
                    audilog.DEL_ADDR = info.DEL_ADDR;
                    audilog.DEL_ADDR_CITY = info.DEL_ADDR_CITY;
                    audilog.DEL_ADDR_DISTRICT = info.DEL_ADDR_DISTRICT;
                    audilog.DEL_ADDR_WARD = info.DEL_ADDR_WARD;
                    audilog.DEL_ADDR_LINE = info.DEL_ADDR_LINE;
                    audilog.STMT_ADDR = info.STMT_ADDR;
                    audilog.STMT_ADDR_CITY = info.STMT_ADDR_CITY;
                    audilog.STMT_ADDR_DISTRICT = info.STMT_ADDR_DISTRICT;
                    audilog.STMT_ADDR_WARD = info.STMT_ADDR_WARD;
                    audilog.STMT_ADDR_LINE = info.STMT_ADDR_LINE;
                    audilog.ACNT_CONTRACT_ID = info.ACNT_CONTRACT_ID;
                    audilog.CONTRACT_NUMBER = info.CONTRACT_NUMBER;
                    audilog.CONTRACT_NAME = info.CONTRACT_NAME;
                    audilog.CONTR_TYPE_NAME = info.CONTR_TYPE_NAME;
                    audilog.INTERVAL_TYPE = info.INTERVAL_TYPE;
                    audilog.CONTR_STATUS = info.CONTR_STATUS;
                    audilog.DATE_OPEN = info.DATE_OPEN;
                    audilog.DATE_EXPIRE = info.DATE_EXPIRE;
                    audilog.CARD_EXPIRE = info.CARD_EXPIRE;
                    audilog.PRODUCT = info.PRODUCT;
                    audilog.PRODUCTION_STATUS = info.PRODUCTION_STATUS;
                    audilog.PRODUCT_NAME = info.PRODUCT_NAME;
                    audilog.PRODUCT_CODE = info.PRODUCT_CODE;
                    audilog.CARD_BRANCH_NO = info.CARD_BRANCH_NO;
                    audilog.CARD_BRANCH_NAME = info.CARD_BRANCH_NAME;
                    audilog.CLIENT_BRANCH_NO = info.CLIENT_BRANCH_NO;
                    audilog.CONTR_STATUS_CODE = info.CONTR_STATUS_CODE;
                    audilog.CONTR_STATUS_NAME = info.CONTR_STATUS_NAME;
                    audilog.PRODUCT_GROUP = info.PRODUCT_GROUP;
                    audilog.RBS_NUMBER = info.RBS_NUMBER;
                    audilog.ACNT_CONTRACT__OID = info.ACNT_CONTRACT__OID;
                    audilog.QUESTION = info.QUESTION;
                    audilog.AWS = info.AWS;
                    audilog.SHARED_BLOCKED = info.SHARED_BLOCKED;
                    audilog.CLIENT_BIRTH_PLACE = info.CLIENT_BIRTH_PLACE;
                    audilog.CITIZENSHIP = info.CITIZENSHIP;
                    audilog.CCAT = info.CCAT;
                    audilog.PHONE_WORK = info.PHONE_WORK;
                    audilog.REG_DETAILS = info.REG_DETAILS;
                    audilog.COMPANY_NAM = info.COMPANY_NAM;
                    audilog.MS_ACNT_CONTRACT_ID = info.MS_ACNT_CONTRACT_ID;
                    audilog.MS_CONTRACT_NUMBER = info.MS_CONTRACT_NUMBER;
                    audilog.MS_CONTRACT_NAME = info.MS_CONTRACT_NAME;
                    audilog.MS_CONTR_TYPE_NAME = info.MS_CONTR_TYPE_NAME;
                    audilog.MS_INTERVAL_TYPE = info.MS_INTERVAL_TYPE;
                    audilog.MS_DATE_EXPIRE = info.MS_DATE_EXPIRE;
                    audilog.PHONE_M = info.PHONE_M;
                    audilog.PHONE_CL = info.PHONE_CL;
                    audilog.ACCT_CORE_CR = info.ACCT_CORE_CR;
                    audilog.ADD_INFO_01 = info.ADD_INFO_01;
                    audilog.INTEREST_RATE = info.INTEREST_RATE;
                    audilog.TR_FIRST_NAM = info.TR_FIRST_NAM;
                    audilog.TR_LAST_NAM = info.TR_LAST_NAM;
                    audilog.PIN_ATTEMPTS = info.PIN_ATTEMPTS;
                    audilog.MAX_PIN_ATTEMPTS = info.MAX_PIN_ATTEMPTS;
                    audilog.ACTIVE_DATE = info.ACTIVE_DATE;
                    audilog.CNTR_STAT_NAME = info.CNTR_STAT_NAME;
                    audilog.CNTR_STAT_EX_CODE = info.CNTR_STAT_EX_CODE;
                    audilog.BRANCH = info.BRANCH;
                    audilog.VCS_STATUS_VALUE_CODE = info.VCS_STATUS_VALUE_CODE;
                    audilog.LOC_ACCT = info.LOC_ACCT;
                    audilog.CONTR_STATUS_ACCT_CODE = info.CONTR_STATUS_ACCT_CODE;
                    audilog.LOC_ACCT_STATUS = info.LOC_ACCT_STATUS;
                    audilog.CARD_PRODUCT_CODE = info.CARD_PRODUCT_CODE;
                    audilog.IS_CLOSE_CONTRACT = info.IS_CLOSE_CONTRACT;
                    audilog.LOCKCARD_ID = P_LOCKCARD_ID;
                    audilog.LOCK_TYPE = P_LOCK_TYPE;
                    audilog.LOCK_CODE = P_LOCK_CODE;
                    audilog.LOCK_STATUS = "1";
                    audilog.LOCK_FROM = LOCK_FROM;
                    audilog.LOCK_TO = LOCK_TO;
                    audilog.OP_MAKER = OP_MAKER;
                    audilog.NOTE = txtP_NOTE;
                    audilog.CHECKER = "";
                    audilog.CHECK_DATE = DateTime.MinValue;
                    audilog.NOTE = txtP_NOTE;
                    audilog.BLD_CODE = P_BLD_CODE;
                    audilog.YC_FILENAME = P_YC_FILENAME;
                    audilog.YC_FILEPATH = P_YC_FILEPATH;
                    audilog.OP_MAKER_BRANCH = OP_MAKER_BRANCH;
                    audilog.REASON = P_REASON;
                    audilog.REG_NUMBER = P_REG_NUMBER;
                    audilog.CONTR_STATUS_NAME = P_CONTR_STATUS_NAME;
                    AuditLog.Log(CShared.GetIPAddress(), CShared.getSession().oAccount.Branch, CShared.getSession().oAccount.Tellerid, "ChangeCardStatusController", "SaveAction",
                    "SaveLockCard", "Create Record Change Status Card", audilog);
                    #endregion

                    OracleParameter[] parameters = new OracleParameter[52];
                    parameters[0] = connect.Parameter("P_LOCKCARD_ID", OracleDbType.Varchar2, P_LOCKCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                    parameters[1] = connect.Parameter("P_LOCK_TYPE", OracleDbType.Varchar2, P_LOCK_TYPE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                    parameters[2] = connect.Parameter("P_LOCK_CODE", OracleDbType.Varchar2, P_LOCK_CODE); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                    parameters[3] = connect.Parameter("P_LOCK_STATUS", OracleDbType.Varchar2, 1); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                    parameters[4] = connect.Parameter("P_LOCK_FROM", OracleDbType.Date, LOCK_FROM); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                    parameters[5] = connect.Parameter("P_LOCK_TO", OracleDbType.Date, LOCK_TO); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                    parameters[6] = connect.Parameter("P_MAKER", OracleDbType.Varchar2, OP_MAKER); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                    parameters[7] = connect.Parameter("P_MAKE_DATE", OracleDbType.Date, DateTime.Now); parameters[7].IsNullable = true; parameters[7].Direction = ParameterDirection.Input;
                    parameters[8] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, null); parameters[8].IsNullable = true; parameters[8].Direction = ParameterDirection.Input;
                    parameters[9] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, null); parameters[9].IsNullable = true; parameters[9].Direction = ParameterDirection.Input;
                    parameters[10] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2, ""); parameters[10].IsNullable = true; parameters[10].Direction = ParameterDirection.Input;
                    parameters[11] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2, ""); parameters[11].IsNullable = true; parameters[11].Direction = ParameterDirection.Input;
                    parameters[12] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, txtP_NOTE); parameters[12].IsNullable = true; parameters[12].Direction = ParameterDirection.Input;

                    parameters[13] = connect.Parameter("P_BLD_CODE", OracleDbType.Varchar2, P_BLD_CODE); parameters[13].IsNullable = true; parameters[13].Direction = ParameterDirection.Input;
                    parameters[14] = connect.Parameter("P_YC_FILENAME", OracleDbType.Varchar2, P_YC_FILENAME); parameters[14].IsNullable = true; parameters[14].Direction = ParameterDirection.Input;
                    parameters[15] = connect.Parameter("P_YC_FILEPATH", OracleDbType.Varchar2, P_YC_FILEPATH); parameters[15].IsNullable = true; parameters[15].Direction = ParameterDirection.Input;

                    parameters[16] = connect.Parameter("P_MAKER_BRANCH", OracleDbType.Varchar2, OP_MAKER_BRANCH); parameters[16].IsNullable = true; parameters[16].Direction = ParameterDirection.Input;
                    parameters[17] = connect.Parameter("P_CLIENT_SHORT_NAME", OracleDbType.Varchar2, info.CLIENT_SHORT_NAME); parameters[17].IsNullable = true; parameters[17].Direction = ParameterDirection.Input;
                    parameters[18] = connect.Parameter("P_CLIENT_NUMBER", OracleDbType.Varchar2, info.CLIENT_NUMBER); parameters[18].IsNullable = true; parameters[18].Direction = ParameterDirection.Input;
                    parameters[19] = connect.Parameter("P_CLIENT_DATE_OPEN", OracleDbType.Varchar2, info.CLIENT_OPEN_DATE); parameters[19].IsNullable = true; parameters[19].Direction = ParameterDirection.Input;
                    parameters[20] = connect.Parameter("P_CLIENT_BIRTH_DATE", OracleDbType.Varchar2, info.CLIENT_BIRTH_DATE); parameters[20].IsNullable = true; parameters[20].Direction = ParameterDirection.Input;
                    parameters[21] = connect.Parameter("P_CLIENT_PHONE", OracleDbType.Varchar2, info.CLIENT_PHONE); parameters[21].IsNullable = true; parameters[21].Direction = ParameterDirection.Input;
                    parameters[22] = connect.Parameter("P_CLIENT_PHONE_H", OracleDbType.Varchar2, info.CLIENT_PHONE_H); parameters[22].IsNullable = true; parameters[22].Direction = ParameterDirection.Input;
                    parameters[23] = connect.Parameter("P_CLIENT_PHONE_M", OracleDbType.Varchar2, info.CLIENT_PHONE_M); parameters[23].IsNullable = true; parameters[23].Direction = ParameterDirection.Input;
                    parameters[24] = connect.Parameter("P_CLIENT_E_MAIL", OracleDbType.Varchar2, info.CLIENT_EMAIL); parameters[24].IsNullable = true; parameters[24].Direction = ParameterDirection.Input;
                    parameters[25] = connect.Parameter("P_CLIENT_ADDRESS_LINE", OracleDbType.Varchar2, info.CLIENT_ADDRESS_LINE); parameters[25].IsNullable = true; parameters[25].Direction = ParameterDirection.Input;
                    parameters[26] = connect.Parameter("P_ACNT_CONTRACT_ID", OracleDbType.Varchar2, info.ACNT_CONTRACT_ID); parameters[26].IsNullable = true; parameters[26].Direction = ParameterDirection.Input;
                    parameters[27] = connect.Parameter("P_CONTRACT_NUMBER", OracleDbType.Varchar2, info.CONTRACT_NUMBER); parameters[27].IsNullable = true; parameters[27].Direction = ParameterDirection.Input;
                    parameters[28] = connect.Parameter("P_CONTRACT_NAME", OracleDbType.Varchar2, info.CONTRACT_NAME); parameters[28].IsNullable = true; parameters[28].Direction = ParameterDirection.Input;
                    parameters[29] = connect.Parameter("P_CONTR_TYPE_NAME", OracleDbType.Varchar2, info.CONTR_TYPE_NAME); parameters[29].IsNullable = true; parameters[29].Direction = ParameterDirection.Input;
                    parameters[30] = connect.Parameter("P_INTERVAL_TYPE", OracleDbType.Varchar2, info.INTERVAL_TYPE); parameters[30].IsNullable = true; parameters[30].Direction = ParameterDirection.Input;
                    parameters[31] = connect.Parameter("P_CONTR_STATUS", OracleDbType.Varchar2, info.CONTR_STATUS); parameters[31].IsNullable = true; parameters[31].Direction = ParameterDirection.Input;
                    parameters[32] = connect.Parameter("P_DATE_OPEN", OracleDbType.Varchar2, info.DATE_OPEN); parameters[32].IsNullable = true; parameters[32].Direction = ParameterDirection.Input;
                    parameters[33] = connect.Parameter("P_DATE_EXPIRE", OracleDbType.Varchar2, info.DATE_EXPIRE); parameters[33].IsNullable = true; parameters[33].Direction = ParameterDirection.Input;
                    parameters[34] = connect.Parameter("P_CARD_EXPIRE", OracleDbType.Varchar2, info.CARD_EXPIRE); parameters[34].IsNullable = true; parameters[34].Direction = ParameterDirection.Input;
                    parameters[35] = connect.Parameter("P_PRODUCT", OracleDbType.Varchar2, info.PRODUCT); parameters[35].IsNullable = true; parameters[35].Direction = ParameterDirection.Input;
                    parameters[36] = connect.Parameter("P_PRODUCTION_STATUS", OracleDbType.Varchar2, info.PRODUCTION_STATUS); parameters[36].IsNullable = true; parameters[36].Direction = ParameterDirection.Input;
                    parameters[37] = connect.Parameter("P_PRODUCT_NAME", OracleDbType.Varchar2, info.PRODUCT_NAME); parameters[37].IsNullable = true; parameters[37].Direction = ParameterDirection.Input;
                    parameters[38] = connect.Parameter("P_CARD_BRANCH_NO", OracleDbType.Varchar2, info.CARD_BRANCH_NO); parameters[38].IsNullable = true; parameters[38].Direction = ParameterDirection.Input;
                    parameters[39] = connect.Parameter("P_CARD_BRANCH_NAME", OracleDbType.Varchar2, info.CARD_BRANCH_NAME); parameters[39].IsNullable = true; parameters[39].Direction = ParameterDirection.Input;
                    parameters[40] = connect.Parameter("P_CLIENT_BRANCH_NO", OracleDbType.Varchar2, info.CLIENT_BRANCH_NO); parameters[40].IsNullable = true; parameters[40].Direction = ParameterDirection.Input;
                    parameters[41] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, rs.code); parameters[41].Size = 100; parameters[41].Direction = ParameterDirection.Output;
                    parameters[42] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, rs.message); parameters[42].Size = 100; parameters[42].Direction = ParameterDirection.Output;
                    parameters[43] = connect.Parameter("P_REASON", OracleDbType.Varchar2, P_REASON); parameters[43].IsNullable = true; parameters[43].Direction = ParameterDirection.Input;
                    parameters[44] = connect.Parameter("P_REG_NUMBER", OracleDbType.Varchar2, P_REG_NUMBER); parameters[44].IsNullable = true; parameters[44].Direction = ParameterDirection.Input;
                    parameters[45] = connect.Parameter("P_CONTR_STATUS_NAME", OracleDbType.Varchar2, P_CONTR_STATUS_NAME); parameters[45].IsNullable = true; parameters[45].Direction = ParameterDirection.Input;
                    parameters[46] = connect.Parameter("P_LOC_ACCT", OracleDbType.Varchar2, LOC_ACCT); parameters[46].IsNullable = true; parameters[46].Direction = ParameterDirection.Input;
                    parameters[47] = connect.Parameter("P_CONTR_STATUS_ACCT_CODE", OracleDbType.Varchar2, CONTR_STATUS_ACCT_CODE); parameters[47].IsNullable = true; parameters[47].Direction = ParameterDirection.Input;
                    parameters[48] = connect.Parameter("P_LOC_ACCT_STATUS", OracleDbType.Varchar2, LOC_ACCT_STATUS); parameters[48].IsNullable = true; parameters[48].Direction = ParameterDirection.Input;
                    parameters[49] = connect.Parameter("P_CARD_PRODUCT_CODE", OracleDbType.Varchar2, CARD_PRODUCT_CODE); parameters[49].IsNullable = true; parameters[49].Direction = ParameterDirection.Input;
                    parameters[50] = connect.Parameter("P_IS_CLOSE_CONTRACT", OracleDbType.Varchar2, IS_CLOSE_CONTRACT); parameters[50].IsNullable = true; parameters[50].Direction = ParameterDirection.Input;
                    parameters[51] = connect.Parameter("P_LOCK_TYPE_OLD", OracleDbType.Varchar2, LOCK_TYPE_OLD); parameters[51].IsNullable = true; parameters[51].Direction = ParameterDirection.Input;
                    connect.ExecuteProc(storeName, parameters);
                    rs.code = parameters[41].Value.ToString();
                    rs.message = parameters[42].Value.ToString();
                    if (rs.code != "00")
                    {
                        log.Error("[UtilsCard][SaveLockCard][Code] :" + rs.code);
                        log.Error("[UtilsCard][SaveLockCard][Message] :" + rs.message);
                        rs.message = "Lỗi ghi dữ liệu!";
                        rs.para1 = parameters[42].Value.ToString();
                    }
                    return rs;
                }
                catch (Exception ex)
                {
                    log.Error("[UtilsCard][SaveLockCard] Exception: " + ex.Message, ex);
                    rs.code = "001";
                    rs.message = "Lỗi ghi dữ liệu!";
                    rs.para1 = ex.Message;
                    return rs;
                }
                finally
                {
                    try
                    {
                        connect.Close();
                    }
                    catch { }
                }
            }
            return rs;
        }

        public Result SaveApproveLockCard(string OP_MAKER_BRANCH, string OP_CHECKER, string P_LOCKCARD_ID, string P_NOTE, string P_LOCK_STATUS)
        {
            Result rs = new Result();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_LOCKCARDS_BY_CHECKER";
            log.InfoFormat("[UtilsCard][SaveApproveLockCard] storeName={0}", storeName);
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[7];
                parameters[0] = connect.Parameter("P_LOCKCARD_ID", OracleDbType.Varchar2, P_LOCKCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_LOCK_STATUS", OracleDbType.Varchar2, P_LOCK_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, DateTime.Now); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_RESULT", OracleDbType.Varchar2); parameters[5].Size = 100; parameters[5].Direction = ParameterDirection.Output;
                parameters[6] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2); parameters[6].Size = 100; parameters[6].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                rs.code = parameters[5].Value.ToString();
                
                if (rs.code == "00")
                {
                    rs.message = "Cập nhật thông tin thành công";
                }
                else
                {
                    log.Error("[UtilsCard][SaveApproveLockCard][Code] :" + rs.code);
                    log.Error("[UtilsCard][SaveApproveLockCard][Message] :" + parameters[6].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                rs.code = "99";
                rs.message = "Lỗi khi cập nhật thông tin trên Portal";
                rs.para1 = ex.Message;
                log.Error("[UtilsCard][SaveApproveLockCard] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return rs;
        }
        public Result SaveApproveLockCardStatus(string OP_MAKER_BRANCH, string OP_CHECKER, string P_LOCKCARD_ID, string P_NOTE, string P_STATUS, string P_RESULT_CODE, string P_RESULT_MESSAGE)
        {
            Result rs = new Result();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_LOCKCARDS_STATUS";
            log.InfoFormat("[UtilsCard][SaveApproveLockCard] storeName={0}", storeName);
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[9];
                parameters[0] = connect.Parameter("P_LOCKCARD_ID", OracleDbType.Varchar2, P_LOCKCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_LOCK_STATUS", OracleDbType.Varchar2, P_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, DateTime.Now); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2, P_RESULT_CODE); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2, P_RESULT_MESSAGE); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("P_RESULT", OracleDbType.Varchar2); parameters[7].Size = 100; parameters[7].Direction = ParameterDirection.Output;
                parameters[8] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2); parameters[8].Size = 100; parameters[8].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                if (parameters[7].Value.ToString() == "00")
                {
                    rs.code = "00";
                    rs.message = "Cập nhật dữ liệu portal thành công!";
                }
                else
                {
                    rs.code = parameters[7].Value.ToString();
                    rs.message = "Lỗi khi lưu thông tin Portal thẻ";
                    rs.para1 = parameters[8].Value.ToString();
                    log.Error("[UtilsCard][SaveApproveLockCard][Code] :" + rs.code);
                    log.Error("[UtilsCard][SaveApproveLockCard][Message] :" + rs.message);
                }
            }
            catch (Exception ex)
            {
                rs.code = "99";
                rs.message = "Lỗi khi lưu thông tin Portal thẻ";
                rs.para1 = ex.Message;
                log.Error("[UtilsCard][SaveApproveLockCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return rs;
        }
        public string SaveApproveLockCardError(string OP_MAKER_BRANCH, string OP_CHECKER, string P_LOCKCARD_ID, string P_NOTE, string P_LOCK_STATUS, string P_CODE_ERROR, string P_MSG_ERROR)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_LOCKCARDS_ERROR";
            log.InfoFormat("[UtilsCard][SaveApproveLockCard] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[9];
                parameters[0] = connect.Parameter("P_LOCKCARD_ID", OracleDbType.Varchar2, P_LOCKCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_LOCK_STATUS", OracleDbType.Varchar2, P_LOCK_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("OP_CHECK_DATE", OracleDbType.Date, DateTime.Now); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_CODE_ERROR", OracleDbType.Varchar2, P_CODE_ERROR); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_MSG_ERROR", OracleDbType.Varchar2, P_MSG_ERROR); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("OP_P_RESULT", OracleDbType.Varchar2, code); parameters[7].Size = 100; parameters[7].Direction = ParameterDirection.Output;
                parameters[8] = connect.Parameter("OP_P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[8].Size = 100; parameters[8].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveApproveLockCard][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveLockCard][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveLockCard] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        public List<LockCard> LoadDataApproveLockCard(string OP_LOCKCARD_ID, string OP_LOCK_TYPE, string OP_LOCK_CODE, string OP_LOCK_STATUS,
            string OP_MAKER, string OP_BRANCH_NO, string OP_CLIENT_NUMBER, string OP_CLIENT_NAME
            , string OP_CLIENT_TEXT, string OP_CARD_NUMBER
            , string OP_CARD_NUMBER_A, string OP_CARD_NUMBER_B, string OP_LOCK_FROM, string OP_LOCK_TO)
        {
            List<LockCard> lsResult = new List<LockCard>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_LIST_LOCKCARDS";
            log.InfoFormat("[UtilsCard][LoadDataApproveLockCard] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                //        OP_LOCKCARD_ID VARCHAR2,
                //OP_LOCK_TYPE     VARCHAR2,
                //OP_LOCK_CODE VARCHAR2,
                //OP_LOCK_STATUS   VARCHAR2,
                //OP_MAKER VARCHAR2,
                //OP_BRANCH_NO     VARCHAR2,
                //OP_CLIENT_NUMBER VARCHAR2,
                //OP_CLIENT_NAME   VARCHAR2,
                //OP_CLIENT_TEXT VARCHAR2,
                //OP_CARD_NUMBER   VARCHAR2,
                //OP_CARD_NUMBER_A VARCHAR2,
                //OP_CARD_NUMBER_B VARCHAR2,
                //OP_LOCK_FROM VARCHAR2,
                //OP_LOCK_TO       VARCHAR2,
                //OP_RESULT_CUR OUT SYS_REFCURSOR

                OracleParameter[] parameters = new OracleParameter[15];

                parameters[0] = connect.Parameter("OP_LOCKCARD_ID", OracleDbType.Varchar2, OP_LOCKCARD_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;


                parameters[1] = connect.Parameter("OP_LOCK_TYPE", OracleDbType.Varchar2, OP_LOCK_TYPE);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_LOCK_CODE", OracleDbType.Varchar2, OP_LOCK_CODE);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_LOCK_STATUS", OracleDbType.Varchar2, OP_LOCK_STATUS);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_MAKER", OracleDbType.Varchar2, OP_MAKER);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_BRANCH_NO", OracleDbType.Varchar2, OP_BRANCH_NO);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_CLIENT_NUMBER", OracleDbType.Varchar2, OP_CLIENT_NUMBER);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("OP_CLIENT_NAME", OracleDbType.Varchar2, OP_CLIENT_NAME);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                parameters[8] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;

                parameters[9] = connect.Parameter("OP_CARD_NUMBER", OracleDbType.Varchar2, OP_CARD_NUMBER);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;


                parameters[10] = connect.Parameter("OP_CARD_NUMBER_A", OracleDbType.Varchar2, OP_CARD_NUMBER_A);
                parameters[10].IsNullable = true;
                parameters[10].Direction = ParameterDirection.Input;
                parameters[11] = connect.Parameter("OP_CARD_NUMBER_B", OracleDbType.Varchar2, OP_CARD_NUMBER_B);
                parameters[11].IsNullable = true;
                parameters[11].Direction = ParameterDirection.Input;

                DateTime? LOCK_FROM = null, LOCK_TO = null;
                if (!string.IsNullOrEmpty(OP_LOCK_FROM))
                {
                    DateTime date = new DateTime();
                    if (DateTime.TryParseExact(OP_LOCK_FROM, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                        LOCK_FROM = date;
                }
                if (!string.IsNullOrEmpty(OP_LOCK_TO))
                {
                    DateTime date = new DateTime();
                    if (DateTime.TryParseExact(OP_LOCK_TO, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out date))
                        LOCK_TO = date;
                }

                parameters[12] = connect.Parameter("OP_LOCK_FROM", OracleDbType.Date, LOCK_FROM);
                parameters[12].IsNullable = true;
                parameters[12].Direction = ParameterDirection.Input;
                parameters[13] = connect.Parameter("OP_LOCK_TO", OracleDbType.Date, LOCK_TO);
                parameters[13].IsNullable = true;
                parameters[13].Direction = ParameterDirection.Input;

                (parameters[14] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<LockCard>();
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataApproveLockCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;

        }

        /// <summary>
        /// CancelCard Utils
        /// </summary>
        #region ActiveCard Utils
        public List<CardInfo> LoadDataForActiveCard(string OP_ACNT_CONTRACT_ID, string OP_CARD_BRANCH, string OP_CLIENT_TEXT, string OP_CARD_NUMBER_A
            , string OP_CARD_NUMBER_B, string OP_CARD_DATE_FROM, string OP_CARD_DATE_TO, string OP_CARD_NUMBER = null)
        {
            List<CardInfo> lsResult = new List<CardInfo>();
            DataTable dt = null;
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_LIST_ACTIVECARD";
            log.InfoFormat("[UtilsCard][LoadDataListCardInfo] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[10];

                parameters[0] = connect.Parameter("OP_ACNT_CONTRACT_ID", OracleDbType.Varchar2, OP_ACNT_CONTRACT_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("OP_CARD_BRANCH", OracleDbType.Varchar2, OP_CARD_BRANCH);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_CARD_NUMBER_A", OracleDbType.Varchar2, OP_CARD_NUMBER_A);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CARD_NUMBER_B", OracleDbType.Varchar2, OP_CARD_NUMBER_B);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_CARD_DATE_FROM", OracleDbType.Varchar2, OP_CARD_DATE_FROM);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_CARD_DATE_TO", OracleDbType.Varchar2, OP_CARD_DATE_TO);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("OP_CARD_TYPE", OracleDbType.Varchar2, null);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                (parameters[8] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                parameters[9] = connect.Parameter("OP_CARD_NUMBER", OracleDbType.Varchar2, OP_CARD_NUMBER);
                parameters[9].IsNullable = true;
                parameters[9].Direction = ParameterDirection.Input;

                connect.FillProc(storeName, parameters, ref ds);
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataListCardInfo] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            if (dt != null)
                lsResult = dt.ToList<CardInfo>();
            return lsResult;
        }

        public List<ActiveCard> LoadDataForApproveActiveCard(string OP_ACTIVECARD_ID, string OP_ACTIVE_STATUS, string OP_MAKER, string OP_BRANCH_NO, string OP_CLIENT_TEXT, string OP_DATE_FROM, string OP_DATE_TO)
        {
            DataTable dt = null;
            List<ActiveCard> lsResult = new List<ActiveCard>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_LIST_ACTIVECARD";
            log.InfoFormat("[UtilsCard][LoadDataForApproveActiveCard] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[8];

                parameters[0] = connect.Parameter("OP_ACTIVECARD_ID", OracleDbType.Varchar2, OP_ACTIVECARD_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("OP_ACTIVE_STATUS", OracleDbType.Varchar2, OP_ACTIVE_STATUS);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_MAKER", OracleDbType.Varchar2, OP_MAKER);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_BRANCH_NO", OracleDbType.Varchar2, OP_BRANCH_NO);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_DATE_FROM", OracleDbType.Varchar2, OP_DATE_FROM);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_DATE_TO", OracleDbType.Varchar2, OP_DATE_TO);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                (parameters[7] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataForApproveActiveCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            if (dt != null)
                lsResult = dt.ToList<ActiveCard>();
            return lsResult;
        }
        public string SaveActiveCard(string OP_ACNT_CONTRACT_ID, string OP_MAKER, string OP_MAKER_BRANCH, ref string OP_ACTIVECARD_ID, string P_ACTIVECARD_ID, string P_NOTE = "")
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".INS_ACTIVECARDS";
            log.InfoFormat("[UtilsCard][SaveActiveCard] storeName={0}", storeName);
            List<CardInfo> Listinfo = LoadDataForActiveCard(OP_ACNT_CONTRACT_ID, null, null, null, null, null, null);
            CardInfo info = Listinfo[0];
            /// Ghi audilog
            AudilogChangeStatus audilog = new AudilogChangeStatus();
            audilog.REG_NUMBER = info.REG_NUMBER;
            audilog.CLIENT_SHORT_NAME = info.CLIENT_SHORT_NAME;
            audilog.CLIENT_NUMBER = info.CLIENT_NUMBER;
            audilog.CLIENT_OPEN_DATE = info.CLIENT_OPEN_DATE;
            audilog.CLIENT_BIRTH_DATE = info.CLIENT_BIRTH_DATE;
            audilog.CLIENT_PHONE = info.CLIENT_PHONE;
            audilog.CLIENT_PHONE_H = info.CLIENT_PHONE_H;
            audilog.CLIENT_PHONE_M = info.CLIENT_PHONE_M;
            audilog.CLIENT_EMAIL = info.CLIENT_EMAIL;
            audilog.ADDRESS_LINE_1 = info.ADDRESS_LINE_1;
            audilog.ADDRESS_LINE_2 = info.ADDRESS_LINE_2;
            audilog.ADDRESS_LINE_3 = info.ADDRESS_LINE_3;
            audilog.ADDRESS_LINE_4 = info.ADDRESS_LINE_4;
            audilog.CLIENT_ADDRESS_LINE = info.CLIENT_ADDRESS_LINE;
            audilog.REG_ADDR = info.REG_ADDR;
            audilog.REG_ADDR_CITY = info.REG_ADDR_CITY;
            audilog.REG_ADDR_DISTRICT = info.REG_ADDR_DISTRICT;
            audilog.REG_ADDR_WARD = info.REG_ADDR_WARD;
            audilog.REG_ADDR_LINE = info.REG_ADDR_LINE;
            audilog.DEL_ADDR = info.DEL_ADDR;
            audilog.DEL_ADDR_CITY = info.DEL_ADDR_CITY;
            audilog.DEL_ADDR_DISTRICT = info.DEL_ADDR_DISTRICT;
            audilog.DEL_ADDR_WARD = info.DEL_ADDR_WARD;
            audilog.DEL_ADDR_LINE = info.DEL_ADDR_LINE;
            audilog.STMT_ADDR = info.STMT_ADDR;
            audilog.STMT_ADDR_CITY = info.STMT_ADDR_CITY;
            audilog.STMT_ADDR_DISTRICT = info.STMT_ADDR_DISTRICT;
            audilog.STMT_ADDR_WARD = info.STMT_ADDR_WARD;
            audilog.STMT_ADDR_LINE = info.STMT_ADDR_LINE;
            audilog.ACNT_CONTRACT_ID = info.ACNT_CONTRACT_ID;
            audilog.CONTRACT_NUMBER = info.CONTRACT_NUMBER;
            audilog.CONTRACT_NAME = info.CONTRACT_NAME;
            audilog.CONTR_TYPE_NAME = info.CONTR_TYPE_NAME;
            audilog.INTERVAL_TYPE = info.INTERVAL_TYPE;
            audilog.CONTR_STATUS = info.CONTR_STATUS;
            audilog.DATE_OPEN = info.DATE_OPEN;
            audilog.DATE_EXPIRE = info.DATE_EXPIRE;
            audilog.CARD_EXPIRE = info.CARD_EXPIRE;
            audilog.PRODUCT = info.PRODUCT;
            audilog.PRODUCTION_STATUS = info.PRODUCTION_STATUS;
            audilog.PRODUCT_NAME = info.PRODUCT_NAME;
            audilog.PRODUCT_CODE = info.PRODUCT_CODE;
            audilog.CARD_BRANCH_NO = info.CARD_BRANCH_NO;
            audilog.CARD_BRANCH_NAME = info.CARD_BRANCH_NAME;
            audilog.CLIENT_BRANCH_NO = info.CLIENT_BRANCH_NO;
            audilog.CONTR_STATUS_CODE = info.CONTR_STATUS_CODE;
            audilog.CONTR_STATUS_NAME = info.CONTR_STATUS_NAME;
            audilog.PRODUCT_GROUP = info.PRODUCT_GROUP;
            audilog.RBS_NUMBER = info.RBS_NUMBER;
            audilog.ACNT_CONTRACT__OID = info.ACNT_CONTRACT__OID;
            audilog.QUESTION = info.QUESTION;
            audilog.AWS = info.AWS;
            audilog.SHARED_BLOCKED = info.SHARED_BLOCKED;
            audilog.CLIENT_BIRTH_PLACE = info.CLIENT_BIRTH_PLACE;
            audilog.CITIZENSHIP = info.CITIZENSHIP;
            audilog.CCAT = info.CCAT;
            audilog.PHONE_WORK = info.PHONE_WORK;
            audilog.REG_DETAILS = info.REG_DETAILS;
            audilog.COMPANY_NAM = info.COMPANY_NAM;
            audilog.MS_ACNT_CONTRACT_ID = info.MS_ACNT_CONTRACT_ID;
            audilog.MS_CONTRACT_NUMBER = info.MS_CONTRACT_NUMBER;
            audilog.MS_CONTRACT_NAME = info.MS_CONTRACT_NAME;
            audilog.MS_CONTR_TYPE_NAME = info.MS_CONTR_TYPE_NAME;
            audilog.MS_INTERVAL_TYPE = info.MS_INTERVAL_TYPE;
            audilog.MS_DATE_EXPIRE = info.MS_DATE_EXPIRE;
            audilog.PHONE_M = info.PHONE_M;
            audilog.PHONE_CL = info.PHONE_CL;
            audilog.ACCT_CORE_CR = info.ACCT_CORE_CR;
            audilog.ADD_INFO_01 = info.ADD_INFO_01;
            audilog.INTEREST_RATE = info.INTEREST_RATE;
            audilog.TR_FIRST_NAM = info.TR_FIRST_NAM;
            audilog.TR_LAST_NAM = info.TR_LAST_NAM;
            audilog.PIN_ATTEMPTS = info.PIN_ATTEMPTS;
            audilog.MAX_PIN_ATTEMPTS = info.MAX_PIN_ATTEMPTS;
            audilog.ACTIVE_DATE = info.ACTIVE_DATE;
            audilog.CNTR_STAT_NAME = info.CNTR_STAT_NAME;
            audilog.CNTR_STAT_EX_CODE = info.CNTR_STAT_EX_CODE;
            audilog.BRANCH = info.BRANCH;
            audilog.VCS_STATUS_VALUE_CODE = info.VCS_STATUS_VALUE_CODE;
            audilog.LOC_ACCT = info.LOC_ACCT;
            audilog.CONTR_STATUS_ACCT_CODE = info.CONTR_STATUS_ACCT_CODE;
            audilog.LOC_ACCT_STATUS = info.LOC_ACCT_STATUS;
            audilog.CARD_PRODUCT_CODE = info.CARD_PRODUCT_CODE;
            audilog.IS_CLOSE_CONTRACT = info.IS_CLOSE_CONTRACT;
            audilog.OP_ACNT_CONTRACT_ID = OP_ACNT_CONTRACT_ID;
            audilog.OP_ACTIVECARD_ID = OP_ACTIVECARD_ID;
            audilog.OP_MAKER_BRANCH = OP_MAKER_BRANCH;
            audilog.OP_MAKER = OP_MAKER;
            AuditLog.Log(CShared.GetIPAddress(), CShared.getSession().oAccount.Branch, CShared.getSession().oAccount.Tellerid, "ActiveCardController", "doActiveCard",
            "SaveActiveCard", "Create Record Active Card", audilog);
            //end

            DataSet ds = new DataSet();
            string code = "";
            string message = "";
            try
            {

                string CREATE_ACTIVECARD_STATUS = "1";
                OracleParameter[] parameters = new OracleParameter[36];

                parameters[0] = connect.Parameter("P_ACTIVECARD_ID", OracleDbType.Varchar2, P_ACTIVECARD_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_ACTIVECARD_STATUS", OracleDbType.Varchar2, CREATE_ACTIVECARD_STATUS); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_MAKER", OracleDbType.Varchar2, OP_MAKER); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_MAKE_DATE", OracleDbType.Date, DateTime.Now.ToString("dd-MMM-yyyy")); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, null); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, null); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2, ""); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2, ""); parameters[7].IsNullable = true; parameters[7].Direction = ParameterDirection.Input;
                parameters[8] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[8].IsNullable = true; parameters[8].Direction = ParameterDirection.Input;
                parameters[9] = connect.Parameter("P_MAKER_BRANCH", OracleDbType.Varchar2, OP_MAKER_BRANCH); parameters[9].IsNullable = true; parameters[9].Direction = ParameterDirection.Input;

                parameters[10] = connect.Parameter("P_CLIENT_SHORT_NAME", OracleDbType.Varchar2, info.CLIENT_SHORT_NAME); parameters[10].IsNullable = true; parameters[10].Direction = ParameterDirection.Input;
                parameters[11] = connect.Parameter("P_CLIENT_NUMBER", OracleDbType.Varchar2, info.CLIENT_NUMBER); parameters[11].IsNullable = true; parameters[11].Direction = ParameterDirection.Input;
                parameters[12] = connect.Parameter("P_CLIENT_DATE_OPEN", OracleDbType.Varchar2, info.CLIENT_OPEN_DATE); parameters[12].IsNullable = true; parameters[12].Direction = ParameterDirection.Input;
                parameters[13] = connect.Parameter("P_CLIENT_BIRTH_DATE", OracleDbType.Varchar2, info.CLIENT_BIRTH_DATE); parameters[13].IsNullable = true; parameters[13].Direction = ParameterDirection.Input;
                parameters[14] = connect.Parameter("P_CLIENT_PHONE", OracleDbType.Varchar2, info.CLIENT_PHONE); parameters[14].IsNullable = true; parameters[14].Direction = ParameterDirection.Input;
                parameters[15] = connect.Parameter("P_CLIENT_PHONE_H", OracleDbType.Varchar2, info.CLIENT_PHONE_H); parameters[15].IsNullable = true; parameters[15].Direction = ParameterDirection.Input;
                parameters[16] = connect.Parameter("P_CLIENT_PHONE_M", OracleDbType.Varchar2, info.CLIENT_PHONE_M); parameters[16].IsNullable = true; parameters[16].Direction = ParameterDirection.Input;
                parameters[17] = connect.Parameter("P_CLIENT_E_MAIL", OracleDbType.Varchar2, info.CLIENT_EMAIL); parameters[17].IsNullable = true; parameters[17].Direction = ParameterDirection.Input;
                parameters[18] = connect.Parameter("P_CLIENT_ADDRESS_LINE", OracleDbType.Varchar2, info.CLIENT_ADDRESS_LINE); parameters[18].IsNullable = true; parameters[18].Direction = ParameterDirection.Input;
                parameters[19] = connect.Parameter("P_ACNT_CONTRACT_ID", OracleDbType.Varchar2, info.ACNT_CONTRACT_ID); parameters[19].IsNullable = true; parameters[19].Direction = ParameterDirection.Input;
                parameters[20] = connect.Parameter("P_CONTRACT_NUMBER", OracleDbType.Varchar2, info.CONTRACT_NUMBER); parameters[20].IsNullable = true; parameters[20].Direction = ParameterDirection.Input;
                parameters[21] = connect.Parameter("P_CONTRACT_NAME", OracleDbType.Varchar2, info.CONTRACT_NAME); parameters[21].IsNullable = true; parameters[21].Direction = ParameterDirection.Input;
                parameters[22] = connect.Parameter("P_CONTR_TYPE_NAME", OracleDbType.Varchar2, info.CONTR_TYPE_NAME); parameters[22].IsNullable = true; parameters[22].Direction = ParameterDirection.Input;
                parameters[23] = connect.Parameter("P_INTERVAL_TYPE", OracleDbType.Varchar2, info.INTERVAL_TYPE); parameters[23].IsNullable = true; parameters[23].Direction = ParameterDirection.Input;
                parameters[24] = connect.Parameter("P_CONTR_STATUS", OracleDbType.Varchar2, info.CONTR_STATUS); parameters[24].IsNullable = true; parameters[24].Direction = ParameterDirection.Input;
                parameters[25] = connect.Parameter("P_DATE_OPEN", OracleDbType.Varchar2, info.DATE_OPEN); parameters[25].IsNullable = true; parameters[25].Direction = ParameterDirection.Input;
                parameters[26] = connect.Parameter("P_DATE_EXPIRE", OracleDbType.Varchar2, info.DATE_EXPIRE); parameters[26].IsNullable = true; parameters[26].Direction = ParameterDirection.Input;
                parameters[27] = connect.Parameter("P_CARD_EXPIRE", OracleDbType.Varchar2, info.CARD_EXPIRE); parameters[27].IsNullable = true; parameters[27].Direction = ParameterDirection.Input;
                parameters[28] = connect.Parameter("P_PRODUCT", OracleDbType.Varchar2, info.PRODUCT); parameters[28].IsNullable = true; parameters[28].Direction = ParameterDirection.Input;
                parameters[29] = connect.Parameter("P_PRODUCTION_STATUS", OracleDbType.Varchar2, info.PRODUCTION_STATUS); parameters[29].IsNullable = true; parameters[29].Direction = ParameterDirection.Input;
                parameters[30] = connect.Parameter("P_PRODUCT_NAME", OracleDbType.Varchar2, info.PRODUCT_NAME); parameters[30].IsNullable = true; parameters[30].Direction = ParameterDirection.Input;
                parameters[31] = connect.Parameter("P_CARD_BRANCH_NO", OracleDbType.Varchar2, info.CARD_BRANCH_NO); parameters[31].IsNullable = true; parameters[31].Direction = ParameterDirection.Input;
                parameters[32] = connect.Parameter("P_CARD_BRANCH_NAME", OracleDbType.Varchar2, info.CARD_BRANCH_NAME); parameters[32].IsNullable = true; parameters[32].Direction = ParameterDirection.Input;
                parameters[33] = connect.Parameter("P_CLIENT_BRANCH_NO", OracleDbType.Varchar2, info.CLIENT_BRANCH_NO); parameters[33].IsNullable = true; parameters[33].Direction = ParameterDirection.Input;

                parameters[34] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, code); parameters[34].Size = 100; parameters[34].Direction = ParameterDirection.Output;
                parameters[35] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[35].Size = 100; parameters[35].Direction = ParameterDirection.Output;

                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveActiveCard][Code] :" + code);
                log.Error("[UtilsCard][SaveActiveCard][Message] :" + message);
                OP_ACTIVECARD_ID = P_ACTIVECARD_ID;
                return code;
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveActiveCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return code;
        }
        public string SaveApproveActiveCard(string OP_MAKER_BRANCH, string OP_CHECKER, string P_ACTIVECARD_ID, string P_NOTE, string P_LOCK_STATUS)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_ACTIVECARDS_BY_CHECKER";
            log.InfoFormat("[UtilsCard][SaveApproveActiveCard] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[7];
                parameters[0] = connect.Parameter("P_ACTIVECARD_ID", OracleDbType.Varchar2, P_ACTIVECARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_ACTIVECARD_STATUS", OracleDbType.Varchar2, P_LOCK_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, DateTime.Now.ToString("dd-MMM-yyyy")); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, code); parameters[5].Size = 100; parameters[5].Direction = ParameterDirection.Output;
                parameters[6] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[6].Size = 100; parameters[6].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveApproveActiveCard][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveActiveCard][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveActiveCard] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        public string SaveApproveActiveCardStatus(string OP_MAKER_BRANCH, string OP_CHECKER, string P_ACTIVECARD_ID, string P_NOTE, string P_STATUS, string P_RESULT_CODE, string P_RESULT_MESSAGE)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_ACTIVECARDS_STATUS";
            log.InfoFormat("[UtilsCard][SaveApproveActiveCardStatus] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[9];
                parameters[0] = connect.Parameter("P_ACTIVECARD_ID", OracleDbType.Varchar2, P_ACTIVECARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_ACTIVECARD_STATUS", OracleDbType.Varchar2, P_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, null); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2, P_RESULT_CODE); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2, P_RESULT_MESSAGE); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, code); parameters[7].Size = 100; parameters[7].Direction = ParameterDirection.Output;
                parameters[8] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[8].Size = 100; parameters[8].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);

                if (parameters[7].Value.ToString() == "00")
                {
                    code = "00";
                    message = "Tạo yêu cầu Thành công!";
                }
                else
                {
                    code = parameters[7].Value.ToString();
                    message = parameters[8].Value.ToString();
                }
                log.Error("[UtilsCard][SaveApproveActiveCardStatus][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveActiveCardStatus][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveActiveCardStatus] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        public string SaveApproveActiveCardError(string OP_MAKER_BRANCH, string OP_CHECKER, string P_ACTIVECARD_ID, string P_NOTE, string P_LOCK_STATUS, string P_CODE_ERROR, string P_MSG_ERROR)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_ACTIVECARDS_ERROR";
            log.InfoFormat("[UtilsCard][SaveApproveActiveCardError] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[7];
                parameters[0] = connect.Parameter("P_ACTIVECARD_ID", OracleDbType.Varchar2, P_ACTIVECARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_ACTIVE_STATUS", OracleDbType.Varchar2, P_LOCK_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("OP_CHECK_DATE", OracleDbType.Date, DateTime.Now); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_CODE_ERROR", OracleDbType.Varchar2, P_CODE_ERROR); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_MSG_ERROR", OracleDbType.Varchar2, P_MSG_ERROR); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("OP_P_RESULT", OracleDbType.Varchar2, code); parameters[7].Size = 100; parameters[7].Direction = ParameterDirection.Output;
                parameters[8] = connect.Parameter("OP_P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[8].Size = 100; parameters[8].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveApproveActiveCardError][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveActiveCardError][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveActiveCardError] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        /// <summary>
        /// AcvitceCard Utils
        /// </summary>
        #endregion

        /// <summary>
        /// CancelCard Utils
        /// </summary>
        #region CancelCard Utils
        public List<CardInfo> LoadDataForCancelCard(string OP_ACNT_CONTRACT_ID, string OP_CARD_BRANCH, string OP_CLIENT_TEXT, string OP_CARD_NUMBER_A
            , string OP_CARD_NUMBER_B, string OP_CARD_DATE_FROM, string OP_CARD_DATE_TO)
        {
            List<CardInfo> lsResult = new List<CardInfo>();
            DataTable dt = null;
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_LIST_CANCELCARD";
            log.InfoFormat("[UtilsCard][LoadDataListCardInfo] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[9];

                parameters[0] = connect.Parameter("OP_ACNT_CONTRACT_ID", OracleDbType.Varchar2, OP_ACNT_CONTRACT_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("OP_CARD_BRANCH", OracleDbType.Varchar2, OP_CARD_BRANCH);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_CARD_NUMBER_A", OracleDbType.Varchar2, OP_CARD_NUMBER_A);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CARD_NUMBER_B", OracleDbType.Varchar2, OP_CARD_NUMBER_B);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_CARD_DATE_FROM", OracleDbType.Varchar2, OP_CARD_DATE_FROM);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_CARD_DATE_TO", OracleDbType.Varchar2, OP_CARD_DATE_TO);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("OP_CARD_TYPE", OracleDbType.Varchar2, null);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;

                (parameters[8] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataListCardInfo] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            if (dt != null)
                lsResult = dt.ToList<CardInfo>();
            return lsResult;
        }

        public List<CancelCard> LoadDataForApproveCancelCard(string OP_CANCELCARD_ID, string OP_CANCELCARD_STATUS, string OP_MAKER, string OP_BRANCH_NO, string OP_CLIENT_TEXT, string OP_DATE_FROM, string OP_DATE_TO)
        {
            DataTable dt = null;
            List<CancelCard> lsResult = new List<CancelCard>();
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".GET_LIST_CANCELCARD";
            log.InfoFormat("[UtilsCard][LoadDataForApproveCancelCard] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[8];

                parameters[0] = connect.Parameter("OP_CANCELCARD_ID", OracleDbType.Varchar2, OP_CANCELCARD_ID);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                parameters[1] = connect.Parameter("OP_CANCELCARD_STATUS", OracleDbType.Varchar2, OP_CANCELCARD_STATUS);
                parameters[1].IsNullable = true;
                parameters[1].Direction = ParameterDirection.Input;

                parameters[2] = connect.Parameter("OP_MAKER", OracleDbType.Varchar2, OP_MAKER);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;

                parameters[3] = connect.Parameter("OP_BRANCH_NO", OracleDbType.Varchar2, OP_BRANCH_NO);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;

                parameters[4] = connect.Parameter("OP_CLIENT_TEXT", OracleDbType.Varchar2, OP_CLIENT_TEXT);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;

                parameters[5] = connect.Parameter("OP_DATE_FROM", OracleDbType.Varchar2, OP_DATE_FROM);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;

                parameters[6] = connect.Parameter("OP_DATE_TO", OracleDbType.Varchar2, OP_DATE_TO);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;

                (parameters[7] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;


                connect.FillProc(storeName, parameters, ref ds);
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadDataForApproveCancelCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            if (dt != null)
                lsResult = dt.ToList<CancelCard>();
            return lsResult;
        }
        public string SaveCancelCard(string OP_ACNT_CONTRACT_ID, string OP_MAKER, string OP_MAKER_BRANCH, string OP_YC_FILENAME, string OP_YC_FILEPATH, ref string OP_CANCELCARD_ID)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".INS_CANCELCARDS";
            log.InfoFormat("[UtilsCard][SaveCancelCard] storeName={0}", storeName);
            List<CardInfo> Listinfo = LoadDataForCancelCard(OP_ACNT_CONTRACT_ID, null, null, null, null, null, null);
            CardInfo info = Listinfo[0];
            DataSet ds = new DataSet();
            string code = "";
            string message = "";
            try
            {
                string P_CANCELCARD_ID = "";
                if (OP_CANCELCARD_ID == "")
                    P_CANCELCARD_ID = Guid.NewGuid().ToString();
                else P_CANCELCARD_ID = OP_CANCELCARD_ID;
                string CREATE_ACTIVECARD_STATUS = "1";
                OracleParameter[] parameters = new OracleParameter[39];

                parameters[0] = connect.Parameter("P_CANCELCARD_ID", OracleDbType.Varchar2, P_CANCELCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_CANCELCARD_STATUS", OracleDbType.Varchar2, CREATE_ACTIVECARD_STATUS); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_MAKER", OracleDbType.Varchar2, OP_MAKER); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_MAKE_DATE", OracleDbType.Date, DateTime.Now.ToString("dd-MMM-yyyy")); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, null); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, null); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2, ""); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2, ""); parameters[7].IsNullable = true; parameters[7].Direction = ParameterDirection.Input;
                parameters[8] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, ""); parameters[8].IsNullable = true; parameters[8].Direction = ParameterDirection.Input;
                parameters[9] = connect.Parameter("P_BLD_CODE", OracleDbType.Varchar2, ""); parameters[9].IsNullable = true; parameters[9].Direction = ParameterDirection.Input;
                parameters[10] = connect.Parameter("P_YC_FILENAME", OracleDbType.Varchar2, OP_YC_FILENAME); parameters[10].IsNullable = true; parameters[10].Direction = ParameterDirection.Input;
                parameters[11] = connect.Parameter("P_YC_FILEPATH", OracleDbType.Varchar2, OP_YC_FILEPATH); parameters[11].IsNullable = true; parameters[11].Direction = ParameterDirection.Input;
                parameters[12] = connect.Parameter("P_MAKER_BRANCH", OracleDbType.Varchar2, OP_MAKER_BRANCH); parameters[12].IsNullable = true; parameters[12].Direction = ParameterDirection.Input;

                parameters[13] = connect.Parameter("P_CLIENT_SHORT_NAME", OracleDbType.Varchar2, info.CLIENT_SHORT_NAME); parameters[13].IsNullable = true; parameters[13].Direction = ParameterDirection.Input;
                parameters[14] = connect.Parameter("P_CLIENT_NUMBER", OracleDbType.Varchar2, info.CLIENT_NUMBER); parameters[14].IsNullable = true; parameters[14].Direction = ParameterDirection.Input;
                parameters[15] = connect.Parameter("P_CLIENT_DATE_OPEN", OracleDbType.Varchar2, info.CLIENT_OPEN_DATE); parameters[15].IsNullable = true; parameters[15].Direction = ParameterDirection.Input;
                parameters[16] = connect.Parameter("P_CLIENT_BIRTH_DATE", OracleDbType.Varchar2, info.CLIENT_BIRTH_DATE); parameters[16].IsNullable = true; parameters[16].Direction = ParameterDirection.Input;
                parameters[17] = connect.Parameter("P_CLIENT_PHONE", OracleDbType.Varchar2, info.CLIENT_PHONE); parameters[17].IsNullable = true; parameters[17].Direction = ParameterDirection.Input;
                parameters[18] = connect.Parameter("P_CLIENT_PHONE_H", OracleDbType.Varchar2, info.CLIENT_PHONE_H); parameters[18].IsNullable = true; parameters[18].Direction = ParameterDirection.Input;
                parameters[19] = connect.Parameter("P_CLIENT_PHONE_M", OracleDbType.Varchar2, info.CLIENT_PHONE_M); parameters[19].IsNullable = true; parameters[19].Direction = ParameterDirection.Input;
                parameters[20] = connect.Parameter("P_CLIENT_E_MAIL", OracleDbType.Varchar2, info.CLIENT_EMAIL); parameters[20].IsNullable = true; parameters[20].Direction = ParameterDirection.Input;

                parameters[21] = connect.Parameter("P_CLIENT_ADDRESS_LINE", OracleDbType.Varchar2, info.CLIENT_ADDRESS_LINE); parameters[21].IsNullable = true; parameters[21].Direction = ParameterDirection.Input;
                parameters[22] = connect.Parameter("P_ACNT_CONTRACT_ID", OracleDbType.Varchar2, info.ACNT_CONTRACT_ID); parameters[22].IsNullable = true; parameters[22].Direction = ParameterDirection.Input;
                parameters[23] = connect.Parameter("P_CONTRACT_NUMBER", OracleDbType.Varchar2, info.CONTRACT_NUMBER); parameters[23].IsNullable = true; parameters[23].Direction = ParameterDirection.Input;
                parameters[24] = connect.Parameter("P_CONTRACT_NAME", OracleDbType.Varchar2, info.CONTRACT_NAME); parameters[24].IsNullable = true; parameters[24].Direction = ParameterDirection.Input;
                parameters[25] = connect.Parameter("P_CONTR_TYPE_NAME", OracleDbType.Varchar2, info.CONTR_TYPE_NAME); parameters[25].IsNullable = true; parameters[25].Direction = ParameterDirection.Input;
                parameters[26] = connect.Parameter("P_INTERVAL_TYPE", OracleDbType.Varchar2, info.INTERVAL_TYPE); parameters[26].IsNullable = true; parameters[26].Direction = ParameterDirection.Input;
                parameters[27] = connect.Parameter("P_CONTR_STATUS", OracleDbType.Varchar2, info.CONTR_STATUS); parameters[27].IsNullable = true; parameters[27].Direction = ParameterDirection.Input;
                parameters[28] = connect.Parameter("P_DATE_OPEN", OracleDbType.Varchar2, info.DATE_OPEN); parameters[28].IsNullable = true; parameters[28].Direction = ParameterDirection.Input;
                parameters[29] = connect.Parameter("P_DATE_EXPIRE", OracleDbType.Varchar2, info.DATE_EXPIRE); parameters[29].IsNullable = true; parameters[29].Direction = ParameterDirection.Input;
                parameters[30] = connect.Parameter("P_CARD_EXPIRE", OracleDbType.Varchar2, info.CARD_EXPIRE); parameters[30].IsNullable = true; parameters[30].Direction = ParameterDirection.Input;

                parameters[31] = connect.Parameter("P_PRODUCT", OracleDbType.Varchar2, info.PRODUCT); parameters[31].IsNullable = true; parameters[31].Direction = ParameterDirection.Input;
                parameters[32] = connect.Parameter("P_PRODUCTION_STATUS", OracleDbType.Varchar2, info.PRODUCTION_STATUS); parameters[32].IsNullable = true; parameters[32].Direction = ParameterDirection.Input;
                parameters[33] = connect.Parameter("P_PRODUCT_NAME", OracleDbType.Varchar2, info.PRODUCT_NAME); parameters[33].IsNullable = true; parameters[33].Direction = ParameterDirection.Input;
                parameters[34] = connect.Parameter("P_CARD_BRANCH_NO", OracleDbType.Varchar2, info.CARD_BRANCH_NO); parameters[34].IsNullable = true; parameters[34].Direction = ParameterDirection.Input;
                parameters[35] = connect.Parameter("P_CARD_BRANCH_NAME", OracleDbType.Varchar2, info.CARD_BRANCH_NAME); parameters[35].IsNullable = true; parameters[35].Direction = ParameterDirection.Input;
                parameters[36] = connect.Parameter("P_CLIENT_BRANCH_NO", OracleDbType.Varchar2, info.CLIENT_BRANCH_NO); parameters[36].IsNullable = true; parameters[36].Direction = ParameterDirection.Input;

                parameters[37] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, code); parameters[37].Size = 100; parameters[37].Direction = ParameterDirection.Output;
                parameters[38] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[38].Size = 100; parameters[38].Direction = ParameterDirection.Output;

                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveCancelCard][Code] :" + code);
                log.Error("[UtilsCard][SaveCancelCard][Message] :" + message);
                OP_CANCELCARD_ID = P_CANCELCARD_ID;
                return code;
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveCancelCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return code;
        }
        public string SaveApproveCancelCard(string OP_MAKER_BRANCH, string OP_CHECKER, string P_CANCELCARD_ID, string P_NOTE, string P_CANCELCARD_STATUS)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_CANCELCARDS_BY_CHECKER";
            log.InfoFormat("[UtilsCard][SaveApproveCancelCard] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[7];
                parameters[0] = connect.Parameter("P_CANCELCARD_ID", OracleDbType.Varchar2, P_CANCELCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_CANCELCARD_STATUS", OracleDbType.Varchar2, P_CANCELCARD_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, DateTime.Now.ToString("dd-MMM-yyyy")); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, code); parameters[5].Size = 100; parameters[5].Direction = ParameterDirection.Output;
                parameters[6] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[6].Size = 100; parameters[6].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveApproveCancelCard][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveCancelCard][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveCancelCard] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        public string SaveApproveCancelCardStatus(string OP_MAKER_BRANCH, string OP_CHECKER, string P_CANCELCARD_ID, string P_NOTE, string P_STATUS, string P_RESULT_CODE, string P_RESULT_MESSAGE)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_CANCELCARDS_STATUS";
            log.InfoFormat("[UtilsCard][SaveApproveCancelCardStatus] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[9];
                parameters[0] = connect.Parameter("P_CANCELCARD_ID", OracleDbType.Varchar2, P_CANCELCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_CANCELCARD_STATUS", OracleDbType.Varchar2, P_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, null); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2, P_RESULT_CODE); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2, P_RESULT_MESSAGE); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;

                parameters[7] = connect.Parameter("P_RESULT", OracleDbType.Varchar2, code); parameters[7].Size = 100; parameters[7].Direction = ParameterDirection.Output;
                parameters[8] = connect.Parameter("P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[8].Size = 100; parameters[8].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);

                if (parameters[7].Value.ToString() == "00")
                {
                    code = "00";
                    message = "Tạo yêu cầu Thành công!";
                }
                else
                {
                    code = parameters[7].Value.ToString();
                    message = parameters[8].Value.ToString();
                }
                log.Error("[UtilsCard][SaveApproveCancelCardStatus][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveCancelCardStatus][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveCancelCardStatus] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        public string SaveApproveCancelCardError(string OP_MAKER_BRANCH, string OP_CHECKER, string P_CANCELCARD_ID, string P_NOTE, string P_CANCELCARD_STATUS, string P_CODE_ERROR, string P_MSG_ERROR)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = schemaPortal + "." + packagePortal + ".UPD_CANCELCARDS_ERROR";
            log.InfoFormat("[UtilsCard][SaveApproveCancelCardError] storeName={0}", storeName);
            string code = "";
            string message = "";
            string P_CHECKER = OP_CHECKER;
            try
            {
                OracleParameter[] parameters = new OracleParameter[9];
                parameters[0] = connect.Parameter("P_CANCELCARD_ID", OracleDbType.Varchar2, P_CANCELCARD_ID); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_NOTE", OracleDbType.Varchar2, P_NOTE); parameters[1].IsNullable = true; parameters[1].Direction = ParameterDirection.Input;
                parameters[2] = connect.Parameter("P_CANCELCARD_STATUS", OracleDbType.Varchar2, P_CANCELCARD_STATUS); parameters[2].IsNullable = true; parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_CHECKER", OracleDbType.Varchar2, P_CHECKER); parameters[3].IsNullable = true; parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_CHECK_DATE", OracleDbType.Date, DateTime.Now); parameters[4].IsNullable = true; parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_CODE_ERROR", OracleDbType.Varchar2, P_CODE_ERROR); parameters[5].IsNullable = true; parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_MSG_ERROR", OracleDbType.Varchar2, P_MSG_ERROR); parameters[6].IsNullable = true; parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("OP_P_RESULT", OracleDbType.Varchar2, code); parameters[7].Size = 100; parameters[7].Direction = ParameterDirection.Output;
                parameters[8] = connect.Parameter("OP_P_RESULT_DESC", OracleDbType.Varchar2, message); parameters[8].Size = 100; parameters[8].Direction = ParameterDirection.Output;
                connect.ExecuteProc(storeName, parameters);
                log.Error("[UtilsCard][SaveApproveActiveCardError][Code] :" + code);
                log.Error("[UtilsCard][SaveApproveActiveCardError][Message] :" + message);
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][SaveApproveCancelCardError] Exception: " + ex.Message, ex);
                try
                {
                    connect.Close();
                }
                catch { }
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return code;
        }
        /// <summary>
        /// CancelCard Utils
        /// </summary>
        #endregion



        public List<AcctInfo> getAccNo(string P_ACCT_NO)
        {
            List<AcctInfo> lsResult = new List<AcctInfo>();
            SqlUtil connect = new SqlUtil("EOC");
            string storeName = schemaEOC + "." + packageEOC + ".GET_ACCT_INFO";
            log.InfoFormat("[Utils][getAccNo] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[2];
                parameters[0] = connect.Parameter("P_ACCT_NO", OracleDbType.Varchar2, P_ACCT_NO);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                (parameters[1] = connect.Parameter("P_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                lsResult = ds.Tables[0].ToList<AcctInfo>();
            }
            catch (Exception ex)
            {
                log.Error("[Utils][getAccNo] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return lsResult;
        }

        public Result InsertTableLockCardECRM(LockCard lockCard)
        {
            Result rs = new Result();
            SqlUtil connect = new SqlUtil("ECRM");
            string storeName = string.Format("{0}.{1}.{2}", schemaECRM, packageECRM, "INSERT_LOG_LOCKCARD");
            log.InfoFormat("[UtilsCard][InsertTableLockCardECRM] storeName={0}", storeName);
            DateTime date;
            if (string.IsNullOrEmpty(lockCard.LOCK_TO))
            {
                rs.code = "01";
                rs.message = "Lock to is null!";
                log.Error("[UtilsCard][InsertTableLockCardECRM] Parameter exception: " + rs.message);
                return rs;
            }
            else
            {
                DateTime.TryParseExact(lockCard.LOCK_TO, "yyyy-MM-dd hh:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out date);
            }
            try
            {
                OracleParameter[] parameters = new OracleParameter[8];
                (parameters[0] = connect.Parameter("P_USER", OracleDbType.Varchar2)).Value = "ISSUING_CARD";
                (parameters[1] = connect.Parameter("P_BLOCKNUM", OracleDbType.Varchar2)).Value = "";
                (parameters[2] = connect.Parameter("P_CARDNUM", OracleDbType.Varchar2)).Value = lockCard.CONTRACT_NUMBER.Substring(12, 4);
                (parameters[3] = connect.Parameter("P_CRNNUM", OracleDbType.Varchar2)).Value = lockCard.ACNT_CONTRACT_ID;
                (parameters[4] = connect.Parameter("P_DATE", OracleDbType.Varchar2)).Value = lockCard.LOCK_TO;
                (parameters[5] = connect.Parameter("P_REASON", OracleDbType.Varchar2)).Value = "TB";
                (parameters[6] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2)).Direction = ParameterDirection.Output;
                parameters[6].Size = 2;
                (parameters[7] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2)).Direction = ParameterDirection.Output;
                parameters[7].Size = 1000;
                connect.ExecuteProc(storeName, parameters);
                rs.code = parameters[6].Value.ToString();
                rs.message = parameters[7].Value.ToString();
                if (rs.code == "00")
                {
                    return rs;
                }
                else
                {
                    log.Info("[UtilsCard][InsertTableLockCardECRM] Database info code: " + rs.code);
                    log.Info("[UtilsCard][InsertTableLockCardECRM] Database info message: " + rs.message);
                    log.Error("[UtilsCard][InsertTableLockCardECRM] Database exception: " + rs.message);
                    rs.code = "01";
                    rs.para1 = parameters[7].Value.ToString();
                    rs.message = "Lỗi ghi dữ liệu!";
                    return rs;
                }
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][InsertTableLockCardECRM] Exception: " + ex.Message, ex);
                rs.code = "01";
                rs.message = "Lỗi ghi dữ liệu!";
                rs.para1 = ex.Message;
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return rs;
        }


        public Result UpdateTableLockCardECRM(LockCard lockCard)
        {
            Result rs = new Result();
            SqlUtil connect = new SqlUtil("ECRM");
            string storeName = string.Format("{0}.{1}.{2}", schemaECRM, packageECRM, "UPDATE_LOG_LOCKCARD");
            log.InfoFormat("[UtilsCard][UpdateTableLockCardECRM] storeName={0}", storeName);
            try
            {
                OracleParameter[] parameters = new OracleParameter[3];
                (parameters[0] = connect.Parameter("P_CRNNUM", OracleDbType.Varchar2)).Value = lockCard.ACNT_CONTRACT_ID;

                (parameters[1] = connect.Parameter("P_RESULT_CODE", OracleDbType.Varchar2)).Direction = ParameterDirection.Output;
                parameters[1].Size = 2;
                (parameters[2] = connect.Parameter("P_RESULT_MESSAGE", OracleDbType.Varchar2)).Direction = ParameterDirection.Output;
                parameters[2].Size = 1000;
                connect.ExecuteProc(storeName, parameters);
                rs.code = parameters[1].Value.ToString();
                rs.message = parameters[2].Value.ToString();
                if (rs.code == "00")
                {
                    return rs;
                }
                else
                {
                    log.Info("[UtilsCard][UpdateTableLockCardECRM] Database info code: " + rs.code);
                    log.Info("[UtilsCard][UpdateTableLockCardECRM] Database info message: " + rs.message);
                    log.Error("[UtilsCard][UpdateTableLockCardECRM] Database exception: " + rs.message);
                    rs.code = "01";
                    rs.message = "Lỗi update dữ liệu ECRM!";
                    return rs;
                }
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][UpdateTableLockCardECRM] Exception: " + ex.Message, ex);
                rs.code = "01";
                rs.message = "Lỗi update dữ liệu ECRM!";
                rs.para1 = ex.Message;
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return rs;
        }

        public List<CardInfo> LoadSubCard(string OP_CARD_NUMBER)
        {
            List<CardInfo> lsResult = new List<CardInfo>();
            DataTable dt = null;
            SqlUtil connect = new SqlUtil(schemaWAY4);
            string storeName = packageWAY4 + ".GET_SUB_CARD";
            log.InfoFormat("[UtilsCard][LoadSubCard] storeName={0}", storeName);

            DataSet ds = new DataSet();
            try
            {
                OracleParameter[] parameters = new OracleParameter[2];

                parameters[0] = connect.Parameter("OP_ACNT_CONTRACT_ID", OracleDbType.Varchar2, OP_CARD_NUMBER);
                parameters[0].IsNullable = true;
                parameters[0].Direction = ParameterDirection.Input;

                (parameters[1] = connect.Parameter("OP_RESULT_CUR", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                connect.FillProc(storeName, parameters, ref ds);
                if (ds != null && ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                log.Error("[UtilsCard][LoadSubCard] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            if (dt != null)
                lsResult = dt.ToList<CardInfo>();
            return lsResult;
        }
        public Result updateInfoFeeRegisterCard(string ACTN_TYPE, string VALUE1, string VALUE2, string VALUE3, string VALUE4, string VALUE5, string USERID)
        {
            Result rs = new Result();
            SqlUtil connect = new SqlUtil("EOC");
            OracleParameter[] parameters = new OracleParameter[9];
            string procedureName = string.Format("{0}.{1}.PROC_CARD_SERV_CHARGE_REG_UPD", schemaEOC, "HDB_SERV_CHARGE");
            try
            {
                parameters[0] = connect.Parameter("P_ERR_CODE", OracleDbType.Varchar2);
                parameters[0].Size = 10;
                parameters[0].Direction = ParameterDirection.Output;
                parameters[1] = connect.Parameter("P_ERR_MSG", OracleDbType.Varchar2);
                parameters[1].Size = 500;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2] = connect.Parameter("P_ACTN_TYPE", OracleDbType.Varchar2, ACTN_TYPE);
                parameters[2].IsNullable = true;
                parameters[2].Direction = ParameterDirection.Input;
                parameters[3] = connect.Parameter("P_VALUE1", OracleDbType.Varchar2, VALUE1);
                parameters[3].IsNullable = true;
                parameters[3].Direction = ParameterDirection.Input;
                parameters[4] = connect.Parameter("P_VALUE2", OracleDbType.Varchar2, VALUE2);
                parameters[4].IsNullable = true;
                parameters[4].Direction = ParameterDirection.Input;
                parameters[5] = connect.Parameter("P_VALUE3", OracleDbType.Varchar2, VALUE3);
                parameters[5].IsNullable = true;
                parameters[5].Direction = ParameterDirection.Input;
                parameters[6] = connect.Parameter("P_VALUE4", OracleDbType.Varchar2, VALUE4);
                parameters[6].IsNullable = true;
                parameters[6].Direction = ParameterDirection.Input;
                parameters[7] = connect.Parameter("P_VALUE5", OracleDbType.Varchar2, VALUE5);
                parameters[7].IsNullable = true;
                parameters[7].Direction = ParameterDirection.Input;
                parameters[8] = connect.Parameter("P_USERID", OracleDbType.Varchar2, USERID);
                parameters[8].IsNullable = true;
                parameters[8].Direction = ParameterDirection.Input;
                connect.ExecuteProc(procedureName, parameters);
                log.Info("[PROC_CARD_SERV_CHARGE_REG_UPD][Code] " + parameters[0].Value.ToString());
                log.Info("[PROC_CARD_SERV_CHARGE_REG_UPD][Message] " + parameters[1].Value.ToString());
                rs.code = parameters[0].Value.ToString();
                rs.message = parameters[1].Value.ToString();
            }
            catch (Exception ex)
            {
                log.Error("[PROC_CARD_SERV_CHARGE_REG_UPD][Code]: 999");
                log.Error("[PROC_CARD_SERV_CHARGE_REG_UPD][Message] {0}", ex);
                rs.code = "99";
                rs.message = "Lỗi hệ thống! Vui lòng cntt.";
            }
            return rs;
        }


        public bool checkContractAllowClose(string contractOid, string cardNumber)
        {
            SqlUtil connect = new SqlUtil("WAY4");
            List<OracleParameter> parameters = new List<OracleParameter>();
            string procedureName = string.Format("{0}.CHECK_CONTRACT_ALLOW_CLOSE", packageWAY4);
            try
            {
                OracleParameter parametersOID = connect.Parameter("P_CONTRACT_ID", OracleDbType.Varchar2, contractOid);
                parametersOID.Direction = ParameterDirection.Input;

                OracleParameter parametersCount = connect.Parameter("P_COUNT", OracleDbType.Decimal);
                parametersCount.Size = 10;
                parametersCount.Direction = ParameterDirection.Output;

                OracleParameter parametersResult = connect.Parameter("P_RESULT", OracleDbType.Decimal);
                parametersResult.Size = 100;
                parametersResult.Direction = ParameterDirection.Output;

                OracleParameter parametersContractNumber = connect.Parameter("P_CONTRACT_NUMBER", OracleDbType.Varchar2, cardNumber);
                parametersResult.IsNullable = true;
                parametersResult.Direction = ParameterDirection.Input;

                parameters.Add(parametersOID);
                parameters.Add(parametersCount);
                parameters.Add(parametersResult);
                parameters.Add(parametersContractNumber);
                OracleParameter[] parameters2 = parameters.ToArray();
                connect.ExecuteProc(procedureName, parameters2);
                log.Info("[CHECK_CONTRACT_ALLOW_CLOSE][Code] " + parameters2[1].Value.ToString());
                log.Info("[CHECK_CONTRACT_ALLOW_CLOSE][Message] " + parameters2[2].Value.ToString());
                if (Decimal.Parse(parameters2[1].Value.ToString()) > 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error("[CHECK_CONTRACT_ALLOW_CLOSE][Code]: 999");
                log.Error("[CHECK_CONTRACT_ALLOW_CLOSE][Message] {0}", ex);
                return false;
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return true;
        }
		/// <summary>
		/// Added by Leln2
		/// Added date 28/11/2019
		/// note: cập nhật mã lo trường hợp hủy phát hành thẻ credit
		/// </summary>
		/// <param name="info">P_ID</param>
		/// <returns>null</returns>
		/// 


		public string CheckResultIsuingCardLos(string contractNumber, string cardProduct)
		{
			string result = "99";
			DataSet ds = new DataSet();
			SqlUtil connect = new SqlUtil("ECRM");
			string procedureName = string.Format("{0}.{1}.CHECK_RESULT_CARD_LOS", CShared.schemaECRM, packagePortal);
			log.Info("[ChangeCardStatus][CheckResultIsuingCardLos] call procedure: " + procedureName);
			try
			{
				List<OracleParameter> listParam = new List<OracleParameter>();
				listParam.Add(new OracleParameter() { ParameterName = "P_CONTRACT_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = contractNumber, Direction = ParameterDirection.Input });
				listParam.Add(new OracleParameter() { ParameterName = "P_CARD_PRODUCT", OracleDbType = OracleDbType.Varchar2, Value = cardProduct, Direction = ParameterDirection.Input });

				listParam.Add(new OracleParameter() { ParameterName = "O_RESULT", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });

				connect.FillProc(procedureName, listParam.ToArray(), ref ds);

				result = listParam[listParam.Count - 1].Value.ToString();
				log.Info("[ChangeCardStatus][CheckResultIsuingCardLos] Result: " + listParam[listParam.Count - 1].Value.ToString());
			}
			catch (Exception ex)
			{
				log.Error("[ChangeCardStatus][CheckResultIsuingCardLos] Exception: " + ex.Message, ex);
			}
			finally
			{
				try
				{
					connect.Close();
				}
				catch
				{
					log.Error("[ChangeCardStatus][CheckResultIsuingCardLos] Error close connect db");
				}
			}
			return result;
		}




		public ResultMessage CommentCollatRef(string id)
        {
            SqlUtil connect = new SqlUtil("PORTAL");
            string storeName = string.Format("{0}.{1}.{2}", schemaPortal, packagePortal, "COMMENT_COLLAT_REF"); //packagePortal + ".UPD_ADJTRANSFER_STATUS"; 
            log.InfoFormat("[CANCLECARD][CommentCollatRef] storeName={0}", storeName);
            string code = "";
            string message = "";
            try
            {
                OracleParameter[] parameters = new OracleParameter[3];
                parameters[0] = connect.Parameter("P_ID", OracleDbType.Varchar2, id); parameters[0].IsNullable = true; parameters[0].Direction = ParameterDirection.Input;
                parameters[1] = connect.Parameter("P_CODE", OracleDbType.Varchar2, code); parameters[1].Size = 100;
                parameters[1].Direction = ParameterDirection.Output;
                parameters[2] = connect.Parameter("P_MESSAGE", OracleDbType.Varchar2, message); parameters[2].Size = 200;
                parameters[2].Direction = ParameterDirection.Output;

                connect.ExecuteProc(storeName, parameters);

                code = parameters[1].Value.ToString();
                message = parameters[2].Value.ToString();
                log.Error("[CANCLECARD][CommentCollatRef][Code] :" + code);
                log.Error("[CANCLECARD][CommentCollatRef][Message] :" + message);
            }
            catch (Exception ex)
            {
                code = "02";
                message = "ERROR IN TRY CATCH";
                log.Error("[CANCLECARD][CommentCollatRef] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }
            return new ResultMessage
            {
                code = code,
                message = message
            };
        }

        public string CheckEventRDPC(string cardNumber)
        {
            string ev = "";
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = packageWAY4 + ".GET_PRODUCT_EVEN_CARD_V";
            log.InfoFormat("[Utils][CheckEventRDPC] storeName={0}", storeName);
            log.InfoFormat("[Utils][CheckEventRDPC] cardNumber={0}", cardNumber);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_CARD_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = cardNumber, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_PRODUCT_EVEN", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT", OracleDbType = OracleDbType.Int32, Direction = ParameterDirection.Output, Size = 250 });
                connect.FillProc(storeName, listParam.ToArray(), ref ds);
                ev = listParam[listParam.Count - 2].Value.ToString();
                log.InfoFormat("[Utils][CheckEventRDPC] RESULT={0}", listParam[listParam.Count - 1].Value.ToString());
                log.InfoFormat("[Utils][CheckEventRDPC] PRODUCT_EVEN={0}", listParam[listParam.Count - 2].Value.ToString());
            }
            catch (Exception ex)
            {
                log.Error("[Utils][CheckEventRDPC] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return ev;
        }

        public string CheckEventRDPCByContract(string contractNumber, string cardNumber)
        {
            string ev = "";
            SqlUtil connect = new SqlUtil("WAY4");
            string storeName = packageWAY4 + ".GET_PRODUCT_EVEN_CONTRACT_V";
            log.InfoFormat("[Utils][CheckEventRDPCByContract] storeName={0}", storeName);
            log.InfoFormat("[Utils][CheckEventRDPCByContract] cardNumber={0}", contractNumber);
            log.InfoFormat("[Utils][CheckEventRDPCByContract] cardNumber={0}", cardNumber);

            DataSet ds = new DataSet();
            try
            {
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_CONTRACT_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = contractNumber, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_CARD_NUMBER", OracleDbType = OracleDbType.Varchar2, Value = cardNumber, Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_PRODUCT_EVEN", OracleDbType = OracleDbType.Varchar2, Direction = ParameterDirection.Output, Size = 250 });
                listParam.Add(new OracleParameter() { ParameterName = "P_RESULT", OracleDbType = OracleDbType.Int32, Direction = ParameterDirection.Output, Size = 250 });
                connect.FillProc(storeName, listParam.ToArray(), ref ds);
                ev = listParam[listParam.Count - 2].Value.ToString();
                log.InfoFormat("[Utils][CheckEventRDPCByContract] RESULT={0}", listParam[listParam.Count - 1].Value.ToString());
                log.InfoFormat("[Utils][CheckEventRDPCByContract] PRODUCT_EVEN={0}", listParam[listParam.Count - 2].Value.ToString());
            }
            catch (Exception ex)
            {
                log.Error("[Utils][CheckEventRDPCByContract] Exception: " + ex.Message, ex);
            }
            finally
            {
                try
                {
                    connect.Close();
                }
                catch { }
            }

            return ev;
        }

        public Result clearAllAttachmentsActiveCard(string uuid)
        {
            Result rs = new Result();
            rs.code = "00";
            string package = "HDB_ISSUING";
            SqlUtil connect = new SqlUtil("PORTAL");
            try
            {
                string sqlFilter = string.Format("{0}.{1}.ACTIVE_CARD_REMOVEALL_ATTACHMENTS", CShared.schemaPortal, package);
                OracleParameter[] parameters3 = new OracleParameter[1];
                parameters3[0] = connect.Parameter("P_UUID", OracleDbType.Varchar2, uuid);

                connect.ExecuteProc(sqlFilter, parameters3);
            }
            catch (Exception ex)
            {
                log.Error("[clearAllAttachmentsActiveCard][Error] " + ex);
                rs.code = "099";
                rs.message = "Lỗi hệ thống!";
                return rs;
            }
            return rs;
        }

        public Result insertAttachmentsActiveCard(string uuid, List<Attachment> attachments)
        {
            Result rs = new Result();
            rs.code = "00";
            string package = "HDB_ISSUING";
            SqlUtil connect = new SqlUtil("PORTAL");
            try
            {
                int size = attachments.Count();
                string sqlFilter = string.Format("{0}.{1}.ACTIVE_CARD_ADD_ATTACHMENTS", CShared.schemaPortal, package);
                List<OracleParameter> listParam = new List<OracleParameter>();

                listParam.Add(new OracleParameter() { ParameterName = "P_UUID", OracleDbType = OracleDbType.Varchar2, Value = attachments.Select(x => uuid).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_ORG", OracleDbType = OracleDbType.Varchar2, Value = attachments.Select(x => x.FILE_ORG).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_NAME", OracleDbType = OracleDbType.Varchar2, Value = attachments.Select(x => x.FILE_NAME).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_EXT", OracleDbType = OracleDbType.Varchar2, Value = attachments.Select(x => x.FILE_EXT).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_SIZE", OracleDbType = OracleDbType.Varchar2, Value = attachments.Select(x => x.FILE_SIZE).ToArray(), Direction = ParameterDirection.Input });
                listParam.Add(new OracleParameter() { ParameterName = "P_FILE_PATH", OracleDbType = OracleDbType.Varchar2, Value = attachments.Select(x => x.FILE_PATH).ToArray(), Direction = ParameterDirection.Input });
                
                connect.InsertImportBatches(sqlFilter, size, listParam.ToArray());
                log.Info("[insertAttachmentsActiveCard] Import success: " + size);

            }
            catch (Exception ex)
            {
                log.Error("[insertAttachmentsActiveCard][Error] " + ex);
                rs.code = "099";
                rs.message = "Lỗi upload file đính kèm!";
                return rs;
            }
            return rs;
        }
    }
}