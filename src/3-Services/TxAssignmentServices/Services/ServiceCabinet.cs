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


        public async Task<ServiceResponse> UpdateCabinet(Guid IdCabinet, ModelCabinet updatedCabinetModel)
        {
            try
            {
                var existingCabinetResponse = await _repositoryCabinet.GetCabinetById(IdCabinet);
                if (!existingCabinetResponse.Success)
                    return new ServiceResponse { Success = false, Message = "Cabinet not found." };

                var existingCabinet = _mapper.Map<ModelCabinet>(existingCabinetResponse.Data);

                foreach (var updatedRow in updatedCabinetModel.Rows)
                {
                    foreach (var updatedLane in updatedRow.Lanes)
                    {
                        foreach (var updatedProduct in updatedLane.Products)
                        {
                            var existingProduct = FindProductByJanCode(existingCabinet, updatedProduct.JanCode);
                            if (existingProduct != null)
                            {
                                if (!IsProductInCorrectLane(existingCabinet, updatedProduct.JanCode, updatedLane.Id))
                                {
                                    if (CanProductBeMovedToLane(updatedRow, updatedLane, updatedProduct))
                                    {
                                        MoveProductToLane(existingCabinet, updatedProduct, updatedLane.Id);
                                    }
                                    else
                                    {
                                        return new ServiceResponse { Success = false, Message = $"No space in lane for product {updatedProduct.JanCode}." };
                                    }
                                }
                            }
                            else
                            {
                                // If the product is new, try to add it to an existing or new lane
                                if (!TryAddProductToExistingOrNewLane(existingCabinet, updatedProduct))
                                {
                                    return new ServiceResponse { Success = false, Message = $"No space in lane for new product {updatedProduct.JanCode}." };
                                }
                            }
                        }
                    }
                }

                var updateResult = await _repositoryCabinet.UpdateCabinet(IdCabinet, _mapper.Map<Cabinet>(existingCabinet));
                return updateResult.Success ?
                    new ServiceResponse { Success = true, Message = "Cabinet updated successfully." } :
                    new ServiceResponse { Success = false, Message = "Failed to update cabinet." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when trying to update the cabinet.");
                return new ServiceResponse<ModelCabinet> { Success = false, Message = ex.Message };
            }
        }

        private ModelProduct FindProductByJanCode(ModelCabinet cabinet, string janCode)
        {
            foreach (var row in cabinet.Rows)
            {
                foreach (var lane in row.Lanes)
                {
                    var product = lane.Products.FirstOrDefault(p => p.JanCode == janCode);
                    if (product != null)
                    {
                        return product;
                    }
                }
            }
            return null; // Return null if product not found
        }

        private bool IsProductInCorrectLane(ModelCabinet cabinet, string janCode, Guid laneId)
        {
            return cabinet.Rows.SelectMany(row => row.Lanes)
                               .Any(lane => lane.Id == laneId && lane.Products.Any(p => p.JanCode == janCode));
        }

        private bool CanProductBeMovedToLane(ModelRow row, ModelLane lane, ModelProduct product)
        {
            double totalSpaceUsed = lane.Products.Sum(p => p.Width);
            double availableSpace = row.Size.Width - totalSpaceUsed;
            return availableSpace >= product.Width;
        }

        private void MoveProductToLane(ModelCabinet cabinet, ModelProduct product, Guid targetLaneId)
        {
            // Remove product from current lane
            foreach (var row in cabinet.Rows)
            {
                foreach (var lane in row.Lanes)
                {
                    lane.Products.RemoveAll(p => p.JanCode == product.JanCode);
                }
            }

            // Add product to target lane
            var targetLane = cabinet.Rows.SelectMany(r => r.Lanes).FirstOrDefault(l => l.Id == targetLaneId);
            targetLane?.Products.Add(product);
        }

        private bool CanProductBeAddedToLane(ModelCabinet cabinet, ModelLane lane, ModelProduct product)
        {
            // Calculate the total width used by the products in the lane.
            double totalWidthUsed = lane.Products.Sum(p => p.Width);
            // The available width is the cabinet's width minus the used width.
            double availableWidth = cabinet.Size.Width - totalWidthUsed;
            // The product can be added if there's enough available width in the lane.
            return availableWidth >= product.Width;
        }

        private bool TryAddProductToExistingOrNewLane(ModelCabinet cabinet, ModelProduct product)
        {
            foreach (var row in cabinet.Rows)
            {
                foreach (var lane in row.Lanes)
                {
                    if (CanProductBeAddedToLane(cabinet, lane, product))
                    {
                        lane.Products.Add(product);
                        return true;
                    }
                }

                // No space in existing lanes, try to add a new lane if the total lanes' width does not exceed the cabinet width
                double totalLanesWidth = row.Lanes.Sum(l => l.Width);
                if (cabinet.Size.Width - totalLanesWidth >= product.Width)
                {
                    // The depth of the product must also not exceed the cabinet's depth
                    if (product.Depth <= cabinet.Size.Depth)
                    {
                        row.Lanes.Add(new ModelLane
                        {
                            Products = new List<ModelProduct> { product }
                        });
                        return true;
                    }
                }
            }

            // No space found or could not add a new lane within the cabinet's constraints
            return false;
        }


    }
}

