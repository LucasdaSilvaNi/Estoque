using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Sam.View
{
    public interface ILogErroView
    {
        int Id
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

        string StrackTrace
        {
            get;
            set;
        }

        DateTime Data 
        { 
            get;
            set;
        }
    }
}
