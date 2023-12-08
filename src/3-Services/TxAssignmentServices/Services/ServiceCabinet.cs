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
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ServiceCabinet(IRepositoryCabinet repositoryCabinet, IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _repositoryCabinet = repositoryCabinet;
            _logger = logger;
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

        public async Task<ServiceResponse> UpadteCabinet(Guid IdCabinet, ModelCabinet cabinet)
        {
            try
            {
                var result = await _repositoryCabinet.UpadteCabinet(IdCabinet, _mapper.Map<Cabinet>(cabinet));

                if (result.Success)
                    return new ServiceResponse { Success = true, Message = "Cabinet updated successfully." };
                else
                    return new ServiceResponse { Success = false, Message = "Failed to update cabinet." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when try to update the cabinet.");
                return new ServiceResponse<ModelCabinet> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ServiceResponse<ModelCabinet>> GetEmptySpaceOnCabinet(Guid cabinetId, ModelProduct newProduct)
        {
            try
            {
                // Use _cabinetRepository to retrieve the cabinet
                var cabinetResponse = await _repositoryCabinet.GetCabinetById(cabinetId);
                if (!cabinetResponse.Success)
                {
                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "Cabinet not found." };
                }

                var cabinet = cabinetResponse.Data;

                // Implement the logic to find the appropriate row for the product based on the height
                // and type, and then find the space in the lane for this row.
                var row = _mapper.Map<ModelRow>(FindRowForProduct(_mapper.Map<ModelCabinet>(cabinet), newProduct));
                if (row == null)
                {
                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "No suitable row found for product." };
                }

                var (lane, positionX) = FindSpaceInLaneForProduct(row, newProduct);
                if (lane == null || positionX == null)
                {
                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "No empty space found in any lane." };
                }

                // If a space is found, possibly update the cabinet with the new product's position
                // This may involve adding the product to the lane's product list and saving the cabinet
                // back to the database via the _cabinetRepository.

                // Return the cabinet with the updated space information
                return new ServiceResponse<ModelCabinet>
                {
                    Success = true,
                    Data = _mapper.Map<ModelCabinet>(cabinet),
                    Message = "Empty space found."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when look for a empty space.");
                return new ServiceResponse<ModelCabinet> { Success = false, Message = ex.Message };
            }
        }

        private ModelRow? FindRowForProduct(ModelCabinet cabinet, ModelProduct product)
        {
            foreach (var row in cabinet.Rows.OrderBy(r => r.PositionZ))
            {
                if (row.Size.Height >= product.Height) // Ensure the row can accommodate the product's height
                {
                    var productTypeExists = row.Lanes.Any(lane => lane.Products.Any(p => p.JanCode == product.JanCode));
                    if (productTypeExists)
                    {
                        return row;
                    }
                }
            }
            return null;
        }

        private (ModelLane? lane, double? positionX) FindSpaceInLaneForProduct(ModelRow row, ModelProduct newProduct)
        {
            foreach (var lane in row.Lanes)
            {
                double currentPositionX = lane.PositionX;
                double laneEndPositionX = lane.PositionX + row.Size.Width;

                // If lane is empty, place the product at the start of the lane
                if (!lane.Products.Any())
                {
                    return (lane, currentPositionX);
                }

                // Check for space within the current lane
                double availableSpace = CalculateAvailableSpaceInLane(lane, laneEndPositionX);
                if (availableSpace >= newProduct.Width)
                {
                    double positionToPlaceProduct = FindPositionToPlaceProduct(lane, newProduct.Width);
                    if (positionToPlaceProduct >= 0)
                    {
                        return (lane, positionToPlaceProduct);
                    }
                }
            }

            // Check adjacent lanes if no space is found in the current lane
            foreach (var adjacentLane in row.Lanes)
            {
                double availableSpace = CalculateAvailableSpaceInLane(adjacentLane, row.Size.Width + adjacentLane.PositionX);
                if (availableSpace >= newProduct.Width)
                {
                    double positionToPlaceProduct = FindPositionToPlaceProduct(adjacentLane, newProduct.Width);
                    if (positionToPlaceProduct >= 0)
                    {
                        return (adjacentLane, positionToPlaceProduct);
                    }
                }
            }

            return (null, null);
        }

        private double CalculateAvailableSpaceInLane(ModelLane lane, double laneEndPositionX)
        {
            double totalUsedSpace = lane.Products.Sum(p => p.Width);
            return laneEndPositionX - totalUsedSpace;
        }

        private double FindPositionToPlaceProduct(ModelLane lane, double productWidth)
        {
            double currentPositionX = lane.PositionX;
            foreach (var product in lane.Products.OrderBy(p => p.Width))
            {
                double spaceAfterCurrentProduct = currentPositionX + product.Width;
                if (spaceAfterCurrentProduct >= productWidth)
                {
                    return currentPositionX;
                }
                currentPositionX += product.Width;
            }

            return -1;
        }

    }
}

