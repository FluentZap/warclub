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
      var count = unit.BaseUnit.UnitLines.Count > 1 ? (unit.DeployedCount + 1).ToString() + "*" : unit.DeployedCount.ToString();


      foreach (var commander in s.Commanders)
        if (commander.Units.Contains(unit))
          spriteBatch.Draw(icons[commander.Icon], new Rectangle(new Point((int)position.X - 72, (int)position.Y + 8), new Point(64, 64)), commander.Color);

      spriteBatch.DrawString(basicFontLarge, (offset + 1).ToString() + ". " + TrimString(unit.BaseUnit.DataSheet.Name), position, i == s.SelectedUnit ? Color.OrangeRed : Color.White);
      spriteBatch.DrawString(basicFontLarge, count, position + new Vector2(1000, 0), Color.OrangeRed);
      spriteBatch.DrawString(basicFontLarge, unit.Points.ToString(), position + new Vector2(1200, 0), Color.YellowGreen);

      offset += 1;
    }




    spriteBatch.End();

    DrawUnitLoadout(Matrix.CreateTranslation(screenSize.X / 2, 100, 0));
  }

  void DrawUnitLoadout(Matrix transformMatrix)
  {
    var unit = s.SelectableUnits[s.SelectedUnit];
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix, blendState: BlendState.NonPremultiplied);
    spriteBatch.Draw(BlankTexture, new Rectangle(-50, -50, 1600, 1600), new Color(0, 0, 0, 150));
    Vector2 offset = new Vector2(0);
    foreach (var (name, line) in unit.BaseUnit.UnitLines)
    {
      var stats = line.UnitStats;
      if (stats.MaxModelsPerUnit == 1)
      {
        spriteBatch.DrawString(basicFont, "1", offset, Color.White);
        offset.X += 200;
      }
      else
      {
        spriteBatch.DrawString(basicFont, $"{unit.DeployedCount}/{line.Count}", offset, Color.White);
        offset.X += 100;
        spriteBatch.DrawString(basicFont, $"{stats.MinModelsPerUnit}/{stats.MaxModelsPerUnit}", offset, Color.White);
        offset.X += 100;
      }

      spriteBatch.DrawString(basicFont, TrimString(name), offset, Color.White);
      offset.X += 300;

      // draw stats
      spriteBatch.DrawString(basicFont, stats.Cost.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Movement.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.WS.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.BS.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.S.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.T.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.W.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.A.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Ld.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Sv.ToString(), offset, Color.White); offset.X += 100;
      spriteBatch.DrawString(basicFont, stats.Size.X.ToString(), offset, Color.White); offset.X += 100;


      offset.Y += 50;
      offset.X = 0;

    }
    DrawString(basicFont, "Wargear", new Rectangle((int)offset.X, (int)offset.Y, 1000, 50), Alignment.Center, Color.White);
    offset.Y += 50;

    // weapons and stats
    foreach (var wargear in unit.BaseUnit.DataSheet.Wargear)
    {
      spriteBatch.DrawString(basicFont, TrimString(wargear.Name), offset, Color.White); offset.X += 250;
      var cost = s.WargearCost[(unit.BaseUnit.DataSheet.Id, wargear.Id)];
      if (cost != 0) spriteBatch.DrawString(basicFont, cost.ToString(), offset, Color.White); offset.X += 100;
      if (wargear.WargearLine.Count == 1 && wargear.Description.Length == 0)
      {
        spriteBatch.DrawString(basicFont, wargear.WargearLine.First().Value.Range, offset, Color.White); offset.X += 100;
        spriteBatch.DrawString(basicFont, wargear.WargearLine.First().Value.Type, offset, Color.White); offset.X += 200;
        spriteBatch.DrawString(basicFont, wargear.WargearLine.First().Value.S, offset, Color.White); offset.X += 100;
        spriteBatch.DrawString(basicFont, wargear.WargearLine.First().Value.AP, offset, Color.White); offset.X += 100;
        spriteBatch.DrawString(basicFont, wargear.WargearLine.First().Value.D, offset, Color.White); offset.X += 100;

        offset.Y = DrawStringWithWrap(wargear.WargearLine.First().Value.Abilities, new Rectangle((int)offset.X, (int)offset.Y, 300, 50), Alignment.Left, 40, Color.White).Y;
        // spriteBatch.DrawString(basicFont, TrimString(wargear.WargearLine.First().Value.Abilities, 100), offset, Color.White);

        // offset.Y += 35;
      }
      else
      {
        offset.Y = DrawStringWithWrap(wargear.Description, new Rectangle((int)offset.X, (int)offset.Y, 1000, 35), Alignment.Left, 80, Color.White).Y;

        offset.X = 50;
        // offset.Y += 35;
        foreach (var gearLine in wargear.WargearLine)
        {
          spriteBatch.DrawString(basicFont, TrimString(gearLine.Value.Name), offset, Color.White); offset.X += 300;
          spriteBatch.DrawString(basicFont, gearLine.Value.Range, offset, Color.White); offset.X += 100;
          spriteBatch.DrawString(basicFont, gearLine.Value.Type, offset, Color.White); offset.X += 200;
          spriteBatch.DrawString(basicFont, gearLine.Value.S, offset, Color.White); offset.X += 100;
          spriteBatch.DrawString(basicFont, gearLine.Value.AP, offset, Color.White); offset.X += 100;
          spriteBatch.DrawString(basicFont, gearLine.Value.D, offset, Color.White); offset.X += 100;
          spriteBatch.DrawString(basicFont, TrimString(gearLine.Value.Abilities, 100), offset, Color.White);
          offset.Y += 35;
          offset.X = 50;
        }
      }


      offset.Y += 15;
      offset.X = 0;
    }

    spriteBatch.End();
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
    spriteBatch.End();

    DrawMissionMessages(Matrix.CreateScale(0.5f) * Matrix.CreateRotationZ(3.14f) * Matrix.CreateTranslation(screenSize.X, 300, 0));
    DrawMissionMessages(Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(0, screenSize.Y - 300, 0));
    // DrawMissionMessages(Matrix.CreateRotationZ(3.14f) * Matrix.CreateTranslation(0, screenSize.Y - 200, 0));

    // Draw deployment zones
    DrawZone(transformMatrix, s.MissionState);


    // DrawDataCard(Matrix.CreateScale(0.5f) * Matrix.CreateRotationZ((float)(Math.PI)) * Matrix.CreateTranslation(screenSize.X / 2 + 250, 200, 0), s.ActiveMission.PlayerUnits.First());

    // DrawDataCard(Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(screenSize.X / 2 - 250, screenSize.Y - 200, 0), s.ActiveMission.PlayerUnits.First());

    // DrawDataCard(Matrix.CreateScale(0.5f, -0.5f, 0.5f) * Matrix.CreateTranslation(screenSize.X / 2 - 250, screenSize.Y, 0) * Matrix.CreateRotationZ(3.14f), s.ActiveMission.PlayerUnits.First());

    var offset = 0;
    if (s.MissionState.PlayerTurn)
    {
      foreach (var unit in s.MissionState.AIUnits)
      {
        DrawEnemyDataCard(Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(0, offset, 0), unit);
        DrawEnemyDataCard(Matrix.CreateRotationZ(3.14f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(screenSize.X, screenSize.Y - offset, 0), unit);
        offset += 200;
      }
      offset = 0;
      foreach (var unit in s.MissionState.PCUnits)
      {
        DrawDataCard(Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(offset, screenSize.Y - 200, 0), unit);
        // DrawDataCard(Matrix.CreateRotationZ(3.14f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(screenSize.X, screenSize.Y - offset, 0), unit);
        offset += 820;
      }
    }

    offset = 0;
    if (!s.MissionState.PlayerTurn)
    {
      if (s.MissionState.ActiveUnit != null)
      {
        DrawDataCard(Matrix.CreateScale(0.5f) * Matrix.CreateTranslation((screenSize.X / 2) - 400, 0, 0), s.MissionState.ActiveUnit);
        DrawDataCard(Matrix.CreateRotationZ(3.14f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation((screenSize.X / 2) + 400, screenSize.Y, 0), s.MissionState.ActiveUnit);
      }
      foreach (var unit in s.MissionState.PCUnits)
      {
        DrawEnemyDataCard(Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(0, offset, 0), unit);
        DrawEnemyDataCard(Matrix.CreateRotationZ(3.14f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(screenSize.X, screenSize.Y - offset, 0), unit);
        offset += 200;
      }
    }

    // DrawDataCard(Matrix.CreateTranslation(screenSize.X - 1600, screenSize.Y - 400, 0), UnitUtils.ActivateUnit(s.cosmos.PlayerFaction.Units[UnitRole.Troops].First()));
    // DrawDataCard(Matrix.CreateTranslation(screenSize.X, 100, 0), UnitUtils.ActivateUnit(s.cosmos.PlayerFaction.Units[UnitRole.Troops].First()));
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

  void DrawMissionMessages(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    var offset = 0;
    DrawString(basicFontLarge, $"T: {s.MissionState.Turn} - R: {s.MissionState.Round} - Ally: {s.MissionState.PCUnitsReady.Count}/{s.MissionState.PCUnits.Count} - Enemy: {s.MissionState.AIUnitsReady.Count}/{s.MissionState.AIUnits.Count}", new Rectangle(0, offset++ * 75, (int)screenSize.X, 100), Alignment.Left, Color.White);
    foreach (var message in s.MissionState.Messages)
    {
      DrawString(basicFontLarge, message.Text, new Rectangle(0, offset++ * 75, (int)screenSize.X, 100), Alignment.Left, message.Color);
      if (message.Model != null) DrawModelIcon(message.Model, new Rectangle(0, offset++ * 75, (int)screenSize.X, 100));
    }
    spriteBatch.End();
  }

  void DrawZone(Matrix transformMatrix, MissionState state)
  {

    var aniTime = GetAnimation(3000);

    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix, blendState: BlendState.Additive);
    foreach (var missionZone in state.PermZones.Concat(state.TempZones))
      foreach (var zone in missionZone.Zones)
      {
        var startColor = missionZone.Color;
        var endColor = missionZone.Color;
        startColor.A = 50;
        endColor.A = 150;
        spriteBatch.Draw(BlankTexture, zone, Color.Lerp(startColor, endColor, aniTime));
      }
    spriteBatch.End();

    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix);
    foreach (var missionZone in state.PermZones.Concat(state.TempZones))
      foreach (var zone in missionZone.Zones)
      {
        // draw spawn icon
        if (missionZone.Icon != Icon.None) spriteBatch.Draw(icons[missionZone.Icon], new Rectangle(zone.Center - new Point(32), new Point(64)), Color.White);
        if (missionZone.Model != null) DrawModelIcon(missionZone.Model, zone);
        if (missionZone.Message != null) DrawString(basicFont, missionZone.Message.Text, new Rectangle(zone.Center.X - 100, zone.Top - 32, 200, 32), Alignment.Center, missionZone.Message.Color);
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

  string TrimString(string raw, int limit = 20)
  {
    return raw.Length > limit ? raw.Substring(0, limit) + "..." : raw;
  }

  Rectangle DrawStringWithWrap(string text, Rectangle rect, Alignment align, int wrap, Color color)
  {
    var sentence = "";
    foreach (var line in text.Split(" "))
    {
      sentence += line + " ";
      if (sentence.Length > wrap)
      {
        DrawString(basicFont, sentence, rect, align, color); rect.Y += 35;
        sentence = "";
      }
    }
    DrawString(basicFont, sentence, rect, align, color); rect.Y += 35;
    return rect;
  }

  void DrawEnemyDataCard(Matrix transformMatrix, ActiveUnit unit)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix, blendState: BlendState.NonPremultiplied);
    spriteBatch.Draw(DataCard, new Rectangle(0, 0, 500, 400), new Color(50, 50, 200, 255));
    var offset = 0;

    offset = DrawStringWithWrap(unit.BaseUnit.DataSheet.Name, new Rectangle(0, 50, 500, 35), Alignment.Center, 200, Color.White).Y;

    // draw stats
    DrawLine(new string[] { "M", "WS", "BS", "S", "T", "W", "A", "Ld", "Sv" }, Color.HotPink, 35, offset, xStep: new int[] { 50 }); offset += 35;
    foreach (var (name, line) in unit.BaseUnit.UnitLines)
    {
      var stats = line.UnitStats;
      // DrawLine(new string[] { TrimString(name) }, Color.White, 0, offset);
      DrawLine(new string[] { stats.Movement, stats.WS, stats.BS, stats.S, stats.T, stats.W, stats.A, stats.Ld, stats.Sv }, Color.White, 35, offset, xStep: new int[] { 50 });
      offset += 25;
    }
    offset += 25;

    // weapons and stats
    DrawLine(new string[] { "Range", "Type", "S", "AP", "D" }, Color.Magenta, 35, offset, xStep: new int[] { 100, 150, 100, 50 }); offset += 35;

    foreach (var wargear in unit.BaseUnit.DataSheet.DefaultWargear)
    {
      if (wargear.WargearLine.Count == 1 && wargear.Description.Length == 0)
      {
        // single line wargear
        var line = wargear.WargearLine.First().Value;
        DrawLine(new string[] { line.Range, line.Type, line.S, line.AP, line.D }, Color.White, 35, offset, xStep: new int[] { 100, 150, 100, 50 });
        offset += 25;
      }
      else
      {
        // multi line wargear
        offset += 25;
        foreach (var gearLine in wargear.WargearLine.Values)
        {
          DrawLine(new string[] { TrimString(gearLine.Name), gearLine.Range, gearLine.Type, gearLine.S, gearLine.AP, gearLine.D }, Color.White, 35, offset, xStep: new int[] { 300, 100, 200, 100, 100, 100 });
          offset += 25;
        }
      }
    }

    spriteBatch.End();
  }

  void DrawDataCard(Matrix transformMatrix, ActiveUnit unit)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * s.ViewMatrix, blendState: BlendState.NonPremultiplied);
    spriteBatch.Draw(BlankTexture, new Rectangle(0, 0, 1600, 400), new Color(50, 50, 50, 255));
    int offset = 0;

    // draw stats
    DrawLine(new string[] { "M", "WS", "BS", "S", "T", "W", "A", "Ld", "Sv" }, Color.HotPink, 300, xStep: new int[] { 50 }); offset += 35;
    foreach (var (name, line) in unit.BaseUnit.UnitLines)
    {
      var stats = line.UnitStats;
      DrawLine(new string[] { TrimString(name) }, Color.White, 0, offset);
      DrawLine(new string[] { stats.Movement, stats.WS, stats.BS, stats.S, stats.T, stats.W, stats.A, stats.Ld, stats.Sv }, Color.White, 300, offset, xStep: new int[] { 50 });
      offset += 25;
    }
    offset += 35;

    // weapons and stats
    DrawLine(new string[] { "Range", "Type", "S", "AP", "D" }, Color.Magenta, 300, offset, xStep: new int[] { 100, 200, 100, 100, 100 }); offset += 35;

    foreach (var wargear in unit.BaseUnit.DataSheet.DefaultWargear)
    {
      DrawLine(new string[] { TrimString(wargear.Name) }, Color.Orange, 50, offset);
      if (wargear.WargearLine.Count == 1 && wargear.Description.Length == 0)
      {
        // single line wargear
        var line = wargear.WargearLine.First().Value;
        DrawLine(new string[] { line.Range, line.Type, line.S, line.AP, line.D }, Color.White, 300, offset, xStep: new int[] { 100, 200, 100, 100, 100 });
        offset = DrawStringWithWrap(wargear.WargearLine.First().Value.Abilities, new Rectangle(850, offset, 300, 50), Alignment.Left, 40, Color.White).Y;
      }
      else
      {
        // multi line wargear
        offset = DrawStringWithWrap(wargear.Description, new Rectangle(850, offset, 1000, 35), Alignment.Left, 80, Color.White).Y;
        foreach (var gearLine in wargear.WargearLine.Values)
        {
          DrawLine(new string[] { TrimString(gearLine.Name), gearLine.Range, gearLine.Type, gearLine.S, gearLine.AP, gearLine.D, TrimString(gearLine.Abilities, 100) }, Color.White, 50, offset, xStep: new int[] { 300, 100, 200, 100, 100, 100 });
          offset += 35;
        }
      }
    }

    spriteBatch.End();
  }

  void DrawLine(string[] items, Color color, int x = 0, int y = 0, int[] xStep = null, int[] yStep = null)
  {
    var offset = new Vector2(x, y);
    for (int i = 0; i < items.Count(); i++)
    {
      spriteBatch.DrawString(basicFont, items[i], offset, color);
      offset.X += xStep != null ? xStep[i % xStep.Length] : 0;
      offset.Y += yStep != null ? yStep[i % yStep.Length] : 0;
    }
  }

  Color ChangeColorBrightness(Color color, float correctionFactor)
  {
    float red = (float)color.R;
    float green = (float)color.G;
    float blue = (float)color.B;

    if (correctionFactor < 0)
    {
      correctionFactor = 1 + correctionFactor;
      red *= correctionFactor;
      green *= correctionFactor;
      blue *= correctionFactor;
    }
    else
    {
      red = (255 - red) * correctionFactor + red;
      green = (255 - green) * correctionFactor + green;
      blue = (255 - blue) * correctionFactor + blue;
    }

    return new Color(color.A, (int)red, (int)green, (int)blue);
  }

  void DrawModelIcon(Models model, Rectangle zone)
  {
    float ratio = model.Texture.Height / (float)model.Texture.Width;
    int size = 128;
    spriteBatch.Draw(model.Texture, new Rectangle(zone.Center - new Point((int)(size / 2), (int)((size / 2) * ratio)), new Point(size, (int)(size * ratio))), Color.White);
  }
}
