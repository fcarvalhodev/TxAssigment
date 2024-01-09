using Cabinet;
using Grpc.Core;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Strategies.Cabinets;
using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace TxAssignmentGRPC.Services;

public class CabinetService : CabinetServiceImp.CabinetServiceImpBase
{
    private readonly IRepositoryCabinet _repositoryCabinet;

    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    private readonly IStrategyCreateCabinetOperation _strategyCreateCabinetOperation;
    private readonly IStrategyUpdateCabinetOperation _strategyUpdateCabinetOperation;
    private readonly IStrategyDeleteCabinetOperation _strategyDeleteCabinetOperation;

    public CabinetService(IRepositoryCabinet repositoryCabinet, IMapper mapper, ILogger<CabinetService> logger,
        IStrategyCreateCabinetOperation strategyCreateCabinetOperation,
        IStrategyUpdateCabinetOperation strategyUpdateCabinetOperation,
        IStrategyDeleteCabinetOperation strategyDeleteCabinetOperation)
    {
        _mapper = mapper;
        _repositoryCabinet = repositoryCabinet;
        _logger = logger;

        _strategyCreateCabinetOperation = strategyCreateCabinetOperation;
        _strategyUpdateCabinetOperation = strategyUpdateCabinetOperation;
        _strategyDeleteCabinetOperation = strategyDeleteCabinetOperation;
    }

    public override async Task<ServiceResponse> CreateCabinet(ModelCabinet request, ServerCallContext context)
    {
        try
        {
            var cabinet = _mapper.Map<TxAssignmentServices.Models.ModelCabinet>(request);
            var result = await _strategyCreateCabinetOperation.ExecuteAsync(cabinet);

            if (result.Success)
                return new ServiceResponse { Success = result.Success, Message = result.Message };
            else
                return new ServiceResponse { Success = result.Success, Message = result.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a cabinet.");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<ServiceResponse> UpdateCabinet(UpdateCabinetRequest request, ServerCallContext context)
    {
        try
        {
            var cabinet = _mapper.Map<TxAssignmentServices.Models.ModelCabinet>(request.Cabinet);
            var result = await _strategyUpdateCabinetOperation.ExecuteAsync(Guid.Parse(request.Id), cabinet);

            if (result.Success)
                return new ServiceResponse { Success = result.Success, Message = result.Message };
            else
                return new ServiceResponse { Success = result.Success, Message = result.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a cabinet.");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<ServiceResponse> DeleteCabinet(CabinetIdRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _strategyDeleteCabinetOperation.ExecuteAsync(Guid.Parse(request.Id));
            if (result.Success)
                return new ServiceResponse { Success = result.Success, Message = result.Message };
            else
                return new ServiceResponse { Success = result.Success, Message = result.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while delete the cabinet.");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<ServiceResponse> GetAllCabinets(EmptyRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _repositoryCabinet.GetAllCabinets();
    
            if (!result.Success)
            {
                var failMessage = new StringValue { Value = JsonConvert.SerializeObject(result.Message) };
                return new ServiceResponse { Success = result.Success,Data = Google.Protobuf.WellKnownTypes.Any.Pack(failMessage), Message = result.Message };
            }
    
            var modelCabinetsJson = JsonConvert.SerializeObject(result.Data);
            var cabinetsStringValue = new StringValue { Value = modelCabinetsJson };
            var anyData = Google.Protobuf.WellKnownTypes.Any.Pack(cabinetsStringValue);
    
            return new ServiceResponse { Success = result.Success,Data = Google.Protobuf.WellKnownTypes.Any.Pack(anyData), Message = result.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all cabinets.");
            var exceptionMessage = new StringValue { Value = ex.Message };
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }

    public override async Task<ServiceResponse> GetCabinetById(CabinetIdRequest request, ServerCallContext context)
    {
        try
        {
            var result = await _repositoryCabinet.GetCabinetById(Guid.Parse(request.Id));

            if (result.Success)
            {
                var jsonString = JsonConvert.SerializeObject(result.Data);
                var stringValue = new StringValue { Value = jsonString };
                var anyData = Google.Protobuf.WellKnownTypes.Any.Pack(stringValue);

                return new ServiceResponse { Success = true, Data = anyData, Message = "Success" };
            }

            return new ServiceResponse { Success = result.Success, Message = result.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred trying to get the cabinet.");
            return new ServiceResponse { Success = false, Message = ex.Message };
        }
    }
}