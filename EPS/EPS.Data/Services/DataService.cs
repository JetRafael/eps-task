using EPS.Data.DatabaseConnection;
using EPS.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.Data.Services
{
    public class DataService
    {
        private readonly ApplicationDBContext _context;

        public DataService(ApplicationDBContext context)
        {
            _context = context;
        }

        /**
        * Improvement: Apply better exception handling in GetDiscountCode
        */
        public DiscountCode GetDiscountCode(string discountCode) {
            return _context.DiscountCodes.SingleOrDefault(p => p.Code == discountCode);
        }

        /**
        * Improvement: Apply better exception handling in SaveDiscountCodes
        */
        public List<DiscountCode> SaveDiscountCodes(string[] discountCodes) { 
            List<DiscountCode> discountCodesObj = new List<DiscountCode>();

            for (int i = 0; i < discountCodes.Length; i++)
            {
                DiscountCode discountCode = new DiscountCode()
                {
                    Code = discountCodes[i],
                    IsActivated = false
                };

                _context.DiscountCodes.Add(discountCode);
                discountCodesObj.Add(discountCode);
            }
            _context.SaveChanges();

            return discountCodesObj;
        }

        public DiscountCode ActivateDiscountCode(DiscountCode discountCode) { 
            discountCode.IsActivated = true;
            _context.Update(discountCode);

            _context.SaveChanges();

            return discountCode;
        }
    }
}
