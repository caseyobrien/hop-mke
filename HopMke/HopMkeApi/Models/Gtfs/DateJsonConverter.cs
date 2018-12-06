// from https://gist.github.com/randyburden/5924981

using System;
using System.Globalization;
using Newtonsoft.Json;

namespace HopMkeApi.Models.Gtfs
{
    /// <summary>
    /// Handles converting JSON string values into a C# boolean data type.
    /// </summary>
    public class DateJsonConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            // Handle only boolean types.
            return objectType == typeof(DateTime);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();
            return DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Specifies that this converter will not participate in writing results.
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        #endregion Overrides of JsonConverter
    }
}

