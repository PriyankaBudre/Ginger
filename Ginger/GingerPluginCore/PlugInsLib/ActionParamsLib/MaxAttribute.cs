﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Amdocs.Ginger.Plugin.Core
{

    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false)]
    public class MaxAttribute : Attribute, IActionParamProperty
    {        
        // when saved to services json the attr property name will be:
        public string PropertyName => "Max";

        public int Max { get; set; }

        public MaxAttribute(int max)
        {
            Max = max;
        }

        public MaxAttribute()
        {            
        }
    }
}