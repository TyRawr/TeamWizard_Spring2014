using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2013 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Movie Capture/From Texture")]
public class AVProMovieCaptureFromTexture : AVProMovieCaptureBase
{
	private Texture _sourceTexture;
	private Rect _sourceTextureArea;
	public bool _useFastPixelFormat = true;
	public Shader _shaderSwapRedBlue;
	public Shader _shaderRGBA2YCbCr;
	private Material _materialSwapRedBlue;
	private Material _materialRGBA2YCbCr;
	private Material _materialConversion;

	public override void Start()
	{
		_materialSwapRedBlue = new Material(_shaderSwapRedBlue);
		_materialSwapRedBlue.hideFlags = HideFlags.HideAndDontSave;
		_materialRGBA2YCbCr = new Material(_shaderRGBA2YCbCr);
		_materialRGBA2YCbCr.hideFlags = HideFlags.HideAndDontSave;
		_materialRGBA2YCbCr.SetFloat("flipY", 1.0f);

		base.Start();
	}
	
	public override void OnDestroy()
	{
		_materialConversion = null;
		if (_materialSwapRedBlue != null)
		{
			Material.Destroy(_materialSwapRedBlue);
			_materialSwapRedBlue = null;
		}
		
		if (_materialRGBA2YCbCr != null)
		{
			Material.Destroy(_materialRGBA2YCbCr);
			_materialRGBA2YCbCr = null;
		}
		
		base.OnDestroy();
	}	
	
	public void SetSourceTexture(Texture texture)
	{
		SetSourceTextureRegion(texture, new Rect(0.0f, 0.0f, 1.0f, 1.0f));
	}

	public void SetSourceTextureRegion(Texture texture, Rect rect)
	{
		_sourceTexture = texture;
		_sourceTextureArea = rect;
	}
	
	void Capture()
	{
		if (_capturing && !_paused && _sourceTexture)
		{
			while (!AVProMovieCapturePlugin.IsNewFrameDue(_handle))
			{
				System.Threading.Thread.Sleep(1);
			}
			
			RenderTexture old = RenderTexture.active;
			RenderTexture buffer = RenderTexture.GetTemporary(_texture.width, _texture.height, 0);

			// Resize and convert pixel format
			
			// TODO: copy a region based on AREA
			//Graphics.Blit(_sourceTexture, buffer, _materialConversion);
			

			RenderTexture.active = buffer;
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, _texture.width, _texture.height, 0);
			Graphics.DrawTexture(new Rect(0, 0, _texture.width, _texture.height), _sourceTexture, _sourceTextureArea, 0, 0, 0, 0, _materialConversion);
			GL.PopMatrix();
			
			// Read out the pixels and send the frame to the encoder
			RenderTexture.active = buffer;
			_texture.ReadPixels(new Rect(0, 0, buffer.width, buffer.height), 0, 0, false);
			EncodeTexture(_texture);
			
			RenderTexture.ReleaseTemporary(buffer);
			RenderTexture.active = old;
			
			UpdateFPS();
		}
	}
	
	public override void UpdateFrame()
	{
		Capture();
		
		base.UpdateFrame();
	}

	public override void PrepareCapture()
	{
		if (_capturing)
			return;
		
		// Setup material
		_pixelFormat = AVProMovieCapturePlugin.PixelFormat.RGBA32;
		if (_useFastPixelFormat)
			_pixelFormat = AVProMovieCapturePlugin.PixelFormat.YCbCr422_YUY2;

		switch (_pixelFormat)
		{
			case AVProMovieCapturePlugin.PixelFormat.RGBA32:
				_materialConversion = _materialSwapRedBlue;
				_isTopDown = true;
				break;
			case AVProMovieCapturePlugin.PixelFormat.YCbCr422_YUY2:
				_materialConversion = _materialRGBA2YCbCr;
				_materialRGBA2YCbCr.SetFloat("flipY", 1.0f);
				_isTopDown = true;
				// If we're capturing uncompressed video in a YCbCr format we don't need to flip Y
				/*if (_codecIndex < 0)
				{
					_materialRGBA2YCbCr.SetFloat("flipY", 0.0f);
				}*/
				break;
		}
		if (_materialConversion == null)
		{
			Debug.LogError("Invalid pixel format");
			return;
		}
		
		SelectRecordingResolution(_sourceTexture.width, _sourceTexture.height);

		// When capturing YCbCr format we only need half the width texture
		int textureWidth = _targetWidth;
		if (_pixelFormat == AVProMovieCapturePlugin.PixelFormat.YCbCr422_YUY2)
			textureWidth /= 2;

		_texture = new Texture2D(textureWidth, _targetHeight, TextureFormat.ARGB32, false);

		GenerateFilename();

		base.PrepareCapture();
	}
}