namespace MediatorLibrary
{
    public interface IRequest<TResponse> { }

    public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }

    public interface IMediator
    {
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    }

    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            //what runtime request.getType will be the thing that extends from IRequest
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse)); 

            // dynamic here is like any in typescript. You essentially say this is whatever so you can call .Handle on it
            dynamic handler = _serviceProvider.GetRequiredService(handlerType);

            //dynamic cast here
            return await handler.Handle((dynamic)request, cancellationToken);
        }
    }
}