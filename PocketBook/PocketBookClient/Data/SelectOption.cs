using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public partial class SelectOption:ObservableObject
    {
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public bool isSelected;
    }
}
