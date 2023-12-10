using TxAssignmentInfra.Entities;
using TxAssignmentInfra.Repositories;
using TxAssignmentServices.Models;
using TxAssignmentServices.Services;

namespace TxAssignmentServices.Strategies.Cabinets
{
    internal static class HelpersCabinets
    {
        internal static (bool isValid, string errorMessage) ValidateLaneWidth(List<ModelLane> lanes, int cabinetWidth)
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

        internal static bool ValidateRowsForCabinet(List<ModelRow> rows, int cabinetHeight)
        {
            return rows.Sum(me => me.Size.Height) > cabinetHeight;
        }

        internal static ServiceResponse ValidateLaneProducts(IEnumerable<ModelLane> lanes, RepositoryResponse<List<Product>> productResponse)
        {

            if (!productResponse.Success)
                return new ServiceResponse { Success = false, Message = $"Not able to fetch the produts" };

            if (productResponse.Data.Count <= 0)
                return new ServiceResponse { Success = false, Message = $"There are no products on the database, please insert a product to continue" };

            HashSet<string> uniqueProductJanCodes = new HashSet<string>();

            foreach (var lane in lanes)
            {
                var product = productResponse.Data.FirstOrDefault(me => me.JanCode.Equals(lane.JanCode));
                if (product == null)
                    return new ServiceResponse { Success = false, Message = $"The product with JanCode {lane.JanCode} was not found in the database, you must register the product first." };

                if (!uniqueProductJanCodes.Add(lane.JanCode))
                    return new ServiceResponse { Success = false, Message = $"The product with JanCode {lane.JanCode} appears more than once in the lanes." };
                
            }

            return new ServiceResponse { Success = true };
        }

        internal static (bool isValid, string message) ModelIsValid(ModelCabinet cabinet)
        {
            if (cabinet == null)
                return (false, "The model can not be empty");

            if (cabinet.Number <= 0)
                return (false, "The number of the cabinet must be bigger than 0");

            if (cabinet.Position.Y < 0)
                return (false, "The position X can not be negative");

            if (cabinet.Position.X < 0)
                return (false, "The position X can not be negative");

            if (cabinet.Position.Z < 0)
                return (false, "The position Z can not be negative");

            if (cabinet.Size.Width < 0)
                return (false, "The position Width can not be negative");

            if (cabinet.Size.Height < 0)
                return (false, "The position Height can not be negative");

            if (cabinet.Size.Depth < 0)
                return (false, "The position Depth can not be negative");

            return (true, "");
        }
    }
}