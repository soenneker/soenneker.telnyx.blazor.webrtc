using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Soenneker.Telnyx.Blazor.WebRtc;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, ReadCommentHandling = JsonCommentHandling.Skip, UseStringEnumConverter = true, Converters = new[] { typeof(TelnyxEnvironmentMetadataConverter), typeof(TelnyxCallStateMetadataConverter) })]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(List<TelnyxDeviceResolution>))]
[JsonSerializable(typeof(List<TelnyxIceServer>))]
[JsonSerializable(typeof(List<TelnyxWebRtcCall>))]
[JsonSerializable(typeof(TelnyxAnswerOptions))]
[JsonSerializable(typeof(TelnyxAudioSettings))]
[JsonSerializable(typeof(TelnyxCallOptions))]
[JsonSerializable(typeof(TelnyxClientOptions))]
[JsonSerializable(typeof(TelnyxFunctionCallOutput))]
[JsonSerializable(typeof(TelnyxHangupOptions))]
[JsonSerializable(typeof(TelnyxLoginOptions))]
[JsonSerializable(typeof(TelnyxPreCallDiagnosisOptions))]
[JsonSerializable(typeof(TelnyxPreCallDiagnosisReport))]
[JsonSerializable(typeof(TelnyxScreenShareOptions))]
[JsonSerializable(typeof(TelnyxVideoSettings))]
[JsonSerializable(typeof(TelnyxWebRtcCall))]
[JsonSerializable(typeof(TelnyxWebRtcInfo))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(System.Text.Json.JsonElement))]
[JsonSerializable(typeof(System.Collections.Generic.Dictionary<string, object?>))]
[JsonSerializable(typeof(System.Collections.Generic.List<object?>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(object[]))]
internal partial class LibraryJsonContext : JsonSerializerContext
{
    internal static JsonTypeInfo<T> Get<T>() =>
        (JsonTypeInfo<T>)(Default.GetTypeInfo(typeof(T)) ?? throw new NotSupportedException($"No generated JSON metadata for {typeof(T)}."));

    internal static JsonSerializerOptions WithContext(JsonSerializerContext? additionalContext)
    {
        JsonSerializerOptions defaults = Get<object>().Options;
        if (additionalContext is null)
            return defaults;
        var options = new JsonSerializerOptions(defaults)
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(defaults.TypeInfoResolver!, additionalContext)
        };
        options.MakeReadOnly();
        return options;
    }
}

internal sealed class TelnyxEnvironmentMetadataConverter : JsonConverter<Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxEnvironment>
{
    public override Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxEnvironment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String && Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxEnvironment.TryFromValue(reader.GetString(), out var value) ? value : throw new JsonException("Unknown TelnyxEnvironment value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxEnvironment value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

internal sealed class TelnyxCallStateMetadataConverter : JsonConverter<Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxCallState>
{
    public override Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxCallState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String && Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxCallState.TryFromValue(reader.GetString(), out var value) ? value : throw new JsonException("Unknown TelnyxCallState value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Telnyx.Blazor.WebRtc.Enums.TelnyxCallState value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
