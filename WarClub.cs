﻿using System;
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

    BasicEffect basicEffect;
    Effect planetEffect;

    Texture2D planetNoise;

    Model model;
    // VertexPositionColor[] triangleVertices;
    // VertexBuffer vertexBuffer;

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
          float height = elvNoise.GetNoise(x * scale, y * scale);
          Color c = new Color(255 * height, 255 * height, 255 * height, 255);
          arr[(y * size.Y) + x] = c.PackedValue;
        }

      planetNoise.SetData<UInt32>(arr, 0, size.X * size.Y);

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
      projectionMatrix = Matrix.CreateOrthographic(16, 9, 1f, 1000f);
      viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
      worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);

      basicEffect = new BasicEffect(GraphicsDevice);
      basicEffect.Alpha = 1f;
      basicEffect.VertexColorEnabled = true;
      basicEffect.LightingEnabled = false;

      // triangleVertices = new VertexPositionColor[3];
      // triangleVertices[0] = new VertexPositionColor(new Vector3(0, 20, 0), Color.Red);
      // triangleVertices[1] = new VertexPositionColor(new Vector3(-20, -20, 0), Color.Green);
      // triangleVertices[2] = new VertexPositionColor(new Vector3(20, -20, 0), Color.Blue);

      // terrainFace.ConstructMesh();

      // vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), terrainFace.vertices.Length, BufferUsage.WriteOnly);
      // vertexBuffer.SetData<VertexPositionColor>(terrainFace.vertices);
      // vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 3, BufferUsage.WriteOnly);
      // vertexBuffer.SetData<VertexPositionColor>(triangleVertices);

    }

    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      // TODO: use this.Content to load your game content here
      simulation = new Simulation();
      simulation.Generate();
      model = Content.Load<Model>("IcoSphere");
      planetEffect = Content.Load<Effect>("effect");
      GeneratePlanet();
    }

    protected override void Update(GameTime gameTime)
    {
      worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(0.1f)) * Matrix.CreateRotationX(MathHelper.ToRadians(0.1f));

      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      // if (Keyboard.GetState().IsKeyDown(Keys.Right))
      // {
      //   worldMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(0.1f));
      // }

      // TODO: Add your update logic here

      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      // basicEffect.Projection = projectionMatrix;
      // basicEffect.View = viewMatrix;
      // basicEffect.World = worldMatrix;

      GraphicsDevice.Clear(new Color(10, 10, 10, 255));
      // GraphicsDevice.SetVertexBuffer(vertexBuffer);

      RasterizerState rasterizerState = new RasterizerState();
      // rasterizerState.CullMode = CullMode.None;
      rasterizerState.FillMode = FillMode.WireFrame;
      GraphicsDevice.RasterizerState = rasterizerState;
      // planetEffect.CurrentTechnique.Passes[0].Apply();

      spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
      spriteBatch.Draw(planetNoise, new Rectangle(0, 0, 512, 512), Color.White);
      spriteBatch.End();

      DrawPlanet(new Vector3(0, 0, 0));
      // foreach (int x in Enumerable.Range(0, 4))
      // foreach (int y in Enumerable.Range(0, 4))
      // DrawPlanet(new Vector3(x * 2, y * 2, 0));

      // foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
      // {
      //   pass.Apply();
      //   GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, terrainFace.vertices.Length);
      // }

      base.Draw(gameTime);
    }


    void DrawPlanet(Vector3 position)
    {
      ModelMesh mesh = model.Meshes[0];


      foreach (ModelMeshPart part in mesh.MeshParts)
      {
        part.Effect = planetEffect;
        planetEffect.Parameters["World"].SetValue(worldMatrix * Matrix.CreateTranslation(position) * Matrix.CreateScale(3));
        planetEffect.Parameters["View"].SetValue(viewMatrix);
        planetEffect.Parameters["Projection"].SetValue(projectionMatrix);
        planetEffect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
        planetEffect.Parameters["AmbientIntensity"].SetValue(0.5f);
        planetEffect.Parameters["NoiseTexture"].SetValue(planetNoise);
      }
      mesh.Draw();
    }
  }
}
