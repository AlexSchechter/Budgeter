using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Budgeter.Models;
using System.Net.Mail;
using System.Text;
using System.Linq;
using System.Net.Mime;
using System.Net;

namespace Budgeter.Controllers
{
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
            if (ModelState.IsValid)
            {
                Household household = GetHouseholdInfo();
                invitation.HouseholdId = household.Id;
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
                    var SendGridCredentials = db.SendgridCredentials.First();
                    NetworkCredential credentials = new NetworkCredential(SendGridCredentials.UserName, SendGridCredentials.Password);
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
