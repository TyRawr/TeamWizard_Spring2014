using UnityEngine;
using System.Collections;
using Mixamo;

public class FaceVideoAVProWMV : AVProWindowsMediaMovie, IFacePlusVideo {

	public uint DurationFrames { 
		get {
			return base.MovieInstance.DurationFrames;
		} 
	}

	public uint PositionFrames { 
		get {
			return base.MovieInstance.PositionFrames;
		}
		set {
			base.MovieInstance.PositionFrames = value;
		}
	}
	public float DurationSeconds { 
		get {
			return base.MovieInstance.DurationSeconds;
		}
	}

	public float PositionSeconds { 
		get {
			return base.MovieInstance.PositionSeconds;
		}
		set {
			base.MovieInstance.PositionSeconds = value;
		}
	}

	public int DisplayFrame { 
		get {
			return base.MovieInstance.DisplayFrame;
		}
	}

	public bool LoadMovie(string folder, string filename, bool val) {
		base._folder = folder;
		base._filename = filename;
		return LoadMovie (val);
	}

	public Texture OutputTexture { get {
			return base.MovieInstance.OutputTexture;
		}
	}

	public float FrameRate { 
		get {
			return base.MovieInstance.FrameRate;
		}
	}

	public void Rewind() {
		base.MovieInstance.Rewind ();
	}

	public void Dispose(){
		if (MovieInstance != null) base.MovieInstance.Dispose ();
	}

	public void Play() {
		base.MovieInstance.Play ();
	}

	public void Pause() {
		base.MovieInstance.Pause ();
	}

	public void UpdateMovie(bool force) {
		base.MovieInstance.Update (force);
	}

	void Start() {
		var mediaDisplay = gameObject.AddComponent<AVProWindowsMediaGUIDisplay>();
		var mediaManager = gameObject.AddComponent<AVProWindowsMediaManager>();
		mediaDisplay._movie = this;
		mediaDisplay._x = 0.7f;
		mediaDisplay._width = 0.3f;
		mediaDisplay._height = 0.3f;
		mediaDisplay._fullScreen = false;
		mediaManager._shaderBGRA32 = Shader.Find ("Hidden/AVProWindowsMedia/CompositeBGRA_2_RGBA");
		mediaManager._shaderHDYC = Shader.Find ("Hidden/AVProWindowsMedia/CompositeHDYC_2_RGBA");
		mediaManager._shaderUYVY = Shader.Find ("Hidden/AVProWindowsMedia/CompositeUYVY_2_RGBA");
		mediaManager._shaderNV12 = Shader.Find ("Hidden/AVProWindowsMedia/CompositeNV12_709");
		mediaManager._shaderYUY2 = Shader.Find ("Hidden/AVProWindowsMedia/CompositeYUY2_2_RGBA");
		mediaManager._shaderYUY2_709 = Shader.Find ("Hidden/AVProWindowsMedia/CompositeYUY2709_2_RGBA");
		mediaManager._shaderYVYU = Shader.Find ("Hidden/AVProWindowsMedia/CompositeYVYU_2_RGBA");
		base.Start ();
	}
}
