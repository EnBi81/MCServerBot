using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IDatabaseSetup
    {
        public Task Setup(string connectionString);
        public Task ResetDatabase();
    }
}
