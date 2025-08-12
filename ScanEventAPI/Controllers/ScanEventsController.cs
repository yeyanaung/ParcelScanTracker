using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ScanEventAPI.Models;

namespace ScanEventAPI.Controllers
{
    [ApiController]
    public class ScanEventsController : ControllerBase
    {
        private string JsonFilePath = @"Jsons\ScanEvent.json";

        [HttpGet]
        [Route("v1/scans/scanevents")]
        public IActionResult GetScanEvents([FromQuery] int FromEventId = 1, [FromQuery] int Limit = 100)
        {
            if (!System.IO.File.Exists(JsonFilePath))
                return NotFound("Scan events file not found.");

            var json = System.IO.File.ReadAllText(JsonFilePath);
            var scanEventsRoot = JsonSerializer.Deserialize<ScanEventsRoot>(json);

            var filteredEvents = scanEventsRoot?.ScanEvents
                .Where(e => e.EventId >= FromEventId)
                .OrderBy(e => e.EventId)
                .Take(Limit)
                .ToList();

            return Ok(new { ScanEvents = filteredEvents });
        }

        [HttpGet]
        [Route("v1/scans/scanevents100")]
        public IActionResult GetScanEvents100([FromQuery] int FromEventId = 1, [FromQuery] int Limit = 100)
        {
            JsonFilePath = @"Jsons\ScanEvent100.json";
            if (!System.IO.File.Exists(JsonFilePath))
                return NotFound("Scan events file not found.");

            var json = System.IO.File.ReadAllText(JsonFilePath);
            var scanEventsRoot = JsonSerializer.Deserialize<ScanEventsRoot>(json);

            var filteredEvents = scanEventsRoot?.ScanEvents
                .Where(e => e.EventId >= FromEventId)
                .OrderBy(e => e.EventId)
                .Take(Limit)
                .ToList();

            return Ok(new { ScanEvents = filteredEvents });
        }

        [HttpGet]
        [Route("v1/scans/scanevents200")]
        public IActionResult GetScanEvents200([FromQuery] int FromEventId = 90000, [FromQuery] int Limit = 100)
        {
            JsonFilePath = @"Jsons\ScanEvent200.json";
            if (!System.IO.File.Exists(JsonFilePath))
                return NotFound("Scan events file not found.");

            var json = System.IO.File.ReadAllText(JsonFilePath);
            var scanEventsRoot = JsonSerializer.Deserialize<ScanEventsRoot>(json);

            var filteredEvents = scanEventsRoot?.ScanEvents
                .Where(e => e.EventId >= FromEventId)
                .OrderBy(e => e.EventId)
                .Take(Limit)
                .ToList();

            return Ok(new { ScanEvents = filteredEvents });
        }

        [HttpGet]
        [Route("v1/scans/scanevents600")]
        public IActionResult GetScanEvents600([FromQuery] int FromEventId = 90000, [FromQuery] int Limit = 100)
        {
            JsonFilePath = @"Jsons\ScanEvent600.json";
            if (!System.IO.File.Exists(JsonFilePath))
                return NotFound("Scan events file not found.");

            var json = System.IO.File.ReadAllText(JsonFilePath);
            var scanEventsRoot = JsonSerializer.Deserialize<ScanEventsRoot>(json);

            var filteredEvents = scanEventsRoot?.ScanEvents
                .Where(e => e.EventId >= FromEventId)
                .OrderBy(e => e.EventId)
                .Take(Limit)
                .ToList();

            return Ok(new { ScanEvents = filteredEvents });
        }
    }
}
