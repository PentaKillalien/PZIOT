using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Common.Helper
{
    /// <summary>
    /// 反射机制获取实现了指定接口的所有类
    /// </summary>
    public static class InterfaceImplementationHelper
    {
        public static Type[] GetImplementingTypes(Type interfaceType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && !p.IsInterface)
                .ToArray();
        }
    }
}
