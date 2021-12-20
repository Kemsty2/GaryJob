using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.Liquid.Messages;
using MediatR;

namespace GaryJob.Workflows.Scripting.Liquid
{
    public class ConfigureLiquidEngine : INotificationHandler<EvaluatingLiquidExpression>
    {
        public Task Handle(EvaluatingLiquidExpression notification, CancellationToken cancellationToken)
        {
            var memberAccessStrategy = notification.TemplateContext.Options.MemberAccessStrategy;

            return Task.CompletedTask;
        }
    }
}