using Godot;
using Godot.Collections;
using SkillQuest.Graphics;
using System;
using System.Threading;

public partial class Main : Control {
	public ComputeShader shader;
	[ Export ] public MeshInstance2D Mesh { get; set; }

	private Rid colors;
	private Rid settings;
	private Rid uniforms;
	
	public override void _Ready() {
		
		shader = new(
			"res://Client/shaders/main.comp"
			);
		
		var size = GetViewportRect().Size;
		var len = ( uint ) GetViewportRect().Size.X * ( uint ) GetViewportRect().Size.Y;
		var bytes = new byte[len * sizeof( float ) * 4  ];

		colors = shader.Device.StorageBufferCreate( ( uint ) bytes.Length, bytes );
		RDUniform uniform_colors = new() {
			UniformType = RenderingDevice.UniformType.StorageBuffer,
			Binding = 0,
		};
		uniform_colors.AddId( colors );

		byte[] s = GetSettings();
		
		settings = shader.Device.StorageBufferCreate( (uint)s.Length, s );
		
		RDUniform uniform_settings = new() {
			UniformType = RenderingDevice.UniformType.StorageBuffer,
			Binding = 1,
		};
		uniform_settings.AddId( settings );

		uniforms = shader.Device.UniformSetCreate( new Array<RDUniform>() { uniform_colors, uniform_settings }, shader.ID, 0 );
		
		DispatchShader();
		RetrieveShaderData();
	}

	private byte[] GetSettings() {
		//Vector2 viewportSize =  GetViewportRect().Size;
		
		int[] settings = {(int) GetViewportRect().Size.X, (int) GetViewportRect().Size.Y};
		byte[] settingsBytes = new byte[settings.Length * sizeof(int)];
		Buffer.BlockCopy(settings, 0, settingsBytes, 0, settingsBytes.Length);

		return settingsBytes;
	}

	private void DispatchShader(){
		Rid pipelineRid = shader.Device.ComputePipelineCreate(shader.ID);
		long computeList = shader.Device.ComputeListBegin();
		shader.Device.ComputeListBindComputePipeline(computeList, pipelineRid);
		shader.Device.ComputeListBindUniformSet(computeList, uniforms, 0);
		shader.Device.ComputeListDispatch(computeList, xGroups: (uint) (GetViewportRect().Size.X), yGroups: (uint) (GetViewportRect().Size.Y), zGroups: 1);
		shader.Device.ComputeListEnd();
		
		shader.Device.Submit();
		shader.Device.Sync();
	}
	
	private void RetrieveShaderData(){
		//retrieving particle data from the shader
		var data = shader.Device.BufferGetData(colors);

		var image = new Image();
		image.SetData( ( int ) GetViewportRect().Size.X, ( int ) GetViewportRect().Size.Y, false, Image.Format.Rgbaf, data );
		
		(Mesh.Texture as ImageTexture).SetImage( image );

	}
	
	public override void _Notification(int what){
		if (what == NotificationPredelete) {
			shader.Dispose();
			shader = null;
		}
	}
}