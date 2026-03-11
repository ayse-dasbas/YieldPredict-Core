using Microsoft.AspNetCore.Mvc;
using YieldPredict.API.ML;
using YieldPredict.API.Services;

namespace YieldPredict.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelBookingController(PredictionService predictionService) : ControllerBase
{
    [HttpPost("predict")]
    public ActionResult<object> Predict([FromBody] HotelPredictionRequest request)
    {
        var input = new HotelData
        {
            Hotel = request.Hotel,
            IsCanceled = request.IsCanceled ? 1f : 0f,
            LeadTime = request.LeadTime,
            Adults = request.Adults,
            Children = request.Children,
            Meal = request.Meal,
            MarketSegment = request.MarketSegment,
            ReservedRoomType = request.ReservedRoomType,
            ArrivalDateMonth = request.ArrivalDateMonth,
            Adr = 0f
        };

        var prediction = predictionService.Predict(input);

        return Ok(new
        {
            PredictedAdr = prediction.Score
        });
    }
}

