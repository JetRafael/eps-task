using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.Data.DataModel
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsActivated { get; set; }
    }
}
