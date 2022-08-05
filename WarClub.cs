using System;
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

    Vector2 screenSize = new Vector2(1920, 1024);
    // Vector2 screenSize = new Vector2(4096, 2160);

    Vector3 camTarget;
    Vector3 camPosition;
    Matrix projectionMatrix;
    Matrix viewMatrix;
    Matrix worldMatrix;

    Matrix pvm;

    BasicEffect basicEffect;
    Effect planetEffect;
    Effect starfieldEffect;

    Texture2D planetNoise;

    Model model;
    Model plane;
    Texture2D screenTexture;

    float timeAdvance;
    float animationTime = 0;
    VertexPositionColor[] triangleVertices;
    VertexBuffer vertexBuffer;

    // TerrainFace terrainFace = new TerrainFace(100, Vector3.Up);

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

      graphics.PreferredBackBufferHeight = (int)screenSize.Y;
      graphics.PreferredBackBufferWidth = (int)screenSize.X;
      graphics.ApplyChanges();

      camTarget = new Vector3(0f, 0f, 0f);
      camPosition = new Vector3(0f, 0f, -50f);

      // projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
      // projectionMatrix = Matrix.CreateOrthographic(160, 90, 1f, 1000f);
      // projectionMatrix = Matrix.CreateOrthographic(16, 9, 1f, 1000f);
      // projectionMatrix = Matrix.CreateOrthographic(100 * 16, 100 * 9, 1f, 1000f);
      // projectionMatrix = Matrix.CreateOrthographic(-1200, -1200, 1f, 1000f);
      projectionMatrix = Matrix.CreateOrthographicOffCenter(0, 1600, 900, 0, -10f, 1000f);

      // projectionMatrix = Matrix.create
      viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
      // worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up) * Matrix.CreateRotationY(MathHelper.ToRadians(-12.0f));
      // worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
      worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
      // worldMatrix = Matrix.CreateTranslation(0.5f, 0.5f, 0);
      // worldMatrix = Matrix.CreateTranslation(1.5f, 0.5f, 0);
      // worldMatrix = Matrix.Identity;

      // pvm = projectionMatrix * viewMatrix * worldMatrix;
      pvm = projectionMatrix;

      basicEffect = new BasicEffect(GraphicsDevice);
      basicEffect.Alpha = 1f;
      basicEffect.VertexColorEnabled = true;
      basicEffect.LightingEnabled = false;

      triangleVertices = new VertexPositionColor[3];
      triangleVertices[0] = new VertexPositionColor(new Vector3(0, 20, 0), Color.Red);
      triangleVertices[1] = new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green);
      triangleVertices[2] = new VertexPositionColor(new Vector3(20, -20, 0), Color.Blue);

      // terrainFace.ConstructMesh();

      // vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), terrainFace.vertices.Length, BufferUsage.WriteOnly);
      // vertexBuffer.SetData<VertexPositionColor>(terrainFace.vertices);
      vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), triangleVertices.Length, BufferUsage.WriteOnly);
      vertexBuffer.SetData<VertexPositionColor>(triangleVertices);

      screenTexture = new Texture2D(GraphicsDevice, (int)screenSize.X, (int)screenSize.Y);

    }

    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      simulation = new Simulation();
      simulation.Generate();
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
      // basicEffect.Projection = projectionMatrix;
      // basicEffect.View = Matrix.Identity;
      // basicEffect.World = worldMatrix;

      GraphicsDevice.Clear(new Color(10, 10, 10, 255));

      DrawStarfield();

      int x = 0;
      int y = 0;

      DrawPlanetShader(simulation.cosmos.Worlds.First().Value, new Point(0), new Point(1080));
      // foreach (var world in simulation.cosmos.Worlds)
      // {
      //   // DrawPlanetShader(world.Value, new Point(x, y), new Point((int)(128 * world.Value.size * 0.85)));
      //   DrawPlanetShader(world.Value, new Point(x, y), new Point(128));
      //   x += 128;
      //   if (x >= 1920)
      //   {
      //     x = 0;
      //     y += 128;
      //   }
      // }

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

    void DrawPlanet(World planet)
    {
      ModelMesh mesh = model.Meshes[0];
      mesh.MeshParts[0].Effect = basicEffect;
      // Matrix matrix = Matrix.CreateScale(10.0f) * Matrix.CreateFromQuaternion(planet.rotation) * Matrix.CreateTranslation(planet.location.X, planet.location.Y, 0);

      foreach (BasicEffect effect in mesh.Effects)
      {
        effect.View = Matrix.Identity;
        // effect.World = worldMatrix * matrix;
        effect.Projection = projectionMatrix;
        effect.EnableDefaultLighting();
        // effect.PreferPerPixelLighting = true;
        effect.DiffuseColor = new Vector3(1, 1, 1);
        // effect.EmissiveColor = new Vector3(1, 1, 1);
      }

      mesh.Draw();
    }

    void DrawPlanetShader(World world, Point location, Point size)
    {
      // planetEffect.Parameters["iTime"].SetValue(-(world.Id * 100 + animationTime * world.rotationSpeed * 0.0005f));
      planetEffect.Parameters["iTime"].SetValue(-(world.Id * 100 + animationTime * 0.0002f));
      planetEffect.Parameters["col_top"].SetValue(world.color_top.ToVector3());
      planetEffect.Parameters["col_bot"].SetValue(world.color_bot.ToVector3());
      planetEffect.Parameters["col_mid1"].SetValue(world.color_mid1.ToVector3());
      planetEffect.Parameters["col_mid2"].SetValue(world.color_mid2.ToVector3());
      planetEffect.Parameters["col_mid3"].SetValue(world.color_mid3.ToVector3());


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

      spriteBatch.Begin(effect: planetEffect, sortMode: SpriteSortMode.Deferred);
      // spriteBatch.Draw(screenTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
      // spriteBatch.Draw(screenTexture, new Rectangle(0, 0, 512, 512), Color.White);
      spriteBatch.Draw(screenTexture, new Rectangle(location, size), Color.White);
      spriteBatch.End();
    }

    void DrawStarfield()
    {
      starfieldEffect.Parameters["iTime"].SetValue(animationTime / 100000f);
      spriteBatch.Begin(effect: starfieldEffect, sortMode: SpriteSortMode.Deferred);
      spriteBatch.Draw(screenTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
      spriteBatch.End();
    }
  }
}