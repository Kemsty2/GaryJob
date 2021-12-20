using Elsa.Scripting.JavaScript.Messages;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GaryJob.Workflows.Scripting.JavaScript
{
    public class ConfigureJavaScriptEngineWithCustomFunctions : INotificationHandler<EvaluatingJavaScriptExpression>
    {
        public Task Handle(EvaluatingJavaScriptExpression notification, CancellationToken cancellationToken)
        {
            var engine = notification.Engine;

            return Task.CompletedTask;
        }
    }
}