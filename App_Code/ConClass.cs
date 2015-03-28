using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class ConClass
{

    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["WebApi"].ToString());

    //public bool InsertProduct(string SubName, string SubCName, string PriceNow, string Barcode, ref  int retunvalue, string ERPProdID, ref string result)
    //{


    //    try
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.Connection = con;
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = "SpInsertProduct";
    //        cmd.Parameters.AddWithValue("@SubName", SubName);
    //        cmd.Parameters.AddWithValue("@SubCName ", SubCName);
    //        cmd.Parameters.AddWithValue("@PriceNow", PriceNow);
    //        cmd.Parameters.AddWithValue("@Barcode", Barcode);
    //        cmd.Parameters.AddWithValue("@ERPProdID", ERPProdID);
    //        cmd.Parameters.Add("@outpute", SqlDbType.Int);
    //        cmd.Parameters["@outpute"].Direction = ParameterDirection.Output;


    //        cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
    //        cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
    //        con.Open();
    //        int i = cmd.ExecuteNonQuery();
    //        con.Close();
    //        retunvalue = Convert.ToInt32(cmd.Parameters["@outpute"].Value);
    //        result = Convert.ToString(cmd.Parameters["@Result"].Value);

    //        return true;
    //    }
    //    catch (Exception ex)
    //    {

    //        return false;
    //    }
    //}

    public bool InsertProduct(string SubName, string SubCName, string PriceNow, string Barcode, ref  int retunvalue, string ERPProdID, ref string result, string Username, string Token, string ClientIP)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpInsertProduct";
            cmd.Parameters.AddWithValue("@SubName", SubName);
            cmd.Parameters.AddWithValue("@SubCName ", SubCName);
            cmd.Parameters.AddWithValue("@PriceNow", PriceNow);
            cmd.Parameters.AddWithValue("@Barcode", Barcode);
            cmd.Parameters.AddWithValue("@ERPProdID", ERPProdID);
            cmd.Parameters.AddWithValue("@Token", Token);
            cmd.Parameters.AddWithValue("@username", Username);
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.Add("@outpute", SqlDbType.Int);
            cmd.Parameters["@outpute"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            retunvalue = Convert.ToInt32(cmd.Parameters["@outpute"].Value);
            result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return true;
        }
        catch (Exception)
        {
            retunvalue = 0;
            result = "Fail - Invalid data set in xml format";
            con.Close();
            return false;
        }
    }

    public DataTable InventoryInquryERP(int ERP_PRODUCT_ID, int SubProdStockPlaceID, ref string Result, string ClientIP)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InventoryInquryERP";
            cmd.Parameters.AddWithValue("@ERP_PRODUCT_ID", ERP_PRODUCT_ID);
            cmd.Parameters.AddWithValue("@SubProdStockPlaceID", SubProdStockPlaceID);
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
            SqlAdp.Fill(dt);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return dt;
        }
        catch (Exception)
        {
            Result = "Fail - Invalid data set in xml format.";
            return dt;
        }
    }

    public DataTable InventoryInqury(int ERP_PRODUCT_ID, string Barcode, int SubProdStockPlaceID, string Token, ref string Result, string ClientIP)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InventoryInqury";
            cmd.Parameters.AddWithValue("@ERP_PRODUCT_ID", ERP_PRODUCT_ID);
            cmd.Parameters.AddWithValue("@Barcode ", Barcode);
            cmd.Parameters.AddWithValue("@SubProdStockPlaceID", SubProdStockPlaceID);
            cmd.Parameters.AddWithValue("@Token", Token);
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
            SqlAdp.Fill(dt);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return dt;
        }
        catch (Exception)
        {

            Result = "Fail - Invalid data set in xml format.";
            return dt;
        }
    }

    public Boolean ExportInventory(int ERP_PRODUCT_ID, string Barcode, int SubProdStockPlaceID, string Token, ref string Result, string ClientIP, string Note1, string Note2, int Qty, ref int retunvalue)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ExportInventory";
            cmd.Parameters.AddWithValue("@Token", Token);
            cmd.Parameters.AddWithValue("@NOTE_ONE", Note1);
            cmd.Parameters.AddWithValue("@NOTE_TWO", Note2);
            cmd.Parameters.AddWithValue("@SubProdStockPlaceID", SubProdStockPlaceID);
            cmd.Parameters.AddWithValue("@ERP_PRODUCT_ID", ERP_PRODUCT_ID);

            cmd.Parameters.AddWithValue("@Barcode", Barcode);
            cmd.Parameters.AddWithValue("@Qty", Qty);
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);

            cmd.Parameters.Add("@outpute", SqlDbType.Int);
            cmd.Parameters["@outpute"].Direction = ParameterDirection.Output;

            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            retunvalue = Convert.ToInt32(cmd.Parameters["@outpute"].Value);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);

            return true;
        }
        catch (Exception)
        {
            retunvalue = 0;
            Result = "Fail - Invalid data set in xml format.";
            con.Close();
            return false;
        }
    }

    public DataTable InventoryStockUpdate(int ERP_PRODUCT_ID, int SubProductId, int SubProdStockPlaceID, ref string Result, string ClientIP)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InventoryInquryERP";
            cmd.Parameters.AddWithValue("@ERP_PRODUCT_ID", ERP_PRODUCT_ID);
            cmd.Parameters.AddWithValue("@SubProductId", SubProductId);
            cmd.Parameters.AddWithValue("@SubProdStockPlaceID", SubProdStockPlaceID);
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
            SqlAdp.Fill(dt);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return dt;
        }
        catch (Exception)
        {

            Result = "Fail - Invalid data set in xml format.";
            return dt;
        }

        //UpdateInventory   
    }

    public Boolean UpdateInventory(int ERP_PRODUCT_ID, string Barcode, int SubProdStockPlaceID, string Token, ref string Result, string ClientIP, int Qty, ref int retunvalue)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "UpdateInventory";
            cmd.Parameters.AddWithValue("@Token", Token);

            cmd.Parameters.AddWithValue("@SubProdStockPlaceID", SubProdStockPlaceID);
            cmd.Parameters.AddWithValue("@ERP_PRODUCT_ID", ERP_PRODUCT_ID);

            cmd.Parameters.AddWithValue("@Qty", Qty);
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);

            cmd.Parameters.Add("@outpute", SqlDbType.Int);
            cmd.Parameters["@outpute"].Direction = ParameterDirection.Output;

            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            retunvalue = Convert.ToInt32(cmd.Parameters["@outpute"].Value);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return true;
        }
        catch (Exception)
        {
            retunvalue = 0;
            Result = "Fail - Invalid data set in xml format.";
            con.Close();
            return false;
        }
    }

    public DataTable QuotationDetails()
    {
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpQuotationDetails";
            SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
            SqlAdp.Fill(dt);

            return dt;
        }
        catch (Exception)
        {
            return dt;
        }
    }


    public DataTable CreateQuotation(string Orderlogdate, string Token, ref string Result, string ClientIP,string status)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CreateQuotation";
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.AddWithValue("@Token ", Token);
            cmd.Parameters.AddWithValue("@OrderStatus", status);
            cmd.Parameters.AddWithValue("@Orderlogdate", Orderlogdate);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
            SqlAdp.Fill(dt);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return dt;
        }
        catch (Exception)
        {
            Result = "Fail - Invalid data set in xml format.";
            return dt;
        }
    }


    public DataTable OrderBatch(string Orderlogdate, string Token, ref string Result, string ClientIP)
    {
        DataTable dt = new DataTable();
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OrderBatch";
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.AddWithValue("@Token ", Token);
            cmd.Parameters.AddWithValue("@Orderlogdate", Orderlogdate);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
            SqlAdp.Fill(dt);
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return dt;
        }
        catch (Exception)
        {
            Result = "Fail - Invalid data set in xml format.";
            return dt;
        }
    }


    public bool Orderlog(int OrderId, int OrderActionID, string Token, ref string Result, string ClientIP)
    {
       
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SpOrderlog";
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.AddWithValue("@Token ", Token);
            cmd.Parameters.AddWithValue("@OrderId", OrderId);
            cmd.Parameters.AddWithValue("@ActionID", OrderActionID);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return true;
        }
        catch (Exception)
        {
            con.Close();
            Result = "Fail - Invalid data set in xml format.";
            return false;
        }
    }


    //public DataTable CreateBatchOrder(string Orderlogdate, string Token, ref string Result, string ClientIP)
    //{
    //    DataTable dt = new DataTable();
    //    try
    //    {
    //        SqlCommand cmd = new SqlCommand();
    //        cmd.Connection = con;
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = "OrderBatch";
    //        cmd.Parameters.AddWithValue("@clientIP", ClientIP);
    //        cmd.Parameters.AddWithValue("@Token ", Token);
    //        cmd.Parameters.AddWithValue("@Orderlogdate", Orderlogdate);
    //        cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
    //        cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
             
    //        SqlDataAdapter SqlAdp = new SqlDataAdapter(cmd);
    //        SqlAdp.Fill(dt);
    //        Result = Convert.ToString(cmd.Parameters["@Result"].Value);
    //        return dt;
    //    }
    //    catch (Exception)
    //    {
    //        Result = "Fail - Invalid data set in xml format.";
    //        return dt;
    //    }
    //}

    public bool InsertExportSaleID(string orderID, string ERP_SALE_ORDER_ID)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "InsertExportSaleID";
            cmd.Parameters.AddWithValue("@orderID", orderID);
            cmd.Parameters.AddWithValue("@ERP_SALE_ORDER_ID", ERP_SALE_ORDER_ID);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public bool CreateBarchOrder(string orderID, string Orderlogdate, string orderBatchStatusId, string orderDetailsId, string ClientIp, string Token, ref string Result, int state, ref int output, int OrderBatch)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "CreateBatchOrder";
            cmd.Parameters.AddWithValue("@orderID", orderID);
            cmd.Parameters.AddWithValue("@clientIP", ClientIp);
            cmd.Parameters.AddWithValue("@OrderbatchDate", Orderlogdate);
            cmd.Parameters.AddWithValue("@orderBatchStatusId", orderBatchStatusId);
            cmd.Parameters.AddWithValue("@orderDetailsId", orderDetailsId);
            cmd.Parameters.AddWithValue("@Token", Token);
            cmd.Parameters.AddWithValue("@state", state);
            cmd.Parameters.AddWithValue("@OrderBatch", OrderBatch);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@output", SqlDbType.VarChar, 100);
            cmd.Parameters["@output"].Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            if (state==1)
            output = Convert.ToInt32(cmd.Parameters["@output"].Value);
            con.Close();
            return true;
        }
        catch (Exception)
        {
            con.Close();
            return false;
        }
    }



    public bool ChangeBatchOrderStatus(int OrderBatchID, string ClientIP, string Token, int OrderBatchStatusID, ref string Result)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ChangeBatchOrderStatus";
            cmd.Parameters.AddWithValue("@clientIP", ClientIP);
            cmd.Parameters.AddWithValue("@Token ", Token);
            cmd.Parameters.AddWithValue("@OrderBatchStatusID", OrderBatchStatusID);
            cmd.Parameters.AddWithValue("@OrderBatch", OrderBatchID);
            cmd.Parameters.Add("@Result", SqlDbType.VarChar, 100);
            cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            Result = Convert.ToString(cmd.Parameters["@Result"].Value);
            return true;
        }
        catch (Exception)
        {
            con.Close();
            Result = "Fail - Invalid data set in xml format.";
            return false;
        }
    }

}
