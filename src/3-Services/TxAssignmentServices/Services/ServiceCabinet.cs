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
                                if (CanProductBeAddedToLane(updatedRow, updatedLane, updatedProduct))
                                {
                                    updatedLane.Products.Add(updatedProduct);
                                }
                                else
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


        private bool CanProductBeAddedToLane(ModelRow row, ModelLane lane, ModelProduct product)
        {
            double totalSpaceUsed = lane.Products.Sum(p => p.Width);
            double availableSpace = row.Size.Width - totalSpaceUsed;
            return availableSpace >= product.Width;
        }

        //public async Task<ServiceResponse<ModelCabinet>> IncludeProductIntoCabinet(Guid cabinetId, ModelProduct newProduct, ModelCabinet cabinetParam = null)
        //{
        //    try
        //    {
        //        var cabinet = new ModelCabinet();
        //        if (cabinetParam == null)
        //        {
        //            var cabinetResponse = await _repositoryCabinet.GetCabinetById(cabinetId);
        //            if (!cabinetResponse.Success)
        //            {
        //                return new ServiceResponse<ModelCabinet> { Success = false, Message = "Cabinet not found." };
        //            }

        //            cabinet = _mapper.Map<ModelCabinet>(cabinetResponse.Data);

        //        }
        //        else
        //        {
        //            cabinet = cabinetParam;
        //        }

        //        // First, try to find a row with the same product type (JanCode)
        //        var rowWithSameProduct = FindRowWithSameProductType(cabinet, newProduct.JanCode);
        //        if (rowWithSameProduct != null)
        //        {
        //            var (lane, positionX) = FindSpaceInLaneForProduct(rowWithSameProduct, newProduct);
        //            if (lane != null && positionX.HasValue)
        //            {
        //                // Update cabinet and return response
        //                // Find and update the lane in the cabinet
        //                foreach (var row in cabinet.Rows)
        //                {
        //                    var targetLane = row.Lanes.FirstOrDefault(l => l.Id == lane.Id);
        //                    if (targetLane != null)
        //                    {
        //                        targetLane.Products.Add(newProduct);
        //                        break; // Break if the correct lane is found and updated
        //                    }
        //                }

        //                // Update the entire cabinet in the repository
        //                var result = await _repositoryCabinet.UpdateCabinet(cabinet.Id, _mapper.Map<Cabinet>(cabinet));
        //                if (result.Success)
        //                    return new ServiceResponse<ModelCabinet> { Success = true, Message = "Cabinet updated successfully." };
        //                else
        //                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "Failed to update cabinet." };
        //            }
        //        }

        //        // If no suitable row is found or no space in the lane, find a new suitable row
        //        var newRow = FindNewRowForProduct(cabinet, newProduct);
        //        if (newRow != null)
        //        {
        //            var (newLane, newPositionX) = FindSpaceInLaneForProduct(newRow, newProduct);
        //            if (newLane != null && newPositionX.HasValue)
        //            {
        //                // Add the new product to the identified lane
        //                newLane.Products.Add(newProduct);

        //                // Now, find and update this lane in the cabinet model
        //                foreach (var row in cabinet.Rows)
        //                {
        //                    var targetLane = row.Lanes.FirstOrDefault(l => l.Id == newLane.Id);
        //                    if (targetLane != null)
        //                    {
        //                        targetLane.Products = newLane.Products;
        //                        break;
        //                    }
        //                }

        //                // Save the updated cabinet to the repository
        //                var result = await _repositoryCabinet.UpdateCabinet(cabinet.Id, _mapper.Map<Cabinet>(cabinet));
        //                if (result.Success)
        //                    return new ServiceResponse<ModelCabinet> { Success = true, Message = "Cabinet updated successfully." };
        //                else
        //                    return new ServiceResponse<ModelCabinet> { Success = false, Message = "Failed to update cabinet." };
        //            }
        //        }

        //        return new ServiceResponse<ModelCabinet> { Success = false, Message = "No empty space found in any lane." };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred when looking for empty space.");
        //        return new ServiceResponse<ModelCabinet> { Success = false, Message = ex.Message };
        //    }
        //}

        //private ModelRow FindRowWithSameProductType(ModelCabinet cabinet, string janCode)
        //{
        //    foreach (var row in cabinet.Rows)
        //    {
        //        foreach (var lane in row.Lanes)
        //        {
        //            if (lane.Products.Any(product => product.JanCode == janCode))
        //            {
        //                return row;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private ModelRow FindNewRowForProduct(ModelCabinet cabinet, ModelProduct product)
        //{
        //    // Assuming we prefer to place the product in the lowest empty row that can accommodate its height
        //    foreach (var row in cabinet.Rows.OrderBy(r => r.PositionZ))
        //    {
        //        bool isRowEmptyOrHasSpace = !row.Lanes.Any() || row.Lanes.Any(lane => lane.Products.Sum(p => p.Width) < row.Size.Width);
        //        if (row.Size.Height >= product.Height && isRowEmptyOrHasSpace)
        //        {
        //            return row;
        //        }
        //    }
        //    return null; // Return null if no suitable row is found
        //}

        //private (ModelLane? lane, double? positionX) FindSpaceInLaneForProduct(ModelRow row, ModelProduct newProduct)
        //{
        //    foreach (var lane in row.Lanes)
        //    {
        //        double currentPositionX = lane.PositionX;
        //        double laneEndPositionX = lane.PositionX + row.Size.Width;

        //        // If lane is empty, place the product at the start of the lane
        //        if (!lane.Products.Any())
        //        {
        //            return (lane, currentPositionX);
        //        }

        //        // Check for space within the current lane
        //        double availableSpace = CalculateAvailableSpaceInLane(lane, laneEndPositionX);
        //        if (availableSpace >= newProduct.Width)
        //        {
        //            double positionToPlaceProduct = FindPositionToPlaceProduct(lane, newProduct.Width);
        //            if (positionToPlaceProduct >= 0)
        //            {
        //                return (lane, positionToPlaceProduct);
        //            }
        //        }
        //    }

        //    // Check adjacent lanes if no space is found in the current lane
        //    foreach (var adjacentLane in row.Lanes)
        //    {
        //        double availableSpace = CalculateAvailableSpaceInLane(adjacentLane, row.Size.Width + adjacentLane.PositionX);
        //        if (availableSpace >= newProduct.Width)
        //        {
        //            double positionToPlaceProduct = FindPositionToPlaceProduct(adjacentLane, newProduct.Width);
        //            if (positionToPlaceProduct >= 0)
        //            {
        //                return (adjacentLane, positionToPlaceProduct);
        //            }
        //        }
        //    }

        //    return (null, null);
        //}

        //private double CalculateAvailableSpaceInLane(ModelLane lane, double laneEndPositionX)
        //{
        //    double totalUsedSpace = lane.Products.Sum(p => p.Width);
        //    return laneEndPositionX - totalUsedSpace;
        //}

        //private double FindPositionToPlaceProduct(ModelLane lane, double productWidth)
        //{
        //    double currentPositionX = lane.PositionX;
        //    foreach (var product in lane.Products.OrderBy(p => p.Width))
        //    {
        //        double spaceAfterCurrentProduct = currentPositionX + product.Width;
        //        if (spaceAfterCurrentProduct >= productWidth)
        //        {
        //            return currentPositionX;
        //        }
        //        currentPositionX += product.Width;
        //    }

        //    return -1;
        //}

    }
}

