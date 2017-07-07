using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSA_MODULE2.Model
{
    class CognitiveAccuracyDbModel
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "caption")]
        public float caption { get; set; }

        [JsonProperty(PropertyName = "correct")]
        public float isCorrect { get; set; }
    }
}
