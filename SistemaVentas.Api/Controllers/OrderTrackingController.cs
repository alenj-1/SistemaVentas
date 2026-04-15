using Microsoft.AspNetCore.Mvc;
using SistemaVentas.Api.Data;
using SistemaVentas.Api.Models;

namespace SistemaVentas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class OrderTrackingController : ControllerBase
    {
        // GET que permite obtener todos los tracking de pedidos
        [HttpGet]
        public ActionResult<List<OrderTrackingResponse>> GetAll()
        {
            var trackingList = OrderTrackingData.GetOrderTracking();

            return Ok(trackingList);
        }


        // GET que permite obtener un tracking según el id de la orden
        [HttpGet("{orderId}")]
        public ActionResult<OrderTrackingResponse> GetByOrderId(int orderId)
        {
            var tracking = OrderTrackingData
                .GetOrderTracking()
                .FirstOrDefault(x => x.OrderId == orderId);

            if (tracking == null)
            {
                return NotFound($"No tracking was found for the order {orderId}.");
            }

            return Ok(tracking);
        }
    }
}