using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allocate.Common.Database.DataModels;

namespace Allocate.Web.DemoApi.DataModels
{
    public class ValueOverTime : DataModelBase
    {
        public Guid SecurityId { get; set; }

        public DateTime Date { get; set; }

        public decimal OpenAmt { get; set; }

        public decimal HighAmt { get; set; }

        public decimal LowAmt { get; set; }

        public decimal CloseAmt { get; set; }

        public long Volume { get; set; }
    }
}