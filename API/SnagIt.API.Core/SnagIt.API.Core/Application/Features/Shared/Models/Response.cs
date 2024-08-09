using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagIt.API.Core.Application.Features.Shared.Models
{
    public abstract class Response
    {
        public string ApiVersion { get; set; }

        public string Context { get; set; }

        public Guid Id { get; set; }

        public string Method { get; set; }

        public virtual ResponseData Data { get; set; }

        public virtual ResponseError Error { get; set; }
    }
}
