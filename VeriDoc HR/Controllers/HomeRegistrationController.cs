using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using MyApp.db.MyAppBal;
using System.Net.Mail;
using System.Net;
using MyApp.db.Encryption;
using System.IO;
using MyApp.Entity;

namespace VeriDoc_HR.Controllers
{
    public class HomeRegistrationController : Controller
    {
        // GET: HomeRegistration

        DataTable dt;
        String errMsg = String.Empty;
        String vString = String.Empty, vString1 = String.Empty, vString2 = String.Empty, vString3 = String.Empty, vString4 = String.Empty;

        public ActionResult Index()
        {
            FillPageSettingGrid();
            FillSiteSettingEdit();
            BannerPortion();
            FillDdPackage();
            return View();
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
                    dt = oBMC.GetPageSetting<Int32>("SRH_HEAD_KEY", 1, "", ref errMsg, "2020", 1);
                    // System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.Title = dt.Rows[0]["PAGE_TITLE"].ToString();
                    ViewBag.MetaDescription = dt.Rows[0]["META_DESCRIPTION"].ToString();
                    ViewBag.MetaKeywords = dt.Rows[0]["META_KEYWORD"].ToString();
                }

            }
            catch (Exception ex)
            {

            }

            return dt;
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
                    ViewBag.CONTACT_NO = Convert.ToString(dt.Rows[0]["CONTACT_NO"]);
                    ViewBag.MAIL = Convert.ToString(dt.Rows[0]["MAIL"]);
                    ViewBag.ADDRESS = Convert.ToString(dt.Rows[0]["ADDRESS"]);
                    ViewBag.img_Logo = ConfigurationManager.AppSettings["HOME"].ToString() + Convert.ToString(dt.Rows[0]["LOGO_NAME"]);
                    //ViewBag.hf_LOGO_NAME = Convert.ToString(dt.Rows[0]["LOGO_NAME"]);
                    ViewBag.FOOTER_NOTE = Convert.ToString(dt.Rows[0]["FOOTER_NOTE"]);

                    ViewBag.FACEBOOK_LINK = Convert.ToString(dt.Rows[0]["FACEBOOK_LINK"]);
                    ViewBag.LINKEDIN_LINK = Convert.ToString(dt.Rows[0]["LINKEDIN_LINK"]);
                    ViewBag.TWITTER_LINK = Convert.ToString(dt.Rows[0]["TWITTER_LINK"]);
                    ViewBag.INSTAGRAM_LINK = Convert.ToString(dt.Rows[0]["INSTAGRAM_LINK"]);
                    ViewBag.TELEGRAM_LINK = Convert.ToString(dt.Rows[0]["TELEGRAM_LINK"]);
                    ViewBag.PRINTEREST_LINK = Convert.ToString(dt.Rows[0]["PRINTEREST_LINK"]);
                    ViewBag.MEDIUM_LINK = Convert.ToString(dt.Rows[0]["MEDIUM_LINK"]);
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

        private DataTable BannerPortion()
        {
            try
            {
                String vCOMPANY_ACCESS = String.Empty;
                errMsg = String.Empty;
                dt = new DataTable();
                using (BAHomeSettings oBME = new BAHomeSettings())
                {
                    dt = oBME.GetData("GET", "", ref errMsg, null, 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.GOOGLEPLAY_LINK = Convert.ToString(dt.Rows[0]["GOOGLEPLAY_LINK"]);
                    
                    ViewBag.APPSTORE_LINK = Convert.ToString(dt.Rows[0]["APPSTORE_LINK"]);
                }
                // return errMsg;
            }
            catch (Exception ex)
            {
                //  return ex.Message;
            }
            finally
            {
                dt = null;
            }

            return dt;

        }

        
        [HttpPost]
        public ActionResult btn_submit_Click(FormCollection form)
        {
            EntityRegister oBMST = null;
            Byte vRef = 0; Int32 vKey = 0;
            errMsg = String.Empty;
            DataTable dt4 = new DataTable();
            //Int32 pricingkey = Convert.ToInt32(Session["price"]);
            EntityPricing oBMSTP = new EntityPricing();
            try
            {
                oBMST = new EntityRegister();

                oBMST.DTLS_REGISTER_KEY = Convert.ToInt32(ViewBag.hf_DTLS_REGISTER_KEY);
                oBMST.FIRSTNAME = form["txt_FistName"];
                oBMST.LASTNAME = form["txt_LastName"];
                oBMST.EMAIL = form["txt_Eamil"];
                oBMST.PHONE = form["txt_Phone"];
                oBMST.COMPANY_NAME = form["txt_Company_Name"];
                oBMST.COMPANY_ADDRESS = form["txt_Company_Address"];
                oBMST.COUNTRY = form["Country"];
                oBMST.STATE = form["txt_State"];
                oBMST.CITY = form["txt_City"];
                oBMST.ZIP = form["txt_ZIP"];
                oBMST.DTLS_PACKAGE_KEY = Convert.ToInt32(form["ddl_DTLS_PACKAGE_KEY"]);
                //oBMST.DTLS_PACKAGE_KEY = 1;

                oBMST.ALTER_COMPANY_NAME = form["txt_Alter_Company_Name"];
                oBMST.ALTER_COMPANY_ADDRESS = form["txt_Alter_Company_Address"];
                oBMST.ALTER_COUNTRY = form["AlterCountry"];
                oBMST.ALTER_STATE = form["txt_Alter_State"];
                oBMST.ALTER_CITY = form["txt_Alter_City"];
                oBMST.ALTER_ZIP = form["txt_Alter_ZIP"];

                oBMST.ENT_USER_KEY = oBMST.DTLS_REGISTER_KEY;
                oBMST.EDIT_USER_KEY = oBMST.DTLS_REGISTER_KEY;
                oBMST.TAG_ACTIVE = 1;
                oBMST.TAG_DELETE = 0;

                Session["FIRSTNAME"] = oBMST.FIRSTNAME;
                Session["LASTNAME"] = oBMST.LASTNAME;
                Session["EMAIL"] = oBMST.EMAIL;
                Session["PHONE"] = oBMST.PHONE;
                Session["COMPANY_NAME"] = oBMST.COMPANY_NAME;
                Session["COMPANY_ADDRESS"] = oBMST.COMPANY_ADDRESS;
                Session["COUNTRY"] = oBMST.COUNTRY;
                Session["STATE"] = oBMST.STATE;
                Session["CITY"] = oBMST.CITY;
                Session["ZIP"] = oBMST.ZIP;
                Session["COUNTRY"] = oBMST.COUNTRY;

                using (BaRegister oBDT = new BaRegister())
                {

                    if (oBMST.DTLS_REGISTER_KEY == 0)
                    {
                        if (form["gender"] == "yes")
                        {
                            if (form["txt_FistName"] != "" && form["txt_LastName"] != "" && form["txt_Eamil"] != "" && form["txt_Phone"] != "")
                            {
                                if (form["Country"] != "" && form["txt_Company_Name"] != "" && form["txt_Company_Address"] != "")
                                {
                                    vRef = oBDT.SaveChanges<Object, Int32>("INSERT", oBMST, null, ref vKey, ref errMsg, "2019", 1);
                                    if (vRef == 1)
                                    {
                                        //MessageBox(2, Message.msgSaveNew, "");
                                        //ClientScript.RegisterStartupScript(GetType(), "popup", "popup();", true);
                                        //Response.Redirect("/Index/Index", false);
                                        using (BaPricing oBDTP = new BaPricing())
                                        {

                                            dt4 = oBDTP.Get<Int32>("SRH_KEY", oBMST.DTLS_PACKAGE_KEY, "", ref errMsg, "2019", 1);
                                            oBMSTP.PACKAGE_NAME = Convert.ToString(dt4.Rows[0]["PACKAGE_NAME"]);
                                        }
                                        if (oBMSTP.PACKAGE_NAME == "FREE TRIAL")
                                        {
                                            Session["popup"] = "1";
                                            Response.Redirect("/Index/Index", false);

                                        }
                                        else if (oBMSTP.PACKAGE_NAME == "PRO")
                                        {
                                            Response.Redirect("#", false);

                                        }
                                        else if (oBMSTP.PACKAGE_NAME == "STANDARD")
                                        {
                                            Response.Redirect("#", false);

                                        }

                                        send(form["txt_Eamil"].ToString());
                                        ClearControl(form);
                                    }
                                }
                                else
                                {
                                    //ClientScript.RegisterStartupScript(GetType(), "popuperror", "popuperror();", true);
                                    // //ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Please Fill All * mark filed ');", true);
                                }
                            }
                            else
                            {
                                //ClientScript.RegisterStartupScript(GetType(), "popuperror", "popuperror();", true);
                                ////ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Please Fill All * mark filed ');", true);
                            }
                        }
                        else if (form["gender"] == "no")
                        {
                            if (form["txt_Alter_Company_Name"] != "" && form["txt_Alter_Company_Address"] != "" && form["txt_Alter_City"] != "")
                            {
                                if (form["Country"] != "")
                                {
                                    vRef = oBDT.SaveChanges<Object, Int32>("INSERT", oBMST, null, ref vKey, ref errMsg, "2019", 1);
                                    if (vRef == 1)
                                    {
                                        //MessageBox(2, Message.msgSaveNew, "");
                                        //ClientScript.RegisterStartupScript(GetType(), "popup", "popup();", true);

                                        using (BaPricing oBDTP = new BaPricing())
                                        {

                                            dt4 = oBDTP.Get<Int32>("SRH_KEY", oBMST.DTLS_PACKAGE_KEY, "", ref errMsg, "2019", 1);
                                            oBMSTP.PACKAGE_NAME = Convert.ToString(dt4.Rows[0]["PACKAGE_NAME"]);
                                        }
                                        if (oBMSTP.PACKAGE_NAME == "FREE TRIAL")
                                        {
                                            Session["popup"] = "1";
                                            Response.Redirect("/Index/Index", false);

                                        }
                                        else if (oBMSTP.PACKAGE_NAME == "PRO")
                                        {
                                            Response.Redirect("#", false);

                                        }
                                        else if (oBMSTP.PACKAGE_NAME == "STANDARD")
                                        {
                                            Response.Redirect("#", false);

                                        }

                                        send(form["txt_Eamil"].ToString());
                                        ClearControl(form);
                                    }
                                }
                                else
                                {
                                    //ClientScript.RegisterStartupScript(GetType(), "popuperror", "popuperror();", true);
                                    // //ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Please Fill All * mark filed ');", true);
                                }
                            }
                            else
                            {
                                //ClientScript.RegisterStartupScript(GetType(), "popuperror", "popuperror();", true);
                                ////ClientScript.RegisterStartupScript(GetType(), "alert", "alert('Please Fill All * mark filed ');", true);

                            }
                        }
                        else if (vRef == 2)
                        {
                            //MessageBox(2, Message.msgValidation, errMsg);
                            //ClientScript.RegisterStartupScript(GetType(), "popuperror", "popuperror();", true);
                        }
                        else
                        {
                            //MessageBox(2, Message.msgSaveErr, errMsg);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // //ClientScript.RegisterStartupScript(GetType(), "popupservererror", "popupservererror(); console.log('" + ex.Message + "');", true);


            }

            return Redirect("~/Register/Index");
        }



        private String FillDdPackage()
        {
            try
            {
                errMsg = String.Empty;
                DataTable dt2 = new DataTable();
                using (BaPricing oBDTP = new BaPricing())
                {
                    dt2 = oBDTP.GetData("GET", "", ref errMsg, "2019", 1);
                }
                List<EntityPricing> page = new List<EntityPricing>();
                if (dt2.Rows.Count > 0)
                {
                    EntityPricing oBmast = new EntityPricing();
                    oBmast.DTLS_PACKAGE_KEY = 0;
                    oBmast.PACKAGE_DESC = "-- Select Your Plan --";
                    page.Add(oBmast);

                    for (int i = 0; i < dt2.Rows.Count; i++)
                    {

                        oBmast = new EntityPricing();
                        oBmast.DTLS_PACKAGE_KEY = Convert.ToInt32(dt2.Rows[i]["DTLS_PACKAGE_KEY"]);
                        oBmast.PACKAGE_DESC = dt2.Rows[i]["PACKAGE_DESC"].ToString();
                        oBmast.PACKAGE_NAME = dt2.Rows[i]["PACKAGE_NAME"].ToString();
                        page.Add(oBmast);
                        Session["packagename"] = oBmast.PACKAGE_NAME;
                    }

                    var getpackage = page.ToList();

                    SelectList list = new SelectList(getpackage, "DTLS_PACKAGE_KEY", "PACKAGE_DESC", ViewBag.ddl_DTLS_PACKAGE_KEY);
                    ViewBag.Package = list;

                }
                return errMsg;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        protected void send(string recepientEmail)
        {

            try
            {

                string to_username = ConfigurationManager.AppSettings["to_username"].ToString();
                string form_username = ConfigurationManager.AppSettings["form_username"].ToString();
                string form_password = ConfigurationManager.AppSettings["form_password"].ToString();
                string smtpAddress = "smtp.gmail.com"; //103.21.58.247
                int portNumber = 587;
                bool enableSSL = true; //false

                using (MailMessage mail = new MailMessage())
                {
                    string template = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/main-assets/mailtemplate.html")))
                    {
                        template = reader.ReadToEnd();
                        template = template.Replace("FULLNAME", Session["FIRSTNAME"] + " " + Session["LASTNAME"]);
                        template = template.Replace("EMAILID", Session["EMAIL"].ToString());
                        template = template.Replace("MOBILENO", Session["PHONE"].ToString());
                        template = template.Replace("COMNAME", Session["COMPANY_NAME"].ToString());
                        template = template.Replace("COMADDRESS", Session["COMPANY_NAME"].ToString());
                        template = template.Replace("COUNTRY", Session["COUNTRY"].ToString());
                        template = template.Replace("STATE", Session["STATE"].ToString());
                        template = template.Replace("CITY", Session["CITY"].ToString());
                        template = template.Replace("ZIP", Session["ZIP"].ToString());
                        template = template.Replace("PRICINGPLAN", Session["packagename"].ToString());
                    }
                    mail.From = new MailAddress(form_username);
                    mail.To.Add(to_username);
                    mail.Subject = "New registration from " + Session["FIRSTNAME"] + " " + ViewBag.txt_LastName;
                    mail.Body = template.ToString();
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(form_username, Encrypted.Decryptdata(form_password));
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(mail);
                    }
                }

                using (MailMessage mail1 = new MailMessage())
                {
                    string templatebody = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/main-assets/regmailtemplate.html")))
                    {
                        templatebody = reader.ReadToEnd();
                        templatebody = templatebody.Replace("FULLNAME", Session["FIRSTNAME"] + " " + ViewBag.txt_LastName);
                        templatebody = templatebody.Replace("PRICINGPLAN", Session["packagename"].ToString());

                    }
                    mail1.From = new MailAddress(form_username);
                    mail1.To.Add(Session["EMAIL"].ToString());
                    mail1.Subject = "Thank you for registering on Veridoc HR";
                    mail1.Body = templatebody.ToString();
                    mail1.IsBodyHtml = true;

                    using (SmtpClient smtp1 = new SmtpClient(smtpAddress, portNumber))
                    {
                        smtp1.UseDefaultCredentials = false;
                        smtp1.Credentials = new NetworkCredential(form_username, Encrypted.Decryptdata(form_password));
                        smtp1.EnableSsl = enableSSL;
                        smtp1.Send(mail1);
                    }
                }
            }
            catch (Exception ex)
            {
                // ClientScript.RegisterStartupScript(GetType(), "popupservererror", "popupservererror(); console.log('" + ex.Message + "');", true);
            }
            TempData["JavaScriptFunction"] = string.Format("popup();");
            //return Redirect("~/Register/Index");
        }




        private void ClearControl(FormCollection form)
        {
            form["txt_FistName"] = "";
            form["txt_LastName"] = "";
            form["txt_Eamil"] = "";
            form["txt_Phone"] = "";
            form["txt_Company_Name"] = "";
            form["txt_Company_Address"] = "";
            form["txt_State"] = "";
            form["txt_City"] = "";
            form["txt_ZIP"] = "";
            form["txt_Alter_Company_Name"] = "";
            form["txt_Alter_Company_Address"] = "";
            form["txt_Alter_City"] = "";
            form["txt_Alter_State"] = "";
            form["txt_Alter_ZIP"] = "";
        }
    }
}