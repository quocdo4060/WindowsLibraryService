using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powell.Mfg.LibraryWinService
{
    public class DxfLayers
    {
        public string LayerIn { get; set; }
        public string LayerOut { get; set; }
        public DxfLayers(string inlayer, string outlayer)
        {
            LayerIn = inlayer;
            LayerOut = outlayer;
        }
    }
}
