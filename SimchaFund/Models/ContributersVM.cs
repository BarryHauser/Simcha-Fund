using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class ContributersVM
    {
        public IEnumerable<PersonWithBalance> PersonWithBalance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}