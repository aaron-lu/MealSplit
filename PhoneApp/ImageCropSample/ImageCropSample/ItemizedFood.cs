using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCropSample
{

    
    public class ItemizedFood
    {
        public String Item { get; set; }
        public String Cost { get; set; }

        public ItemizedFood(String item, Double cost)
        {
            this.Item = item;
            this.Cost = cost.ToString();
        }
    }
}
