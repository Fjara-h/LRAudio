using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LRAudio {
	class Program {
		static void Main(string[] args) {
			MultiplexingWaveProvider waveProvider;
			double time = 0;
			Console.Write("Path to audio files: ");
			string path = Console.ReadLine();
			DirectoryInfo dir = new DirectoryInfo(path);
			FileInfo[] files = dir.GetFiles("*.flac");

			while(true) {
				using(var outputDevice = new WaveOutEvent() { DeviceNumber = 0 }) {
					int file1 = Program.getRandomNum(files.Length - 1);
					int file2 = Program.getRandomNum(files.Length - 1);
					Console.Clear();
					Console.Write(files[file1].Name + "\n" + files[file2].Name);
					using(AudioFileReader input1 = new AudioFileReader(path + "\\" + files[file1].Name)) {
						time = input1.TotalTime.TotalMilliseconds;
						using(AudioFileReader input2 = new AudioFileReader(path + "\\" + files[file2].Name)) {
							waveProvider = new MultiplexingWaveProvider(new IWaveProvider[] { input1, input2 }, 2);
							waveProvider.ConnectInputToOutput(Program.getRandomNum(4), 0);
							waveProvider.ConnectInputToOutput(Program.getRandomNum(4), 1);
						}
					}
					outputDevice.Init(waveProvider);
					outputDevice.Play();
					Thread.Sleep((int)time);
				}
			}
		}
		static int getRandomNum(int count) {
			using(RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider()) {
				uint scale = uint.MaxValue;
				byte[] byteArray;
				while(scale == uint.MaxValue) {
					byteArray = new byte[4];
					provider.GetBytes(byteArray);
					scale = BitConverter.ToUInt32(byteArray, 0);
				}
				return (int)((count) * (scale / (double)uint.MaxValue));
			}
		}
	}
}
