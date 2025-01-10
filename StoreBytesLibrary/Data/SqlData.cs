using StoreBytesLibrary.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreBytesLibrary.Data
{
    public class SqlData
    {
        private readonly ISqlDataAccess db;

        public SqlData(ISqlDataAccess db)
        {
            this.db = db;
        }
        
    }
}
