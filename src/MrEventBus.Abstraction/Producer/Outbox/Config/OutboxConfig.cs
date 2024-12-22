﻿namespace MrEventBus.Abstraction.Producer.Outbox.Config;

public class OutboxConfig
{
    public bool EnabledProcessor { get; set; } = true;
    public ushort Concurrency { get; set; } = 1;
    public TimeSpan ReaderInterval { get; set; } = new TimeSpan(0, 0, 5);
    public TimeSpan PersistenceDuration { get; set; } = TimeSpan.Zero;
    public IReadOnlyCollection<Type> EnabledEvents { get; set; } = new List<Type>();
}