using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using Newtonsoft.Json;

namespace Helper.Extensions {
  public class EnumStringConverter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
      // Get the field in the enum.
      FieldInfo field = value.GetType().GetField(value.ToString());

      // Check if this element has a StringValueAttribute.
      var attribute = field.GetCustomAttribute<StringValueAttribute>();

      // If it has an attribute, then return the specified text, otherwise use the default overload.
      writer.WriteValue(attribute != null ? attribute.Text : value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
      if (reader.TokenType == JsonToken.String) {
        for (int index = 0; index < objectType.GetFields().Length; index++) {
          var enumType = objectType.GetFields()[index].GetCustomAttribute<StringValueAttribute>();
          if (enumType != null) {
            if (enumType.Text == reader.Value.ToString()) {
              return Enum.Parse(objectType, objectType.GetFields()[index].Name);
            }
          }
        }
      }

      throw new JsonSerializationException(objectType.Name + existingValue);
    }

    public override bool CanConvert(Type objectType) {
      return objectType.GetCustomAttribute<StringEnumAttribute>() != null;
    }
  }

  /// <summary>
  ///   Defines an attribute containing a string representation of the member
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
  public class StringEnumAttribute : Attribute {
  }

  /// <summary>
  ///   Defines an attribute containing a string representation of the member
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public class StringValueAttribute : Attribute {
    private readonly string _text;

    /// <summary>
    ///   Creates a new StringValue attribute with the specified text
    /// </summary>
    /// <param name="text"></param>
    public StringValueAttribute(string text) {
      Contract.Requires(text.NotEmpty());
      _text = text;
    }

    /// <summary>
    ///   The text which belongs to this member
    /// </summary>
    public string Text {
      get { return _text; }
    }
  }


  public static class EnumExtensions {
    /// <summary>
    ///   Retrieves the underlying type if the specified type is a Nullable, or the type itself otherwise.
    /// </summary>
    public static Type GetNonNullableType(Type type) {
      if (!type.IsGenericType || !typeof (Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition())) {
        return type; // Not a Nullable.
      }

      // First: Find the Nullable type.
      while (!(type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))) {
        type = type.BaseType;

        if (type == null) {
          return null;
        }
      }

      // Return the type which is encapsulated by the Nullable.
      return type.GetGenericArguments()[0];
    }

    /// <summary>
    ///   Returns the first matching custom attribute (or null) of the specified member
    /// </summary>
    public static T GetCustomAttribute<T>(this MemberInfo info) where T : Attribute {
      object[] results = info.GetCustomAttributes(typeof (T), false);
      return results.Length == 0 ? null : (T) results[0];
    }

    /// <summary>
    ///   Returns the defined string value of an enum value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetStringValue(this Enum value) {
      FieldInfo entry = value.GetType().GetField(value.ToString());
      if (entry == null) throw new ArgumentNullException("entry");

      // If set, return the value
      var attribute = entry.GetCustomAttribute<StringValueAttribute>();
      if (attribute != null) {
        return attribute.Text;
      }

      // Otherwise throw an exception
      throw new ArgumentException(
        string.Format("Enum value '{0}' does not contain a StringValue attribute", entry), "value");
    }
  }
}