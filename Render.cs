﻿using System;
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
    if (simulation.SelectedView == View.GalaxyOverview)
    {
      DrawStarfield();
      DrawGalaxyOverview();
    }

    if (simulation.SelectedView == View.MissionSelect)
    {
      DrawStarfield();
      DrawPlanetShader(simulation.SelectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * simulation.SelectedWorld.size * 0.85)));
      DrawWorldOverview(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
      DrawWorldOverview(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
    }

    if (simulation.SelectedView == View.MissionBriefing)
    {
      DrawStarfield();
      // DrawPlanetShader(simulation.SelectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * simulation.SelectedWorld.size * 0.85)));
      DrawMissionBriefing(Matrix.Identity);
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
    }

    if (simulation.SelectedView == View.Battlefield)
    {
      // DrawPlanetShader(simulation.SelectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * simulation.SelectedWorld.size * 0.85)));
      DrawBattlefield(Matrix.Identity);
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
      // DrawMissionBriefing(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
    }

    if (simulation.SelectedView == View.MainMenu)
    {
      DrawStarfield();
      DrawMainMenu(Matrix.Identity);
    }

    if (simulation.SelectedView == View.NewGame)
    {
      DrawStarfield();
      DrawNewGame(Matrix.Identity);
    }

    if (simulation.SelectedView == View.LoadGame)
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


  void DrawGalaxyOverview()
  {
    foreach (var world in simulation.cosmos.Worlds)
      DrawPlanetShader(world.Value, world.Value.location.ToPoint(), new Point((int)(128 * world.Value.size * 0.85)));
    // DrawPlanetShader(world.Value, world.Value.location.ToPoint(), new Point(480, 540));


    spriteBatch.Begin(transformMatrix: simulation.ViewMatrix);

    var pos = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(simulation.ViewMatrix));
    spriteBatch.DrawString(basicFont, pos.X.ToString(), new Vector2(0, 128), Color.White);
    spriteBatch.DrawString(basicFont, pos.Y.ToString(), new Vector2(0, 160), Color.White);


    spriteBatch.DrawString(basicFont, timeAdvance.ToString(), new Vector2(0, 0), Color.White);
    spriteBatch.DrawString(basicFont, simulation.cosmos.Day.ToString(), new Vector2(0, 32), Color.White);

    foreach (var world in simulation.cosmos.Worlds)
    {
      DrawWorldOverlay(world.Value, world.Value.location.ToPoint() - new Point(240, 270), new Point((int)(128 * world.Value.size * 0.85)));
      DrawWorldTraits(world.Value.location.ToPoint() - new Point(256), world.Value.GetTraits());
    }
    spriteBatch.End();
  }

  void DrawWorldOverview(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix);

    var pos = Mouse.GetState().Position;
    spriteBatch.DrawString(basicFont, pos.X.ToString(), new Vector2(0, 128), Color.White);
    spriteBatch.DrawString(basicFont, pos.Y.ToString(), new Vector2(0, 160), Color.White);

    spriteBatch.DrawString(basicFont, timeAdvance.ToString(), new Vector2(0, 0), Color.White);
    spriteBatch.DrawString(basicFont, simulation.cosmos.Day.ToString(), new Vector2(0, 32), Color.White);

    spriteBatch.DrawString(basicFont, simulation.SelectedWorld.Name, new Vector2(1024, 0), Color.White);

    var y = 0;

    DrawWorldTraits(new Point(1000, 64), simulation.SelectedWorld.GetTraits());

    foreach (var mission in simulation.SelectedWorld.GetEventList(simulation.OrderEventLists["Mercenary"]))
      spriteBatch.DrawString(basicFont, $"{++y}. {mission.Name}", new Vector2(256, y * 64), Color.White);

    spriteBatch.End();
  }

  void DrawMissionBriefing(Matrix transformMatrix)
  {
    var m = simulation.ActiveMission;
    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix);
    var commanders = simulation.Commanders.Where(x => x.Units.Count > 0).ToList();
    var totalPoints = 0;
    var commanderPC = Math.Ceiling((decimal)(simulation.ActiveMission.PointCapacity / (commanders.Count == 0 ? 1 : commanders.Count)));

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

    spriteBatch.DrawString(basicFontLarge, totalPoints.ToString() + " / " + simulation.ActiveMission.PointCapacity, new Vector2(0, 100), Color.White);

    // draw units
    int pageCount = simulation.SelectableUnits.Count / 9;
    var left = simulation.CurrentPage > 0 ? "< " : "  ";
    var right = simulation.CurrentPage < pageCount ? " >" : "  ";
    spriteBatch.DrawString(basicFontLarge, $"{left}{simulation.CurrentPage + 1}{right}", new Vector2(screenSize.X / 2, 1700), Color.White);

    int offset = 0;
    var max = MathHelper.Min(simulation.SelectableUnits.Count, (simulation.CurrentPage + 1) * 9);
    for (int i = simulation.CurrentPage * 9; i < max; i++)
    {
      var unit = simulation.SelectableUnits[i];
      var position = new Vector2(300, 650 + offset * 100);
      var name = unit.BaseUnit.DataSheet.Name.Length > 20 ? unit.BaseUnit.DataSheet.Name.Substring(0, 20) + "..." : unit.BaseUnit.DataSheet.Name;
      var count = unit.BaseUnit.UnitLines.Count > 1 ? (unit.DeployedCount + 1).ToString() + "*" : unit.DeployedCount.ToString();


      foreach (var commander in simulation.Commanders)
        if (commander.Units.Contains(unit))
          spriteBatch.Draw(icons[commander.Icon], new Rectangle(new Point((int)position.X - 72, (int)position.Y + 8), new Point(64, 64)), commander.Color);

      spriteBatch.DrawString(basicFontLarge, (offset + 1).ToString() + ". " + name, position, i == simulation.SelectedUnit ? Color.OrangeRed : Color.White);
      spriteBatch.DrawString(basicFontLarge, count, position + new Vector2(1000, 0), Color.OrangeRed);
      spriteBatch.DrawString(basicFontLarge, unit.Points.ToString(), position + new Vector2(1200, 0), Color.YellowGreen);

      offset += 1;
    }




    spriteBatch.End();
  }

  void DrawBattlefield(Matrix transformMatrix)
  {
    var m = simulation.ActiveMission;
    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix);

    spriteBatch.DrawString(basicFont, simulation.SelectedWorld.Name, new Vector2(1024, 0), Color.White);
    spriteBatch.DrawString(basicFont, simulation.SelectedMission.Name, new Vector2(1024, 64), Color.White);

    var (left, right) = m.Tiles;
    if (left != null && right != null)
    {
      spriteBatch.Draw(MapTextures[left.Texture], new Rectangle(new Point(), new Point((int)(screenSize.X / 2), (int)screenSize.Y)), Color.White);
      spriteBatch.Draw(MapTextures[right.Texture], new Rectangle(new Point((int)(screenSize.X / 2), 0), new Point((int)(screenSize.X / 2), (int)screenSize.Y)), Color.White);
    }
    spriteBatch.End();

    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix, blendState: BlendState.Additive);
    foreach (var zone in m.PlayerDeploymentZones)
    {
      spriteBatch.Draw(BlankTexture, zone, new Color(119, 221, 119, 100));
    }
    spriteBatch.End();
  }

  void DrawMainMenu(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "War Club", new Vector2(screenSize.X / 2, 300), Color.White);
    spriteBatch.DrawString(basicFontLarge, "1. Start New Game", new Vector2(screenSize.X / 2, 600), Color.White);
    spriteBatch.DrawString(basicFontLarge, "2. Load Game", new Vector2(screenSize.X / 2, 700), Color.White);
    spriteBatch.End();
  }

  void DrawNewGame(Matrix transformMatrix)
  {
    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "new game", new Vector2(screenSize.X / 2, 300), Color.White);
    spriteBatch.DrawString(basicFontLarge, "Select two units per player", new Vector2(screenSize.X / 2, 400), Color.White);

    var totalPoints = simulation.SelectedUnits.Aggregate(0, (acc, x) => acc + x.Points);

    spriteBatch.DrawString(basicFontLarge, $"{simulation.SelectedUnits.Count.ToString()} Selected Units - {totalPoints} Points", new Vector2(screenSize.X / 2, 500), Color.White);

    int pageCount = simulation.SelectableUnits.Count / 9;
    var left = simulation.CurrentPage > 0 ? "< " : "  ";
    var right = simulation.CurrentPage < pageCount ? " >" : "  ";
    spriteBatch.DrawString(basicFontLarge, $"{left}{simulation.CurrentPage + 1}{right}", new Vector2(screenSize.X / 2, 1700), Color.White);
    spriteBatch.DrawString(basicFontLarge, "Press enter to continue", new Vector2(screenSize.X / 2, 1800), Color.White);

    int offset = 0;
    var max = MathHelper.Min(simulation.SelectableUnits.Count, (simulation.CurrentPage + 1) * 9);
    for (int i = simulation.CurrentPage * 9; i < max; i++)
    {
      var unit = simulation.SelectableUnits[i];
      if (simulation.SelectedUnits.Contains(unit))
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
    spriteBatch.Begin(transformMatrix: transformMatrix * simulation.ViewMatrix);
    spriteBatch.DrawString(basicFontLarge, "load game", new Vector2(screenSize.X / 2, 300), Color.White);
    // spriteBatch.DrawString(basicFontLarge, "1. Start New Game", new Vector2(screenSize.X / 2, 600), Color.White);
    // spriteBatch.DrawString(basicFontLarge, "2. Load Game", new Vector2(screenSize.X / 2, 700), Color.White);
    spriteBatch.End();
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
      effect.Projection = simulation.ViewMatrix;
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

    spriteBatch.Begin(effect: planetEffect, sortMode: SpriteSortMode.Deferred, transformMatrix: simulation.ViewMatrix);
    spriteBatch.Draw(screenTexture, new Rectangle(location - size / new Point(2), size), Color.White);
    spriteBatch.End();
  }

  void DrawStarfield()
  {
    starfieldEffect.Parameters["iTime"].SetValue(animationTime / 10000f);
    spriteBatch.Begin(effect: starfieldEffect, sortMode: SpriteSortMode.Deferred, transformMatrix: simulation.ViewMatrix);
    spriteBatch.Draw(screenTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
    spriteBatch.End();
  }

}
