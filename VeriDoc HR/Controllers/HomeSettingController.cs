using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyApp.Entity;
using System.Data;
using MyApp.db.MyAppBal;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;
using MyApp.db;

namespace VeriDoc_HR.Controllers
{
    public class HomeSettingController : Controller
    {
        // GET: HomeSetting
        EntitySysUser oSysUser = null;
        String errMsg = String.Empty;
        DataTable dt;
        DataTable dt1;


        #region homepage

        public ActionResult Index()
        {
            if (Session["oSysUser"] != null)
            {
                oSysUser = (EntitySysUser)Session["oSysUser"];
                oSysUser.USER_KEY = Convert.ToInt32(Session["USER_KEY"]);

                oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
                errMsg = String.Empty;
                FillHomeSettingEdit();
                ViewBag.PATENT_KEY = "0";
                ViewBag.QUICK_FACT_KEY = "0";
                dt = FillQuickFactTableGrid();

                return View(dt.Rows);

            }
            else
            {
                return Redirect("~/Admin/Index");
            }

        }

        #region homesetting
        private String FillHomeSettingEdit()
        {
            try
            {
                String vCOMPANY_ACCESS = String.Empty;
                errMsg = String.Empty;
                dt1 = new DataTable();
                using (BAHomeSettings oBME = new BAHomeSettings())
                {
                    dt1 = oBME.GetData("GET", "", ref errMsg, null, 1);
                }
                if (dt1 != null && dt1.Rows.Count > 0)
                {
                    ViewBag.BANNER_HEADING = Convert.ToString(dt1.Rows[0]["BANNER_HEADING"]);
                    ViewBag.BANNER_DESC = Convert.ToString(dt1.Rows[0]["BANNER_DESC"]);
                    ViewBag.BANNER_DESC_1 = Convert.ToString(dt1.Rows[0]["BANNER_DESC_1"]);
                    ViewBag.BANNER_IMG = Convert.ToString(dt1.Rows[0]["BANNER_IMG"]);
                    ViewBag.BANNER_NOTE = Convert.ToString(dt1.Rows[0]["BANNER_NOTE"]);
                    ViewBag.GOOGLEPLAY_LINK = Convert.ToString(dt1.Rows[0]["GOOGLEPLAY_LINK"]);
                    ViewBag.PATENTED_HEADING = Convert.ToString(dt1.Rows[0]["PATENTED_HEADING"]);
                    ViewBag.PATENTED_IMG = Convert.ToString(dt1.Rows[0]["PATENTED_IMG"]);
                    ViewBag.APPSTORE_LINK = Convert.ToString(dt1.Rows[0]["APPSTORE_LINK"]);

                    ViewBag.img_banner = ConfigurationManager.AppSettings["HOME"].ToString() + Convert.ToString(dt1.Rows[0]["BANNER_IMG"]);
                    ViewBag.BANNER_IMG= Convert.ToString(dt1.Rows[0]["BANNER_IMG"]);
                    ViewBag.img_QuickFact = ConfigurationManager.AppSettings["HOME"].ToString() + Convert.ToString(dt1.Rows[0]["PATENTED_IMG"]);
                    ViewBag.PATENTED_IMG = Convert.ToString(dt1.Rows[0]["PATENTED_IMG"]);

                }
                return errMsg;
            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + ex.Message + "');");
                return ex.Message;
            }
            finally
            {
                dt1 = null;
            }
        }


        private String ProcessImage(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.BANNER_IMG = Request["hf_BANNER_IMG"];
                return String.Empty;
            }

            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOME"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-A" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.BANNER_IMG = myfile;
                    }
                    else
                    {
                        ViewBag.message = "Please choose only Image file";
                    }


                    return String.Empty;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    ImageAcceptedExtensions = null;
                }
            }
        }
        private String ProcessQuickFactImage(ref String Doc_Name, HttpPostedFileBase file)
        {


            if (file == null)
            {
                ViewBag.QUICK_FACT_IMG = Request["hf_QUICK_FACT_IMG"];
                return String.Empty;
            }

            else
            {

                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOME"].ToString();
                try
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-B" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.PATENTED_IMG= myfile;
                    }
                    else
                    {
                        ViewBag.message = "Please choose only Image file";
                    }
                    return String.Empty;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                finally
                {
                    ImageAcceptedExtensions = null;
                }
            }
        }

        [HttpPost]
        public ActionResult btn_Head_Save_Click(FormCollection form, HttpPostedFileBase fu_Banner, HttpPostedFileBase fu_QuickFact)
        {
            Byte vRef = 0; Int32 vKey = 0;
            String STAT1 = String.Empty, STAT2 = String.Empty, STAT3 = String.Empty;
            EntityHomeSetting oBMAST = null;

            try
            {
                if (ModelState.IsValid)
                {

                    errMsg = ProcessImage(ref STAT1, fu_Banner);
                    if (String.IsNullOrEmpty(errMsg))
                    {
                        errMsg = ProcessQuickFactImage(ref STAT2, fu_QuickFact);
                        if (!String.IsNullOrEmpty(errMsg))
                        {

                            return Redirect("~/HomeSetting/Index");

                        }
                    }
                    errMsg = String.Empty;
                    oBMAST = new EntityHomeSetting();
                    oBMAST.BANNER_HEADING = form["txt_BANNER_HEADING"];
                    oBMAST.BANNER_DESC = form["txt_BANNER_DESC"];
                    oBMAST.BANNER_DESC_1 = form["txt_BANNER_DESC_1"];
                    oBMAST.BANNER_IMG = ViewBag.BANNER_IMG;
                    oBMAST.BANNER_NOTE = form["txt_BANNER_NOTE"];
                    oBMAST.GOOGLEPLAY_LINK = form["txt_GOOGLEPLAY_LINK"];

                    oBMAST.PATENTED_HEADING = form["txt_PATENTED_HEADING"];
                    oBMAST.PATENTED_IMG = ViewBag.PATENTED_IMG;
                    oBMAST.APPSTORE_LINK = form["txt_APPSTORE_LINK"];


                    oBMAST.ENT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.EDIT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.TAG_ACTIVE = 1;
                    oBMAST.TAG_DELETE = 0;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");

                    using (BAHomeSettings oBMC = new BAHomeSettings())
                    {
                        if (oBMAST.ENT_USER_KEY == 0)
                        {
                            vRef = oBMC.SaveChanges<Object, Int32>("INSERT", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                            if (vRef == 1)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveNew + "');");
                                // MessageBox(2, Message.msgSaveNew, "");
                                FillHomeSettingEdit();

                            }
                            else if (vRef == 2)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgValidation + "');");
                                // MessageBox(2, Message.msgValidation, errMsg);
                            }

                            else
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                                // MessageBox(2, Message.msgSaveErr, errMsg);
                            }


                        }
                        else
                        {
                            vRef = oBMC.SaveChanges<Object, Int32>("UPDATE", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                            if (vRef == 1)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveEdit + "');");
                                //MessageBox(2, Message.msgSaveEdit, "");
                                FillHomeSettingEdit();
                            }
                            else if (vRef == 2)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDuplicate + "');");
                                //  MessageBox(2, Message.msgSaveDuplicate, errMsg);
                            }

                            else
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                                // MessageBox(2, Message.msgSaveErr, errMsg);
                            }

                        }


                    }

                    // oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());

                }
                else
                {
                    //    MessageBox(3, errMsg, "");
                }

            }
            catch (Exception ex)
            {

                // MessageBox(2, Message.msgErrCommon, ex.Message);
            }
            finally
            {
                if (oBMAST != null)
                    oBMAST = null;

            }
            return Redirect("~/HomeSetting/Index");
        }


        #endregion

        #region Patent Table
        private DataTable FillQuickFactTableGrid()
        {
            try
            {

                errMsg = String.Empty;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                dt = new DataTable();
                using (BAPatent oBMC = new BAPatent())
                {
                    dt = oBMC.Get<Int32>("GET", 0, "", ref errMsg, "2019", 1);
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                }
                ViewData["dt"] = dt;
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }

        }

        [HttpPost]
        public ActionResult edit(FormCollection form)
        {
            string edit = form[0];

            try
            {
                EntityPatent oBMAST = null;
                errMsg = String.Empty;
                oBMAST = new EntityPatent();
                oBMAST.PATENT_KEY = Convert.ToInt32(edit);
                ViewBag.PATENT_KEY = edit;

                errMsg = FillQuickFactDataEdit(Convert.ToInt32(edit));
                if (String.IsNullOrEmpty(errMsg))
                {
                    dt = FillQuickFactTableGrid();
                    FillHomeSettingEdit();
                    // aPageName.InnerText = Message.modName21 + "(Edit)";
                    // MessageBox(2, "", "");
                }
                else
                {
                    TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrEditPop + "');");
                    //   MessageBox(1, Message.msgErrEditPop, errMsg);
                }

            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                //  MessageBox(1, Message.msgErrCommon, ex.Message);
            }
            return View("Index", dt.Rows);




        }


        private String FillQuickFactDataEdit(Int32 pQUICK_FACT_KEY)
        {
            try
            {
                EntityPatent oBMAST = null;
                errMsg = String.Empty;
                dt = new DataTable();
                using (BAPatent oBMG = new BAPatent())
                {
                    dt = oBMG.Get<Int32>("SRH_KEY", pQUICK_FACT_KEY, "", ref errMsg, "2019", 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.PATENT_DESC = Convert.ToString(dt.Rows[0]["PATENT_DESC"]);

                    // oBMAST.QUICK_FACT_DESC = Convert.ToString(dt.Rows[0]["QUICK_FACT_DESC"]);


                }
                return errMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                dt = null;
            }
        }


        [HttpPost]
        public ActionResult delete(FormCollection form)
        {
            string edit = form[0];
            try
            {

                errMsg = DeleteQuickFact(Convert.ToInt32(edit));

                if (String.IsNullOrEmpty(errMsg))
                {
                    TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDelete + "');");
                    //  MessageBox(1, Message.msgSaveDelete, "");
                    dt = FillQuickFactTableGrid();
                    return Redirect("~/HomeSetting/Index");


                }
                else
                {
                    TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                    //  MessageBox(3, Message.msgErrCommon, errMsg);
                }
            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                // MessageBox(3, Message.msgErrCommon, ex.Message);
            }

            return View("Index", dt);
        }

        private String DeleteQuickFact(Int32 pQUICK_FACT_KEY)
        {
            try
            {
                errMsg = String.Empty;
                Int32 vKey = 0;

                using (BAPatent oBHH = new BAPatent())
                {
                    oBHH.QuickFactDelete<Int32>("DELETE", pQUICK_FACT_KEY, ref vKey, ref errMsg, Convert.ToInt32(Session["USER_KEY"]), null, 1);
                }

                return errMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        public ActionResult btn_QuickFactTable_Save_Click(FormCollection form)
        {
            Byte vRef = 0; Int32 vKey = 0;
            String STAT1 = String.Empty;
            EntityPatent oBMAST = null;

            try
            {
                if (ModelState.IsValid)
                {


                    //errMsg = ProcessQuickFactTableImage(ref STAT1, fu_Quick_Image);
                    if (!String.IsNullOrEmpty(errMsg))
                    {
                        return Redirect("~/HomeSetting/Index");
                    }

                    errMsg = String.Empty;
                    oBMAST = new EntityPatent();

                    oBMAST.PATENT_KEY = Convert.ToInt32(ViewBag.PATENT_KEY);



                    oBMAST.PATENT_DESC = form["txt_PATENT_DESC"].ToString();
                    // oBMAST.QUICK_FACT_IMG = hf_QUICK_FACT_TABLE_IMG.Value;


                    oBMAST.ENT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.EDIT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.TAG_ACTIVE = 1;
                    oBMAST.TAG_DELETE = 0;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");

                    using (BAPatent oBMC = new BAPatent())
                    {
                        if (form["hf_PATENT_KEY"].ToString() == "0")
                        {
                            vRef = oBMC.SaveChanges<Object, Int32>("INSERT", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                            if (vRef == 1)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveNew + "');");
                                // MessageBox(2, Message.msgSaveNew, "");
                                FillQuickFactTableGrid();
                                return Redirect("~/HomeSetting/Index");
                            }
                            else if (vRef == 2)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDuplicate + "');");
                                return Redirect("~/HomeSetting/Index");
                                //  MessageBox(2, Message.msgSaveDuplicate, errMsg);
                            }

                            else
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                                return Redirect("~/HomeSetting/Index");
                                //  MessageBox(2, Message.msgSaveErr, errMsg);
                            }

                        }
                        else
                        {
                            vRef = oBMC.SaveChanges<Object, Int32>("UPDATE", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                            if (vRef == 1)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveEdit + "');");
                                // MessageBox(2, Message.msgSaveEdit, "");
                                FillQuickFactTableGrid();
                                return Redirect("~/HomeSetting/Index");

                            }
                            else if (vRef == 2)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDuplicate + "');");
                                return Redirect("~/HomeSetting/Index");
                                //MessageBox(2, Message.msgSaveDuplicate, errMsg);
                            }

                            else
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                                return Redirect("~/HomeSetting/Index");
                                //MessageBox(2, Message.msgSaveErr, errMsg);
                            }

                        }


                    }

                    //  oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
                    //ClearControl();
                }
                else
                {
                    //MessageBox(3, errMsg, "");
                }

                return Redirect("~/HomeSetting/Index");

            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                return Redirect("~/HomeSetting/Index");
                //  MessageBox(2, Message.msgErrCommon, ex.Message);
            }
            finally
            {
                if (oBMAST != null)
                    oBMAST = null;
            }
        }

        #endregion

        #endregion homepage
    }
}