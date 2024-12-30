using System.ComponentModel;
using AutoEvent.Configs.Converters;
using AutoEvent.Configs.Tools;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NodeDeserializers;

namespace AutoEvent.Configs;
public static class Serialization
{
  /// <summary>
  /// Gets or sets the serializer for configs and translations.
  /// </summary>
  public static ISerializer Serializer { get; set; } = new SerializerBuilder()
    .WithTypeConverter(new VectorsConverter())
    .WithTypeConverter(new ColorConverter())
    .WithTypeConverter(new AttachmentIdentifiersConverter())
    .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
    .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
    .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .IgnoreFields()
    .DisableAliases()
    .Build();

  /// <summary>
  /// Gets or sets the deserializer for configs and translations.
  /// </summary>
  public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
    .WithTypeConverter(new VectorsConverter())
    .WithTypeConverter(new ColorConverter())
    .WithTypeConverter(new AttachmentIdentifiersConverter())
    .WithNamingConvention(UnderscoredNamingConvention.Instance)
    .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner),
      deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
    .IgnoreFields()
    .IgnoreUnmatchedProperties()
    .Build();

  /// <summary>
  /// Gets or sets the quotes wrapper type.
  /// </summary>
  [Description(
    "Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
  public static ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;

  /// <summary>
  /// Gets or sets the quotes wrapper type.
  /// </summary>
  [Description(
    "Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
  public static ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;
}