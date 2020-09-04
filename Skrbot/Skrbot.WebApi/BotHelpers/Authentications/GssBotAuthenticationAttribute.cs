using Microsoft.Bot.Connector;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Skrbot.WebApi.BotHelpers.Authentications
{
    [Serializable]
    public class GssBotAuthenticationAttribute : BotAuthentication
    {
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            try
            {
                if (!VaildAuthorized(actionContext))
                {   // 驗證不通過
                    actionContext.Response = BotAuthenticator.GenerateUnauthorizedResponse(actionContext.Request, $"BotAuthentication failed to authenticate incoming request!");
                }
            }
            catch (Exception e)
            {   // 有Exception
                actionContext.Response = BotAuthenticator.GenerateUnauthorizedResponse(actionContext.Request, $"Failed authenticating incoming request: {e.ToString()}");
            }

            await base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        // 驗證LDL
        private bool VaildAuthorized(HttpActionContext actionContext)
        {
            var service = new GssBotAuthenticationService();

            return service.VaildAuthorizedWithAccessToken(actionContext);
        }
    }
}