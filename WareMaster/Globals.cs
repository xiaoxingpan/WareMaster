using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public class Globals
    {
        public static int Id { get; set; }
        public static string Username {  get; set; }
        
        public static RoleEnum Role { get; set; }

        static public WareMasterEntities wareMasterEntities;
        public static WareMasterEntities DbContext //singleton pattern
        {
            get
            {
                if (wareMasterEntities == null)
                {
                    wareMasterEntities = new WareMasterEntities();
                }
                return wareMasterEntities;
            }
        }

    }
}
