namespace Cyclon.Utils.Objects {

    public class FacebookProfile {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id { get; set; }
        public DataImage picture_large { get; set; }

    }

    public class DataImage {
        public ImageProperties data { get; set; }
    }

    public class ImageProperties {
        public int height { get; set; }
        public int width { get; set; }
        public bool is_silhouette { get; set; }
        public string url { get; set; }
    }

}
