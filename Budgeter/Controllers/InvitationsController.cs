using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Budgeter.Models;
using System.Net.Mail;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Configuration;

namespace Budgeter.Controllers
{
    [Authorize]
    [RequireHttps]
    public class InvitationsController : BaseController
    {      
        // GET: Invitations
        public async Task<ActionResult> Index()
        {
            var invitations = db.Invitations.Include(i => i.Household);
            return View(await invitations.ToListAsync());
        }

        // GET: Invitations/Create
        public ActionResult Create()
        {
            ViewBag.HouseholdName = GetHouseholdInfo().Name;
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email,HouseholdId")] Invitation invitation)
        {
            if (ModelState.IsValid && User.Identity.Name != DemoEmail)
            {
                Household household = GetHouseholdInfo();
                invitation.HouseholdId = household.Id;
                if (await db.Invitations.Where(i => i.HouseholdId == household.Id).AnyAsync(i => i.Email == invitation.Email))
                    return RedirectToAction("Index", "Home");

                db.Invitations.Add(invitation);
                await db.SaveChangesAsync();
                try
                {
                    //Build Email Message
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.To.Add(new MailAddress(invitation.Email, invitation.Email));
                    mailMessage.From = new MailAddress(GetUserInfo().Email, "From");
                    mailMessage.Subject = "Budgeter App: Invitation to Join a Household";
                    var callbackUrl = Url.Action("Index", "Households", null, protocol: Request.Url.Scheme);
                    string html = String.Concat(@"<p>I would like to invite you to join my household <i> ", household.Name,
                                    " </i>in the Budgeter App budgeting system.</p> <p><a href='", callbackUrl, "'>Join</a></p>");
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                    //Initialise SmtpClient and send
                    SmtpClient smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
                    NetworkCredential credentials = new NetworkCredential(
                        ConfigurationManager.AppSettings["mailAccount"], ConfigurationManager.AppSettings["mailPassword"]);
                                       
                    smtpClient.Credentials = credentials;
                    smtpClient.Send(mailMessage);

                    return RedirectToAction("Index", "Home");

                }
                catch(Exception ex)
                {
                    ViewBag.Exception = ex.Message;
                    return View(invitation);
                }           
            }
            return View(invitation);
        }
    }
}
