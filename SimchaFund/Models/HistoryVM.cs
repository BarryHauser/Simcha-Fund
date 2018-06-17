using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaFund.Data;

namespace SimchaFund.Models
{
    public class HistoryVM
    {
        public IEnumerable<Trancaction> Trancactions { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}