using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarClub;

public partial class WarClub : Game
{
  protected override void Draw(GameTime gameTime)
  {

    GraphicsDevice.Clear(new Color(10, 10, 10, 255));


    // DrawPlanetShader(simulation.cosmos.Worlds.First().Value, new Point(0), new Point(1080));
    if (s.SelectedView == View.GalaxyOverview)
    {
      DrawStarfield();
      DrawGalaxyOverview();
    }

    if (s.SelectedView == View.MissionSelect)
    {
      DrawStarfield();
      DrawPlanetShader(s.SelectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * s.SelectedWorld.size * 0.85)));
      DrawWorldOverview(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
      DrawWorldOverview(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
    }

    if (s.SelectedView == View.MissionBriefing)
    {
      DrawStarfield();
      // DrawPlanetShader(simulation.SelectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * simulation.SelectedWorld.size * 0.85)));
      DrawMissionBriefing(Matrix.Identity);
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
    }

    if (s.SelectedView == View.Battlefield)
    {
      // DrawPlanetShader(simulation.SelectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * simulation.SelectedWorld.size * 0.85)));
      DrawBattlefield(Matrix.Identity);
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
    }

    if (s.SelectedView == View.MainMenu)
    {
      DrawStarfield();
      DrawMainMenu(Matrix.Identity);
    }

    if (s.SelectedView == View.NewGame)
    {
      DrawStarfield();
      DrawNewGame(Matrix.Identity);
    }

    if (s.SelectedView == View.LoadGame)
    {
      DrawStarfield();
      DrawLoadGame(Matrix.Identity);
    }

    // spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
    // spriteBatch.Draw(planetTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
    // spriteBatch.Draw(planetTexture, new Rectangle(0, 0, 3840, 2160), Color.White);
    // spriteBatch.End();

    // RasterizerState rasterizerState = new RasterizerState();
    // rasterizerState.CullMode = CullMode.None;
    // rasterizerState.FillMode = FillMode.WireFrame;
    // GraphicsDevice.RasterizerState = rasterizerState;

    // foreach (var planet in simulation.cosmos.Worlds)
    //   DrawPlanet(planet.Value);

    // foreach (EffectPass pass in starfieldEffect.CurrentTechnique.Passes)
    // {
    //   pass.Apply();
    //   GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, triangleVertices, 0, 1);
    // }

    base.Draw(gameTime);
  }

  // Screens

  void DrawGalaxyOverview()
  {
    foreach (var world in s.cosmos.Worlds)
      DrawPlanetShader(world.Value, world.Value.location.ToPoint(), new Point((int)(128 * world.Value.size * 0.85)));
    // DrawPlanetShader(world.Value, world.Value.location.ToPoint(), new Point(480, 540));


    spriteBatch.Begin(transformMatrix: s.ViewMatrix);

    var pos = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(s.ViewMatrix));
    spriteBatch.DrawString(basicFont, pos.X.ToString(), new Vector2(0, 128), Color.White);
    spriteBatch.DrawString(basicFont, pos.Y.ToString(), new Vector2(0, 160), Color.White);


    spriteBatch.DrawString(basicFont, timeAdvance.ToString(), new Vector2(0, 0), Color.White);
    spriteBatch.DrawString(basicFont, s.cosmos.Day.ToString(), new Vector2(0, 32), Color.White);

    foreach (var world in s.cosmos.Worlds)
    {
      DrawWorldOverlay(world.Value, world.Value.location.ToPoint() - new Point(240, 270), new Point((int)(128 * world.Value.size * 0.85)));
      DrawWorldTraits(world.Value.location.ToPoint() - new Point(256), world.Value.GetTraits());
    }
    spriteBatch.End();
  }

  void DrawWorldOverview(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);

    var pos = Mouse.GetState().Position;
    spriteBatch.DrawString(basicFont, pos.X.ToString(), new Vector2(0, 128), Color.White);
    spriteBatch.DrawString(basicFont, pos.Y.ToString(), new Vector2(0, 160), Color.White);

    spriteBatch.DrawString(basicFont, timeAdvance.ToString(), new Vector2(0, 0), Color.White);
    spriteBatch.DrawString(basicFont, s.cosmos.Day.ToString(), new Vector2(0, 32), Color.White);

    spriteBatch.DrawString(basicFont, s.SelectedWorld.Name, new Vector2(1024, 0), Color.White);

    var y = 0;

    DrawWorldTraits(new Point(1000, 64), s.SelectedWorld.GetTraits());

    foreach (var mission in s.SelectedWorld.GetEventList(s.OrderEventLists["Mercenary"]))
      spriteBatch.DrawString(basicFont, $"{++y}. {mission.Name}", new Vector2(256, y * 64), Color.White);

    spriteBatch.End();
  }

  void DrawMissionBriefing(Matrix transformMatrix)
  {
    var m = s.ActiveMission;
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    var commanders = s.Commanders.Where(x => x.Units.Count > 0).ToList();
    var totalPoints = 0;
    var commanderPC = Math.Ceiling((decimal)(s.ActiveMission.PointCapacity / (commanders.Count == 0 ? 1 : commanders.Count)));

    // draw commanders
    for (int i = 0; i < commanders.Count; i++)
    {
      var padding = (int)((3840 - 400) / commanders.Count);
      var commander = commanders[i];
      var points = commander.Units.Aggregate(0, (acc, x) => acc + x.Points);
      totalPoints += points;
      spriteBatch.Draw(icons[commander.Icon], new Rectangle(new Point(128 + i * padding, 8), new Point(64, 64)), commander.Color);
      spriteBatch.DrawString(basicFontLarge, points.ToString() + "/" + commanderPC.ToString() + " - " + commander.Name, new Vector2(200 + i * padding, 0), commander.Color);
    }

    spriteBatch.DrawString(basicFontLarge, totalPoints.ToString() + " / " + s.ActiveMission.PointCapacity, new Vector2(0, 100), Color.White);

    // draw units
    int pageCount = s.SelectableUnits.Count / 9;
    var left = s.CurrentPage > 0 ? "< " : "  ";
    var right = s.CurrentPage < pageCount ? " >" : "  ";
    spriteBatch.DrawString(basicFontLarge, $"{left}{s.CurrentPage + 1}{right}", new Vector2(screenSize.X / 2, 1700), Color.White);

    int offset = 0;
    var max = MathHelper.Min(s.SelectableUnits.Count, (s.CurrentPage + 1) * 9);
    for (int i = s.CurrentPage * 9; i < max; i++)
    {
      var unit = s.SelectableUnits[i];
      var position = new Vector2(300, 650 + offset * 100);
      var name = unit.BaseUnit.DataSheet.Name.Length > 20 ? unit.BaseUnit.DataSheet.Name.Substring(0, 20) + "..." : unit.BaseUnit.DataSheet.Name;
      var count = unit.BaseUnit.UnitLines.Count > 1 ? (unit.DeployedCount + 1).ToString() + "*" : unit.DeployedCount.ToString();


      foreach (var commander in s.Commanders)
        if (commander.Units.Contains(unit))
          spriteBatch.Draw(icons[commander.Icon], new Rectangle(new Point((int)position.X - 72, (int)position.Y + 8), new Point(64, 64)), commander.Color);

      spriteBatch.DrawString(basicFontLarge, (offset + 1).ToString() + ". " + name, position, i == s.SelectedUnit ? Color.OrangeRed : Color.White);
      spriteBatch.DrawString(basicFontLarge, count, position + new Vector2(1000, 0), Color.OrangeRed);
      spriteBatch.DrawString(basicFontLarge, unit.Points.ToString(), position + new Vector2(1200, 0), Color.YellowGreen);

      offset += 1;
    }




    spriteBatch.End();

    DrawUnitLoadout(Matrix.CreateTranslation(screenSize.X / 2, 600, 0));
  }

  void DrawBattlefield(Matrix transformMatrix)
  {
    var m = s.ActiveMission;
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    var (left, right) = m.Tiles;
    if (left != null && right != null)
    {
      spriteBatch.Draw(MapTextures[left.Texture], new Rectangle(new Point(), new Point((int)(screenSize.X / 2), (int)screenSize.Y)), Color.White);
      spriteBatch.Draw(MapTextures[right.Texture], new Rectangle(new Point((int)(screenSize.X / 2), 0), new Point((int)(screenSize.X / 2), (int)screenSize.Y)), Color.White);
    }
    var offset = 0;
    foreach (var message in m.MissionEvents.Select(x => x.Turn == s.Turn ? x.Message : null))
      if (message != null) DrawString(basicFontLarge, message, new Rectangle(0, offset++ * 100, (int)screenSize.X, 100), Alignment.Center, Color.White);

    spriteBatch.DrawString(basicFontLarge, "Turn " + s.Turn.ToString(), new Vector2(0, 0), Color.White);

    spriteBatch.End();

    // Draw deployment zones
    DrawZone(transformMatrix, m.MissionEvents);
  }

  void DrawUnitLoadout(Matrix transformMatrix)
  {
    var unit = s.SelectableUnits[s.SelectedUnit];
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    spriteBatch.Draw(BlankTexture, new Rectangle(-50, -50, 1600, 1600), new Color(255, 255, 255, 50));
    Vector2 offset = new Vector2(0);
    foreach (var (nameRaw, line) in unit.BaseUnit.UnitLines)
    {
      var name = nameRaw.Length > 20 ? nameRaw.Substring(0, 20) + "..." : nameRaw;
      var stats = line.UnitStats;
      if (stats.MaxModelsPerUnit == 1)
      {
        spriteBatch.DrawString(basicFont, "1", offset, Color.Black);
        offset.X += 200;
      }
      else
      {
        spriteBatch.DrawString(basicFont, $"{unit.DeployedCount}/{line.Count}", offset, Color.Black);
        offset.X += 100;
        spriteBatch.DrawString(basicFont, $"{stats.MinModelsPerUnit}/{stats.MaxModelsPerUnit}", offset, Color.Black);
        offset.X += 100;
      }

      spriteBatch.DrawString(basicFont, name, offset, Color.Black);
      offset.X += 300;

      // draw stats
      spriteBatch.DrawString(basicFont, stats.Cost.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Movement.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.WS.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.BS.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.S.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.T.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.W.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.A.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Ld.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Sv.ToString(), offset, Color.Red); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Size.X.ToString(), offset, Color.Red); offset.X += 100;






      offset.Y += 50;
      offset.X = 0;

    }


    spriteBatch.End();
  }

  void DrawMainMenu(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "War Club", new Vector2(screenSize.X / 2, 300), Color.White);
    spriteBatch.DrawString(basicFontLarge, "1. Start New Game", new Vector2(screenSize.X / 2, 600), Color.White);
    spriteBatch.DrawString(basicFontLarge, "2. Load Game", new Vector2(screenSize.X / 2, 700), Color.White);
    spriteBatch.End();
  }

  void DrawNewGame(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "new game", new Vector2(screenSize.X / 2, 300), Color.White);
    spriteBatch.DrawString(basicFontLarge, "Select two units per player", new Vector2(screenSize.X / 2, 400), Color.White);

    var totalPoints = s.SelectedUnits.Aggregate(0, (acc, x) => acc + x.Points);

    spriteBatch.DrawString(basicFontLarge, $"{s.SelectedUnits.Count.ToString()} Selected Units - {totalPoints} Points", new Vector2(screenSize.X / 2, 500), Color.White);

    int pageCount = s.SelectableUnits.Count / 9;
    var left = s.CurrentPage > 0 ? "< " : "  ";
    var right = s.CurrentPage < pageCount ? " >" : "  ";
    spriteBatch.DrawString(basicFontLarge, $"{left}{s.CurrentPage + 1}{right}", new Vector2(screenSize.X / 2, 1700), Color.White);
    spriteBatch.DrawString(basicFontLarge, "Press enter to continue", new Vector2(screenSize.X / 2, 1800), Color.White);

    int offset = 0;
    var max = MathHelper.Min(s.SelectableUnits.Count, (s.CurrentPage + 1) * 9);
    for (int i = s.CurrentPage * 9; i < max; i++)
    {
      var unit = s.SelectableUnits[i];
      if (s.SelectedUnits.Contains(unit))
      {
        spriteBatch.DrawString(basicFontLarge, (offset + 1).ToString() + ". " + unit.Points + " - " + unit.BaseUnit.DataSheet.Name, new Vector2(screenSize.X / 3 + 50, 650 + offset * 100), Color.OrangeRed);
      }
      else
      {
        spriteBatch.DrawString(basicFontLarge, (offset + 1).ToString() + ". " + unit.Points + " - " + unit.BaseUnit.DataSheet.Name, new Vector2(screenSize.X / 3, 650 + offset * 100), Color.White);

      }
      offset += 1;
    }

    spriteBatch.End();
  }

  void DrawLoadGame(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "load game", new Vector2(screenSize.X / 2, 300), Color.White);
    // spriteBatch.DrawString(basicFontLarge, "1. Start New Game", new Vector2(screenSize.X / 2, 600), Color.White);
    // spriteBatch.DrawString(basicFontLarge, "2. Load Game", new Vector2(screenSize.X / 2, 700), Color.White);
    spriteBatch.End();
  }

  // Draw Items

  void DrawZone(Matrix transformMatrix, List<MissionEvent> missionEvents)
  {

    var aniTime = GetAnimation(3000);

    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix, blendState: BlendState.Additive);
    foreach (var mEvent in missionEvents)
      foreach (var spawn in mEvent.Spawns)
        foreach (var zone in spawn.Zones)
        {
          if (spawn.Type == MissionSpawnType.DeploymentZone && mEvent.Turn == s.Turn) spriteBatch.Draw(BlankTexture, zone, Color.Lerp(new Color(119, 221, 119, 50), new Color(119, 221, 119, 150), aniTime));
          if (spawn.Type == MissionSpawnType.EnemySpawn && mEvent.Turn == s.Turn) spriteBatch.Draw(BlankTexture, zone, Color.Lerp(new Color(255, 80, 80, 50), new Color(255, 80, 80, 150), aniTime));
          if (spawn.Type == MissionSpawnType.EvacZone && mEvent.Turn <= s.Turn) spriteBatch.Draw(BlankTexture, zone, Color.Lerp(new Color(119, 221, 119, 50), new Color(119, 221, 119, 150), aniTime));
          if (spawn.Type == MissionSpawnType.LootBox) spriteBatch.Draw(BlankTexture, zone, Color.Lerp(new Color(119, 221, 119, 50), new Color(119, 221, 119, 150), aniTime));
          // draw spawn icon
          if (spawn.ZonesIcon != Icon.None && mEvent.Turn == s.Turn) spriteBatch.Draw(icons[spawn.ZonesIcon], new Rectangle(zone.Center - new Point(32), new Point(64)), Color.White);
        }
    spriteBatch.End();
  }

  float GetAnimation(float milliseconds)
  {
    var adjustedTme = (animationTime % milliseconds) / milliseconds;
    return adjustedTme < 0.5 ? adjustedTme * 2 : 2 - adjustedTme * 2;
  }

  void DrawBattlefieldMessage(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "load game", new Vector2(screenSize.X / 2, 300), Color.White);
    spriteBatch.End();
  }

  void DrawWorldTraits(Point location, Dictionary<Trait, int> traits)
  {
    Point offset = new Point();
    foreach (var (icon, trait) in TraitIcons)
      if (traits.ContainsKey(trait))
      {
        var iconPos = location + new Point(80) + offset * new Point(96);
        spriteBatch.Draw(icons[icon], new Rectangle(iconPos, new Point(64, 64)), Color.White);
        offset.X++;

        if (offset.X > 3)
        {
          offset.Y++;
          offset.X = 0;
        }
      }
  }

  void DrawDataCard()
  {
    // var datacarPos =
    // spriteBatch.Draw(DataCard, new Rectangle(( new Point(64, 64)), Color.White));
  }

  void DrawPlanet(World planet)
  {
    ModelMesh mesh = model.Meshes[0];
    mesh.MeshParts[0].Effect = basicEffect;
    // Matrix matrix = Matrix.CreateScale(10.0f) * Matrix.CreateFromQuaternion(planet.rotation) * Matrix.CreateTranslation(planet.location.X, planet.location.Y, 0);

    foreach (BasicEffect effect in mesh.Effects)
    {
      effect.View = Matrix.Identity;
      // effect.World = worldMatrix * matrix;
      effect.Projection = s.ViewMatrix;
      effect.EnableDefaultLighting();
      // effect.PreferPerPixelLighting = true;
      effect.DiffuseColor = new Vector3(1, 1, 1);
      // effect.EmissiveColor = new Vector3(1, 1, 1);
    }

    mesh.Draw();
  }

  void DrawWorldOverlay(World world, Point location, Point size)
  {
    spriteBatch.DrawString(basicFont, world.Name, location.ToVector2(), Color.White);
  }

  void DrawPlanetShader(World world, Point location, Point size)
  {
    // planetEffect.Parameters["iTime"].SetValue(-(world.Id * 100 + animationTime * world.rotationSpeed * 0.0005f));
    planetEffect.Parameters["iTime"].SetValue(-(world.Id * 100 + animationTime * 0.0002f));
    planetEffect.Parameters["col_top"].SetValue(world.color.top.ToVector3());
    planetEffect.Parameters["col_bot"].SetValue(world.color.bot.ToVector3());
    planetEffect.Parameters["col_mid1"].SetValue(world.color.mid1.ToVector3());
    planetEffect.Parameters["col_mid2"].SetValue(world.color.mid2.ToVector3());
    planetEffect.Parameters["col_mid3"].SetValue(world.color.mid3.ToVector3());


    // planetEffect.Parameters["col_top"].SetValue(new Vector3(1.0f, 1.0f, 1.0f));
    planetEffect.Parameters["col_bot"].SetValue(new Vector3(0.0f, 0.0f, 0.0f));
    // planetEffect.Parameters["col_mid1"].SetValue(new Vector3(0.1f, 0.2f, 0.0f));
    // planetEffect.Parameters["col_mid2"].SetValue(new Vector3(0.7f, 0.4f, 0.3f));
    // planetEffect.Parameters["col_mid3"].SetValue(new Vector3(1.0f, 0.4f, 0.2f));

    // float3 col_top = float3(1.0, 1.0, 1.0);
    // float3 col_bot = float3(0.0, 0.0, 0.0);
    // float3 col_mid1 = float3(0.1, 0.2, 0.0);
    // float3 col_mid2 = float3(0.7, 0.4, 0.3);
    // float3 col_mid3 = float3(1.0, 0.4, 0.2);

    // planetEffect.Parameters["col_top"].SetValue(new Vector3(0.0f, 0.5f, 0.0f));
    // planetEffect.Parameters["col_bot"].SetValue(new Vector3(0.0f, 1.0f, 1.0f));
    // planetEffect.Parameters["col_mid1"].SetValue(new Vector3(0.0f, 1.0f, 0.0f));
    // planetEffect.Parameters["col_mid2"].SetValue(new Vector3(0.0f, 0.0f, 1.0f));
    // planetEffect.Parameters["col_mid3"].SetValue(new Vector3(0.0f, 0.0f, 1.0f));

    spriteBatch.Begin(effect: planetEffect, sortMode: SpriteSortMode.Deferred, transformMatrix: s.ViewMatrix);
    spriteBatch.Draw(screenTexture, new Rectangle(location - size / new Point(2), size), Color.White);
    spriteBatch.End();
  }

  void DrawStarfield()
  {
    starfieldEffect.Parameters["iTime"].SetValue(animationTime / 10000f);
    spriteBatch.Begin(effect: starfieldEffect, sortMode: SpriteSortMode.Deferred, transformMatrix: s.ViewMatrix);
    spriteBatch.Draw(screenTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
    spriteBatch.End();
  }

  [Flags]
  public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

  void DrawString(SpriteFont font, string text, Rectangle bounds, Alignment align, Color color)
  {
    Vector2 size = font.MeasureString(text);
    Point pos = bounds.Center;
    Vector2 origin = size * 0.5f;

    if (align.HasFlag(Alignment.Left))
      origin.X += bounds.Width / 2 - size.X / 2;

    if (align.HasFlag(Alignment.Right))
      origin.X -= bounds.Width / 2 - size.X / 2;

    if (align.HasFlag(Alignment.Top))
      origin.Y += bounds.Height / 2 - size.Y / 2;

    if (align.HasFlag(Alignment.Bottom))
      origin.Y -= bounds.Height / 2 - size.Y / 2;

    spriteBatch.DrawString(font, text, pos.ToVector2(), color, 0, origin, 1, SpriteEffects.None, 0);
  }

}
