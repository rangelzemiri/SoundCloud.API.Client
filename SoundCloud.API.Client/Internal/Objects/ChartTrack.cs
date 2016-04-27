﻿using Newtonsoft.Json;
using SoundCloud.API.Client.Objects;
using System.Collections.Generic;

namespace SoundCloud.API.Client.Internal.Objects
{
    internal class ChartTrack
    {
        [JsonProperty(PropertyName = "track")]
        public Track Track { get; set; }

    }
}
