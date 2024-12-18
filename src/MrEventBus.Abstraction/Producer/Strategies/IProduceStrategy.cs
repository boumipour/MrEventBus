using MrEventBus.Abstraction.Models;

namespace MrEventBus.Abstraction.Producer.Strategies;

public interface IProduceStrategy
{
    Task PublishAsync<T>(MessageContext<T> messageContext, string queueName = "main", CancellationToken cancellationToken = default);
}
