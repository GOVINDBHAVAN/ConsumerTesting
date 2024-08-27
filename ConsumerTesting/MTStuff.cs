using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerTesting
{

    #region States
    public abstract class BaseState : SagaStateMachineInstance
    {
        public string? CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
    }
    public class MyState : BaseState
    {
        public Guid OrderId { get; set; }
    }
    #endregion

    #region Commands
    /// <summary>
    /// The base command class which has reusable stuffs
    /// </summary>
    public class SuperBaseCommand
    {
        public Guid CorrelationId { get; set; }

    }

    public class AnotherBaseCommand<TState> : SuperBaseCommand
        where TState : BaseState
    {
        public DateTimeOffset CreateTimestamp { get; set; } = DateTimeOffset.Now;
        public TState State { get; set; }
    }

    public class MyCommand<TState> : AnotherBaseCommand<TState>
        where TState : BaseState
    {
        public string Msg { get; set; } = string.Empty;
    }

    #endregion

    #region Consumers
    public class Consumer1
        : IConsumer<MyCommand<MyState>>
    {
        public async Task Consume(ConsumeContext<MyCommand<MyState>> context)
        {
            Console.WriteLine("Consumer1.Consume - Consumer1 : IConsumer<MyCommand<MyState>>");
            await Task.CompletedTask;
        }
    }
    
    public class Consumer2
        : IConsumer<AnotherBaseCommand<MyState>>
    {
        public async Task Consume(ConsumeContext<AnotherBaseCommand<MyState>> context)
        {
            Console.WriteLine("Consumer2.Consume - Consumer2 : IConsumer<AnotherBaseCommand<MyState>>");
            await Task.CompletedTask;
        }
    }
    public class Consumer3<TState>
        : IConsumer<MyCommand<TState>>    // it's not getting consumed
        where TState : BaseState, new()
    {
        public async Task Consume(ConsumeContext<MyCommand<TState>> context)
        {
            // not getting consumed here
            Console.WriteLine("Consumer3.Consume - Consumer3<TState> : IConsumer<MyCommand<TState>>");
            await Task.CompletedTask;
        }
    }
    #endregion
}
