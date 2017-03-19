﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventDispatcher.Core
{
    public interface IHandle<in T> where T : IDomainEvent
    {
        void Handle(T @event);
    }
}
