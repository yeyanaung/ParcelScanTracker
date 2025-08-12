using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
namespace ParcelScanTracker.Common.EnumType
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ScanEvent
    {
        /// <summary>
        /// Unknown scan event type.
        /// </summary>
        [EnumMember(Value = "UNKNOWN")]
        Unknown = 0,

        /// <summary>
        /// Parcel has been picked up.
        /// </summary>
        [EnumMember(Value = "PICKUP")]
        PICKUP = 1,

        /// <summary>
        /// Status update scan event.
        /// </summary>
        [EnumMember(Value = "STATUS")]
        STATUS = 2,

        /// <summary>
        /// Parcel has been delivered.
        /// </summary>
        [EnumMember(Value = "DELIVERY")]
        DELIVERY = 3
    }
}
