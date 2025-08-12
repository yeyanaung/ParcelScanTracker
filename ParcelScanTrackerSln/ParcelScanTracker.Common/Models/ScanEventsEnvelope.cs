using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelScanTracker.Common.Models
{
    /// <summary>
    /// Represents the envelope for scan event API responses.
    /// Used to deserialize responses containing a collection of parcel scan events.
    /// </summary>
    public class ScanEventsEnvelope
    {
        /// <summary>
        /// The list of scan events returned from the API.
        /// Each item contains details about a parcel scan event.
        /// </summary>
        public List<ParcelScanEvent> ScanEvents { get; set; }
    }
}
