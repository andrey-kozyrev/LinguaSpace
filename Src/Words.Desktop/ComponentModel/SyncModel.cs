using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Devices;
using System.Windows;
using System.Windows.Media;
using LinguaSpace.Common.ComponentModel;
using LinguaSpace.Words.IO;

namespace LinguaSpace.Words.ComponentModel
{
	internal enum FileInfoPairBuildPolicy
	{
		Both,
		Left,
		Right,
		None
	}
	
	public class FileInfoModel
	{
		private String _name;
		private long? _size;
		private DateTime _modified;
		private bool _busy;

		public FileInfoModel()
		{
		}

		public FileInfoModel(String name, long size, DateTime modified)
		{
			Debug.Assert(name != null);
			Debug.Assert(size > 0);
			Debug.Assert(modified != null);

			_name = name;
			_size = size;
			_modified = modified;
			_busy = false;
		}

		public FileInfoModel(String name)
		{
			Debug.Assert(name != null);
			_name = name;
			_busy = true;
		}

		public String Name
		{
			get
			{
				return _name;
			}
		}

		public String Size
		{
			get
			{
				return _size.HasValue ? (_size.Value/1024).ToString("N0") : "?";
			}
		}

		public String Modified
		{
			get
			{
				return _modified != null ? _modified.ToString() : String.Empty;
			}
		}

		public String Status
		{
			get
			{
				return _busy ? "In use" : "Available";
			}
		}

		public Brush StatusBrush
		{
			get
			{
				return _busy ? Brushes.Red : Brushes.Green;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return _name != null ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return _name == null;
			}
		}
	}

	public class FileInfoPairModel : NotifyPropertyChangedImpl
	{
		private SyncModel _sync;
		private FileInfoModel _desktop;
		private FileInfoModel _device;

		internal FileInfoPairModel(SyncModel sync, FileInfoModel desktop, FileInfoModel device)
		{
			Debug.Assert(sync != null);
			Debug.Assert(desktop != null);
			Debug.Assert(device != null);
			_sync = sync;
			_desktop = desktop;
			_device = device;
		}

		public FileInfoModel Desktop
		{
			get
			{
				return _desktop;
			}
		}
				
		public FileInfoModel Device
		{
			get
			{
				return _device;
			}
		}

		public bool CanCopyDesktopToDevice()
		{
			return !_desktop.IsEmpty;
		}

		public void CopyDesktopToDevice()
		{
			_device = _sync.CopyFromDesktopToDevice(_desktop);
			RaisePropertyChangedEvent("Device");
		}

		public bool CanCopyDeviceToDesktop()
		{
			return !_device.IsEmpty;
		}

		public void CopyDeviceToDesktop()
		{
			_desktop = _sync.CopyFromDeviceToDesktop(_device);
			RaisePropertyChangedEvent("Desktop");
		}
	}

	public class FileInfoPairCollection : ObservableCollection<FileInfoPairModel>
	{
		public FileInfoPairCollection()
		{
		}
		
		internal void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(e);
		}

		internal new IList<FileInfoPairModel> Items
		{
			get
			{
				return base.Items;
			}
		}
	}

	public class SyncModel
	{
		private FileInfoPairCollection _files;
		private RemoteDeviceManager _rmd;
		private RemoteDevice _device;
		private String _repository;
		private String _extension;
		private String _desktopFolder;
		private String _deviceFolder;

		public SyncModel(RemoteDeviceManager rmd, String repository, String extension)
		{
			Debug.Assert(rmd != null);
			Debug.Assert(repository != null);
			Debug.Assert(extension != null);
			_files = new FileInfoPairCollection();
			_repository = repository;
			_extension = extension;
			_rmd = rmd;
			_rmd.DeviceConnected += new EventHandler(OnDeviceConnected);
			_rmd.DeviceDisconnected += new EventHandler(OnDeviceDisconnected);

			_desktopFolder = Path.Combine(FileUtils.DocumentsFolder, _repository);
			_deviceFolder = Path.Combine(@"\Storage Card\Program Files\Words\", _repository);

			DoLoad();
		}

		void OnDeviceDisconnected(Object sender, EventArgs e)
		{
			if (_device.Status == DeviceStatus.Disconnected)
			{
				DoUnload();
			}
		}

		void OnDeviceConnected(Object sender, EventArgs e)
		{
			DoLoad();
		}

		public ObservableCollection<FileInfoPairModel> Files
		{
			get
			{
				return _files;
			}
		}

		public String DesktopName
		{
			get
			{
				return Environment.MachineName;
			}
		}

		public String DeviceName
		{
			get
			{
				return _device.Name;
			}
		}

		public String Repository
		{
			get
			{
				return String.Format("_{0}:", _repository);
			}
		}

		protected void DoLoad()
		{
			if (_rmd.Devices.FirstConnectedDevice == null)
				return;

			_device = _rmd.Devices.FirstConnectedDevice;

			IList<FileInfoModel> desktopFiles = GetDesktopFiles(_desktopFolder, _extension);
			IList<FileInfoModel> deviceFiles = GetDeviceFiles(_deviceFolder, _extension);

			MergeSortedFiles(desktopFiles, deviceFiles, _files.Items);

			_files.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected void DoUnload()
		{
			_files.Clear();
			_device = null;
		}

		IList<FileInfoModel> GetDesktopFiles(String path, String extension)
		{
			IList<FileInfoModel> infos = new List<FileInfoModel>();
			String[] files = Directory.GetFiles(path, String.Format("*.{0}", extension));
			Array.Sort<String>(files);
			foreach (String file in files)
				infos.Add(GetDesktopFileInfo(file));
			return infos;
		}

		IList<FileInfoModel> GetDeviceFiles(String path, String extension)
		{
			IList<FileInfoModel> infos = new List<FileInfoModel>();
			String[] files = _device.GetFiles(path, String.Format("*.{0}", extension));
			Array.Sort<String>(files);
			foreach (String file in files)
				infos.Add(GetDeviceFileInfo(Path.Combine(path, file)));
			return infos;
		}

		private FileInfoModel GetDesktopFileInfo(String path)
		{
			try
			{
				FileInfo fi = new FileInfo(path);
				return new FileInfoModel(fi.Name, fi.Length, fi.LastWriteTime);
			}
			catch (Exception e)
			{
				return new FileInfoModel(Path.GetFileName(path));
			}
		}

		private FileInfoModel GetDeviceFileInfo(String path)
		{
			try
			{
				long size = (long)_device.GetFileSize(path);
				DateTime modified = _device.GetFileLastWriteTime(path);
				return new FileInfoModel(Path.GetFileName(path), size, modified);
			}
			catch (Exception e)
			{
				return new FileInfoModel(Path.GetFileName(path));
			}
		}

		private void MergeSortedFiles(IList<FileInfoModel> list1, IList<FileInfoModel> list2, IList<FileInfoPairModel> list)
		{
			bool live = true;
			while (live)
			{
				FileInfoModel fim1 = list1.FirstOrDefault<FileInfoModel>();
				FileInfoModel fim2 = list2.FirstOrDefault<FileInfoModel>();

				switch (GetFileInfoPairBuildPolicy(fim1, fim2))
				{
					case FileInfoPairBuildPolicy.Both:
						list.Add(new FileInfoPairModel(this, fim1, fim2));
						list1.Remove(fim1);
						list2.Remove(fim2);
						break;
					case FileInfoPairBuildPolicy.Left:
						list.Add(new FileInfoPairModel(this, fim1, new FileInfoModel()));
						list1.Remove(fim1);
						break;
					case FileInfoPairBuildPolicy.Right:
						list.Add(new FileInfoPairModel(this, new FileInfoModel(), fim2));
						list2.Remove(fim2);
						break;
					default:
						live = false;
						break;
				}
			}
		}

		private FileInfoPairBuildPolicy GetFileInfoPairBuildPolicy(FileInfoModel fim1, FileInfoModel fim2)
		{
			if (fim1 != null && fim2 != null)
			{
				int cmp = fim1.Name.CompareTo(fim2.Name);
				if (cmp < 0)
				{
					return FileInfoPairBuildPolicy.Left;
				}
				else if (cmp > 0)
				{
					return FileInfoPairBuildPolicy.Right;
				}
				else
				{
					return FileInfoPairBuildPolicy.Both;
				}
			}
			else if (fim1 != null && fim2 == null)
			{
				return FileInfoPairBuildPolicy.Left;
			}
			else if (fim1 == null && fim2 != null)
			{
				return FileInfoPairBuildPolicy.Right;
			}
			else
			{
				return FileInfoPairBuildPolicy.None;
			}
		}

		internal FileInfoModel CopyFromDesktopToDevice(FileInfoModel desktopFileInfo)
		{
			String desktopFile = Path.Combine(_desktopFolder, desktopFileInfo.Name);
			String deviceFile = Path.Combine(_deviceFolder, desktopFileInfo.Name);
			String deviceTempFile = Path.ChangeExtension(deviceFile, "tmp");
			_device.CopyFileToDevice(desktopFile, deviceTempFile, true);
			try
			{
				_device.DeleteFile(deviceFile);
			}
			catch
			{
			}
			_device.MoveFile(deviceTempFile, deviceFile);
			return GetDeviceFileInfo(deviceFile);
		}

		internal FileInfoModel CopyFromDeviceToDesktop(FileInfoModel deviceFileInfo)
		{
			String desktopFile = Path.Combine(_desktopFolder, deviceFileInfo.Name);
			String desktopTempFile = Path.ChangeExtension(desktopFile, "tmp");
			String deviceFile = Path.Combine(_deviceFolder, deviceFileInfo.Name);
			_device.CopyFileFromDevice(deviceFile, desktopTempFile, true);
			if (File.Exists(desktopFile))
				File.Delete(desktopFile);
			File.Move(desktopTempFile, desktopFile);
			return GetDesktopFileInfo(desktopFile);
		}
	}
}
