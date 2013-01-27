using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Windows.Media;
using LinguaSpace.Common.UI;

namespace LinguaSpace.Common.ComponentModel
{
	public enum ValidationMessageType
	{
		Information,
		Warning,
		Error,
		FatalError
	}

	public interface IValidationStatusProvider
	{
		bool IsModified
		{
			get;
		}

		bool IsValid
		{
			get;
		}

		String ValidationMessage
		{
			get;
		}

		ValidationMessageType ValidationMessageType
		{
			get;
		}
		
		ImageSource ValidationImage
		{
			get;
		}
	}

	public interface IValidatorFactory
	{
		IValidationStatusProvider CreateValidator(Object useCase, params Object[] parameters);
	}

	abstract public class Validator : NotifyPropertyChangedImpl, IValidationStatusProvider, INotifyPropertyChanged, IDisposable
	{
		#region Fields

		private bool _modified = false;
		private bool _valid = true;
		private String _validationMessage = String.Empty;
		private ValidationMessageType _validationMessageType = ValidationMessageType.Information;

		private List<INotifyPropertyChanged> _sourceObjects = new List<INotifyPropertyChanged>();
		private List<INotifyCollectionChanged> _sourceCollections = new List<INotifyCollectionChanged>();
		private List<IBindingList> _sourceLists = new List<IBindingList>();
		
		private static ImageSource _imageOK;
		private static ImageSource _imageWarning;
		private static ImageSource _imageError;

		#endregion Fields

		static Validator()
		{
			String assembly = Assembly.GetEntryAssembly().GetName().Name;
			_imageOK = WPFUtils.GetPngImageSource(assembly, "Status", "OK", 32);
			_imageWarning = WPFUtils.GetPngImageSource(assembly, "Status", "Warning", 32);
			_imageError = WPFUtils.GetPngImageSource(assembly, "Status", "Error", 32);
		}

		#region IValidationStatusProvider

        [System.Reflection.Obfuscation(Exclude = true)]
        public bool IsValid
		{
			get
			{
				return this._valid;
			}
			set
			{
				this._valid = value;
				RaisePropertyChangedEvent("IsValid");
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public bool IsModified
		{
			get
			{
				return this._modified;
			}
			set
			{
				this._modified = value;
				RaisePropertyChangedEvent("IsModified");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public String ValidationMessage
		{
			get
			{
				return this._validationMessage;
			}
			set
			{
				Debug.Assert(value != null);
				this._validationMessage = value;
				RaisePropertyChangedEvent("ValidationMessage");
			}
		}

        [System.Reflection.Obfuscation(Exclude = true)]
		public ValidationMessageType ValidationMessageType
		{
			get
			{
				return this._validationMessageType;
			}
			set
			{
				this._validationMessageType = value;
				RaisePropertyChangedEvent("ValidationMessageType");
				RaisePropertyChangedEvent("ValidationImage");
			}
		}

		[System.Reflection.Obfuscation(Exclude = true)]
		public ImageSource ValidationImage
		{
			get
			{
				return MessageType2ImageOverride(this._validationMessageType);
			}
		}

		#endregion

		#region Internal

		protected virtual String ValidationSuccessMessage
		{
			get
			{
				return "Data is valid";
			}
		}

		protected void ResetStatus()
		{
			this._valid = true;
			this._validationMessage = String.Empty;
			this._validationMessageType = ValidationMessageType.Information;
		}

		protected void SetStatus(bool isValid, String validationMessage, ValidationMessageType validationMessageType)
		{
			this.IsValid = isValid;
			this.ValidationMessage = validationMessage;
			this.ValidationMessageType = validationMessageType;
		}

		protected void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			IsModified = true;
			Validate();
		}

		protected void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
		{
			IsModified = true;
			Validate();
		}

		protected void OnListChanged(Object sender, ListChangedEventArgs e)
		{
			IsModified = true;
			Validate();
		}

		protected void Validate()
		{
			SetStatus(true, this.ValidationSuccessMessage, ValidationMessageType.Information);
			ValidateOverride();
		}

		protected virtual void ValidateOverride()
		{
			;
		}

		protected virtual ImageSource MessageType2ImageOverride(ValidationMessageType type)
		{
			ImageSource src = null;
			switch (_validationMessageType)
			{
				case ValidationMessageType.Information:
					src = _imageOK;
					break;
				case ValidationMessageType.Warning:
					src = _imageWarning;
					break;
				case ValidationMessageType.Error:
				case ValidationMessageType.FatalError:
					src = _imageError;
					break;
			}
			return src;
		}

		public void Dispose()
		{
			Unhook();
		}

		protected virtual INotifyPropertyChanged GetNotifyPropertyChanged(Object obj)
		{
			return obj as INotifyPropertyChanged;
		}
		
		protected virtual INotifyCollectionChanged GetNotifyCollectionChanged(Object obj)
		{
			return obj as INotifyCollectionChanged;
		}
		
		protected virtual IBindingList GetBindingList(Object obj)
		{
			IBindingList bl = obj as IBindingList;
			if (bl != null)
				return bl;
				
			IListSource ls = obj as IListSource;
			if (ls != null)
				bl = ls.GetList() as IBindingList;
				
			return bl;
		}

		protected virtual void Hook(Object obj)
		{
			INotifyPropertyChanged npc = GetNotifyPropertyChanged(obj);
			if (npc != null)
			{
				npc.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
				this._sourceObjects.Add(npc);
			}

			INotifyCollectionChanged ncc = GetNotifyCollectionChanged(obj);
			if (ncc != null)
			{
				ncc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
				this._sourceCollections.Add(ncc);
			}
			
			IBindingList bl = GetBindingList(obj);
			if (bl != null)
			{
				bl.ListChanged += new ListChangedEventHandler(OnListChanged);
				this._sourceLists.Add(bl);
			}
		}

		protected virtual void Unhook()
		{
			foreach (INotifyPropertyChanged npc in this._sourceObjects)
			{
				npc.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyChanged);
			}
			this._sourceObjects.Clear();

			foreach (INotifyCollectionChanged ncc in this._sourceCollections)
			{
				ncc.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);
			}
			this._sourceCollections.Clear();
			
			foreach (IBindingList bl in this._sourceLists)
			{
				bl.ListChanged -= new ListChangedEventHandler(OnListChanged);
			}
		}

		#endregion
	}
}
