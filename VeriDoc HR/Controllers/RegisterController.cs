using MyApp.db;
using MyApp.db.MyAppBal;
using MyApp.Entity;
using MyApp.Entity.Message;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VeriDoc_HR.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        // GET: Register
        String errMsg = String.Empty;
        DataTable dt = null;
        EntitySysUser oSysUser = null;

        public ActionResult Index()
        {

            try
            {
                if (Session["oSysUser"] != null)
                {
                    oSysUser = (EntitySysUser)Session["oSysUser"];
                    oSysUser.USER_KEY = Convert.ToInt32(Session["USER_KEY"]);

                    //  oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
                    errMsg = String.Empty;

                    dt = FillUserGrid();
                    if (!String.IsNullOrEmpty(errMsg))
                    {


                        // MessageBox(1, "Activity " + Message.msgErrDdlPop, errMsg);

                    }


                }
                else
                {
                    return Redirect("~/Admin/Index");
                }

            }
            catch (Exception ex)
            {
                // MessageBox(1, Message.msgErrCommon, ex.Message);
            }

            return View(dt.Rows);
        }


        private DataTable FillUserGrid()
        {
            try
            {
                errMsg = String.Empty;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                dt = new DataTable();
                using (BaRegister oBMC = new BaRegister())
                {
                    dt = oBMC.GetData("GET", "", ref errMsg, "2019", 1);
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                }

            }
            catch (Exception ex)
            {

            }

            return dt;

        }


        
        [HttpPost]
        public ActionResult delete(FormCollection form)
        {
            Int32 vKey = 0; Byte vRef = 0; String vDelMsg = String.Empty;
            EntityRegister oBMAST = null;
            try
            {
                if (ModelState.IsValid)
                {
                    errMsg = String.Empty;
                    oBMAST = new EntityRegister();
                    string edit = form[0];
                    oBMAST.DTLS_REGISTER_KEY = Convert.ToInt32(edit);

                    oBMAST.ENT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.EDIT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.TAG_DELETE = 0;

                    using (BaRegister oBMC = new BaRegister())
                    {
                        vRef = oBMC.SaveDelete<Object, Int32>("DELETE", oBMAST, null, ref vKey, ref errMsg, ref vDelMsg, "2019", 1);
                        if (vRef > 0)
                        {
                            if (vRef == 1)
                            {
                                TempData["JavaScriptMsg"] = string.Format("OpenTab1('" + Message.msgSaveDelete + "');");
                                
                            }
                            else
                            {
                                TempData["JavaScriptMsg"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                                
                            }

                        }
                    }
                }
                else
                {
                    TempData["JavaScriptMsg"] = string.Format("OpenTab1('" + Message.msgPageInvalid + "');");
                   
                }

            }
            catch (Exception ex)
            {
                TempData["JavaScriptMsg"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                ;
            }
            finally
            {
                if (oBMAST != null)
                    oBMAST = null;
            }

            return Redirect("~/Register/Index");
        }

    }
}