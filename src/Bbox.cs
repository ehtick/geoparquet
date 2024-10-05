﻿namespace GeoParquet
{
    public class Bbox
    {
        public string[] xmax { get; set; }
        public string[] xmin { get; set; }
        public string[] ymax { get; set; }
        public string[] ymin { get; set; }
        public string[] zmin { get; set; }
        public string[] zmax { get; set; }


        private IDictionary<string, object> _additionalProperties;

        [System.Text.Json.Serialization.JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

    }
}