// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Mqtt.Client.AspNetCore.Services;
//
//    var villasMessage = VillasMessage.FromJson(jsonString);

namespace Mqtt.Client.AspNetCore.Services
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class VillasMessage
    {
        [JsonProperty("data")]
        public List<double> Data { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }

        [JsonProperty("ts")]
        public Ts Ts { get; set; }
    }

    public partial class Ts
    {
        [JsonProperty("origin")]
        public List<long> Origin { get; set; }
    }

    public partial class VillasMessage
    {
        public static List<VillasMessage> FromJson(string json) => JsonConvert.DeserializeObject<List<VillasMessage>>(json, Mqtt.Client.AspNetCore.Services.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<VillasMessage> self) => JsonConvert.SerializeObject(self, Mqtt.Client.AspNetCore.Services.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
