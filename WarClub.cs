using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WarClub
{
  public class WarClub : Game
  {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    Simulation simulation;

    Vector2 viewportSize = new Vector2(1920, 1080);
    // Vector2 viewportSize = new Vector2(3840, 2160);

    Vector2 screenSize = new Vector2(3840, 2160);
    Matrix viewMatrix;

    BasicEffect basicEffect;
    Effect planetEffect;
    Effect starfieldEffect;

    Texture2D planetNoise;

    Model model;
    Model plane;
    Texture2D screenTexture;

    float timeAdvance;
    float animationTime = 0;
    World selectedWorld = null;

    private SpriteFont basicFont;
    private SpriteFont basicFontSmall;
    public Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();

    public WarClub()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }


    void GeneratePlanet()
    {
      FastNoiseLite elvNoise = new FastNoiseLite();

      elvNoise.SetSeed(RNG.Integer());
      elvNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
      elvNoise.SetFrequency(0.010f);
      elvNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
      elvNoise.SetFractalOctaves(8);
      elvNoise.SetFractalLacunarity(2.00f);
      elvNoise.SetFractalGain(0.60f);

      Point size = new Point(512, 512);
      planetNoise = new Texture2D(GraphicsDevice, size.X, size.Y, false, SurfaceFormat.Color);
      GraphicsDevice.Textures[0] = null;

      float scale = 512f / (size.X + size.Y);

      uint[] arr = new uint[size.X * size.Y];

      foreach (int x in Enumerable.Range(0, size.X))
        foreach (int y in Enumerable.Range(0, size.Y))
        {
          float height = (elvNoise.GetNoise(x * scale, y * scale) + 1) * 0.5f;
          Color c = new Color(height, height, height, 1);
          arr[(y * size.Y) + x] = c.PackedValue;
        }

      planetNoise.SetData<UInt32>(arr, 0, size.X * size.Y);
      // triangleVertices = new VertexPositionColor[1000];

      // double t = (1 + Math.Sqrt(5.0)) / 2.0;
      // triangleVertices[0] 

    }

    protected override void Initialize()
    {
      // TODO: Add your initialization logic here

      base.Initialize();

      graphics.PreferredBackBufferHeight = (int)viewportSize.Y;
      graphics.PreferredBackBufferWidth = (int)viewportSize.X;
      // graphics.IsFullScreen = true;
      // Window.Position = new Point(-5760, 0);
      // Window.IsBorderless = true;
      graphics.ApplyChanges();

      viewMatrix = Matrix.CreateScale(viewportSize.X / screenSize.X, viewportSize.Y / screenSize.Y, 1);

      basicEffect = new BasicEffect(GraphicsDevice);
      basicEffect.Alpha = 1f;
      basicEffect.VertexColorEnabled = true;
      basicEffect.LightingEnabled = false;

      screenTexture = new Texture2D(GraphicsDevice, (int)screenSize.X, (int)screenSize.Y);

    }

    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      simulation = new Simulation();
      simulation.Generate();
      icons.Add("axe", Content.Load<Texture2D>("icons/battered-axe"));

      basicFont = Content.Load<SpriteFont>("romulus");
      basicFontSmall = Content.Load<SpriteFont>("romulus_small");
      model = Content.Load<Model>("IcoSphere");
      plane = Content.Load<Model>("Plane");
      planetEffect = Content.Load<Effect>("planetEffect");
      starfieldEffect = Content.Load<Effect>("starfield");
      GeneratePlanet();
    }

    protected override void Update(GameTime gameTime)
    {
      // worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(-0.1f));
      // Quaternion.CreateFromYawPitchRoll()

      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // if (Keyboard.GetState().IsKeyDown(Keys.Right))
      // {
      //   worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(0.1f));
      // }
      if (Keyboard.GetState().IsKeyDown(Keys.Space))
      {
        selectedWorld = null;
      }

      var size = new Point(480, 570);

      if (Mouse.GetState().LeftButton == ButtonState.Pressed && selectedWorld == null)
      {
        var pos = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(viewMatrix));
        foreach (var world in simulation.cosmos.Worlds)
          if (new Rectangle(world.Value.location.ToPoint() - size / new Point(2), size).Contains(pos))
            selectedWorld = world.Value;
      }

      // TODO: Add your update logic here

      if (timeAdvance >= 5000)
      {
        timeAdvance = 0;
        simulation.AdvanceTime();
      }
      timeAdvance += gameTime.ElapsedGameTime.Milliseconds;
      animationTime += gameTime.ElapsedGameTime.Milliseconds;
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

      GraphicsDevice.Clear(new Color(10, 10, 10, 255));
      DrawStarfield();

      // DrawPlanetShader(simulation.cosmos.Worlds.First().Value, new Point(0), new Point(1080));
      if (selectedWorld == null)
        DrawGalaxyOverview();
      else
      {
        DrawPlanetShader(selectedWorld, new Point((int)screenSize.X / 2, (int)screenSize.Y / 2), new Point((int)(1000 * selectedWorld.size * 0.85)));
        DrawPlanetOverview(Matrix.CreateRotationZ(MathHelper.ToRadians(90)) * Matrix.CreateTranslation(512, 0, 0));
        DrawPlanetOverview(Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) * Matrix.CreateTranslation(screenSize.X - 512, screenSize.Y, 0));
      }

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

    void DrawGalaxyOverview()
    {
      foreach (var world in simulation.cosmos.Worlds)
        DrawPlanetShader(world.Value, world.Value.location.ToPoint(), new Point((int)(128 * world.Value.size * 0.85)));
      // DrawPlanetShader(world.Value, world.Value.location.ToPoint(), new Point(480, 540));


      spriteBatch.Begin(transformMatrix: viewMatrix);

      var pos = Vector2.Transform(Mouse.GetState().Position.ToVector2(), Matrix.Invert(viewMatrix));
      spriteBatch.DrawString(basicFont, pos.X.ToString(), new Vector2(0, 128), Color.White);
      spriteBatch.DrawString(basicFont, pos.Y.ToString(), new Vector2(0, 160), Color.White);


      spriteBatch.DrawString(basicFont, timeAdvance.ToString(), new Vector2(0, 0), Color.White);
      spriteBatch.DrawString(basicFont, simulation.cosmos.Day.ToString(), new Vector2(0, 32), Color.White);

      foreach (var world in simulation.cosmos.Worlds)
      {
        DrawPlanetOverlay(world.Value, world.Value.location.ToPoint() - new Point(240, 270), new Point((int)(128 * world.Value.size * 0.85)));
        if (TraitUtil.hasTrait(world.Value.GetTraits(), simulation.Traits["war zone"]))
          spriteBatch.Draw(icons["axe"], new Rectangle(world.Value.location.ToPoint(), new Point(64, 64)), Color.White);

        foreach (var mission in world.Value.GetEventList(simulation.OrderEventLists["Mercenary"]))
          spriteBatch.DrawString(basicFont, mission.Name, world.Value.location, Color.White);
      }
      spriteBatch.End();
    }


    void DrawPlanetOverview(Matrix transformMatrix)
    {
      spriteBatch.Begin(transformMatrix: transformMatrix * viewMatrix);

      var pos = Mouse.GetState().Position;
      spriteBatch.DrawString(basicFont, pos.X.ToString(), new Vector2(0, 128), Color.White);
      spriteBatch.DrawString(basicFont, pos.Y.ToString(), new Vector2(0, 160), Color.White);

      spriteBatch.DrawString(basicFont, timeAdvance.ToString(), new Vector2(0, 0), Color.White);
      spriteBatch.DrawString(basicFont, simulation.cosmos.Day.ToString(), new Vector2(0, 32), Color.White);

      spriteBatch.DrawString(basicFont, selectedWorld.Name, new Vector2(1024, 0), Color.White);

      var y = 0;

      if (TraitUtil.hasTrait(selectedWorld.GetTraits(), simulation.Traits["war zone"]))
        spriteBatch.Draw(icons["axe"], new Rectangle(new Point(1080, y += 64), new Point(64, 64)), Color.White);

      foreach (var mission in selectedWorld.GetEventList(simulation.OrderEventLists["Mercenary"]))
        spriteBatch.DrawString(basicFont, mission.Name, selectedWorld.location + new Vector2(0, y += 64), Color.White);

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
        effect.Projection = viewMatrix;
        effect.EnableDefaultLighting();
        // effect.PreferPerPixelLighting = true;
        effect.DiffuseColor = new Vector3(1, 1, 1);
        // effect.EmissiveColor = new Vector3(1, 1, 1);
      }

      mesh.Draw();
    }

    void DrawPlanetOverlay(World world, Point location, Point size)
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

      spriteBatch.Begin(effect: planetEffect, sortMode: SpriteSortMode.Deferred, transformMatrix: viewMatrix);
      spriteBatch.Draw(screenTexture, new Rectangle(location - size / new Point(2), size), Color.White);
      spriteBatch.End();
    }

    void DrawStarfield()
    {
      starfieldEffect.Parameters["iTime"].SetValue(animationTime / 10000f);
      spriteBatch.Begin(effect: starfieldEffect, sortMode: SpriteSortMode.Deferred, transformMatrix: viewMatrix);
      spriteBatch.Draw(screenTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
      spriteBatch.End();
    }
  }
}