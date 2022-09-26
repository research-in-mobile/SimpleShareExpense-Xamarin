using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Split.Entities
{
        public class Currency
        {
            [JsonProperty("rates")]
            public Dictionary<string, double> Rates { get; set; }

            [JsonProperty("base")]
            public string Base { get; set; }

            [JsonProperty("date")]
            public DateTimeOffset Date { get; set; }
        }
}
