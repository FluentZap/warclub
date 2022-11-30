using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace WarClub;

static class TimeWizard
{
  static Simulation simulation;

  public static void Stasis(Simulation s)
  {
    var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };

    string jsonString = JsonSerializer.Serialize(s.cosmos, options);
    File.WriteAllText("./ChronoStasis.json", jsonString);

  }
}

class EntityJsonConverter : JsonConverter<Entity>
{
  public override Entity Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options) => new Entity(EntityType.World);

  public override void Write(
      Utf8JsonWriter writer,
      Entity entity,
      JsonSerializerOptions options) =>
          writer.WriteNumberValue(entity.Id);
}

class TraitsJsonConverter : JsonConverter<Dictionary<Trait, int>>
{
  public override Dictionary<Trait, int> Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options) => new Dictionary<Trait, int>();

  public override void Write(
      Utf8JsonWriter writer,
      Dictionary<Trait, int> traits,
      JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    foreach ((Trait key, int value) in traits)
    {
      writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(key.Name) ?? key.Name);
      writer.WriteNumberValue(value);
    }

    writer.WriteEndObject();
  }
}

class DataSheetJsonConverter : JsonConverter<DataSheet>
{
  public override DataSheet Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options) => new DataSheet();

  public override void Write(
      Utf8JsonWriter writer,
      DataSheet dataSheet,
      JsonSerializerOptions options) =>
          writer.WriteNumberValue(dataSheet.Id);
}

class SectorJsonConverter : JsonConverter<Sector>
{
  public override Sector Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options) => new Sector();

  public override void Write(
      Utf8JsonWriter writer,
      Sector sector,
      JsonSerializerOptions options) =>
          writer.WriteNumberValue(sector.Id);
}