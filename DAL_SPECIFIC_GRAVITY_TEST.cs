using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DAL
{
    public class DAL_SPECIFIC_GRAVITY_TEST
    {


        public string INSERT_SPECIFIC_GRAVITY(string Emp_Code, Specific_Gravity_Abs _SPGRAVITY, out ResultMessage oblMsg)
        {

            string errorMsg = "";
            oblMsg = new ResultMessage();
            using (var connection = new SqlConnection(sqlConnection.GetConnectionString_SGX()))
            {

                connection.Open();
                SqlCommand command;
                SqlTransaction transactionScope = null;
                transactionScope = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {

                    SqlParameter[] param =
                    { 
                      new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                     ,new SqlParameter("@Test_ID", SqlDbType.Decimal) 
                     ,new SqlParameter("@Test_No", SqlDbType.VarChar, 20)  
                     ,new SqlParameter("@Date", _SPGRAVITY.Date)
                     ,new SqlParameter("@Source", _SPGRAVITY.Source)  
                     ,new SqlParameter("@Type", _SPGRAVITY.Type )    
                     ,new SqlParameter("@Colour", _SPGRAVITY.Colour)   
                     ,new SqlParameter("@Texture", _SPGRAVITY.Texture) 
                     ,new SqlParameter("@Shape", _SPGRAVITY.Shape)  
                     ,new SqlParameter("@Rock_Type", _SPGRAVITY.Rock_Type)
                     ,new SqlParameter("@Tested_By", _SPGRAVITY.TESTED_BY) 
                     ,new SqlParameter("@Qc_Incharge", _SPGRAVITY.QC_INCHARGE) 
                      ,new SqlParameter("@Remarks", _SPGRAVITY.REMARKS) 
                     ,new SqlParameter("@Added_By",Emp_Code)  
                    };
                    param[0].Direction = ParameterDirection.Output;
                    param[1].Direction = ParameterDirection.Output;
                    param[2].Direction = ParameterDirection.Output;
                    new DataAccess().InsertWithTransaction("[AGG].[USP_INSERT_SPECIFIC_GRAVITY_HDR]", CommandType.StoredProcedure, out command, connection, transactionScope, param);
                    decimal Test_ID = (decimal)command.Parameters["@Test_ID"].Value;
                    string Test_No = (string)command.Parameters["@Test_No"].Value;
                    string error_1 = (string)command.Parameters["@ERRORSTR"].Value;

                    if (Test_ID == -1) { errorMsg = error_1; }
                    else if (Test_ID > 0)
                    {
                        //List<Gravity_list> _list = new List<Gravity_list>();

                        SqlParameter[] param2 =
                                {
                                new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                               ,new SqlParameter("@Test_Dtl_ID", SqlDbType.Decimal)  
                               ,new SqlParameter("@Test_ID" ,Test_ID)  
                               //,new SqlParameter("@Determination", (_SPGRAVITY.Determination == null) ? "" : _SPGRAVITY.Determination)
                               ,new SqlParameter("@Sample_Wt" , (_SPGRAVITY.Sample_Wt == null)? (object)DBNull.Value : _SPGRAVITY.Sample_Wt) 
                               ,new SqlParameter("@Wb_Agg_Wt", (_SPGRAVITY.Wb_Agg_Wt == null)? (object)DBNull.Value : _SPGRAVITY.Wb_Agg_Wt) 
                               ,new SqlParameter("@Wb_Wt", (_SPGRAVITY.Wb_Wt == null)? (object)DBNull.Value : _SPGRAVITY.Wb_Wt) 
                               ,new SqlParameter("@Dry_Agg_Wt", (_SPGRAVITY.Dry_Agg_Wt	 == null)? (object)DBNull.Value: _SPGRAVITY.Dry_Agg_Wt)  
                               ,new SqlParameter("@Oven_Dry_Agg_Wt", (_SPGRAVITY.Oven_Dry_Agg_Wt == null)? (object)DBNull.Value : _SPGRAVITY.Oven_Dry_Agg_Wt)  
                                    };
                        param2[0].Direction = ParameterDirection.Output;
                        param2[1].Direction = ParameterDirection.Output;
                        new DataAccess().InsertWithTransaction("[AGG].[USP_INSERT_SPECIFIC_GRAVITY_Dtl]", CommandType.StoredProcedure, out command, connection, transactionScope, param2);
                        decimal Test_Dtl_ID = (decimal)command.Parameters["@Test_Dtl_ID"].Value;
                        string error_2 = (string)command.Parameters["@ERRORSTR"].Value;
                        if (Test_Dtl_ID == -1) { errorMsg = error_2; }
                    }


                    if (errorMsg == "")
                    {

                        oblMsg.ID = Test_ID;
                        oblMsg.CODE = Test_No;
                        oblMsg.MsgType = "Success";
                        transactionScope.Commit();
                    }
                    else
                    {
                        oblMsg.Msg = errorMsg;
                        oblMsg.MsgType = "Error";
                        transactionScope.Rollback();
                    }

                }

                catch (Exception ex)
                {
                    try
                    {
                        transactionScope.Rollback();
                    }
                    catch (Exception ex1)
                    {
                        errorMsg = "Error: Exception occured. " + ex1.StackTrace.ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return errorMsg;
        }


        public List<Specific_GRavity_Datalist> Select_SpeceficGravity_List(Gravity_list Gravity_Datalist)
        {
            SqlParameter[] param = {    new SqlParameter("@FROM_DT", Gravity_Datalist.From_DT),
                                       new SqlParameter("@TO_DT", Gravity_Datalist.To_DT) ,
                                      
                                    
                                       //new SqlParameter("@Source", materiallist.Source),
                                       //new SqlParameter("@Type", materiallist.Type) ,
                                       //new SqlParameter("@Colour", materiallist.Colour) ,
                                       //new SqlParameter("@Texture", materiallist.Texture) ,
                                       //new SqlParameter("@Shape", materiallist.Shape) ,
                                       //new SqlParameter("@Rock_Type", materiallist.Rock_Type) 
                                   };

            DataSet ds = new DataAccess(sqlConnection.GetConnectionString_SGX()).GetDataSet("[AGG].[USP_SELECT_SPECIFIC_GRAVITY]", CommandType.StoredProcedure, param);

            List<Specific_GRavity_Datalist> _list = new List<Specific_GRavity_Datalist>();
            DataTable dt = ds.Tables[0];
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    _list.Add(new Specific_GRavity_Datalist
                    {
                        Test_ID = Convert.ToDecimal(row["Test_ID"] == DBNull.Value ? 0 : row["Test_ID"]),
                        Test_No = Convert.ToString(row["Test_No"] == DBNull.Value ? "" : row["Test_No"]),
                        Date = Convert.ToString(row["Date"]),
                        Source = Convert.ToString(row["Source"]),
                        Type = Convert.ToString(row["Type"]),
                        Colour = Convert.ToString(row["Colour"]),
                        Texture = Convert.ToString(row["Texture"]),
                        Shape = Convert.ToString(row["Shape"]),
                        Rock_Type = Convert.ToString(row["Rock_Type"]),
                        Test_Dtl_ID = Convert.ToDecimal(row["Test_Dtl_ID"] == DBNull.Value ? 0 : row["Test_Dtl_ID"]),
                        Status = Convert.ToString(row["Status"]),
                    });
                }
            }

            return _list;
        }
        public SpGRavitySample2cs Edit_Specific_Gravity_Entry(decimal Test_ID)
        {
            SqlParameter[] param = { new SqlParameter("@Test_ID", Test_ID) };

            DataSet ds = new DataAccess(sqlConnection.GetConnectionString_SGX()).GetDataSet("[AGG].[USP_INSERT_SPECIFIC_GRAVITY_EDIT]", CommandType.StoredProcedure, param);
            SpGRavitySample2cs _objSPGR = new SpGRavitySample2cs();

            List<SpGRavitySample2cs> _list = new List<SpGRavitySample2cs>();
            SpGRavitySample2cs dtl = null;
            DataTable dt = ds.Tables[0];
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                _objSPGR = new SpGRavitySample2cs();
                _objSPGR.Test_ID = Convert.ToInt32(dt.Rows[0]["Test_ID"] == DBNull.Value ? 0 : dt.Rows[0]["Test_ID"]);
                _objSPGR.Test_No = Convert.ToString(dt.Rows[0]["Test_No"]);
                _objSPGR.Date = Convert.ToDateTime(dt.Rows[0]["Date"]);
                _objSPGR.Source = Convert.ToString(dt.Rows[0]["Source"]);
                _objSPGR.Type = Convert.ToString(dt.Rows[0]["Type"]);
                _objSPGR.Colour = Convert.ToString(dt.Rows[0]["Colour"]);
                _objSPGR.Texture = Convert.ToString(dt.Rows[0]["Texture"]);
                _objSPGR.Shape = Convert.ToString(dt.Rows[0]["Shape"]);
                _objSPGR.Rock_Type = Convert.ToString(dt.Rows[0]["Rock_Type"]);
                _objSPGR.TESTED_BY = Convert.ToString(dt.Rows[0]["Tested_By"]);
                _objSPGR.QC_INCHARGE = Convert.ToString(dt.Rows[0]["Qc_Incharge"]);
                _objSPGR.Added_By = Convert.ToString(dt.Rows[0]["Added_By"]);
                //_objSPGR.Determination = Convert.ToString(dt.Rows[0]["Determination"]);
                _objSPGR.Dry_Agg_Wt = Convert.ToDecimal(dt.Rows[0]["Dry_Agg_Wt"] == DBNull.Value ? 0 : dt.Rows[0]["Dry_Agg_Wt"]);
                _objSPGR.Oven_Dry_Agg_Wt = Convert.ToDecimal(dt.Rows[0]["Oven_Dry_Agg_Wt"] == DBNull.Value ? 0 : dt.Rows[0]["Oven_Dry_Agg_Wt"]);
                _objSPGR.Sample_Wt = Convert.ToDecimal(dt.Rows[0]["Sample_Wt"] == DBNull.Value ? 0 : dt.Rows[0]["Sample_Wt"]);
                _objSPGR.Wb_Agg_Wt = Convert.ToDecimal(dt.Rows[0]["Wb_Agg_Wt"] == DBNull.Value ? 0 : dt.Rows[0]["Wb_Agg_Wt"]);
                _objSPGR.Wb_Wt = Convert.ToDecimal(dt.Rows[0]["Wb_Wt"] == DBNull.Value ? 0 : dt.Rows[0]["Wb_Wt"]);
             
            }

            return _objSPGR;
        }
        public string INSERT_SPECIFIC_GRAVITY(string Emp_Code, SpGRavitySample2cs _SPGRAVITY, out ResultMessage oblMsg)
        {

            string errorMsg = "";
            oblMsg = new ResultMessage();
            using (var connection = new SqlConnection(sqlConnection.GetConnectionString_SGX()))
            {

                connection.Open();
                SqlCommand command;
                SqlTransaction transactionScope = null;
                transactionScope = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                try
                {

                    SqlParameter[] param2 =
                                {
                                new SqlParameter("@ERRORSTR", SqlDbType.VarChar, 200)
                               ,new SqlParameter("@Test_Dtl_ID", SqlDbType.Decimal)  
                               ,new SqlParameter("@Test_ID" ,_SPGRAVITY.Test_ID) 
                               //,new SqlParameter("@Determination", (_SPGRAVITY.Determination == null) ? "" : _SPGRAVITY.Determination)
                               ,new SqlParameter("@Sample_Wt" , (_SPGRAVITY.Sample_Wt1 == null)? (object)DBNull.Value : _SPGRAVITY.Sample_Wt1) 
                               ,new SqlParameter("@Wb_Agg_Wt", (_SPGRAVITY.Wb_Agg_Wt1 == null)? (object)DBNull.Value : _SPGRAVITY.Wb_Agg_Wt1) 
                               ,new SqlParameter("@Wb_Wt",     (_SPGRAVITY.Wb_Wt1 == null)? (object)DBNull.Value : _SPGRAVITY.Wb_Wt1) 
                               ,new SqlParameter("@Dry_Agg_Wt",  (_SPGRAVITY.Dry_Agg_Wt1  == null)? (object)DBNull.Value: _SPGRAVITY.Dry_Agg_Wt1)  
                               ,new SqlParameter("@Oven_Dry_Agg_Wt", (_SPGRAVITY.Oven_Dry_Agg_Wt1 == null)? (object)DBNull.Value : _SPGRAVITY.Oven_Dry_Agg_Wt1)  

                                    };
                    param2[0].Direction = ParameterDirection.Output;
                    param2[1].Direction = ParameterDirection.Output;
                    new DataAccess().InsertWithTransaction("[AGG].[USP_INSERT_SPECIFIC_GRAVITY_Dtl]", CommandType.StoredProcedure, out command, connection, transactionScope, param2);
                    decimal Test_Dtl_ID = (decimal)command.Parameters["@Test_Dtl_ID"].Value;
                    string error_2 = (string)command.Parameters["@ERRORSTR"].Value;
                    if (Test_Dtl_ID == -1) { errorMsg = error_2; }

                    if (errorMsg == "")
                    {

                        oblMsg.ID = Test_Dtl_ID;
                        oblMsg.CODE = _SPGRAVITY.Test_No;
                        oblMsg.MsgType = "Success";
                        transactionScope.Commit();
                    }

                    else
                    {
                        oblMsg.Msg = errorMsg;
                        oblMsg.MsgType = "Error";
                        transactionScope.Rollback();
                    }

                }

                catch (Exception ex)
                {
                    try
                    {
                        transactionScope.Rollback();
                    }
                    catch (Exception ex1)
                    {
                        errorMsg = "Error: Exception occured. " + ex1.StackTrace.ToString();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
            return errorMsg;
        }

    }
}
