using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Data
{
    public  class GridCategoryData
    {
        public string Date { get; set; }
        public List<double> CategoryMoney
        {
            get
            {
                return _categoryMoney.ToList();
            }
        }

        public List<bool> SelectedCategoryColumnVisible
        {
            get
            {
                return _selectedColumnVisible.ToList();
            }
        }

        private bool[] _selectedColumnVisible = new bool[4];
        private double[] _categoryMoney = new double[4];

        public GridCategoryData(string date,double[] moneys,List<bool> visibles)  {
            Date = date;
            for(int i = 0;i<moneys.Length;i++)
            {
                _categoryMoney[i] = moneys[i];
                _selectedColumnVisible[i] = visibles[i];
            }
        }

        public void AppendMoney(int index,double money) {
            if(index>=0 || index<4)
            {
                _categoryMoney[index] = money;
            }
        }

        public void SetColumnVisible(int index, bool visible)
        {
            if (index >= 0 || index < 4)
            {
                _selectedColumnVisible[index] = visible;
            }
        }
    }
}
