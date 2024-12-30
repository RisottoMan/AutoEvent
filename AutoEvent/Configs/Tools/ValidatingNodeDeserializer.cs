using System;
using System.ComponentModel.DataAnnotations;
using PluginAPI.Core;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace AutoEvent.Configs.Tools;

/// <summary>
/// Basic configs validation.
/// </summary>
public sealed class ValidatingNodeDeserializer : INodeDeserializer
{
    private readonly INodeDeserializer nodeDeserializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingNodeDeserializer"/> class.
    /// </summary>
    /// <param name="nodeDeserializer">The node deserializer instance.</param>
    public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
    {
        this.nodeDeserializer = nodeDeserializer;
    }

    /// <inheritdoc cref="INodeDeserializer"/>
    public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer,
        out object value)
    {
        if (nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
        {
            if (value is null)
                Log.Error("Null value");
            Validator.ValidateObject(value, new ValidationContext(value, null, null), true);

            return true;
        }

        return false;
    }
}