using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Helper.Extensions;
using StreamNotifier.Properties;

using RegameStream = response;
using Own3dStream = own3dReply;
using JustinStream = streams;

namespace StreamNotifier
{
    [Serializable]
    public class LiveStream
    {
        public string Name { get; set; }
        public string Identifier { get; set; }
        public StreamVendor Vendor { get; set; }

         [NonSerialized]
        private string _eventDescription;
        public string EventDescription
        {
            get { return _eventDescription; }
            private set { _eventDescription = value; }
        }

         [NonSerialized]
        private int _viewer;
        public int Viewer
        {
            get { return _viewer; }
            private set { _viewer = value; }
        }

         [NonSerialized]
        private Uri _url;
        public Uri URL
        {
            get { return _url; }
            private set { _url = value; }
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

        public enum StreamVendor
        {
            JustinTV = 0,
            Own3dTV = 1,
            RegameTV = 2
        }

        public LiveStream(string name,string identifier,StreamVendor vendor)
        {
            Contract.Requires(name.NotEmpty());
            Contract.Requires(identifier.NotEmpty());

            Name = name;
            Identifier = identifier;
            Vendor = vendor;
        }
        public LiveStream(string completeURL)
        {
            Contract.Requires(completeURL.IsUri());

            const string justinTVPattern = @"http://(.+)?justin\.tv/(?<id>.*)#?(.+)?";
            const string twitchTVPattern = @"http://(.+)?twitch\.tv/(?<id>.*)#?(.+)?";
            const string own3dTVPattern = @"http://(.+)?own3d.tv/live/(?<id>.*)/(?<name>.*)";
            const string regameTVPattern = @"http://(.+)?regame.tv/live/(?<id>.*)";

            if (Regex.IsMatch(completeURL, justinTVPattern, RegexOptions.Singleline))
            {
                Vendor = StreamVendor.JustinTV;
                Identifier = Regex.Match(completeURL, justinTVPattern, RegexOptions.Singleline).Groups["id"].Value;
                Name = Identifier;
            }
            else if (Regex.IsMatch(completeURL, twitchTVPattern, RegexOptions.Singleline))
            {
                Vendor = StreamVendor.JustinTV;
                Identifier = Regex.Match(completeURL, twitchTVPattern, RegexOptions.Singleline).Groups["id"].Value;
                Name = Identifier;
            }
            else if (Regex.IsMatch(completeURL, own3dTVPattern, RegexOptions.Singleline))
            {
                Vendor = StreamVendor.Own3dTV;
                var match = Regex.Match(completeURL, own3dTVPattern, RegexOptions.Singleline);

                Identifier = match.Groups["id"].Value;
                Name = match.Groups["name"].Value;
            }
            else if (Regex.IsMatch(completeURL, regameTVPattern, RegexOptions.Singleline))
            {
                Vendor = StreamVendor.RegameTV;
                Identifier = Regex.Match(completeURL, regameTVPattern, RegexOptions.Singleline).Groups["id"].Value;
                Name = Identifier;
            }
            else
            {
                throw new ArgumentOutOfRangeException("completeURL");
            }
        }
        public void Update()
        {
            switch (Vendor)
            {
                case StreamVendor.JustinTV:
                    {
                        var justinStream = FetchStreamData<JustinStream>(Resources.justinTVAPIURL + Identifier);
                        if (justinStream == null) return;

                            EventDescription = justinStream.stream.title.IsEmpty() ? "No event specified!" : justinStream.stream.title.Trim();
                            Viewer = Convert.ToInt32(justinStream.stream.channel_count);
                            URL = new Uri(Resources.justinTVURL + Identifier);
                        
                    }
                    break;
                case StreamVendor.Own3dTV:
                    {
                        var own3DStream = FetchStreamData<Own3dStream>(Resources.own3dTVAPIURL + Identifier);
                        if (own3DStream == null) return;

                        EventDescription = "Live Event!";
                        Viewer = own3DStream.liveEvent.isLive ? own3DStream.liveEvent.liveViewers : 0;
                        URL = new Uri(Resources.own3dTVURL + Identifier);
                    }
                    break;
                case StreamVendor.RegameTV:
                    {
                        var regameStream = FetchStreamData<RegameStream>(Resources.regameTVAPIURL + Identifier);
                        if (regameStream == null) return;

                        EventDescription = regameStream.stream.streamdescription.IsEmpty() ? "No event specified!" : regameStream.stream.streamdescription.Trim();
                        Viewer = regameStream.stream.online ? regameStream.stream.viewer : 0;
                        URL = new Uri(regameStream.stream.streamlink);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        static T FetchStreamData<T>(string uri) where T : class 
        {
            Contract.Requires(uri.IsUri());

            using (var xmlReader =  XmlReader.Create(uri, new XmlReaderSettings()))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                if (!xmlSerializer.CanDeserialize(xmlReader)) return null;
                return (T) xmlSerializer.Deserialize(xmlReader);
            }
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Name.NotEmpty());
            Contract.Invariant(Identifier.NotEmpty());
        }
    }
}
