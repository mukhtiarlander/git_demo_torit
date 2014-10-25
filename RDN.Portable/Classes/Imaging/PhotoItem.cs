using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RDN.Portable.Classes.Imaging
{
    [ProtoContract]
    [DataContract]
    public class PhotoItem
    {
        public PhotoItem(string url, bool isPrimary, string altText)
        {
            this.Alt = altText;
            this.ImageUrl = url;
            this.IsPrimaryPhoto = isPrimary;

        }
        public PhotoItem(string url, string thumbUrl, bool isPrimary, string altText)
        {
            this.Alt = altText;
            this.ImageThumbUrl = thumbUrl;
            this.ImageUrl = url;
            this.IsPrimaryPhoto = isPrimary;
        }

        public PhotoItem(int photoId, string url, string thumbUrl, bool isPrimary, string altText)
        {
            this.PhotoId = photoId;
            this.Alt = altText;
            this.ImageUrl = url;
            this.IsPrimaryPhoto = isPrimary;
            this.ImageThumbUrl = thumbUrl;
        }
        public PhotoItem()
        { }
        [ProtoMember(1)]
        [DataMember]
        public int PhotoId { get; set; }
        [ProtoMember(2)]
        [DataMember]
        public string ImageThumbUrl { get; set; }
        [ProtoMember(3)]
        [DataMember]
        public string ImageUrl { get; set; }
        [ProtoMember(4)]
        [DataMember]
        public bool IsPrimaryPhoto { get; set; }
        [ProtoMember(5)]
        [DataMember]
        public string Alt { get; set; }
    }
}
