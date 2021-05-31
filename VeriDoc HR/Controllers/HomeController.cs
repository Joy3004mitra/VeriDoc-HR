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


namespace VeriDoc_HR.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        DataTable dt;
        String errMsg = String.Empty;
        String vString = String.Empty;

        public ActionResult Index()
        {
            FillPageSettingGrid();
            FillSiteSettingEdit();
            BannerPortion();
            FillPatentTableGrid();
            FillMastBenefitGrid();
            FillHowItWorkUserEdit();
            FillHowItWorkAdminApp();
            FillHowItWorkAdminWeb();
            FillMastPackageGrid();
            @ViewBag.RegisterLink = "../Subscribe/Plan";
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
                    ViewBag.BANNER_HEADING = Convert.ToString(dt.Rows[0]["BANNER_HEADING"]);
                    ViewBag.BANNER_DESC = Convert.ToString(dt.Rows[0]["BANNER_DESC"]);
                    ViewBag.BANNER_DESC_1 = Convert.ToString(dt.Rows[0]["BANNER_DESC_1"]);
                    ViewBag.BANNER_IMG = Convert.ToString(dt.Rows[0]["BANNER_IMG"]);
                    ViewBag.BANNER_NOTE = Convert.ToString(dt.Rows[0]["BANNER_NOTE"]);
                    ViewBag.GOOGLEPLAY_LINK = Convert.ToString(dt.Rows[0]["GOOGLEPLAY_LINK"]);
                    ViewBag.PatentHeading = Convert.ToString(dt.Rows[0]["PATENTED_HEADING"]);
                    ViewBag.APPSTORE_LINK = Convert.ToString(dt.Rows[0]["APPSTORE_LINK"]);

                    ViewBag.img_banner = ConfigurationManager.AppSettings["HOME"].ToString() + Convert.ToString(dt.Rows[0]["BANNER_IMG"]);
                    ViewBag.img_Patent = ConfigurationManager.AppSettings["HOME"].ToString() + Convert.ToString(dt.Rows[0]["PATENTED_IMG"]);


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

        private DataTable FillPatentTableGrid()
        {
            try
            {

                errMsg = String.Empty;
                vString = String.Empty;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                dt = new DataTable();
                using (BAPatent oBMC = new BAPatent())
                {
                    dt = oBMC.Get<Int32>("GET", 0, "", ref errMsg, "2019", 1);
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        vString += "<div class='col-xl-6 col-lg-6 col-md-6 col-sm-12 col-12'><ul><li>" + dt.Rows[i]["PATENT_DESC"].ToString() + "</li></ul></div>";

                    }

                }

                ViewBag.PatentDesc = vString;
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }
        private DataTable FillMastBenefitGrid()
        {
            try
            {
                errMsg = String.Empty;
                vString = String.Empty;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                dt = new DataTable();
                using (BaBenefits oBMC = new BaBenefits())
                {
                    dt = oBMC.Get<Int32>("GET", 0, "", ref errMsg, "2019", 1);
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        vString += "<div class='col-xl-4 col-lg-4 col-md-4 col-sm-6 col-12'><div class='single_feature text-center'><div class='feature_img'><img src='" + ConfigurationManager.AppSettings["BENEFITS"].ToString() + Convert.ToString(dt.Rows[i]["BENEFITS_IMG"]) + "' alt='veridocklockbook' class='img-fluid' width='103' height='133' title='featured-img'></div><div class='featured_content'><h4>" + Convert.ToString(dt.Rows[i]["HEADING"]) + "</h4><p>" + Convert.ToString(dt.Rows[i]["DESCRIPTION"]) + "</p></div></div></div>";

                    }
                }

                ViewBag.Benefits = vString;

            }
            catch (Exception ex)
            {
                // return ex.Message;
            }
            return dt;

        }

        private String FillHowItWorkUserEdit()
        {
            try
            {
                String vCOMPANY_ACCESS = String.Empty;
                errMsg = String.Empty;
                dt = new DataTable();
                using (BaHowItWorkUser oBME = new BaHowItWorkUser())
                {
                    dt = oBME.GetData("ALL", "", ref errMsg, null, 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.USER_HEADING_ONE = Convert.ToString(dt.Rows[0]["HEADING_ONE"]);
                    ViewBag.USER_DESC_ONE = Convert.ToString(dt.Rows[0]["DESC_ONE"]);

                    ViewBag.USER_HEADING_TWO = Convert.ToString(dt.Rows[0]["HEADING_TWO"]);
                    ViewBag.USER_DESC_TWO = Convert.ToString(dt.Rows[0]["DESC_TWO"]);

                    ViewBag.USER_HEADING_THREE = Convert.ToString(dt.Rows[0]["HEADING_THREE"]);
                    ViewBag.USER_DESC_THREE = Convert.ToString(dt.Rows[0]["DESC_THREE"]);

                    ViewBag.USER_HEADING_FOUR = Convert.ToString(dt.Rows[0]["HEADING_FOUR"]);
                    ViewBag.txt_DESC_FOUR = Convert.ToString(dt.Rows[0]["DESC_FOUR"]);



                    ViewBag.USER_img_1 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);
                    //  ViewBag.hf_IMG_1 = Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);

                    ViewBag.USER_img_2 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);
                    // ViewBag.hf_IMG_2 = Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);

                    ViewBag.USER_img_3 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);
                    // ViewBag.hf_IMG_3 = Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);

                    ViewBag.USER_img_4 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);
                    // ViewBag.hf_IMG_4 = Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);





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

        private String FillHowItWorkAdminApp()
        {
            try
            {
                String vCOMPANY_ACCESS = String.Empty;
                errMsg = String.Empty;
                dt = new DataTable();
                using (BaHowItWorkAdminApp oBME = new BaHowItWorkAdminApp())
                {
                    dt = oBME.GetData("ALL", "", ref errMsg, null, 1);
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.ADMIN_HEADING_ONE = Convert.ToString(dt.Rows[0]["HEADING_ONE"]);
                    ViewBag.ADMIN_DESC_ONE = Convert.ToString(dt.Rows[0]["DESC_ONE"]);

                    ViewBag.ADMIN_HEADING_TWO = Convert.ToString(dt.Rows[0]["HEADING_TWO"]);
                    ViewBag.ADMIN_DESC_TWO = Convert.ToString(dt.Rows[0]["DESC_TWO"]);

                    ViewBag.ADMIN_HEADING_THREE = Convert.ToString(dt.Rows[0]["HEADING_THREE"]);
                    ViewBag.ADMIN_DESC_THREE = Convert.ToString(dt.Rows[0]["DESC_THREE"]);

                    ViewBag.ADMIN_HEADING_FOUR = Convert.ToString(dt.Rows[0]["HEADING_FOUR"]);
                    ViewBag.ADMIN_DESC_FOUR = Convert.ToString(dt.Rows[0]["DESC_FOUR"]);

                    ViewBag.ADMIN_HEADING_FIVE = Convert.ToString(dt.Rows[0]["HEADING_FIVE"]);
                    ViewBag.ADMIN_DESC_FIVE = Convert.ToString(dt.Rows[0]["DESC_FIVE"]);

                    ViewBag.ADMIN_HEADING_SIX = Convert.ToString(dt.Rows[0]["HEADING_SIX"]);
                    ViewBag.ADMIN_DESC_SIX = Convert.ToString(dt.Rows[0]["DESC_SIX"]);



                    ViewBag.ADMIN_img_1 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);
                    //  ViewBag.hf_IMG_1 = Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);

                    ViewBag.ADMIN_img_2 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);
                    // ViewBag.hf_IMG_2 = Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);

                    ViewBag.ADMIN_img_3 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);
                    // ViewBag.hf_IMG_3 = Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);

                    ViewBag.ADMIN_img_4 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);
                    // ViewBag.hf_IMG_4 = Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);

                    ViewBag.ADMIN_img_5 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FIVE"]);

                    ViewBag.ADMIN_img_6 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_SIX"]);

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

        private String FillHowItWorkAdminWeb()
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
                    ViewBag.AWEB_HEADING_ONE = Convert.ToString(dt.Rows[0]["HEADING_ONE"]);
                    ViewBag.AWEB_DESC_ONE = Convert.ToString(dt.Rows[0]["DESC_ONE"]);

                    ViewBag.AWEB_HEADING_TWO = Convert.ToString(dt.Rows[0]["HEADING_TWO"]);
                    ViewBag.AWEB_DESC_TWO = Convert.ToString(dt.Rows[0]["DESC_TWO"]);

                    ViewBag.AWEB_HEADING_THREE = Convert.ToString(dt.Rows[0]["HEADING_THREE"]);
                    ViewBag.AWEB_DESC_THREE = Convert.ToString(dt.Rows[0]["DESC_THREE"]);

                    ViewBag.AWEB_HEADING_FOUR = Convert.ToString(dt.Rows[0]["HEADING_FOUR"]);
                    ViewBag.AWEB_DESC_FOUR = Convert.ToString(dt.Rows[0]["DESC_FOUR"]);

                    ViewBag.AWEB_HEADING_FIVE = Convert.ToString(dt.Rows[0]["HEADING_FIVE"]);
                    ViewBag.AWEB_DESC_FIVE = Convert.ToString(dt.Rows[0]["DESC_FIVE"]);

                    ViewBag.AWEB_HEADING_SIX = Convert.ToString(dt.Rows[0]["HEADING_SIX"]);
                    ViewBag.AWEB_DESC_SIX = Convert.ToString(dt.Rows[0]["DESC_SIX"]);



                    ViewBag.AWEB_img_1 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);
                    //  ViewBag.hf_IMG_1 = Convert.ToString(dt.Rows[0]["IMAGE_ONE"]);

                    ViewBag.AWEB_img_2 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);
                    // ViewBag.hf_IMG_2 = Convert.ToString(dt.Rows[0]["IMAGE_TWO"]);

                    ViewBag.AWEB_img_3 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);
                    // ViewBag.hf_IMG_3 = Convert.ToString(dt.Rows[0]["IMAGE_THREE"]);

                    ViewBag.AWEB_img_4 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);
                    // ViewBag.hf_IMG_4 = Convert.ToString(dt.Rows[0]["IMAGE_FOUR"]);

                    ViewBag.AWEB_img_5 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_FIVE"]);

                    ViewBag.AWEB_img_6 = ConfigurationManager.AppSettings["HOWITWORK"].ToString() + Convert.ToString(dt.Rows[0]["IMAGE_SIX"]);

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

        private DataTable FillMastPackageGrid()
        {

            try
            {
                errMsg = String.Empty;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                dt = new DataTable();

                using (BaPricing oBAD = new BaPricing())
                {
                    dt = oBAD.Get<Int32>("GET", 0, "", ref errMsg, "2019", 1);
                    //gvpricing.DataSource = dt;
                    //gvpricing.DataBind();
                    //MessageBox(1, "", "");
                    // return errMsg;
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.PACKAGE_NAME_ONE = Convert.ToString(dt.Rows[0]["PACKAGE_NAME"]);
                    ViewBag.PACKAGE_DESC_ONE = Convert.ToString(dt.Rows[0]["PACKAGE_DESC"]);
                    ViewBag.PACKAGE_AMOUNT_ONE = Convert.ToString(dt.Rows[0]["PACKAGE_AMOUNT"]);
                    ViewBag.MONTHLY_PACKAGE_ONE = Convert.ToString(dt.Rows[0]["MONTHLY_PACKAGE"]);

                    ViewBag.PACKAGE_NAME_TWO = Convert.ToString(dt.Rows[1]["PACKAGE_NAME"]);
                    ViewBag.PACKAGE_DESC_TWO = Convert.ToString(dt.Rows[1]["PACKAGE_DESC"]);
                    ViewBag.PACKAGE_AMOUNT_TWO = Convert.ToString(dt.Rows[1]["PACKAGE_AMOUNT"]);
                    ViewBag.MONTHLY_PACKAGE_TWO = Convert.ToString(dt.Rows[1]["MONTHLY_PACKAGE"]);

                    ViewBag.PACKAGE_NAME_THREE = Convert.ToString(dt.Rows[2]["PACKAGE_NAME"]);
                    ViewBag.PACKAGE_DESC_THREE = Convert.ToString(dt.Rows[2]["PACKAGE_DESC"]);
                    ViewBag.PACKAGE_AMOUNT_THREE = Convert.ToString(dt.Rows[2]["PACKAGE_AMOUNT"]);
                    ViewBag.MONTHLY_PACKAGE_THREE = Convert.ToString(dt.Rows[2]["MONTHLY_PACKAGE"]);
                }

            }
            catch (Exception ex)
            {
                //return ex.Message;
            }

            return dt;
        }

        [HttpPost]
        public ActionResult btn_Contact_click(FormCollection form)
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

                    IsValidEmail(form["txt_Email"]);
                    if (form["txt_Name"] != "" && form["txt_Email"] != "" && form["txtMessage"] != "")
                    {
                        if (IsValidEmail(form["txt_Email"]) == true)
                        {
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/main-assets/contactmail.html")))
                            {
                                body = reader.ReadToEnd();
                                body = body.Replace("FULLNAME", form["txt_Name"].ToString());
                                body = body.Replace("EMAILID", form["txt_Email"]);
                                body = body.Replace("MESSAGE", form["txtMessage"]);
                            }

                            mail.From = new MailAddress(form_username);
                            mail.To.Add(to_username);
                            mail.Subject = "New appointment query from " + form["txt_Name"].ToString() + " for VeriDoc HR";
                            mail.Body = body.ToString();
                            mail.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                            {
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential(form_username, Encrypted.Decryptdata(form_password));
                                smtp.EnableSsl = enableSSL;
                                smtp.Send(mail);
                            }
                        }
                    }
                }
                using (MailMessage mail1 = new MailMessage())
                {


                    string body1 = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/main-assets/contactreply.html")))
                    {
                        body1 = reader.ReadToEnd();
                        body1 = body1.Replace("FULLNAME", form["txt_Name"].ToString());

                    }

                    mail1.From = new MailAddress(form_username);
                    mail1.To.Add(form["txt_Email"]);
                    mail1.Subject = "Thank you for contacting on VeriDoc HR";
                    mail1.Body = body1.ToString();
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
                //ClientScript.RegisterStartupScript(GetType(), "popupservererror", "popupservererror(); console.log('" + ex.Message + "');", true);
            }
            TempData["JavaScriptFunction"] = string.Format("modalpopup();");
            return Redirect("~/Home/Index");
        }

        [HttpPost]
        public ActionResult btn_Schedule_click(FormCollection form)
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

                    IsValidEmail(form["txt_mail"]);
                    if (form["txt_name1"] != "" && form["txt_mail"] != "" && form["txt_msg"] != "")
                    {
                        if (IsValidEmail(form["txt_mail"]) == true)
                        {
                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/main-assets/contactmail.html")))
                            {
                                body = reader.ReadToEnd();
                                body = body.Replace("FULLNAME", form["txt_name1"].ToString());
                                body = body.Replace("EMAILID", form["txt_mail"]);
                                body = body.Replace("MESSAGE", form["txt_msg"]);
                            }

                            mail.From = new MailAddress(form_username);
                            mail.To.Add(to_username);
                            mail.Subject = "New appointment query from " + form["txt_name1"].ToString() + " for VeriDoc HR";
                            mail.Body = body.ToString();
                            mail.IsBodyHtml = true;

                            using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                            {
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new NetworkCredential(form_username, Encrypted.Decryptdata(form_password));
                                smtp.EnableSsl = enableSSL;
                                smtp.Send(mail);
                            }
                        }
                    }
                }
                using (MailMessage mail1 = new MailMessage())
                {


                    string body1 = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/main-assets/contactreply.html")))
                    {
                        body1 = reader.ReadToEnd();
                        body1 = body1.Replace("FULLNAME", form["txt_name1"].ToString());

                    }

                    mail1.From = new MailAddress(form_username);
                    mail1.To.Add(form["txt_mail"]);
                    mail1.Subject = "Thank you for contacting on VeriDoc HR";
                    mail1.Body = body1.ToString();
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
                //ClientScript.RegisterStartupScript(GetType(), "popupservererror", "popupservererror(); console.log('" + ex.Message + "');", true);
            }
            TempData["JavaScriptFunction"] = string.Format("modalpopup();");
            return Redirect("~/Home/Index");
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var emailChecked = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}