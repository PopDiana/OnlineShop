using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Data
{
    public class Utils
    {
        public int productNumber = 0;

        private static Utils _instance = null;
        public static Utils Instance
        {
            get
            {
                if (_instance == null) _instance = new Utils();
                return _instance;
            }
        }

    }
}
