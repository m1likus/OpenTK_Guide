using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;


namespace Open_TK
{

	public class Shader
	{
		public int shaderHandle;

		public void LoadShader()
		{
			shaderHandle = GL.CreateProgram();

			int vertexShader = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(vertexShader, LoadShaderSource("shader.vert"));
			GL.CompileShader(vertexShader);

			GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success1);
			if (success1 == 0)
			{
				string infoLog = GL.GetShaderInfoLog(vertexShader);
				Console.WriteLine(infoLog);
			}

			int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
			GL.ShaderSource(fragmentShader, LoadShaderSource("shader.frag"));
			GL.CompileShader(fragmentShader);

			GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int success2);
			if (success2 == 0)
			{
				string infoLog = GL.GetShaderInfoLog(fragmentShader);
				Console.WriteLine(infoLog);
			}

			GL.AttachShader(shaderHandle, vertexShader);
			GL.AttachShader(shaderHandle, fragmentShader);

			GL.LinkProgram(shaderHandle);

			GL.DeleteShader(vertexShader);
			GL.DeleteShader(fragmentShader);
		}

		public void UseShader()
		{
			GL.UseProgram(shaderHandle);
		}
		public void DeleteShader()
		{
			GL.DeleteProgram(shaderHandle);
		}

		public static string LoadShaderSource(string filepath)
		{
			string shaderSource = "";
			try
			{
				using (StreamReader reader = new StreamReader("../../../Shaders/" + filepath))
				{
					shaderSource = reader.ReadToEnd();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to load shader source file: " + e.Message);
			}
			return shaderSource;
		}
		

	}



	internal class Game : GameWindow
	{
		int width, height;



		List<Vector3> vertices = new List<Vector3>()
		{	
			//front face
			new Vector3(-0.5f,  0.5f, 0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, 0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, 0.5f), //bottom-right vertice
			new Vector3(-0.5f, -0.5f, 0.5f), //botom-left vertice
			//right face
			new Vector3( 0.5f,  0.5f, 0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, -0.5f), //bottom-right vertice
			new Vector3( 0.5f, -0.5f, 0.5f), //botom-left vertice
			//back face
			new Vector3(-0.5f,  0.5f, -0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, -0.5f), //bottom-right vertice
			new Vector3(-0.5f, -0.5f, -0.5f), //botom-left vertice
			//left face
			new Vector3( -0.5f,  0.5f, 0.5f), //top-left vertice
			new Vector3( -0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( -0.5f, -0.5f, -0.5f), //bottom-right vertice
			new Vector3( -0.5f, -0.5f, 0.5f), //botom-left vertice
			// top face
			new Vector3(-0.5f,  0.5f, -0.5f), //top-left vertice
			new Vector3( 0.5f,  0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, 0.5f, 0.5f), //bottom-right vertice
			new Vector3(-0.5f, 0.5f, 0.5f), //botom-left vertice
			//bottom face
			new Vector3(-0.5f,  -0.5f, -0.5f), //top-left vertice
			new Vector3( 0.5f,  -0.5f, -0.5f), //top-right vertice
			new Vector3( 0.5f, -0.5f, 0.5f), //bottom-right vertice
			new Vector3(-0.5f, -0.5f, 0.5f), //botom-left vertice
		};

		List<Vector2> texCoords = new List<Vector2>()
		{ 
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),

			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),

			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),

			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),

			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),

			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 0f),
		};

		uint[] indices =
		{
			0, 1, 2,	//top triangle
			2, 3, 0,	//bottom triangle

			4, 5, 6,
			6, 7, 4,

			8, 9, 10,
			10, 11, 8,

			12, 13, 14,
			14, 15, 12,

			16, 17, 18,
			18, 19, 16,

			20, 21, 22,
			22, 23, 20
		};


		int VAO;
		int VBO;
		int EBO;
		Shader shaderProgram = new Shader();
		int textureID;
		int textureVBO;
		Camera camera;


		float yRot = 0f;

		public Game (int width, int height): base (GameWindowSettings.Default, NativeWindowSettings.Default)
		{
			this.CenterWindow(new Vector2i(width, height));
			this.height = height;
			this.width = width;
		}
		
		protected override void OnLoad()
		{
			base.OnLoad();

			//Create VAO
			VAO = GL.GenVertexArray();
			//Create VBO
			VBO = GL.GenBuffer();
			//Bind the VBO 
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
			//Copy vertices data to the buffer
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
			//Bind the VAO
			GL.BindVertexArray(VAO);
			//Point a slot number 0
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			//Enable the slot
			GL.EnableVertexArrayAttrib(VAO, 0);

			//Unbind the VBO
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			//EBO 
			EBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			//Create, bind texture
			textureVBO = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
			GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector3.SizeInBytes , texCoords.ToArray(), BufferUsageHint.StaticDraw);
			//Point a slot number 1
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
			//Enable the slot
			GL.EnableVertexArrayAttrib(VAO, 1);


			//Delete everything
			GL.BindVertexArray(0);

			shaderProgram.LoadShader();

			// Texture Loading
			textureID = GL.GenTexture(); //Generate empty texture
			GL.ActiveTexture(TextureUnit.Texture0); //Activate the texture in the unit
			GL.BindTexture(TextureTarget.Texture2D, textureID); //Bind texture

			//Texture parameters
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

			//Load image
			StbImage.stbi_set_flip_vertically_on_load(1);
			ImageResult boxTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/box.jpg"), ColorComponents.RedGreenBlueAlpha);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boxTexture.Width, boxTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boxTexture.Data);

			//Unbind the texture
			GL.BindTexture(TextureTarget.Texture2D, 0);


			GL.Enable(EnableCap.DepthTest);

			camera = new Camera(width, height, Vector3.Zero);
			CursorState = CursorState.Grabbed;
		}

		protected override void OnUnload()
		{
			base.OnUnload();

			GL.DeleteBuffer(VAO);
			GL.DeleteBuffer(VBO);
			GL.DeleteBuffer(EBO);
			GL.DeleteTexture(textureID);

			shaderProgram.DeleteShader();

		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.ClearColor(0.3f, 0.3f, 1f, 1f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			shaderProgram.UseShader();
			GL.BindTexture(TextureTarget.Texture2D, textureID);

			//Transformation
			Matrix4 model = Matrix4.Identity;
			Matrix4 view = camera.GetViewMatrix();
			Matrix4 projection = camera.GetProjection();
			Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);

			model = Matrix4.CreateRotationY(yRot);
			yRot += 0.001f;

			model *= translation;

			int modelLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "model");
			int viewLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "view");
			int projectionLocation = GL.GetUniformLocation(shaderProgram.shaderHandle, "projection");

			GL.UniformMatrix4(modelLocation, true, ref model);
			GL.UniformMatrix4(viewLocation, true, ref view);
			GL.UniformMatrix4(projectionLocation, true, ref projection);

			GL.BindVertexArray(VAO);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
			//GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

			Context.SwapBuffers();

			base.OnRenderFrame(args);
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			
			if (KeyboardState.IsKeyDown(Keys.Escape))
			{
				Close();
			}
			MouseState mouse = MouseState;
			KeyboardState input = KeyboardState;

			base.OnUpdateFrame(args);
			camera.Update(input, mouse, args);

		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);
			GL.Viewport(0, 0, e.Width, e.Height);
			this.width = e.Width;
			this.height = e.Height;
		}

		


	}


}
