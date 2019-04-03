using System;
using System.Collections.Generic;
using System.Text;
using GingerCore;
using GingerCore.Environments;

namespace Amdocs.Ginger.Common.InterfacesLib
{
    public interface IValueExpressionHelper
    {


        IValueExpression CreateValueExpression(ProjEnvironment mProjEnvironment, BusinessFlow mBusinessFlow);
        IValueExpression CreateValueExpression(ProjEnvironment mProjEnvironment, BusinessFlow mBusinessFlow, object DSList);
        IValueExpression CreateValueExpression(Object obj, string attr);
    }
}
