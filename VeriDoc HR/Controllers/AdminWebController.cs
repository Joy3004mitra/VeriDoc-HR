using MyApp.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyApp.db;
using MyApp.db.MyAppBal;
using System.IO;

namespace VeriDoc_HR.Controllers
{
    public class AdminWebController : Controller
    {
        // GET: AdminWeb
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
                    FillHowItWorkAdminEdit();

                    // errMsg = FillHowItWorkAdminEdit();
                    if (!String.IsNullOrEmpty(errMsg))
                    {

                        //   MessageBox(1, "Page Name" + Message.msgErrDdlPop, errMsg);
                    }
                    return View();

                }
                else
                {
                    return Redirect("~/Admin/Index");
                }
            }
            catch (Exception ex)
            {
                //  MessageBox(1, Message.msgErrCommon, ex.Message);
            }
            return View();
        }

        private String FillHowItWorkAdminEdit()
        {
            try
            {
                String vCOMPANY_ACCESS = String.Empty;
                errMsg = String.Empty;
                dt = new DataTable();
                using (BaHowitWorkAdminWeb oBME = new BaHowitWorkAdminWeb())
                {
                    dt = oBME.GetData("ALL", "", ref errMsg, null, 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.txt_HEADING_ONE = Convert.ToString(dt.Rows[0]["HEADING_ONE"]);
                    ViewBag.txt_DESC_ONE = Convert.ToString(dt.Rows[0]["DESC_ONE"]);

                    ViewBag.txt_HEADING_TWO = Convert.ToString(dt.Rows[0]["HEADING_TWO"]);
                    ViewBag.txt_DESC_TWO = Convert.ToString(dt.Rows[0]["DESC_TWO"]);

                    ViewBag.txt_HEADING_THREE = Convert.ToString(dt.Rows[0]["HEADING_THREE"]);
                    ViewBag.txt_DESC_THREE = Convert.ToString(dt.Rows[0]["DESC_THREE"]);

                    ViewBag.txt_HEADING_FOUR = Convert.ToString(dt.Rows[0]["HEADING_FOUR"]);
                    ViewBag.txt_DESC_FOUR = Convert.ToString(dt.Rows[0]["DESC_FOUR"]);

                    ViewBag.txt_HEADING_FIVE = Convert.ToString(dt.Rows[0]["HEADING_FIVE"]);
                    ViewBag.txt_DESC_FIVE = Convert.ToString(dt.Rows[0]["DESC_FIVE"]);

                    ViewBag.txt_HEADING_SIX = Convert.ToString(dt.Rows[0]["HEADING_SIX"]);
                    ViewBag.txt_DESC_SIX = Convert.ToString(dt.Rows[0]["DESC_SIX"]);


                    ViewBag.getvideo = "<iframe id='source' src='" + ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_ONE"]) + "' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope;'></iframe>";
                    ViewBag.hf_IMG_1 = Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);

                    ViewBag.getvideo_2 = "<iframe id='source' src='" + ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_TWO"]) + "' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope;'></iframe>";
                    ViewBag.hf_IMG_2 = Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);

                    ViewBag.getvideo_3 = "<iframe id='source' src='" + ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_THREE"]) + "' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope;'></iframe>";
                    ViewBag.hf_IMG_3 = Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);

                    ViewBag.getvideo_4 = "<iframe id='source' src='" + ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]) + "' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope;'></iframe>";
                    ViewBag.hf_IMG_4 = Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);

                    ViewBag.getvideo_5 = "<iframe id='source' src='" + ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FIVE"]) + "' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope;'></iframe>";
                    ViewBag.hf_IMG_5 = Convert.ToString(dt.Rows[0]["IMAGE_FIVE"]);

                    ViewBag.getvideo_6 = "<iframe id='source' src='" + ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_SIX"]) + "' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope;'></iframe>";
                    ViewBag.hf_IMG_6 = Convert.ToString(dt.Rows[0]["IMAGE_SIX"]);

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

        [HttpPost]
        public ActionResult btn_Head_Save_Click(FormCollection form, HttpPostedFileBase fu_Image_1, HttpPostedFileBase fu_Image_2, HttpPostedFileBase fu_Image_3, HttpPostedFileBase fu_Image_4, HttpPostedFileBase fu_Image_5, HttpPostedFileBase fu_Image_6)
        {
            Byte vRef = 0; Int32 vKey = 0;
            String LOGO_NAME_1 = String.Empty, LOGO_NAME_2 = String.Empty, LOGO_NAME_3 = String.Empty, LOGO_NAME_4 = String.Empty, LOGO_NAME_5 = String.Empty, LOGO_NAME_6 = String.Empty;
            String values = String.Empty, values_2 = String.Empty, values_3 = String.Empty, values_4 = String.Empty, values_5 = String.Empty, values_6 = String.Empty;
            EntityHowItWorkAdminWeb oBMAST = null;
            String FOOTER_LOGO = String.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    errMsg = ProcessImage1(ref LOGO_NAME_1, fu_Image_1);
                    if (String.IsNullOrEmpty(errMsg))
                    {
                        errMsg = ProcessImage2(ref LOGO_NAME_2, fu_Image_2);
                        if (String.IsNullOrEmpty(errMsg))
                        {
                            errMsg = ProcessImage3(ref LOGO_NAME_3, fu_Image_3);
                            if (String.IsNullOrEmpty(errMsg))
                            {
                                errMsg = ProcessImage4(ref LOGO_NAME_4, fu_Image_4);
                                if (String.IsNullOrEmpty(errMsg))
                                {
                                    errMsg = ProcessImage5(ref LOGO_NAME_5, fu_Image_5);
                                    if (String.IsNullOrEmpty(errMsg))
                                    {
                                        errMsg = ProcessImage6(ref LOGO_NAME_6, fu_Image_6);
                                        if (!String.IsNullOrEmpty(errMsg))
                                        {
                                            
                                        }
                                    }
                                }
                            }
                        }

                    }

                    if (ViewBag.hf_IMG_1 == "")
                    {
                        return Redirect("~/AdminApp/Index");
                    }
                    else
                    {
                        values = ViewBag.hf_IMG_1;
                    }
                    if (ViewBag.hf_IMG_2 == "")
                    {
                        return Redirect("~/AdminApp/Index");
                    }
                    else
                    {
                        values_2 = ViewBag.hf_IMG_2;
                    }
                    if (ViewBag.hf_IMG_3 == "")
                    {
                        return Redirect("~/AdminApp/Index");
                    }
                    else
                    {
                        values_3 = ViewBag.hf_IMG_3;
                    }
                    if (ViewBag.hf_IMG_4 == "")
                    {
                        return Redirect("~/AdminApp/Index");
                    }
                    else
                    {
                        values_4 = ViewBag.hf_IMG_4;
                    }
                    if (ViewBag.hf_IMG_5 == "")
                    {
                        return Redirect("~/AdminApp/Index");
                    }
                    else
                    {
                        values_5 = ViewBag.hf_IMG_5;
                    }
                    if (ViewBag.hf_IMG_6 == "")
                    {
                        return Redirect("~/AdminApp/Index");
                    }
                    else
                    {
                        values_6 = ViewBag.hf_IMG_6;
                    }

                    errMsg = String.Empty;
                    oBMAST = new EntityHowItWorkAdminWeb();
                    oBMAST.HEADING_ONE = form["txt_HEADING_ONE"];
                    oBMAST.DESC_ONE = form["txt_DESC_ONE"];

                    oBMAST.HEADING_TWO = form["txt_HEADING_TWO"];
                    oBMAST.DESC_TWO = form["txt_DESC_TWO"];

                    oBMAST.HEADING_THREE = form["txt_HEADING_THREE"];
                    oBMAST.DESC_THREE = form["txt_DESC_THREE"];

                    oBMAST.HEADING_FOUR = form["txt_HEADING_FOUR"];
                    oBMAST.DESC_FOUR = form["txt_DESC_FOUR"];

                    oBMAST.HEADING_FIVE = form["txt_HEADING_FIVE"];
                    oBMAST.DESC_FIVE = form["txt_DESC_FIVE"];

                    oBMAST.HEADING_SIX = form["txt_HEADING_SIX"];
                    oBMAST.DESC_SIX = form["txt_DESC_SIX"];


                    oBMAST.IMAGE_ONE = values;
                    oBMAST.IMAGE_TWO = values_2;
                    oBMAST.IMAGE_THREE = values_3;
                    oBMAST.IMAGE_FOUR = values_4;
                    oBMAST.IMAGE_FIVE = values_5;
                    oBMAST.IMAGE_SIX = values_6;

                    oBMAST.ENT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.EDIT_USER_KEY = Convert.ToInt32(Session["USER_KEY"]);
                    oBMAST.TAG_ACTIVE = 1;
                    oBMAST.TAG_DELETE = 0;
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");

                    using (BaHowitWorkAdminWeb oBMC = new BaHowitWorkAdminWeb())
                    {
                        vRef = oBMC.SaveChanges<Object, Int32>("UPDATE", oBMAST, null, ref vKey, ref errMsg, "2019", 1);
                        if (vRef == 1)
                        {
                            TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveEdit + "');");
                            // MessageBox(2, Message.msgSaveEdit, "");
                             FillHowItWorkAdminEdit();
                        }
                        else if (vRef == 2)
                        {
                            TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveDuplicate + "');");
                            // MessageBox(2, Message.msgSaveDuplicate, errMsg);
                        }

                        else
                        {
                            TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgSaveErr + "');");
                            //  MessageBox(2, Message.msgSaveErr, errMsg);
                        }

                    }
                }
                //oSysUser.TAG_PAGE_REFRESH = Server.UrlEncode(System.DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                TempData["JavaScriptFunction"] = string.Format("OpenTab1('" + Message.msgErrCommon + "');");
                //MessageBox(2, Message.msgErrCommon, ex.Message);
            }
            finally
            {
                if (oBMAST != null)
                    oBMAST = null;
            }

            return Redirect("~/AdminWeb/Index");
        }
        private String ProcessImage1(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.hf_IMG_1 = Request["hf_IMG_1"];
                return String.Empty;
            }
            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOWITWORK"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg", ".webm", ".mp4" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-AW1" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_IMG_1 = myfile;
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
        private String ProcessImage2(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.hf_IMG_2 = Request["hf_IMG_2"];
                return String.Empty;
            }
            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOWITWORK"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg", ".webm", ".mp4" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-AW2" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_IMG_2 = myfile;
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
        private String ProcessImage3(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.hf_IMG_3 = Request["hf_IMG_3"];
                return String.Empty;
            }
            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOWITWORK"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg", ".webm", ".mp4" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-AW3" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_IMG_3 = myfile;
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
        private String ProcessImage4(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.hf_IMG_4 = Request["hf_IMG_4"];
                return String.Empty;

            }
            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOWITWORK"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg", ".webm", ".mp4" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-AW4" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_IMG_4 = myfile;
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

        private String ProcessImage5(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.hf_IMG_5 = Request["hf_IMG_5"];
                return String.Empty;

            }
            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOWITWORK"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg", ".webm", ".mp4" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-AW5" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_IMG_5 = myfile;
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

        private String ProcessImage6(ref String Doc_Name, HttpPostedFileBase file)
        {

            if (file == null)
            {
                ViewBag.hf_IMG_6 = Request["hf_IMG_6"];
                return String.Empty;

            }
            else
            {


                String[] ImageAcceptedExtensions = null;
                String DOC_PATH = "~" + ConfigurationManager.AppSettings["HOWITWORK"].ToString();
                try
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".svg", ".webm", ".mp4" };


                    var Image_url = file.ToString(); //getting complete url
                    var fileName = Path.GetFileName(file.FileName); //getting only file name(ex-ganesh.jpg)  
                    var ext = Path.GetExtension(file.FileName); //getting the extension(ex-.jpg)  
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        string name = Path.GetFileNameWithoutExtension(fileName); //getting file name without extension  
                        string myfile = System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + "-AW6" + ext; //appending the name with id  
                                                                                                      // store the file inside ~/project folder(Img)  
                        var path = Server.MapPath(DOC_PATH + myfile);
                        Image_url = path;

                        file.SaveAs(path);

                        ViewBag.hf_IMG_6 = myfile;
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