using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CA200SRVRLib;

namespace X2DisplayTest
{
    public class KonicaCa310
    {
        public KonicaCa310()
        {
            objCa200 = new Ca200();
            objCa200.AutoConnect();
            objCa = objCa200.SingleCa;
            objProbe = objCa.SingleProbe;
        }

        private ICa200 objCa200;
        private Ca objCa;
        private Probe objProbe;
        private bool isMeasure;

        public void Zero()
        {
            try { 
                objCa.CalZero();
            }
            catch {
            }       
        }
    }
}
