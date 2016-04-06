using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Colorimeter_Config_GUI
{
    class MTException : Exception
    {
        protected StreamWriter writer;

        public MTException() 
            : this("MTExcepiton") 
        {
        }

        public MTException(string message) 
            : base(message)
        {
            
        }
    }

    class ColorimeterException : MTException
    {
        public ColorimeterException()
            : this("No Colorimeter.")
        {
        }

        public ColorimeterException(string message)
            : base(message)
        {
            
        }
    }

    class FixtureException : MTException
    {
        public FixtureException()
            : this("Cann't connect fixture.")
        {
        }

        public FixtureException(string message)
            : base(message)
        {
            
        }
    }

    class DUTException : MTException
    {
         public DUTException()
            : this("Not finde DUT.")
        {
        }

         public DUTException(string message)
            : base(message)
        {
            
        }
    }
}
