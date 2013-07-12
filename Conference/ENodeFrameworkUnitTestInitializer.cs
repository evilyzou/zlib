using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ENode;

namespace Conference
{
    class ENodeFrameworkUnitTestInitializer
    {
        public void Initialize()
        {
            //全部使用默认配置，一般单元测试时，可以使用该配置
            Configuration.StartWithAllDefault(new Assembly[] { Assembly.GetExecutingAssembly() });
        }
    }
}
