using System.Net;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using MultipleTestBot.Endpoints.Common.Response;
using MultipleTestBot.Exceptions;
using MultipleTestBot.Extensions;

namespace MultipleTestBot.Endpoints.Common;

/// <summary>
/// Базовая конечная точка.
/// </summary>
/// <typeparam name="TRequest">Тип запроса.</typeparam>
/// <typeparam name="TResponse">Тип ответа.</typeparam>
public abstract class BaseEndpoint<TRequest, TResponse> : ControllerBase where TResponse : class, new()
{
    protected readonly IMapper _mapper;
    private readonly ILogger<BaseEndpoint<TRequest, TResponse>> _logger;
    private readonly string _endpointName;
    
    /// <summary>
    /// Конструктор <see><cref>BaseEndpoint</cref></see>.
    /// </summary>
    /// <param name="loggerFactory">Фабрика логгирования.</param>
    protected BaseEndpoint(string endpointName, IServiceProvider serviceProvider)
    {
        _endpointName = endpointName;
        _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<BaseEndpoint<TRequest, TResponse>>();
        _mapper = serviceProvider.GetRequiredService<IMapper>();
    }

    /// <summary>
    /// Обработать конечную точку.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ типа <see cref="TResponse"/>.</returns>
    public virtual async Task<ActionResult<BaseResponse<TResponse>>> HandleAsync(TRequest request, CancellationToken cancellationToken = new())
    {
        _logger.LogEndpointCall(_endpointName);
        var response = new BaseResponse<TResponse>();
        try
        {
            response.Content = await HandleEndpoint(request, cancellationToken);

            if (response.IsError)
                return BadRequest(response.Message);

            return Ok(response);
        }
        catch (NotFoundException e)
        {
            return BuildErrorResponse(response, e, HttpStatusCode.NotFound);
        }
        catch (Exception e)
        {
            return BuildErrorResponse(response, e, HttpStatusCode.BadRequest);
        }
    }

    private ActionResult<BaseResponse<TResponse>> BuildErrorResponse(
        BaseResponse<TResponse> response,
        Exception exception,
        HttpStatusCode statusCode)
    {
        _logger.LogError(exception.Message, exception);
        response.IsError = true;
        response.Message = exception.Message;
        var badResponse = new ObjectResult(response)
        {
            StatusCode = (int)statusCode
        };
        return badResponse;
    }

    /// <summary>
    /// Обработать конечную точку.
    /// </summary>
    /// <param name="request">Запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ типа <see cref="TResponse"/>.</returns>
    protected abstract Task<TResponse> HandleEndpoint(TRequest request, CancellationToken cancellationToken = new());
}