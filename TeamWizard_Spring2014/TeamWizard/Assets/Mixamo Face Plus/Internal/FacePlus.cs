using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
//using System.Reflection;
using System.IO;

namespace Mixamo {
	public static class FacePlus {
		public static volatile int FramesTracked = 0;
		public static bool IsInitStarted = false;

		private const bool shouldLoadDlls = false;
		private static volatile bool initResult = false;
		private static volatile bool trackResult = false;
		private static volatile bool trackForever = true;
		private static Thread initThread = null;
		private static Thread trackingThread = null;

		static FacePlus() {
			string faceplusFolder = @"Assets\Plugins\Face Plus\";
			string fallbackFolder = "";

			string[] dlls = new string[] {
				"OpenCL.dll", // sometimes finds the wrong OpenCL on older NVIDIA machines
				"clAmdBlas.dll",
				"opencv_core249.dll",
				"opencv_highgui249.dll", // NB: obfuscated dll has dependent dlls included, no need for above
				"faceplus.dll"
			};

			if (shouldLoadDlls) {
				foreach(var dll in dlls) {
					if (!LoadFromFolder (faceplusFolder, dll)) {
						Logger.Log ("Trying fallback folder...");
						LoadFromFolder (fallbackFolder, dll);
					}
				}
			}
		}

		private static bool LoadFromFolder(string folder, string dll) {
			string path = Path.GetFullPath ( folder + dll );
			IntPtr ptr = LoadLibrary(path);
			Logger.Log ("DLL exists at path: " + File.Exists (path) + ". Load result: " + (ptr != IntPtr.Zero) 
			            + "\nPath:" + path 
			            + "\nCurrent directory:"+Environment.CurrentDirectory);
			return IntPtr.Zero != ptr;
		}

		public static int Login(string name, string password) {
			return faceplus_log_in (name, password);
		}
		
		public static void Init(string source) {
			IsInitStarted = true;
			initThread = new Thread(() => InitSynch (source));
			initThread.Start ();
			while(!initThread.IsAlive) {} // spin while thread starts
		}

		public static bool InitBufferTracker(int width, int height, float frameRate) {
			initResult = faceplus_init_buffer_tracker(width, height, (double) frameRate, "RGB");
			
			Logger.Log ("Initialization result: " + initResult);
			return initResult;
		}

		public static bool Teardown() {
			return faceplus_teardown();
		}
		
		public static IEnumerator AfterInit(Action<bool> complete) {
			while(!IsInitComplete) yield return new WaitForSeconds(0.1f);
			complete(IsInitSuccessful);
		}
		
		public static bool InitSynch(string source) {
			initResult = /*initResult ||*/ faceplus_init(source);
			return initResult;
		}
		
		public static bool IsInitComplete {
			get {
				return initThread != null 
					&& !initThread.IsAlive;
			}
		}
		
		public static bool IsInitSuccessful {
			get {
				//return IsInitComplete && initResult;
				return initResult;
			}
		}
		
		public static bool IsTracking {
			get {
				return trackResult;
			}
		}

		public static bool TrackSynchBuffer(byte[] frameBuffer,bool outputDebugImages) {
			trackResult = faceplus_synchronous_track_buffer(frameBuffer,outputDebugImages);
			if (trackResult) {
				FramesTracked++;
				UpdateCurrentVector();
			}
			return trackResult;
		}
		
		public static bool TrackSynch() {
			trackResult = faceplus_synchronous_track();

			if (trackResult) {
				FramesTracked++;
				UpdateCurrentVector ();
			}
			return trackResult;
		}
		
		public static IEnumerator TrackForever() {
			trackForever = true;
			while (trackForever) {
				TrackSynch ();
				yield return null;
			}
		}
		
		public static void TrackForeverThreaded() {
			trackingThread = new Thread(() => {
				trackForever = true;
				while (trackForever) TrackSynch ();
			});
			trackingThread.Start ();
			while(!trackingThread.IsAlive) {}
		}
		
		public static void StopTracking() {
			trackForever = false;
			Logger.Log ("Stopping tracking...");
			if (trackingThread != null) 
				trackingThread.Join ();

		}
		
		public static int ChannelCount {
			get {
				return faceplus_output_channels_count();
			}
		}
		
		public static string GetChannelName(int index) {
			if (index < 0 || index >= ChannelCount) 
				throw new IndexOutOfRangeException();
			
			return Marshal.PtrToStringAnsi (faceplus_output_channel_name(index));
		}

		public static void GetCurrentVector(float[] vector) {
			faceplus_current_output_vector(vector);
		}

		private static float[] vector;
		private static object vec_lock = new object();
		public static void UpdateCurrentVector() {
			if (ChannelCount < 0)
								return;
			if (vector == null)
				vector = new float[ChannelCount];

			lock(vector) {
				faceplus_current_output_vector(vector);
			}

		}
		
		public static float[] GetCurrentVector() {
				if(ChannelCount<0) return new float[0];
			float[] copy = new float[ChannelCount];
			lock (vector) {
				vector.CopyTo (copy, 0);
			}
			return copy;
		}

		public static int Echo(int n) {
			return faceplus_echo(n);
		}

		public static void StartUp(){
			Logger.Log ("Face Plus starting up...");
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("faceplus")]
		private static extern int faceplus_echo(int n);
		
		[DllImport("faceplus")]
		private static extern bool faceplus_init(string video_source);
		
		[DllImport("faceplus")]
		private static extern bool faceplus_init_buffer_tracker(int w,int h,double fps, string channels);
		
		[DllImport("faceplus")]
		private static extern bool faceplus_teardown();
		
		[DllImport("faceplus")]
		private static extern int faceplus_output_channels_count();
		
		[DllImport("faceplus")]
		private static extern bool faceplus_synchronous_track();
		
		[DllImport("faceplus")]
		private static extern bool faceplus_synchronous_track_buffer([In] byte[] buffer, bool debugImages);
		
		[DllImport("faceplus")]
		private static extern IntPtr faceplus_output_channel_name(int index);
		
		[DllImport("faceplus")]
		private static extern void faceplus_current_output_vector([Out] float[] vector);

		[DllImport("faceplus")]
		private static extern int faceplus_log_in(string user, string password);
	}
}
