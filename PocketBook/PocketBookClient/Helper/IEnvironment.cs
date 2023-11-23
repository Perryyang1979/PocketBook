using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Helper
{
    public interface IEnvironment
    {
        void SetStatusBarColor(Color color, bool darkStatusBarTint);
    }
}
