using GaryJob.Core.Entities.FileAggregate.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GaryJob.Workflows.Handlers
{
    public class StartSaveFileWorkflowHandler : INotificationHandler<NewFileToSaveReceived>
    {
        public Task Handle(NewFileToSaveReceived notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}