using System;
using System.ComponentModel;

namespace IconifyClientLibrary
{
    public enum IconRotates
    {
        [Description("0°")]
        None = 0,

        [Description("90°")]
        R90 = 1,

        [Description("180°")]
        R180 = 2,

        [Description("270°")]
        R270 = 3,
    }
}
