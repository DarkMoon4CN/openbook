﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Log4Simple.Core
{
    public interface ILoggerWrapper
    {
        ILogger Logger { get; }
    }
}
