using System;
using System.Collections.Generic;
using System.Text;

namespace MSA_MODULE2.Model
{
    class CognitiveResponse
    {
        public List<Tag> tags { get; set; }
        public Description description { get; set; }

        public class Tag
        {
            public string name { get; set; }
            public double confidence { get; set; }
        }
        public class Description
        {
            public List<string> tags { get; set; }
            public List<Caption> captions { get; set; }
        }
        public class Caption
        {
            public string text { get; set; }
            public double confidence { get; set; }
        }
    }
}
