using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using LinguaSpace.Common.ComponentModel;

namespace LinguaSpace.Common.Data
{
	public class DataPresentationModel : PresentationModel, ITransactionContextAware, IBinarySerializable
	{
		public DataPresentationModel(DataRowView rv)
			: base(rv)
		{
		}

		public DataRowView RowView
		{
			get
			{
				return (DataRowView)this.Data;
			}
		}

		public ITransactionContext TransactionContext
		{
			get
			{
				return (ITransactionContext)RowView.DataView.Table.DataSet;
			}
		}

		public override void NotifyPropertyChanged()
		{
			base.NotifyPropertyChanged();
			ICustomTypeDescriptor type = (ICustomTypeDescriptor)this.RowView;
			PropertyDescriptorCollection properties = type.GetProperties();
			foreach (PropertyDescriptor property in properties)
				RaisePropertyChangedEvent(property.Name);
		}

		protected PresentationBindingList<T> CreateFilteredPresentationList<T>(String tableName, String propertyName) where T : IPresentationModel
		{
			DataView view = AdoUtils.GetDataView(this.RowView, tableName);
			Object filter = this.RowView[propertyName];
			return PresentationUtils.CreateFilteredPresentationList<T>(view, propertyName, filter);
		}

        protected DataSetBase DataSet
        {
            get
            {
                return (DataSetBase)RowView.Row.Table.DataSet;
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            SerializeOverride(writer);
        }

        public void Deserialize(BinaryReader reader)
        {
            DeserializeOverride(reader);
        }

        protected virtual void SerializeOverride(BinaryWriter writer)
        {
            DataSet.Serialize(RowView.Row, writer, new String[] { });
        }

        protected virtual void DeserializeOverride(BinaryReader reader)
        {
            DataSet.Deserialize(RowView.Row, reader, null);
        }
	}

	public class DataPresentationList<TPresentationModel> : PresentationBindingList<TPresentationModel> where TPresentationModel : DataPresentationModel
	{
		public DataPresentationList(DataView view)
			: base(view)
		{
		}

		public DataView View
		{
			get
			{
				return (DataView)this.List;
			}
		}
	}
}
