using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSA_MODULE2.Model
{
    public class ComputerVisionInfo
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }

        [JsonProperty(PropertyName = "correct")]
        public bool Correct { get; set; }
    }
}
