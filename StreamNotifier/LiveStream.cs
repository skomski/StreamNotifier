using System;
using System.Diagnostics.Contracts;
using Helper.Extensions;

namespace StreamNotifier {
  [Serializable]
  public abstract class LiveStream {
    [NonSerialized] private string _eventDescription;
    [NonSerialized] private Uri _url;
    [NonSerialized] private int _viewer;

    protected LiveStream(string name, string identifier) {
      Contract.Requires(name.NotEmpty());
      Contract.Requires(identifier.NotEmpty());

      Name = name;
      Identifier = identifier;
    }

    public string Name { get; set; }
    public string Identifier { get; set; }

    public string EventDescription {
      get { return _eventDescription; }
      protected set { _eventDescription = value; }
    }

    public int Viewer {
      get { return _viewer; }
      protected set { _viewer = value; }
    }

    public Uri Url {
      get { return _url; }
      protected set { _url = value; }
    }

    public bool IsLive {
      get { return Viewer > 0; }
    }

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(Name.NotEmpty());
      Contract.Invariant(Identifier.NotEmpty());
    }
  }

  [Serializable]
  public class TwitchTVStream : LiveStream {
    public TwitchTVStream(string identifier, int viewers, string eventDescription, string url)
      : base(identifier, identifier) {
      Contract.Requires(identifier.NotEmpty());

      Viewer = viewers;
      EventDescription = eventDescription;
      Url = new Uri(url);
    }
  }
}