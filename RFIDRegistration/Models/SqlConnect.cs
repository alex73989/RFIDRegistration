using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDRegistration.Models
{
    public class SqlConnect
    {
        public SqlConnection con;

        public void Connection()
        {
            con = new SqlConnection("Data Source=localhost;Initial Catalog=Dek_MachineDB;Integrated Security=True;MultipleActiveResultSets=true");
        }
    
    }
}
