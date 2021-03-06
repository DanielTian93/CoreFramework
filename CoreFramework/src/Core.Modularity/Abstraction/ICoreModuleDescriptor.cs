﻿using System;
using System.Collections.Generic;

namespace Core.Modularity.Abstraction
{
    public interface ICoreModuleDescriptor
    {
        Type ModuleType { get; }

        ICoreModule Instance { get; }

        IReadOnlyList<ICoreModuleDescriptor> Dependencies { get; }
    }
}
