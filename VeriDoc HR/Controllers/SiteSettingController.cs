using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyApp.db;
using MyApp.db.MyAppBal;
using MyApp.Entity;

namespace VeriDoc_HR.Controllers
{
    public class SiteSettingController : Controller
    {
        // GET: SiteSetting
        EntitySysUser oSysUser = null;
        String errMsg = String.Empty;
        DataTable dt;
        DataTable dt1;
        public ActionResult Index()
        {
            try
            {
                if (Session["oSysUser"] != null)
                {
                    oSysUser = (EntitySysUser)Session["oSysUser"];
                    oSysUser.USER_KEY = Convert.ToInt32(Session["USER_KEY"]);

                    oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
                    errMsg = String.Empty;
                    FillSiteSettingEdit();
                    errMsg = FillDdPageName();
                    if (!String.IsNullOrEmpty(errMsg))
                    {
                        //  MessageBox(1, "Page Name" + Message.msgErrDdlPop, errMsg);
                    }
                    dt = FillPageSettingGrid();
                    ViewBag.hf_DTLS_PAGE_SETTING_KEY = "0";



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

        private String FillSiteSettingEdit()
        {
            try
            {
                String vCOMPANY_ACCESS = String.Empty;
                errMsg = String.Empty;
                dt = new DataTable();
                using (BaSiteSetting oBME = new BaSiteSetting())
                {
                    dt = oBME.GetData("ALL", "", ref errMsg, null, 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.txt_CONTACT_NO = Convert.ToString(dt.Rows[0]["CONTACT_NO"]);
                    ViewBag.txt_MAIL = Convert.ToString(dt.Rows[0]["MAIL"]);
                    ViewBag.txt_ADDRESS = Convert.ToString(dt.Rows[0]["ADDRESS"]);
                    ViewBag.img_Logo = ConfigurationManager.AppSettings["HOME"].ToString() + Convert.ToString(dt.Rows[0]["LOGO_NAME"]);
                    ViewBag.hf_LOGO_NAME = Convert.ToString(dt.Rows[0]["LOGO_NAME"]);
                    ViewBag.txt_FOOTER_NOTE = Convert.ToString(dt.Rows[0]["FOOTER_NOTE"]);

                    ViewBag.txt_FACEBOOK_LINK = Convert.ToString(dt.Rows[0]["FACEBOOK_LINK"]);
                    ViewBag.txt_LINKEDIN_LINK = Convert.ToString(dt.Rows[0]["LINKEDIN_LINK"]);
                    ViewBag.txt_TWITTER_LINK = Convert.ToString(dt.Rows[0]["TWITTER_LINK"]);
                    ViewBag.txt_INSTAGRAM_LINK = Convert.ToString(dt.Rows[0]["INSTAGRAM_LINK"]);
                    ViewBag.txt_TELEGRAM_LINK = Convert.ToString(dt.Rows[0]["TELEGRAM_LINK"]);
                    ViewBag.txt_PRINTEREST_LINK = Convert.ToString(dt.Rows[0]["PRINTEREST_LINK"]);
                    ViewBag.txt_MEDIUM_LINK = Convert.ToString(dt.Rows[0]["MEDIUM_LINK"]);
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
                dt = null;
            }
        }


        private String FillDdPageName()
        {
            try
            {
                errMsg = String.Empty;

                using (BaSiteSetting oBMC = new BaSiteSetting())
                {
                    dt = oBMC.BindDdl(0, ref errMsg, null, 0);

                }
                List<EntitySiteSetting> page = new List<EntitySiteSetting>();
                if (dt.Rows.Count > 0)
                {
                    EntitySiteSetting oBmast = new EntitySiteSetting();
                    oBmast.MAST_PAGE_KEY = 0;
                    oBmast.PAGE_NAME = "-SELECT AN OPTION-";
                    page.Add(oBmast);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        oBmast = new EntitySiteSetting();
                        oBmast.MAST_PAGE_KEY = Convert.ToInt32(dt.Rows[i]["MAST_PAGE_KEY"]);
                        oBmast.PAGE_NAME = dt.Rows[i]["PAGE_NAME"].ToString();

                        page.Add(oBmast);

                    }

                    var getpagename = page.ToList();

                    SelectList list = new SelectList(getpagename, "MAST_PAGE_KEY", "PAGE_NAME", ViewBag.ddl_MAST_PAGE_KEY);
                    ViewBag.PageName = list;

                }
                return errMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private DataTable FillPageSettingGrid()
        {
            try
            {
                errMsg = String.Empty;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                dt = new DataTable();
                using (BaSiteSetting oBMC = new BaSiteSetting())
                {
                    dt = oBMC.GetPageSetting<Int32>("GET", 0, "", ref errMsg, "2019", 1);
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                }

            }
            catch (Exception ex)
            {

            }

            return dt;
        }


        [HttpPost]
        public ActionResult edit(FormCollection form)
        {
            string edit = form[0];

            try
            {
                EntitySiteSetting oBMAST = null;
                errMsg = String.Empty;
                oBMAST = new EntitySiteSetting();
                oBMAST.DTLS_PAGE_SETTING_KEY = Convert.ToInt32(edit);
                ViewBag.hf_DTLS_PAGE_SETTING_KEY = edit;

                errMsg = FillPageSettingEdit(Convert.ToInt32(edit));
                if (String.IsNullOrEmpty(errMsg))
                {
                    FillSiteSettingEdit();
                    errMsg = FillDdPageName();
                    if (!String.IsNullOrEmpty(errMsg))
                    {
                        TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrDdlPop + "');");
                        //  MessageBox(1, "Page Name" + Message.msgErrDdlPop, errMsg);
                    }
                    dt = FillPageSettingGrid();

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


        private String FillPageSettingEdit(Int32 pDTLS_PAGE_SETTING_KEY)
        {
            try
            {
                errMsg = String.Empty;
                dt = new DataTable();
                using (BaSiteSetting oBMG = new BaSiteSetting())
                {
                    dt = oBMG.GetPageSetting<Int32>("SRH_KEY", pDTLS_PAGE_SETTING_KEY, "", ref errMsg, "2019", 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    //  ddl_MAST_PAGE_KEY.SelectedIndex = ddl_MAST_PAGE_KEY.Items.IndexOf(ddl_MAST_PAGE_KEY.Items.FindByValue(Convert.ToString(dt.Rows[0]["MAST_PAGE_KEY"])));

                    ViewBag.ddl_MAST_PAGE_KEY = Convert.ToString(dt.Rows[0]["MAST_PAGE_KEY"]);
                    ViewBag.txt_PAGE_TITLE = Convert.ToString(dt.Rows[0]["PAGE_TITLE"]);
                    ViewBag.txt_META_DESCRIPTION = Convert.ToString(dt.Rows[0]["META_DESCRIPTION"]);
                    ViewBag.txt_META_KEYWORD = Convert.ToString(dt.Rows[0]["META_KEYWORD"]);

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
        public ActionResult btn_Page_Settings_Save_Click(FormCollection form)
        {
            Byte vRef = 0; Int32 vKey = 0;
            EntitySiteSetting oBMAST = null;
            try
            {
                if (ModelState.IsValid)
                {
                    errMsg = String.Empty;
                    oBMAST = new EntitySiteSetting();
                    oBMAST.DTLS_PAGE_SETTING_KEY = Convert.ToInt32(form["hf_DTLS_PAGE_SETTING_KEY"]);
                    oBMAST.MAST_PAGE_KEY = Convert.ToInt32(form["ddl_MAST_PAGE_KEY"]);
                    oBMAST.PAGE_TITLE = form["txt_PAGE_TITLE"];
                    oBMAST.META_DESCRIPTION = form["txt_META_DESCRIPTION"];
                    oBMAST.META_KEYWORD = form["txt_META_KEYWORD"];

                    oBMAST.ENT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.EDIT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.TAG_ACTIVE = 0;
                    oBMAST.TAG_DELETE = 0;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");

                    using (BaSiteSetting oBMC = new BaSiteSetting())
                    {
                        if (oBMAST.DTLS_PAGE_SETTING_KEY == 0)
                        {
                            vRef = oBMC.SavePageSetting<Object, Int32>("INSERT", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                            if (vRef == 1)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveNew + "');");
                                //MessageBox(2, Message.msgSaveNew, "");
                                //FillPageSettingGrid();
                                //ClearPageSetting();
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
                        else
                        {
                            vRef = oBMC.SavePageSetting<Object, Int32>("UPDATE", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                            if (vRef == 1)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveEdit + "');");
                                //  FillPageSettingGrid();
                                //ClearPageSetting();
                                //MessageBox(1, Message.msgSaveEdit, "");
                            }
                            else if (vRef == 2)
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDuplicate + "');");
                                //  MessageBox(1, Message.msgSaveDuplicate, errMsg);
                            }

                            else
                            {
                                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                                //  MessageBox(1, Message.msgSaveErr, errMsg);
                            }

                        }
                    }
                }
                // oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                //   MessageBox(2, Message.msgErrCommon, ex.Message);
            }
            finally
            {
                if (oBMAST != null)
                    oBMAST = null;
            }

            return Redirect("~/SiteSetting/Index");

        }


        public ActionResult btn_Head_Save_Click(FormCollection form, HttpPostedFileBase fu_Logo)
        {
            Byte vRef = 0; Int32 vKey = 0;
            String LOGO_NAME = String.Empty;
            EntitySiteSetting oBMAST = null;
            String FOOTER_LOGO = String.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    errMsg = ProcessLogo(ref LOGO_NAME, fu_Logo);
                    if (!String.IsNullOrEmpty(errMsg))
                    {
                        // MessageBox(3, errMsg, "");
                        //return;
                    }
                    errMsg = String.Empty;
                    oBMAST = new EntitySiteSetting();
                    oBMAST.CONTACT_NO = form["txt_CONTACT_NO"];
                    oBMAST.MAIL = form["txt_MAIL"];

                    oBMAST.FACEBOOK_LINK = form["txt_FACEBOOK_LINK"];
                    oBMAST.TWITTER_LINK = form["txt_TWITTER_LINK"];
                    oBMAST.LINKEDIN_LINK = form["txt_LINKEDIN_LINK"];
                    oBMAST.INSTAGRAM_LINK = form["txt_INSTAGRAM_LINK"];
                    oBMAST.TELEGRAM_LINK = form["txt_TELEGRAM_LINK"];
                    oBMAST.PRINTEREST_LINK = form["txt_PRINTEREST_LINK"];
                    oBMAST.MEDIUM_LINK = form["txt_MEDIUM_LINK"];

                    oBMAST.ADDRESS = form["txt_ADDRESS"];
                    oBMAST.LOGO_NAME = ViewBag.hf_LOGO_NAME;
                    oBMAST.FOOTER_NOTE = form["txt_FOOTER_NOTE"];

                    oBMAST.ENT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.EDIT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.TAG_ACTIVE = 1;
                    oBMAST.TAG_DELETE = 0;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");

                    using (BaSiteSetting oBMC = new BaSiteSetting())
                    {
                        vRef = oBMC.SaveChanges<Object, Int32>("UPDATE", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                        if (vRef == 1)
                        {
                            TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveEdit + "');");
                            //MessageBox(2, Message.msgSaveEdit, "");
                            //FillSiteSettingEdit();
                        }
                        else if (vRef == 2)
                        {
                            TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDuplicate + "');");
                            //  MessageBox(2, Message.msgSaveDuplicate, errMsg);
                        }

                        else
                        {
                            TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                            //   MessageBox(2, Message.msgSaveErr, errMsg);
                        }

                    }
                }
                // oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                //   MessageBox(2, Message.msgErrCommon, ex.Message);
            }
            finally
            {
                if (oBMAST != null)
                    oBMAST = null;
            }

            return Redirect("~/SiteSetting/Index");
        }

        private String ProcessLogo(ref String Doc_Name, HttpPostedFileBase file)
        {


            if (file == null)
            {
                ViewBag.hf_LOGO_NAME = Request["hf_LOGO_NAME"];
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
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ext; //appending the name with id  
                                                                                               // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_LOGO_NAME = myfile;
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
    }
}