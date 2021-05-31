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
using Square.Exceptions;
using System.Threading.Tasks;
using Square.Models;
using Square;
using MyApp.db;

namespace VeriDoc_HR.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        private SquareClient client;
        private string locationId;

        #region Models
        public class CardModel
        {
            public string customerId { get; set; }
            public string cardNonce { get; set; }
            public string planName { get; set; }
            public string planPriceAU { get; set; }
        }

        #endregion

        public ActionResult Index()
        {
            if (TempData.ContainsKey("customerId"))
                ViewBag.customerId = TempData["customerId"].ToString();
            else
                return RedirectToAction("Index", "Home");
            if (TempData.ContainsKey("customerEmail"))
                ViewBag.customerEmail = TempData["customerEmail"].ToString();
            else
                return RedirectToAction("Index", "Home");
            if (TempData.ContainsKey("customerName"))
                ViewBag.customerName = TempData["customerName"].ToString();
            else
                return RedirectToAction("Index", "Home");
            if (TempData.ContainsKey("customerId"))
                ViewBag.customerId = TempData["customerId"].ToString();
            else
                return RedirectToAction("Index", "Home");
            if (TempData.ContainsKey("PlanName"))
                ViewBag.PlanName = TempData["PlanName"].ToString();
            else
                return RedirectToAction("Index", "Home");
            if (TempData.ContainsKey("PlanPrice"))
                ViewBag.PlanPrice = TempData["PlanPrice"].ToString();
            else
                return RedirectToAction("Index", "Home");
            if (TempData.ContainsKey("PlanPriceAU"))
                ViewBag.PlanPriceAU = TempData["PlanPriceAU"].ToString();
            else
                return RedirectToAction("Index", "Home");

            return View();
        }

        public ActionResult Success()
        {
            if (TempData.ContainsKey("sPlanName"))
                ViewBag.PlanName = TempData["sPlanName"].ToString();
            if (TempData.ContainsKey("sPlanPriceAU"))
                ViewBag.PlanPriceAU = TempData["sPlanPriceAU"].ToString();

            return View();
        }

        public PaymentController()
        {
            Square.Environment environment = ConfigurationManager.AppSettings["SquareEnvironment"] == "sandbox" ?
                Square.Environment.Sandbox : Square.Environment.Production;
            client = new SquareClient.Builder()
                .Environment(environment)
                .AccessToken(ConfigurationManager.AppSettings["SquareAccessToken"])
                .Build();

            locationId = ConfigurationManager.AppSettings["SquareLocationId"];
        }

        private static string NewIdempotencyKey()
        {
            try
            {
                return Guid.NewGuid().ToString();
            }
            catch(Exception e)
            {
                return "0";
            }
            
        }


        private async Task<double> getAUValue()
        {


            var currencyConvertor = await VeriDocHttpHandler.Get<CurrencyConvertor>(new Uri(ConstantVariables.CurrancyConvertorAPI), 500);
            return Math.Round(currencyConvertor.Rates.AUD, 2);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> initSquarePaymentAsync(FormCollection customer)
        {
            string uuid = NewIdempotencyKey();
            string AddressLine2 = customer["txt_Alter_City"].ToString() + ", " + customer["txt_Alter_State"].ToString();
            string PlanName = customer["hfPlanName"].ToString();
            string PlanPrice = customer["hfPlanPrice"].ToString();
            string FullName = customer["txt_FistName"].ToString() + " " + customer["txt_LastName"].ToString();
            var address = new Address.Builder()
                .AddressLine1(customer["txt_Alter_Company_Address"].ToString())
                .AddressLine2(AddressLine2)
                .PostalCode(customer["txt_Alter_ZIP"].ToString())
                .Country(customer["AlterCountry"].ToString())
                .FirstName(customer["txt_FistName"].ToString())
                .LastName(customer["txt_LastName"].ToString())
                .Organization(customer["txt_Alter_Company_Name"].ToString())
                .Build();

            var body = new CreateCustomerRequest.Builder()
              .IdempotencyKey(uuid)
              .GivenName(customer["txt_FistName"].ToString())
              .FamilyName(customer["txt_LastName"].ToString())
              .CompanyName(customer["txt_Alter_Company_Name"].ToString())
              .EmailAddress(customer["txt_Eamil"].ToString())
              .Address(address)
              .PhoneNumber(customer["Phone-Number"].ToString() + customer["txt_Phone"].ToString())
              .Build();

            try
            {
                var res = await client.CustomersApi.CreateCustomerAsync(body);
                TempData["customerId"] = res.Customer.Id;
                TempData["customerEmail"] = customer["txt_Eamil"].ToString();
                TempData["customerName"] = FullName;
                TempData["PlanName"] = PlanName;
                TempData["PlanPrice"] = PlanPrice;
                ViewBag.FirstName = customer["txt_FistName"].ToString();
                ViewBag.Email = customer["txt_Eamil"].ToString();
                var multiplier = await getAUValue();
                var PlanPriceAU = Convert.ToDouble(PlanPrice) * multiplier;
                TempData["PlanPriceAU"] = PlanPriceAU;

                String returnvaluesubmit = String.Empty;

                returnvaluesubmit = btn_submit_Click(customer, res.Customer.Id);

                if (returnvaluesubmit == "2")
                {
                    return RedirectToAction("Index", "Payment");
                }
                else if(returnvaluesubmit == "1")
                {
                    TempData["JavaScriptFunction"] = string.Format("subscribepopup();");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["JavaScriptFunction"] = string.Format("popupservererror();");
                    return RedirectToAction("Index", "Home");


                }
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction("Plan", "Subscribe");
            }
        }

        [HttpPost]
        public async Task<JsonResult> sqAddCardonFile(CardModel card)
        {
            string result, cardId, error = String.Empty;
            var body = new CreateCustomerCardRequest.Builder(cardNonce: card.cardNonce)
                .Build();
            try
            {
                var res = await client.CustomersApi.CreateCustomerCardAsync(customerId: card.customerId, body: body);
                cardId = res.Card.Id;
                string plan = card.planName == "BASIC" ? ConfigurationManager.AppSettings["BasicPlan"] : card.planName == "STANDARD" ? ConfigurationManager.AppSettings["StandardPlan"] : ConfigurationManager.AppSettings["ProPlan"];
                result = await sqGenerateSubscription(plan, card.customerId, cardId);

                if (result != "error")
                {
                    TempData["sPlanName"] = card.planName;
                    TempData["sPlanPriceAU"] = card.planPriceAU;
                    if(TempData["SameBillingDetails"].ToString()=="false")
                    {
                        send(Session["EMAIL"].ToString(), false);
                    }
                    else
                    {
                        send(Session["EMAIL"].ToString(), true);
                    }
                    return Json(new { success = result });
                }
                else
                    return Json(new { error = result });
            }
            catch (ApiException e)
            {
                return Json(new { error = e.Message });
            }
        }

        private async Task<string> sqGenerateSubscription(string planId, string customerId, string cardId)
        {
            string result, uuid = NewIdempotencyKey();
            var body = new CreateSubscriptionRequest.Builder(
                idempotencyKey: uuid,
                locationId: locationId,
                planId: planId,
                customerId: customerId)
              .CardId(cardId)
              .Build();
            try
            {
                var res = await client.SubscriptionsApi.CreateSubscriptionAsync(body);
                result = res.Subscription.Id;
            }
            catch (ApiException e)
            {
                Console.WriteLine($"Exception: {e.Message}");
                result = "error";
            }
            return result;
        }

        #region Webhook from Square

        [HttpPost]
        public ActionResult sqProcessInvoice()
        {
            string path = Server.MapPath("~/TestWebhook.txt");
            try
            {
                Stream req = Request.InputStream;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(req).ReadToEnd();
                using (StreamWriter sw = System.IO.File.CreateText(path)) sw.WriteLine(json);

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        #endregion

        public string btn_submit_Click(FormCollection form, string customerid)
        {
            EntityRegister oBMST = null;
            Byte vRef = 0; Int32 vKey = 0;
            String errMsg = String.Empty;
            DataTable dt4 = new DataTable();
            //Int32 pricingkey = Convert.ToInt32(Session["price"]);
            ViewBag.hf_CUSTOMER_ID = customerid;
            EntityPricing oBMSTP = new EntityPricing();
            try
            {
                oBMST = new EntityRegister();

                oBMST.DTLS_REGISTER_KEY = Convert.ToInt32(0);
                oBMST.FIRSTNAME = ViewBag.FirstName;
                oBMST.LASTNAME = form["txt_LastName"];
                oBMST.EMAIL = ViewBag.Email;
                oBMST.PHONE = form["txt_Phone"];
                oBMST.COMPANY_NAME = form["txt_Company_Name"];
                oBMST.COMPANY_ADDRESS = form["txt_Company_Address"];
                oBMST.COUNTRY = form["Country1"];
                oBMST.STATE = form["txt_State"];
                oBMST.CITY = form["txt_City"];
                oBMST.ZIP = form["txt_ZIP"];
                oBMST.DTLS_PACKAGE_KEY = Convert.ToInt32(TempData["packagekey"]);
                //oBMST.DTLS_PACKAGE_KEY = 1;

                oBMST.ALTER_COMPANY_NAME = form["txt_Alter_Company_Name"];
                oBMST.ALTER_COMPANY_ADDRESS = form["txt_Alter_Company_Address"];
                oBMST.ALTER_COUNTRY = form["AlterCountry"];
                oBMST.ALTER_STATE = form["txt_Alter_State"];
                oBMST.ALTER_CITY = form["txt_Alter_City"];
                oBMST.ALTER_ZIP = form["txt_Alter_ZIP"];

                oBMST.CUSTOMER_ID = ViewBag.hf_CUSTOMER_ID;

                oBMST.ENT_USER_KEY = oBMST.DTLS_REGISTER_KEY;
                oBMST.EDIT_USER_KEY = oBMST.DTLS_REGISTER_KEY;
                oBMST.TAG_ACTIVE = 1;
                oBMST.TAG_DELETE = 0;
               TempData["PlanName"] = TempData["PlanName"];
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
                

                Session["ALTER_COMPANY_NAME"] = oBMST.ALTER_COMPANY_NAME;
                Session["ALTER_COMPANY_ADDRESS"] = oBMST.ALTER_COMPANY_ADDRESS;
                Session["ALTER_COUNTRY"] = oBMST.ALTER_COUNTRY;
                Session["ALTER_CITY"] = oBMST.ALTER_CITY;
                Session["ALTER_STATE"] = oBMST.ALTER_STATE;
                Session["ALTER_ZIP"] = oBMST.ALTER_ZIP;

                using (BaRegister oBDT = new BaRegister())
                {

                    if (oBMST.DTLS_REGISTER_KEY == 0)
                    {
                        if (form["SameBillingDetails"] == "yes")
                        {
                            if (form["txt_FistName"] != "" && form["txt_LastName"] != "" && form["Eamil"] != "" && form["txt_Phone"] != "")
                            {
                                if (form["Country1"] != "" && form["txt_Company_Name"] != "" && form["txt_Company_Address"] != "")
                                {
                                    vRef = oBDT.SaveChanges<Object, Int32>("INSERT", oBMST, null, ref vKey, ref errMsg, "2019", 1);
                                    if (vRef == 1)
                                    {
                                        //MessageBox(2, Message.msgSaveNew, "");
                                        //ClientScript.RegisterStartupScript(GetType(), "popup", "popup();", true);
                                        //Response.Redirect("/Index/Index", false);
                                        TempData["SameBillingDetails"] = "false";

                                        oBMSTP.PACKAGE_NAME = TempData["PlanName"].ToString();
                                        
                                        if (oBMSTP.PACKAGE_NAME == "FREE TRIAL")
                                        {
                                            TempData["JavaScriptFunction"] = string.Format("subscribepopup();");
                                            Response.Redirect("/Home/Index", false);
                                            send(oBMST.EMAIL,false);
                                            return "1";
                                        }
                                        else if (oBMSTP.PACKAGE_NAME == "PRO")
                                        {
                                            Response.Redirect("#", false);

                                        }
                                        else if (oBMSTP.PACKAGE_NAME == "STANDARD")
                                        {
                                           Response.Redirect("#", false);

                                        }

                                        
                                        // ClearControl(form);
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
                        else if (form["SameBillingDetails"] == "no")
                        {
                            if (form["txt_Alter_Company_Name"] != "" && form["txt_Alter_Company_Address"] != "" && form["txt_Alter_City"] != "")
                            {
                                if (form["AlterCountry"] != "")
                                {
                                    TempData["SameBillingDetails"] = "true";
                                    vRef = oBDT.SaveChanges<Object, Int32>("INSERT", oBMST, null, ref vKey, ref errMsg, "2019", 1);
                                    if (vRef == 1)
                                    {
                                        //MessageBox(2, Message.msgSaveNew, "");
                                        //ClientScript.RegisterStartupScript(GetType(), "popup", "popup();", true);


                                        oBMSTP.PACKAGE_NAME = TempData["PlanName"].ToString();
                                        
                                        if (oBMSTP.PACKAGE_NAME == "FREE TRIAL")
                                        {
                                            //Session["popup"] = "1";
                                            TempData["JavaScriptFunction"] = string.Format("subscribepopup();");
                                            Response.Redirect("/Home/Index", false);
                                            send(oBMST.EMAIL,true);
                                            return "1";

                                        }
                                       

                                        //send(oBMST.EMAIL,true);
                                        // ClearControl(form);
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

                return "2";

            }
            catch (Exception ex)
            {
                return "0";
                // //ClientScript.RegisterStartupScript(GetType(), "popupservererror", "popupservererror(); console.log('" + ex.Message + "');", true);


            }


        }


        protected string send(string recepientEmail, Boolean alterdetails)
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
                        template = template.Replace("COMADDRESS", Session["COMPANY_ADDRESS"].ToString());
                        template = template.Replace("COUNTRY", Session["COUNTRY"].ToString());
                        template = template.Replace("STATE", Session["STATE"].ToString());
                        template = template.Replace("CITY", Session["CITY"].ToString());
                        template = template.Replace("ZIP", Session["ZIP"].ToString());
                        template = template.Replace("PRICINGPLAN", Session["package"].ToString());
                        if (alterdetails == true) 
                        {
                            template = template.Replace("CONAME", Session["ALTER_COMPANY_NAME"].ToString());
                            template = template.Replace("COMPANYADD", Session["ALTER_COMPANY_ADDRESS"].ToString());
                            template = template.Replace("COUNNAME", Session["ALTER_COUNTRY"].ToString());
                            template = template.Replace("STATNAME", Session["ALTER_STATE"].ToString());
                            template = template.Replace("CTYNAME", Session["ALTER_CITY"].ToString());
                            template = template.Replace("PIN", Session["ALTER_ZIP"].ToString());
                        }
                        else
                        {
                            template = template.Replace("CONAME", "N/A");
                            template = template.Replace("COMPANYADD", "N/A");
                            template = template.Replace("COUNNAME", "N/A");
                            template = template.Replace("STATNAME", "N/A");
                            template = template.Replace("CTYNAME", "N/A");
                            template = template.Replace("PIN", "N/A");
                        }
                       
                    }
                    mail.From = new MailAddress(form_username);
                    mail.To.Add(to_username);
                    mail.Subject = "New subscription from " + Session["FIRSTNAME"] + " " + ViewBag.txt_LastName;
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
                        templatebody = templatebody.Replace("PRICINGPLAN", Session["package"].ToString());

                    }
                    mail1.From = new MailAddress(form_username);
                    mail1.To.Add(Session["EMAIL"].ToString());
                    mail1.Subject = "Thank you for subscribe on Veridoc HR";
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
                return "1";
            }
            catch (Exception ex)
            {
                return "0";
                // ClientScript.RegisterStartupScript(GetType(), "popupservererror", "popupservererror(); console.log('" + ex.Message + "');", true);
            }
            //TempData["JavaScriptFunction"] = string.Format("subscribepopup();");
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