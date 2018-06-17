using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class SimchaContributersVW
    {
        public Simcha Simcha { get; set; }
        public List<ContributersWithPast> Contributers { get; set; }
    }
}