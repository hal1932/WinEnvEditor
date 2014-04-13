using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace WinEnvEditor.Models
{
    public enum EditMode
    {
        Add,
        Remove,
    }

    public enum EditOption
    {
        Overwrite,
        Diff,
    }

    public enum EditTarget
    {
        UserEnv,
        SystemEnv,
    }
}
