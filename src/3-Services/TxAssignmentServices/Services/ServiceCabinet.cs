using AutoMapper;
using Microsoft.Extensions.Logging;
using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;

namespace TxAssignmentServices.Services
{
    public class ServiceCabinet : IServiceCabinet
    {
        private readonly IRepositoryCabinet _repositoryCabinet;
        private readonly IRepositoryProduct _repositoryProduct;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ServiceCabinet(IRepositoryCabinet repositoryCabinet, IRepositoryProduct repositoryProduct, IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _repositoryCabinet = repositoryCabinet;
            _logger = logger;
            _repositoryProduct = repositoryProduct;
        }

        public async Task<ServiceResponse> CreateCabinet(ModelCabinet cabinet)
        {
            try
            {
                if (cabinet == null)
                {
                    _logger.LogWarning("CreateCabinet called with null cabinet.");
                    return new ServiceResponse { Success = false, Message = "Cabinet cannot be null" };
                }

                if (ValidateRowsForCabinet(cabinet.Rows, cabinet.Size.Height))
                    return new ServiceResponse { Success = false, Message = "The total height of the rows is larger than the cabinet's height." };

                //Validations
                foreach (var row in cabinet.Rows)
                {
                    var validateLane = ValidateLaneWidth(row.Lanes, cabinet.Size.Width);
                    if (!validateLane.isValid)
                        return new ServiceResponse { Success = false, Message = validateLane.errorMessage };

                    //Validate Products
                    var validationResult = ValidateLaneProducts(row.Lanes, new HashSet<string>());
                    if (!validationResult.Success)
                    {
                        return validationResult;
                    }
                }

                var result = await _repositoryCabinet.CreateCabinet(_mapper.Map<Cabinet>(cabinet));

                if (result.Success)
                    return new ServiceResponse { Success = true, Message = "Cabinet created successfully." };
                else return new ServiceResponse { Success = false, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> UpdateCabinet(Guid IdCabinet, ModelCabinet newCabinet)
        {
            try
            {
                var cabinetEntity = await _repositoryCabinet.GetCabinetById(IdCabinet);

                if (cabinetEntity == null)
                    return new ServiceResponse { Success = false, Message = "Cabinet cannot be null" };


                if (ValidateRowsForCabinet(newCabinet.Rows, newCabinet.Size.Height))
                    return new ServiceResponse { Success = false, Message = "The total height of the rows is larger than the cabinet's height." };

                //Validations
                foreach (var row in newCabinet.Rows)
                {
                    var validateLane = ValidateLaneWidth(row.Lanes, newCabinet.Size.Width);
                    if (!validateLane.isValid)
                        return new ServiceResponse { Success = false, Message = validateLane.errorMessage };

                    //Validate Products
                    var validationResult = ValidateLaneProducts(row.Lanes, new HashSet<string>());
                    if (!validationResult.Success)
                    {
                        return validationResult;
                    }
                }

                var result = await _repositoryCabinet.UpdateCabinet(IdCabinet, _mapper.Map<Cabinet>(newCabinet));
                return result.Success ?
                    new ServiceResponse { Success = true, Message = "Cabinet updated successfully." } :
                    new ServiceResponse { Success = false, Message = result.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse> DeleteCabinet(Guid IdCabinet)
        {
            try
            {
                var result = await _repositoryCabinet.DeleteCabinet(IdCabinet);

                if (result.Success)
                    return new ServiceResponse { Success = true, Message = "Cabinet deleted successfully." };
                else return new ServiceResponse { Success = false, Message = string.Empty };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while delete the cabinet.");
                return new ServiceResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ModelCabinet>> GetCabinetById(Guid IdCabinet)
        {
            try
            {
                var result = await _repositoryCabinet.GetCabinetById(IdCabinet);

                if (result.Success)
                    return new ServiceResponse<ModelCabinet> { Success = true, Data = _mapper.Map<ModelCabinet>(result.Data), Message = "Cabinet retrieved successfully." };
                else
                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "Cabinet not found." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred trying to get the cabinet.");
                return new ServiceResponse<ModelCabinet> { Success = false, Message = ex.Message };
            }

        }


        #region .: Validations :.

        private ServiceResponse ValidateLaneProducts(IEnumerable<ModelLane> lanes, HashSet<string> seenJanCodes)
        {
            foreach (var lane in lanes)
            {
                var productResponse = _repositoryProduct.GetProductByJanCode(lane.JanCode);
                if (productResponse == null)
                {
                    return new ServiceResponse { Success = false, Message = $"The product with JanCode {lane.JanCode} was not found in the database, you must register the product first." };
                }

                if (!seenJanCodes.Add(lane.JanCode))
                {
                    return new ServiceResponse { Success = false, Message = $"The product with JanCode {lane.JanCode} is duplicated, please remove from one of the lanes." };
                }
            }
            return new ServiceResponse { Success = true };
        }

        private bool ValidateRowsForCabinet(List<ModelRow> rows, int cabinetHeight)
        {
            return rows.Sum(me => me.Size.Height) > cabinetHeight;
        }

        private (bool isValid, string errorMessage) ValidateLaneWidth(List<ModelLane> lanes, int cabinetWidth)
        {
            int totalLaneWidth = 0;
            var orderedLanes = lanes.OrderBy(l => l.PositionX).ToList();

            for (int i = 0; i < orderedLanes.Count; i++)
            {
                int laneWidth;
                if (i == orderedLanes.Count - 1) 
                {
                    laneWidth = cabinetWidth - orderedLanes[i].PositionX;
                }
                else
                {
                    laneWidth = orderedLanes[i + 1].PositionX - orderedLanes[i].PositionX;
                }

                if (laneWidth < 0)
                {
                    return (false, "Invalid lane configuration: Overlapping lanes.");
                }

                totalLaneWidth += laneWidth;
            }

            if (totalLaneWidth > cabinetWidth)
            {
                return (false, "The total width of the lanes is larger than the cabinet's width.");
            }

            return (true, string.Empty);
        }


        private bool ValidateLaneWidth(int totalLaneWidth, int cabinetWidth)
        {
            return totalLaneWidth > cabinetWidth;
        }

        #endregion
    }
}

