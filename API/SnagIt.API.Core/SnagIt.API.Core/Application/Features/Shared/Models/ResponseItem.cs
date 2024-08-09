using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Application.Features.Shared.Models
{
    public abstract class ResponseItem<T> : ResponseData
    {
        public string Id { get; set; }

        public bool? Deleted { get; set; }

        public abstract T Item { get; set; }
    }
}
