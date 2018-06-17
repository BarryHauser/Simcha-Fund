using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaFund.Data;
using SimchaFund.Models;

namespace SimchaFund.Controllers
{
    public class ContributorsController : Controller
    {
        public ActionResult Index()
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            var vm = new ContributersVM
            {
                PersonWithBalance = db.GetAllContributers(),
                TotalBalance = db.GetTotalBalance()

            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult NewContributer(Person person,decimal initialDeposit)
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.NewContributer(person, initialDeposit);
            return Redirect("/Contributors/index");
        }
        [HttpPost]
        public ActionResult EditContributer(Person person)
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.EditContributer(person);
            return Redirect("/Contributors/index");
        }
        [HttpPost]
        public ActionResult Deposit(Deposit deposit, int id)
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.Deposit(deposit,id);
            return Redirect("/Contributors/index");
        }
        public ActionResult History(int id)
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            var vm = new HistoryVM
            {
                Trancactions = db.GetTrancactionsById(id),
                Name = db.GetNameById(id),
                Balance = db.GetBalanceOfAPerson(id)
            };
            return View(vm);
        }
    }
}