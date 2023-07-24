using BusinessLayer;
using BusinessLayer.DAL;
using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eSGBIZ.Controllers
{
    public class SpecificController : BaseController
    {
        //
        // GET: /Specific/
         [Authorize]
        public ActionResult SpecificGravityEntry()
        {
            Specific_Gravity_Abs _GRAVISP = new Specific_Gravity_Abs();
            ViewBag.Header = "Specific Gravity & Water Absorption";
            

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _GRAVISP.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");

            return View(_GRAVISP); 
        }


        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("SpecificGravityEntry")]
        public ActionResult INSERT_SPECIFIC_GRAVITY_SAVE(Specific_Gravity_Abs _GRAVISP)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_SPECIFIC_GRAVITY_TEST().INSERT_SPECIFIC_GRAVITY(emp.Employee_Code, _GRAVISP, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>SPECIFIC GRAVITY inserted successfully. Test No : </b> <b style='color:red'> " + objMst.CODE + "</b>"), true);
                        return RedirectToAction("SpecificGravityList", "Specific");
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }
            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _GRAVISP.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");

        
            return View(_GRAVISP);
        }


        [Authorize]
        public ActionResult SpecificGravityList()
        {
            ViewBag.Header = "Specific Gravity List";
            Gravity_list _GRAVISP = new Gravity_list();

            return View(_GRAVISP);

        }

        public ActionResult _SpecificGravityList()
        {
            return PartialView("_SpecificGravityList");
        }
        [HttpPost]
        public ActionResult _SpecificGravity_Data_List(DateTime fDate, DateTime tDate)
        {
            // Server Side Processing
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int totalRow = 0;

            Gravity_list _SpeGrav = new Gravity_list();
            List<Specific_GRavity_Datalist> SpeGravityData = new List<Specific_GRavity_Datalist>();
            try
            {
                _SpeGrav.From_DT = fDate;
                _SpeGrav.To_DT = tDate;


                SpeGravityData = new DAL_SPECIFIC_GRAVITY_TEST().Select_SpeceficGravity_List(_SpeGrav);

                totalRow = SpeGravityData.Count();

            }
            catch (Exception ex)
            {
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }

            if (!string.IsNullOrEmpty(searchValue)) // Filter Operation
            {
                SpeGravityData = SpeGravityData.
                    Where(x => x.Test_No.ToLower().Contains(searchValue.ToLower())
                    || x.Date.ToLower().Contains(searchValue.ToLower()) ||

                        x.Source.ToLower().Contains(searchValue.ToLower()) || x.Rock_Type.ToLower().Contains(searchValue.ToLower())
                         ).ToList<Specific_GRavity_Datalist>();

            }
            int totalRowFilter = SpeGravityData.Count();

            if (length == -1)
            {
                length = totalRow;
            }
            SpeGravityData = SpeGravityData.Skip(start).Take(length).ToList<Specific_GRavity_Datalist>();

            var jsonResult = Json(new { data = SpeGravityData, draw = Request["draw"], recordsTotal = totalRow, recordsFiltered = totalRowFilter }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }


        public ActionResult SpecificGravityEdit(decimal Test_ID)
        {
            SpGRavitySample2cs _objSPGRA = new SpGRavitySample2cs();
            ViewBag.Header = "SpecificGravityEntryEdit";


            _objSPGRA = new DAL_SPECIFIC_GRAVITY_TEST().Edit_Specific_Gravity_Entry(Test_ID);

            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _objSPGRA.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");

            return View(_objSPGRA);
        }
        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("SpecificGravityEdit")]
        public ActionResult SPECIFIC_GRAVITY_ENTRY(SpGRavitySample2cs _GRAVISP)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_SPECIFIC_GRAVITY_TEST().INSERT_SPECIFIC_GRAVITY(emp.Employee_Code, _GRAVISP, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>SPECIFIC GRAVITY inserted successfully. Test No : </b> <b style='color:red'> " + objMst.CODE+ "</b>"), true);
                    
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }
            var empExceptionList = new List<string> { "CAL0229", "CAL0230" };
            List<EMPLOYEE_DETAILS> _empList = new DAL_Common().GetEmployee_List("", "", "", "", "", emp.Employee_Code, "").Where(x => x.activeFlag == "Y" && !empExceptionList.Contains(x.Employee_Code)).ToList<EMPLOYEE_DETAILS>();
            _GRAVISP.EMPLOYEE_LIST = new SelectList(_empList.OrderBy(x => x.EmployeeName), "Employee_Code", "EmployeeName");


            return View(_GRAVISP);
        }


    }
}
