using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using CookComputing.XmlRpc;
using System.Web.Services;
using System.Xml;
using System.IO;



[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }
    ConClass clsData = new ConClass();
    public string SubName;
    public string SubCName;
    public string PriceNow;
    public string Barcode;
    public string ERPProdID;
    public string Username;
    public string Token;
    public string StartOutPut = @"<?xml version='1.0' encoding='UTF-8'?><RESPONSE>";
    public string EndOutPut = @"</RESPONSE>";
    public string TokenId = "123456";

    [XmlRpcUrl("http://203.186.4.58/xmlrpc/common")]
    public interface iFace : IXmlRpcProxy
    {
        [XmlRpcMethod("login")]
        int login(string db, string username, string psw);
    }


    [XmlRpcUrl("http://203.186.4.58/xmlrpc/object")]
    public interface ifaceamthod : IXmlRpcProxy
    {
        [XmlRpcMethod("execute")]
        string execute(string db, int username, string psw, string Apiname, string methodName, string[] vals);
    }



    [WebMethod]
    public System.Xml.XmlDocument InsertProduct(string ERP_PRODUCT_ID, string PRODUCT_NAME, string PRODUCT_CHINESE_B5, string EAN13, double PRICE, string NOTE_ONE, string NOTE_TWO, string SECURITY_TOKEN, string PRODUCT_BRAND_ID)
    {
        try
        {
            string ClientIP = IPNetworking.GetIP4Address();
            DataTable dt = new DataTable();
            int retunvalue = 0;
            string Result = "";
            string Output = "";
            string path = Server.MapPath("sample1.txt");


            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter str = new StreamWriter(fs);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var value = "ERP_PRODUCT_ID:" + ERP_PRODUCT_ID + "," + "PRODUCT_NAME:" + PRODUCT_NAME + "," + "PRODUCT_CHINESE_B5:" + PRODUCT_CHINESE_B5 + "," + "EAN13:" + EAN13 + "," + "PRICE:" + PRICE + "," + "NOTE_ONE:" + NOTE_ONE + "," + "SECURITY_TOKEN:" + SECURITY_TOKEN + "," + "PRODUCT_BRAND_ID:" + PRODUCT_BRAND_ID + "," + "ClinetIp:" + ClientIP;
                    sw.Write(value);
                }
            }
            SubName = Convert.ToString(PRODUCT_NAME);
            if (SubCName == "False")
            {
                SubCName = "False";
            }
            else
            {
                SubCName = PRODUCT_CHINESE_B5;
            }
            PriceNow = Convert.ToString(PRICE);
            Barcode = Convert.ToString(EAN13);
            ERPProdID = Convert.ToString(ERP_PRODUCT_ID);
            Username = Convert.ToString(NOTE_ONE);
            Token = Convert.ToString(SECURITY_TOKEN);

            string ValidationError = "";
            ValidationError = ValidationERP_ProductID(ERPProdID);
            if (ValidationError != "")
            {
                return OutPutXml(ValidationError);
            }
            if (Barcode.ToLower() != "false")
            {
                ValidationError = ValidationBarcode(Barcode);
                if (ValidationError != "")
                {
                    return OutPutXml(ValidationError);
                }
            }
            else
            {
                return OutPutXml(@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL </RESULT> <Error>failed, no barcode provided</Error> ");
            }

            ValidationError = ValidationPrices(PriceNow);
            if (ValidationError != "")
            {
                return OutPutXml(ValidationError);
            }


            ValidationError = ValidationToken(Token);
            if (ValidationError != "")
            {
                return OutPutXml(ValidationError);
            }
            ValidationError = ValidationPRODUCT_NAME(SubName);
            if (ValidationError != "")
            {
                return OutPutXml(ValidationError);
            }

            if (SubCName.Length > 51)
            {
                return OutPutXml(@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL </RESULT> <Error>PRODUCT_CHINESE_B5 Sholud Be  less Then 12 Charter</Error> ");
            }

            if (Username.Length > 13)
            {
                return OutPutXml(@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL </RESULT> <Error>NOTE_ONE Sholud Be  less Then 12 Charter</Error> ");
            }


            if (!clsData.InsertProduct(SubName, SubCName, PriceNow, Barcode, ref retunvalue, ERPProdID, ref Result, Username, Token, ClientIP))
            {
                Output += @"<ERP_PRODUCT_ID> " + ERPProdID + " </ERP_PRODUCT_ID> <RESULT>Failed</RESULT>";
                return OutPutXml(Output);
            }
            string Outputr = @"";
            Outputr += "<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID>";
            Outputr += " <RESULT>" + Result + "</RESULT>";
            return OutPutXml(Outputr);
        }
        catch (Exception)
        {
            string Output = @"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>Invalid data set in xml format.</RESULT> ";
            string path = Server.MapPath("result.txt");

            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter str = new StreamWriter(fs);

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var value = Output;
                    sw.Write(value);

                }
            }

            return OutPutXml(Output);
        }
    }

    //[WebMethod]
    //public System.Xml.XmlDocument InventoryInqury(string Xml)
    //{
    //    string ValidationError = "";
    //    int ERP_PRODUCT_ID = 0;
    //    string Barcode = "";

    //    int SubProdStockPlaceID = 0;
    //    string Result = "";
    //    string Token = "0";
    //    try
    //    {
    //        string Output = "";
    //        string ClientIP = IPNetworking.GetIP4Address();
    //        DataTable dt = new DataTable();

    //        XmlDocument xd = new XmlDocument();
    //        xd.LoadXml(Xml);
    //        using (DataSet ds = new DataSet())
    //        {
    //            ds.ReadXml(new XmlNodeReader(xd));
    //            if (ds.Tables.Count > 0)
    //            {


    //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
    //                {
    //                    ERP_PRODUCT_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ERP_PRODUCT_ID"]);
    //                    Barcode = Convert.ToString(ds.Tables[0].Rows[i]["EAN_13"]);
    //                    SubProdStockPlaceID = Convert.ToInt32(ds.Tables[0].Rows[i]["ERP_WAREHOUSE_ID"]);
    //                    Token = Convert.ToString(ds.Tables[0].Rows[i]["SECURITY_TOKEN"]);
    //                    ERPProdID = ERP_PRODUCT_ID.ToString();
    //                    ValidationError = ValidationBarcode(Barcode);
    //                    if (ValidationError != "")
    //                    {
    //                        return OutPutXml(ValidationError);
    //                    }


    //                    ValidationError = ValidationToken(Token);
    //                    if (ValidationError != "")
    //                    {
    //                        return OutPutXml(ValidationError);
    //                    }

    //                    DataTable ResultDt = clsData.InventoryInqury(ERP_PRODUCT_ID, Barcode, SubProdStockPlaceID, Token, ref Result, ClientIP);

    //                    for (int j = 0; j < ResultDt.Rows.Count; j++)
    //                    {
    //                        Output += @"<RESULT>Success</RESULT><ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>" + ResultDt.Rows[i]["SubProdStockQty"].ToString() + "</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>";
    //                    }
    //                    //if (Output == "")
    //                    //{
    //                    //    Output += @"<RESULT>Success</RESULT><ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>0.00</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>";
    //                    //}
    //                    if (Result != "")
    //                    {
    //                        Output += @"<RESULT>" + Result + "</RESULT>";
    //                    }
    //                    if (Result == "" && ResultDt.Rows.Count == 0)
    //                    {
    //                        Output += @"<RESULT>Success</RESULT><ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>0.00</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>";
    //                    }
    //                }
    //            }
    //        }


    //        return OutPutXml(Output);


    //    }
    //    catch (Exception)
    //    {
    //        //XmlDocument xd1 = new XmlDocument();
    //        //xd1.LoadXml(StartOutPut + @"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>0.00</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>" + EndOutPut);
    //        return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>0.00</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>");
    //    }

    //}

    [WebMethod]
    public System.Xml.XmlDocument InventoryInqury(int SubprodID, int ERP_WAREHOUSE_ID)
    {
        try
        {
            int ERP_PRODUCT_ID = 0;
            string Barcode = "";

            int SubProdStockPlaceID = 0;
            string Result = "";
            string Token = "0";


            int Qty = 0;
            int retunvalue = 0;


            string ClientIP = IPNetworking.GetIP4Address();
            DataTable dt = new DataTable();
            XmlDocument xd = new XmlDocument();
            string[] arr = new string[1];
            DataTable ResultDt = clsData.InventoryInquryERP(SubprodID, ERP_WAREHOUSE_ID, ref Result, ClientIP);

            for (int i = 0; i < ResultDt.Rows.Count; i++)
            {
                arr[0] = @"<?xml version='1.0' encoding='UTF-8'?><INVENTORY_INQUERY><ERP_PRODUCT_ID>" + ResultDt.Rows[i]["ERPProdID"] + "</ERP_PRODUCT_ID> <EAN_13>" + ResultDt.Rows[i]["Barcode"] + "</EAN_13> <ERP_WAREHOUSE_ID>" + ERP_WAREHOUSE_ID + "</ERP_WAREHOUSE_ID> <SECURITY_TOKEN>123456</SECURITY_TOKEN> </INVENTORY_INQUERY>";
                string Db = "kcl_demo";
                string username = "admin";
                string psw = "admin";
                iFace proxy = XmlRpcProxyGen.Create<iFace>();
                proxy.NonStandard = XmlRpcNonStandard.AllowStringFaultCode;
                var value = proxy.login(Db, username, psw);
                ifaceamthod ProxyMethod = XmlRpcProxyGen.Create<ifaceamthod>();
                string XmlResult = ProxyMethod.execute(Db, value, psw, "kcl.api", "aspToErpInventoryInquery", arr);

                xd.LoadXml(XmlResult);
                using (DataSet ds = new DataSet())
                {
                    ds.ReadXml(new XmlNodeReader(xd));

                    for (int j = 0; j < ds.Tables["RESPONSE"].Rows.Count; j++)
                    {
                        if (ds.Tables["RESPONSE"].Rows[j]["RESULT"].ToString().ToLower() == "success")
                        {
                            ERP_PRODUCT_ID = SubprodID;
                            SubProdStockPlaceID = ERP_WAREHOUSE_ID;
                            Qty = (int)Convert.ToDouble(ds.Tables["RESPONSE"].Rows[i]["ERP_PRODUCT_QTY"]);
                            Token = "123456";
                            if (!clsData.UpdateInventory(ERP_PRODUCT_ID, Barcode, SubProdStockPlaceID, Token, ref Result, ClientIP, Qty, ref retunvalue))
                            {
                            }
                            else
                            {
                            }
                        }

                    }
                    if (ds.Tables["RESPONSE"].Rows.Count == 0)
                    {
                        return OutPutXml("<RESULT>Product does not exist</RESULT><ERP_PRODUCT_ID></ERP_PRODUCT_ID><ERP_PRODUCT_QTY></ERP_PRODUCT_QTY><EAN_13></EAN_13>");
                    }
                }

            }
            if (ResultDt.Rows.Count == 0)
            {
                return OutPutXml("<RESULT>Product does not exist</RESULT><ERP_PRODUCT_ID></ERP_PRODUCT_ID><ERP_PRODUCT_QTY></ERP_PRODUCT_QTY><EAN_13></EAN_13>");
            }
            return xd;
        }
        catch (Exception)
        {
            return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_PRODUCT_ID>" + 0 + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>0.00</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>");

        }

    }

    [WebMethod]
    public System.Xml.XmlDocument InventoryUpdate(string Xml)
    {
        string path = Server.MapPath("sample1.txt");
        int ERP_PRODUCT_ID = 0;
        string Barcode = "";
        string ValidationError = "";
        int SubProdStockPlaceID = 0;
        string Result = "";
        string Token = "0";
        string Note1 = "";
        string Note2 = "";
        int Qty = 0;
        int retunvalue = 0;
        string Error = "";
        try
        {
            string Output = "";
            string ClientIP = IPNetworking.GetIP4Address();



            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter str = new StreamWriter(fs);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var value = "InventoryUpdate, Xml  :" + Xml + "@ClientIp :" + ClientIP;
                    sw.Write(value);
                }
            }

            DataTable dt = new DataTable();
            string sucessId = "";
            string FailId = "";
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(Xml);
            using (DataSet ds = new DataSet())
            {
                ds.ReadXml(new XmlNodeReader(xd));
                if (ds.Tables.Count > 0)
                {

                    for (int j = 0; j < ds.Tables["EXPORT"].Rows.Count; j++)
                    {
                        Note1 = Convert.ToString(ds.Tables["EXPORT"].Rows[j]["NOTE_ONE"]);
                        Note2 = Convert.ToString(ds.Tables["EXPORT"].Rows[j]["NOTE_TWO"]);
                        Token = Convert.ToString(ds.Tables["EXPORT"].Rows[j]["TOKEN"]);
                        SubProdStockPlaceID = Convert.ToInt32(ds.Tables["EXPORT"].Rows[j]["ERP_WAREHOUSE_ID"]);
                    }
                    for (int i = 0; i < ds.Tables["PRODUCT"].Rows.Count; i++)
                    {
                        ERP_PRODUCT_ID = Convert.ToInt32(ds.Tables["PRODUCT"].Rows[i]["ERP_PRODUCT_ID"]);
                        Barcode = Convert.ToString(ds.Tables["PRODUCT"].Rows[i]["EAN13"]);
                        Qty = (int)Convert.ToDouble(ds.Tables["PRODUCT"].Rows[i]["PRODUCT_QTY"]);
                        if (Barcode.ToLower() != "false")
                        {
                            ValidationError = ValidationBarcode(Barcode);
                            if (ValidationError != "")
                            {
                                return OutPutXml(ValidationError);
                            }
                        }
                        else
                        {
                            return OutPutXml(@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL </RESULT> <Error>Failed, No barcode provided</Error> ");
                        }
                        ValidationError = ValidationToken(Token);
                        if (ValidationError != "")
                        {
                            return OutPutXml(ValidationError);
                        }

                        //try
                        //{
                        //    double value = Convert.ToDouble(Barcode);
                        //}
                        //catch (Exception)
                        //{

                        //    return OutPutXml(@"<ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID> <RESULT>FAIL (Invalid EAN13)</RESULT>");
                        //}

                        if (!clsData.ExportInventory(ERP_PRODUCT_ID, Barcode, SubProdStockPlaceID, Token, ref Result, ClientIP, Note1, Note2, Qty, ref retunvalue))
                        {
                            FailId += ERP_PRODUCT_ID + ",";
                        }
                        else
                        {
                            if (Result == "")
                            {
                                sucessId += ERP_PRODUCT_ID + ",";
                            }
                            else
                            {

                                FailId += ERP_PRODUCT_ID + ",";
                                Error = Result;
                            }
                        }
                    }
                }
            }
            Output = @"<SUCCESS_IDS>" + sucessId.TrimEnd(',') + "</SUCCESS_IDS>" +
"<FAIL_IDS>" + FailId.TrimEnd(',') + "</FAIL_IDS>" +
"<ERP_WAREHOUSE_ID>" + SubProdStockPlaceID + "</ERP_WAREHOUSE_ID>" +
"<Error>" + Error + "</Error>";

            return OutPutXml(Output);
        }
        catch (Exception)
        {
            //XmlDocument xd1 = new XmlDocument();
            //xd1.LoadXml(StartOutPut + @"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_PRODUCT_ID>" + ERP_PRODUCT_ID + "</ERP_PRODUCT_ID><ERP_PRODUCT_QTY>0.00</ERP_PRODUCT_QTY><EAN_13>" + Barcode + "</EAN_13>" + EndOutPut);
            return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT>");
        }

    }

    [WebMethod]
    public System.Xml.XmlDocument CreateQuotation(string Orderlogdate,string Status)
    {
        try
        {
            DataSet ds = new DataSet();
            DataTable Newdt = new DataTable();
            string Value = "";
            Newdt = clsData.QuotationDetails();
            XmlDocument xd = new XmlDocument();
            if (Newdt.Rows.Count > 0)
            {
                Value = @"<?xml version='1.0' encoding='UTF-8'?><SALE_ORDER>" +
                           "<SECURITY_TOKEN>" + Newdt.Rows[0]["SECURITY_TOKEN"] + "</SECURITY_TOKEN>" +
                          "<PARTNER_ID>" + Newdt.Rows[0]["PARTNER_ID"] + "</PARTNER_ID>" +
                           "<ERP_WAREHOUSE_ID>" + Newdt.Rows[0]["ERP_WAREHOUSE_ID"] + "</ERP_WAREHOUSE_ID>" +
                           "<ORDER_POLICY>" + Newdt.Rows[0]["ORDER_POLICY"] + "</ORDER_POLICY>" +
                           "<PICKING_POLICY>" + Newdt.Rows[0]["PICKING_POLICY"] + "</PICKING_POLICY>" +
                           "<PRICELIST_ID>" + Newdt.Rows[0]["PRICELIST_ID"] + "</PRICELIST_ID>" +
                           "<REFERENCE>" + Newdt.Rows[0]["REFERENCE"] + "</REFERENCE>" +
                           "<CHANNEL_ID>" + Newdt.Rows[0]["CHANNEL_ID"] + "</CHANNEL_ID>" +
                           "<SOURCE_ID>" + Newdt.Rows[0]["SOURCE_ID"] + "</SOURCE_ID>" +
                           "<FISCAL_POSITION_ID>" + Newdt.Rows[0]["FISCAL_POSITION_ID"] + "</FISCAL_POSITION_ID>" +
                           "<TAG_IDS>" + Newdt.Rows[0]["TAG_IDS"] + "</TAG_IDS>" +
                           "<INVOICE_TYPE>" + Newdt.Rows[0]["INVOICE_TYPE"] + "</INVOICE_TYPE>";
            }
            string ClientIP = IPNetworking.GetIP4Address();
            string result = "";
            string path = Server.MapPath("sample1.txt");
            string[] arr = new string[1];

            DataTable dt = clsData.CreateQuotation(Orderlogdate, TokenId, ref result, ClientIP, Status);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Value += @"<ORDER_LINE>
                        <ERP_PRODUCT_ID>" + dt.Rows[i]["ERPProdID"] + "</ERP_PRODUCT_ID> <DESCRIPTION>" + dt.Rows[i]["OrderID"] + "</DESCRIPTION> <PRODUCT_UOM_ID></PRODUCT_UOM_ID> <PRICE_UNIT>" + dt.Rows[i]["UnitPrice"] + "</PRICE_UNIT> <QTY>" + dt.Rows[i]["Quantity"] + "</QTY> <DISCOUNT_PERCENTAGE>" + dt.Rows[i]["Discount"] + "</DISCOUNT_PERCENTAGE> </ORDER_LINE>";
            }
            Value += "</SALE_ORDER>";

            if (result != "")
            {
                return OutPutXml(@"<RESULT>" + result + "</RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
            }
            if (dt.Rows.Count == 0)
            {
                return OutPutXml(@"<RESULT> No order found </RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
            }
            arr[0] = Value;
            string Db = "kcl_demo";
            string username = "admin";
            string psw = "admin";
            iFace proxy = XmlRpcProxyGen.Create<iFace>();
            proxy.NonStandard = XmlRpcNonStandard.AllowStringFaultCode;
            var value = proxy.login(Db, username, psw);
            ifaceamthod ProxyMethod = XmlRpcProxyGen.Create<ifaceamthod>();
            string XmlResult = ProxyMethod.execute(Db, value, psw, "kcl.api", "createQuotation", arr);
            xd.LoadXml(XmlResult);
            ds.ReadXml(new XmlNodeReader(xd));
            if (ds.Tables["RESPONSE"].Rows[0]["RESULT"].ToString().ToLower() == "success")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    clsData.InsertExportSaleID(dt.Rows[i]["OrderID"].ToString(), ds.Tables["RESPONSE"].Rows[0]["ERP_SALE_ORDER_ID"].ToString());
                }
            }
            return xd;
        }
        catch (Exception)
        {
            return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
        }
    }


    [WebMethod]
    public System.Xml.XmlDocument ExportOrderBatch(string OrderBatchDate)
    {
        try
        {
            DataSet ds = new DataSet();
            DataTable Newdt = new DataTable();
            string Value = "";
            Newdt = clsData.QuotationDetails();
            XmlDocument xd = new XmlDocument();
            if (Newdt.Rows.Count > 0)
            {
                Value = @"<?xml version='1.0' encoding='UTF-8'?><SALE_ORDER>" +
                           "<SECURITY_TOKEN>" + Newdt.Rows[0]["SECURITY_TOKEN"] + "</SECURITY_TOKEN>" +
                          "<PARTNER_ID>" + Newdt.Rows[0]["PARTNER_ID"] + "</PARTNER_ID>" +
                           "<ERP_WAREHOUSE_ID>" + Newdt.Rows[0]["ERP_WAREHOUSE_ID"] + "</ERP_WAREHOUSE_ID>" +
                           "<ORDER_POLICY>" + Newdt.Rows[0]["ORDER_POLICY"] + "</ORDER_POLICY>" +
                           "<PICKING_POLICY>" + Newdt.Rows[0]["PICKING_POLICY"] + "</PICKING_POLICY>" +
                           "<PRICELIST_ID>" + Newdt.Rows[0]["PRICELIST_ID"] + "</PRICELIST_ID>" +
                           "<REFERENCE>" + Newdt.Rows[0]["REFERENCE"] + "</REFERENCE>" +
                           "<CHANNEL_ID>" + Newdt.Rows[0]["CHANNEL_ID"] + "</CHANNEL_ID>" +
                           "<SOURCE_ID>" + Newdt.Rows[0]["SOURCE_ID"] + "</SOURCE_ID>" +
                           "<FISCAL_POSITION_ID>" + Newdt.Rows[0]["FISCAL_POSITION_ID"] + "</FISCAL_POSITION_ID>" +
                           "<TAG_IDS>" + Newdt.Rows[0]["TAG_IDS"] + "</TAG_IDS>" +
                           "<INVOICE_TYPE>" + Newdt.Rows[0]["INVOICE_TYPE"] + "</INVOICE_TYPE>";
            }
            string ClientIP = IPNetworking.GetIP4Address();
            string result = "";
            string path = Server.MapPath("sample1.txt");
            string[] arr = new string[1];

            DataTable dt = clsData.OrderBatch(OrderBatchDate, TokenId, ref result, ClientIP);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Value += @"<ORDER_LINE>
                        <ERP_PRODUCT_ID>" + dt.Rows[i]["ERPProdID"] + "</ERP_PRODUCT_ID> <DESCRIPTION>" + dt.Rows[i]["OrderID"] + "</DESCRIPTION> <PRODUCT_UOM_ID></PRODUCT_UOM_ID> <PRICE_UNIT>" + dt.Rows[i]["UnitPrice"] + "</PRICE_UNIT> <QTY>" + dt.Rows[i]["Quantity"] + "</QTY> <DISCOUNT_PERCENTAGE>" + dt.Rows[i]["Discount"] + "</DISCOUNT_PERCENTAGE> </ORDER_LINE>";
            }
            Value += "</SALE_ORDER>";

            if (result != "")
            {
                return OutPutXml(@"<RESULT>" + result + "</RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
            }
            if (dt.Rows.Count == 0)
            {
                return OutPutXml(@"<RESULT> No order found </RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
            }
            arr[0] = Value;
            string Db = "kcl_demo";
            string username = "admin";
            string psw = "admin";
            iFace proxy = XmlRpcProxyGen.Create<iFace>();
            proxy.NonStandard = XmlRpcNonStandard.AllowStringFaultCode;
            var value = proxy.login(Db, username, psw);
            ifaceamthod ProxyMethod = XmlRpcProxyGen.Create<ifaceamthod>();
            string XmlResult = ProxyMethod.execute(Db, value, psw, "kcl.api", "createQuotation", arr);
            xd.LoadXml(XmlResult);
            ds.ReadXml(new XmlNodeReader(xd));
            if (ds.Tables["RESPONSE"].Rows[0]["RESULT"].ToString().ToLower() == "success")
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    clsData.InsertExportSaleID(dt.Rows[i]["OrderID"].ToString(), ds.Tables["RESPONSE"].Rows[0]["ERP_SALE_ORDER_ID"].ToString());
                }
            }
            return xd;
        }
        catch (Exception)
        {
            return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
        }
    }

    [WebMethod]
    public System.Xml.XmlDocument CreateOrderBatch(string OrderLogdate,string Status)
    {
        try
        {
            DataSet ds = new DataSet();
            DataTable Newdt = new DataTable();
            string Value = "";
            //Newdt = clsData.QuotationDetails();
            XmlDocument xd = new XmlDocument();
            string ClientIP = IPNetworking.GetIP4Address();
            
            string result = "";
            int output = 0;
            string path = Server.MapPath("sample1.txt");
            string[] arr = new string[1];
            DataTable dt = clsData.CreateQuotation(OrderLogdate, TokenId, ref result, ClientIP, Status);


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    clsData.CreateBarchOrder(dt.Rows[i]["OrderID"].ToString(), Convert.ToDateTime(dt.Rows[i]["Orderlogdate"].ToString()).ToString("MM/dd/yyyy"), "1", dt.Rows[i]["OrderDetailID"].ToString(), ClientIP, "123456", ref result, 1, ref output, 0);
                    if (result != "")
                        break;
                }
                clsData.CreateBarchOrder(dt.Rows[i]["OrderID"].ToString(), dt.Rows[i]["Orderlogdate"].ToString(), "1", dt.Rows[i]["OrderDetailID"].ToString(), ClientIP, "123456", ref result, 2, ref output, output);
            }
            
            if (result != "")
            {
                return OutPutXml(@"<RESULT>Fail</RESULT> <Error>" + result + "</Error>");
            }

            if (dt.Rows.Count == 0)
            {
                return OutPutXml(@"<RESULT>Fail</RESULT> <Error>No Recode Found</Error>");
            }

            return OutPutXml(@"<RESULT>sucess</RESULT> <Error>" + result + "</Error>");
        }
        catch (Exception)
        {
            return OutPutXml(@"<RESULT>Fail</RESULT><Error>invalid Date </Error>");
        }
    }

    //[WebMethod]
    //public System.Xml.XmlDocument ChangeBatchOrderStatus(string OrderLogdate)
    //{
    //    try
    //    {
    //        DataSet ds = new DataSet();
    //        DataTable Newdt = new DataTable();
    //        string Value = "";
    //        //Newdt = clsData.QuotationDetails();
    //        XmlDocument xd = new XmlDocument();
    //        string ClientIP = IPNetworking.GetIP4Address();

    //        string result = "";
    //        int output = 0;
    //        string path = Server.MapPath("sample1.txt");
    //        string[] arr = new string[1];
    //        DataTable dt = clsData.CreateQuotation(OrderLogdate, "123456", ref result, ClientIP);


    //        for (int i = 0; i < dt.Rows.Count; i++)
    //        {
    //            if (i == 0)
    //            {
    //                clsData.CreateBarchOrder(dt.Rows[i]["OrderID"].ToString(), dt.Rows[i]["Orderlogdate"].ToString(), "1", dt.Rows[i]["OrderDetailID"].ToString(), ClientIP, "123456", ref result, 1, ref output, 0);
    //            }
    //            clsData.CreateBarchOrder(dt.Rows[i]["OrderID"].ToString(), dt.Rows[i]["Orderlogdate"].ToString(), "1", dt.Rows[i]["OrderDetailID"].ToString(), ClientIP, "123456", ref result, 2, ref output, output);

    //        }


    //        //for (int i = 0; i < dt.Rows.Count; i++)
    //        //{
    //        //    clsData.InsertExportSaleID(dt.Rows[i]["OrderID"].ToString(), ds.Tables["RESPONSE"].Rows[0]["ERP_SALE_ORDER_ID"].ToString());
    //        //}



    //        return OutPutXml(@"<RESULT>sucess</RESULT>");
    //    }
    //    catch (Exception)
    //    {
    //        return OutPutXml(@"<RESULT>Fail - Invalid data set in xml </RESULT>");
    //    }
    //}

    [WebMethod]
    public System.Xml.XmlDocument OrderLog(int OrderID, int OrderActionID)
    {
        try
        {
            string ClientIP = IPNetworking.GetIP4Address();
            string result = "";
           
            string path = Server.MapPath("sample1.txt");
            string[] arr = new string[1];
            clsData.Orderlog(OrderID, OrderActionID, TokenId, ref result, ClientIP);
            if (result != "")
            {
                return OutPutXml(@"<RESULT>Fail</RESULT><Error>" + result + "</Error>");
            }
            else
            {
                return OutPutXml(@"<RESULT>success</RESULT><Error></Error>");
            }

            // return  OutPutXml(@"<RESULT>success</RESULT><Error><Error>");
        }
        catch (Exception)
        {
            return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
        }
    }


    [WebMethod]
    public System.Xml.XmlDocument ChangeBachOrderStatus(int OrderBatchID, int OrderBatchStatusID)
    {
        try
        {
            string ClientIP = IPNetworking.GetIP4Address();
            string result = "";
           
            string path = Server.MapPath("sample1.txt");
            string[] arr = new string[1];
            clsData.ChangeBatchOrderStatus(OrderBatchID, ClientIP, TokenId, OrderBatchStatusID, ref result);
            //clsData.ChangeBachOrderStatus(OrderID, OrderActionID, "123456", ref result, ClientIP);
            if (result != "")
            {
                return OutPutXml(@"<RESULT>Fail</RESULT><Error>" + result + "</Error>");
            }
            else
            {
                return OutPutXml(@"<RESULT>Success</RESULT><Error></Error>");
            }

            // return  OutPutXml(@"<RESULT>success</RESULT><Error><Error>");
        }
        catch (Exception)
        {
            return OutPutXml(@"<RESULT>Fail - Invalid data set in xml format.</RESULT><ERP_SALE_ORDER_ID></ERP_SALE_ORDER_ID>");
        }
    }

    public System.Xml.XmlDocument OutPutXml(string Xml)
    {
        string path = Server.MapPath("result.txt");
        if (File.Exists(path))
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            StreamWriter str = new StreamWriter(fs);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                var value = Xml;
                sw.Write(value);
            }
        }
        XmlDocument xd1 = new XmlDocument();
        xd1.LoadXml(StartOutPut + Xml + EndOutPut);
        return xd1;
    }

    public string ValidationERP_ProductID(string ERPProdID)
    {
        try
        {
            int value = Convert.ToInt32(ERPProdID);
            return "";
        }
        catch (Exception)
        {
            return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL (Invalid ERP_PRODUCT_ID)</RESULT>");
        }
    }

    public string ValidationBarcode(string Barcode)
    {
        try
        {
            double value = Convert.ToDouble(Barcode);
            if (Barcode.Length > 21)
            {
                return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL </RESULT> <Error>EAN13 Sholud Be  less Then 20 Charter</Error> ");
            }
            return "";
        }
        catch (Exception)
        {
            return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL(Invalid EAN13)</RESULT>");
        }
    }

    public string ValidationPrices(string PriceNow)
    {
        try
        {
            double value = Convert.ToDouble(PriceNow);
            return "";
        }
        catch (Exception)
        {
            return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL (Invalid  PRICE)</RESULT>");
        }
    }

    public string ValidationPRODUCT_NAME(string PRODUCT_NAME)
    {
        Regex regex = new Regex("[/,:<>!~@#$%^&()+=?()\"|!\\[#$]");
        if (regex.IsMatch(PRODUCT_NAME))
        {
            return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL</RESULT> <Error>Invalid Product Name</Error> ");
        }
        else
        {
            if (PRODUCT_NAME.Length > 81)
            {
                return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL</RESULT><Error>Product Name Sholud Be  less Then 80 Charter</Error>");
            }
            else
            {
                return "";
            }
        }
    }

    public string ValidationToken(string Token)
    {
        if (Token.Length > 13)
        {
            return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL</RESULT> <Error>SECURITY_TOKEN Sholud Be  less Then 12 Charter</Error> ");
        }
        else
        {
            if (Token.Length > 13)
            {
                return (@"<ERP_PRODUCT_ID>" + ERPProdID + "</ERP_PRODUCT_ID> <RESULT>FAIL</RESULT> <Error>SECURITY_TOKEN Sholud Be  less Then 12 Charter</Error> ");
            }
            else
            {
                return "";
            }

        }
    }

    public class IPNetworking
    {
        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
    }

}
