using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Allocate.Common.Database.DataModels;

namespace Allocate.Web.DemoApi.DataModels
{
    public class Security : DataModelBase
    {
        public string Name { get; set; }

        public string LogoUrl { get; set; }

        public string TickerSymbol { get; set; }
    }
}