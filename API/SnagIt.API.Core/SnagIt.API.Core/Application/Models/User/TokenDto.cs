using SnagIt.API.Core.Application.Features.Shared.Models;

namespace SnagIt.API.Core.Application.Models.User
{
    public class TokenDto : Response
    {
        public override ResponseData Data { get; set; }

        public class TokenDetailItem : ResponseItem<TokenDetail>
        {
            public override TokenDetail Item { get; set; }
        }

        public class TokenDetail
        {
            public string TokenType { get; set; }
            public DateTime? ExpiresIn { get; set; }
            public string AccessToken { get; set; }
        }
    }
}
