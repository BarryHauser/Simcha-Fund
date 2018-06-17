using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaFund.Data;
using SimchaFund.Models;

namespace SimchaFund.Controllers
{
    public class SimchaController : Controller
    {
        public ActionResult Index()
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            var vw = new SimchaIndexViewModel
            { 
                Simchas = db.GetAllSimchas()
            };
            return View(vw);
        }
        [HttpPost]
        public ActionResult NewSimcha(Simcha simcha)
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.NewSimcha(simcha);
            return Redirect("/");
        }
        public ActionResult Contributions(int id)
        {
            var db = new SimchaDb(Properties.Settings.Default.ConStr);
            var contributions = db.GetContributionsBySimcha(id);
            IEnumerable<ContributersWithPast> result;
            if(contributions != null)
            {
                result = AddCnbuToContr(contributions, db.GetAllContributers());
            }
            else
            {
                result = db.GetAllContributers().Select(c => new ContributersWithPast
                {
                    PersonWithBalance = c
                });
            }
            
            SimchaContributersVW vm = new SimchaContributersVW
            {
                Contributers = result.ToList(),
                Simcha = db.GetSimchaById(id)

            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Contributions (List<IncludedContribution> contributions, int simchaId)
        {
            List<Contribution> realContributions = contributions.Where(c => c.Included).Select(c => c.Contribution).ToList();
            if(realContributions.Count > 0)
            {
                var db = new SimchaDb(Properties.Settings.Default.ConStr);
                db.ClearContributionsToASimcha(simchaId);
                db.NewContributiosToASimcha(realContributions, simchaId);
            }
            return Redirect("/");

        }
        

        private IEnumerable<ContributersWithPast> AddCnbuToContr(IEnumerable<Contribution> contributions, IEnumerable<PersonWithBalance> contributers)
        {
            var result = new List<ContributersWithPast>();
            foreach(PersonWithBalance p in contributers)
            {
                decimal? temp = contributions.FirstOrDefault(c => c.PersonId == p.Person.Id)?.Amount;
                result.Add(new ContributersWithPast
                {
                    PersonWithBalance = p,
                    Amount = temp

                });
            }
            return result;

        }
        
    }
}