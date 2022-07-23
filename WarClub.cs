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
      planetEffect = Content.Load<Effect>("effect");
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

      if (timeAdvance >= 1000000)
      {
        timeAdvance = 0;
        simulation.AdvanceTime(1);
      }
      timeAdvance += gameTime.ElapsedGameTime.Milliseconds;

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      // basicEffect.Projection = projectionMatrix;
      // basicEffect.View = Matrix.Identity;
      // basicEffect.World = worldMatrix;

      GraphicsDevice.Clear(new Color(10, 10, 10, 255));

      DrawStarfield();

      // RasterizerState rasterizerState = new RasterizerState();
      // rasterizerState.CullMode = CullMode.None;
      // rasterizerState.FillMode = FillMode.WireFrame;
      // GraphicsDevice.RasterizerState = rasterizerState;

      // foreach (var planet in simulation.cosmos.Planets)
      //   DrawPlanet(planet.Value);

      // foreach (var star in simulation.cosmos.Stars)
      //   DrawStar(star.Value);

      // DrawStarfield();
      // DrawPlanet(simulation.cosmos.Planets[0]);


      // spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
      // spriteBatch.Draw(planetNoise, new Rectangle(0, 0, 512, 512), Color.White);
      // spriteBatch.End();

      // foreach (int x in Enumerable.Range(0, 4))
      // foreach (int y in Enumerable.Range(0, 4))
      // DrawPlanet(new Vector3(x * 2, y * 2, 0));

      // GraphicsDevice.SetVertexBuffer(vertexBuffer);

      // starfieldEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateTranslation(100, 100, 0) * projectionMatrix);
      // starfieldEffect.Parameters["NoiseTexture"].SetValue(planetNoise);
      // starfieldEffect.Parameters["_MainTex"].SetValue(planetNoise);

      // starfieldEffect.CurrentTechnique.Passes.First().Apply();

      // GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, triangleVertices, 0, 1);



      // foreach (EffectPass pass in starfieldEffect.CurrentTechnique.Passes)
      // {
      //   pass.Apply();
      //   GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, triangleVertices, 0, 1);
      // }

      base.Draw(gameTime);
    }

    void DrawStar(Star star)
    {
      ModelMesh mesh = model.Meshes[0];
      mesh.MeshParts[0].Effect = basicEffect;
      foreach (BasicEffect effect in mesh.Effects)
      {
        // Matrix matrix = Matrix.CreateScale(star.size * 0.3f) * Matrix.CreateFromQuaternion(star.rotation) * Matrix.CreateTranslation(star.location.X, star.location.Y, 0);
        Matrix matrix = Matrix.CreateScale(1f) * Matrix.CreateFromQuaternion(star.rotation) * Matrix.CreateTranslation(star.location.X, star.location.Y, 0);
        // Matrix matrix = Matrix.CreateTranslation(8, 0, 0);
        // effect.View = viewMatrix;
        effect.View = Matrix.Identity;
        effect.World = worldMatrix * matrix;
        effect.Projection = projectionMatrix;

        // effect.EnableDefaultLighting();
        // effect.PreferPerPixelLighting = true;
        // effect.DiffuseColor = new Vector3(1, 1, 1);
        // effect.EmissiveColor = new Vector3(1, 1, 1);
      }
      mesh.Draw();
    }


    void DrawPlanet(Planet planet)
    {
      // ModelMesh mesh = model.Meshes[0];
      ModelMesh mesh = plane.Meshes[0];

      // foreach (BasicEffect effect in mesh.Effects)
      // {
      //   effect.View = viewMatrix;
      //   effect.World = worldMatrix * Matrix.CreateTranslation(position) * Matrix.CreateScale(3);
      //   effect.Projection = projectionMatrix;

      // effect.EnableDefaultLighting();
      // effect.PreferPerPixelLighting = true;
      // effect.DiffuseColor = new Vector3(0, 1, 0);
      // effect.CurrentTechnique = planetEffect.CurrentTechnique;
      // }
      foreach (ModelMeshPart part in mesh.MeshParts)
      {
        part.Effect = starfieldEffect;
        // Matrix matrix = Matrix.CreateScale(planet.size * 0.1f) * Matrix.CreateFromQuaternion(planet.rotation) * Matrix.CreateTranslation(planet.distance, 0, 0) * Matrix.CreateFromQuaternion(planet.orbit) * Matrix.CreateTranslation(planet.star.location.X, planet.star.location.Y, 0);
        Matrix matrix = Matrix.CreateScale(0.5f) * Matrix.CreateFromQuaternion(planet.rotation) * Matrix.CreateTranslation(planet.distance, 0, 0) * Matrix.CreateFromQuaternion(planet.orbit) * Matrix.CreateTranslation(planet.star.location.X, planet.star.location.Y, 0);

        // starfieldEffect.Parameters["World"].SetValue(worldMatrix * matrix);
        // starfieldEffect.Parameters["View"].SetValue(viewMatrix);
        // starfieldEffect.Parameters["Projection"].SetValue(projectionMatrix);
        // starfieldEffect.Parameters["AmbientColor"].SetValue(planet.color.ToVector4());
        // starfieldEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateScale(100f) * Matrix.CreateTranslation(80, 80, 0) * projectionMatrix);
        starfieldEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateScale(1f) * Matrix.CreateTranslation(0, 0, 0) * projectionMatrix);
        // starfieldEffect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
        // starfieldEffect.Parameters["AmbientIntensity"].SetValue(1f);
        // starfieldEffect.Parameters["NoiseTexture"].SetValue(planetNoise);
        starfieldEffect.Parameters["time"].SetValue(timeAdvance / 3000f);
      }
      mesh.Draw();
    }

    void DrawStarfield()
    {
      starfieldEffect.Parameters["time"].SetValue(timeAdvance / 3000f);
      spriteBatch.Begin(effect: starfieldEffect);
      spriteBatch.Draw(screenTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
      spriteBatch.End();

      // ModelMesh mesh = plane.Meshes[0];
      // ModelMeshPart part = mesh.MeshParts[0];
      // part.Effect = starfieldEffect;
      // starfieldEffect.Parameters["WorldViewProjection"].SetValue(Matrix.Identity * projectionMatrix);
      // starfieldEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateScale(100f) * Matrix.CreateTranslation(80, 80, 0) * projectionMatrix);
      // starfieldEffect.Parameters["WorldViewProjection"].SetValue(Matrix.CreateScale(100f) * Matrix.CreateTranslation(80, 80, 0) * pvm);
      // starfieldEffect.Parameters["time"].SetValue(timeAdvance / 3000f);
      // mesh.Draw();
    }
  }
}
