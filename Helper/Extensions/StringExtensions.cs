using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

namespace Helper.Extensions {
  public static class StringExtensions {
    public static string ToTitleCase(this string str) {
      Contract.Requires(!String.IsNullOrWhiteSpace(str));
      return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }

    [Pure]
    public static bool NotEmpty(this string str) {
      return !String.IsNullOrWhiteSpace(str);
    }

    [Pure]
    public static bool IsEmpty(this string str) {
      return String.IsNullOrWhiteSpace(str);
    }

    [Pure]
    public static bool IsUri(this string str) {
      Contract.Ensures(!Contract.Result<bool>() || str != null);
      return Uri.IsWellFormedUriString(str, UriKind.Absolute);
    }

    [Pure]
    public static string ToStringList(this string[] array) {
      var builder = new StringBuilder();
      foreach (string value in array) {
        builder.Append(value);
        builder.Append('|');
      }
      return builder.ToString();
    }
  }
}