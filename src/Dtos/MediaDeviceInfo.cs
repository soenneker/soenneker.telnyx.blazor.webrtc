using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos
{
    public class MediaDeviceInfo
    {
        public string DeviceId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }
}
