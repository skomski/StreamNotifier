using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;

namespace Helper.Extensions {
  /// Contains methods to extend System.Windows.Forms.Form to have the ability to fade in and fade out
  public static class FormExtensions {
    /// Fade the form in
    public static void FadeIn(this Form form, int seconds) {
      Contract.Requires(form != null);
      Contract.Requires(seconds > 0);

      var fader = new Fader(form, seconds);
      fader.FadeIn();
    }

    /// Fade the form out
    public static void FadeOut(this Form form, Action fadedOut) {
      Contract.Requires(form != null);

      var fader = new Fader(form);
      fader.FadeOut(fadedOut);
    }
  }

  public class Fader {
    private readonly Timer _fadeTimer;
    private readonly Form _targetForm;
    private Action _fadedOut;
    private Double _opacityDelta;

    public Fader(Form targetForm) {
      Contract.Requires(targetForm != null);

      _targetForm = targetForm;
      _fadeTimer = new Timer {Interval = 40};
      _fadeTimer.Tick += FaderTimerTick;
      _opacityDelta = .05;
    }

    public Fader(Form targetForm, int seconds)
      : this(targetForm) {
      Contract.Requires(targetForm != null && seconds > 0);

      const int frameRate = 20;
      _opacityDelta = 1.0/(frameRate*seconds);
      _fadeTimer.Interval = 1000/frameRate;
    }

    private void FaderTimerTick(object sender, EventArgs e) {
      _targetForm.Opacity += _opacityDelta;
      if ((_opacityDelta > 0 && _targetForm.Opacity >= 1)) {
        _fadeTimer.Enabled = false;
      }
      else if (_opacityDelta < 0 && _targetForm.Opacity <= 0) {
        _fadeTimer.Enabled = false;
        _fadedOut();
      }
    }

    public void FadeIn() {
      _targetForm.Opacity = 0;
      _opacityDelta = Math.Abs(_opacityDelta);
      _fadeTimer.Enabled = true;
    }

    public void FadeOut(Action fadedOut) {
      _fadedOut = fadedOut;
      _opacityDelta = -1.0*Math.Abs(_opacityDelta);
      _fadeTimer.Enabled = true;
    }

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(_targetForm != null);
      Contract.Invariant(_fadeTimer != null);
    }
  }
}