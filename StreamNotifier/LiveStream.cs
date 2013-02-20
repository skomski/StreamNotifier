using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Helper.Extensions;
using Newtonsoft.Json.Linq;
using StreamNotifier.Properties;

namespace StreamNotifier
{
    [Serializable]
    public abstract class LiveStream
    {
        public string Name { get; set; }
        public string Identifier { get; set; }

         [NonSerialized]
        private string _eventDescription;
        public string EventDescription
        {
            get { return _eventDescription; }
            protected set { _eventDescription = value; }
        }

         [NonSerialized]
        private int _viewer;
        public int Viewer
        {
            get { return _viewer; }
            protected set { _viewer = value; }
        }

         [NonSerialized]
        private Uri _url;
        public Uri Url
        {
            get { return _url; }
            protected set { _url = value; }
        }

        public bool IsLive
        {
            get { return Viewer > 0; }
        }

        [NonSerialized]
        private bool _isShowed;
        public bool IsShowed
        {
            get { return _isShowed; }
            set { _isShowed = value; }
        }

        protected LiveStream(string name, string identifier) {
          Contract.Requires(name.NotEmpty());
          Contract.Requires(identifier.NotEmpty());

          Name = name;
          Identifier = identifier;
        }

        public abstract void Update();

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Name.NotEmpty());
            Contract.Invariant(Identifier.NotEmpty());
        }
    }

    [Serializable]
    public class TwitchTVStream : LiveStream {
        private class JsonRootObject
        {
            internal class Stream
            {
                internal class Channel
                {
                    internal class Links
                    {
                        public string follows { get; set; }
                        public string chat { get; set; }
                        public string commercial { get; set; }
                        public string subscriptions { get; set; }
                        public string stream_key { get; set; }
                        public string editors { get; set; }
                        public string self { get; set; }
                        public string features { get; set; }
                        public string videos { get; set; }
                    }
                    public string display_name { get; set; }
                    public string video_banner { get; set; }
                    public List<object> teams { get; set; }
                    public string status { get; set; }
                    public string created_at { get; set; }
                    public string updated_at { get; set; }
                    public bool mature { get; set; }
                    public int _id { get; set; }
                    public string background { get; set; }
                    public string banner { get; set; }
                    public string name { get; set; }
                    public string logo { get; set; }
                    public string url { get; set; }
                    public Links _links { get; set; }
                    public string game { get; set; }
                }
                public string broadcaster { get; set; }
                public string preview { get; set; }
                public long _id { get; set; }
                public int viewers { get; set; }
                public Channel channel { get; set; }
                public string name { get; set; }
                public string game { get; set; }
            }
            public Stream stream { get; set; }
        }

        public TwitchTVStream(string identifier) : base(identifier, identifier) {
          Contract.Requires(identifier.NotEmpty());
        }

        public override void Update() {
          var client = new WebClient();
          var response = client.DownloadString(new Uri("https://api.twitch.tv/kraken/streams/" + Identifier));
          var jsonRoot = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonRootObject>(response);

          // Stream is offline if
          if (jsonRoot.stream == null) return;

          var stream = jsonRoot.stream;

          EventDescription = stream.channel.status;
          Viewer = stream.viewers;
          Url = new Uri(stream.channel._links.self);
        }
    }
}
