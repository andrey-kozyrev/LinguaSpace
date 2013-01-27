using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Data;

namespace LinguaSpace.Common.Data
{
	public class RelatedDataView : ListCollectionView
	{
		private DataRowView parent;
		private int[] ordinalsPK;
		private int[] ordinalsFK;

		protected override void RefreshOverride()
		{
			base.RefreshOverride();
		}

		public RelatedDataView(DataRowView parent, DataTable children, String relationName)
			: this(parent, children.DefaultView, children.DataSet.Relations[relationName])
		{
		}

		public RelatedDataView(DataRowView parent, DataTable children, DataRelation relation)
			: this(parent, children.DefaultView, relation)
		{
		}

		public RelatedDataView(DataRowView parent, DataView children, String relationName)
			: this(parent, children, children.Table.DataSet.Relations[relationName])
		{
		}

		public RelatedDataView(DataRowView parent, DataView children, DataRelation relation)
			: base(children)
		{
			Debug.Assert(parent != null);

			this.parent = parent;
			
			this.ordinalsPK = GetOrdinals(relation.ParentColumns);
			this.ordinalsFK = GetOrdinals(relation.ChildColumns);
			
			Debug.Assert(this.ordinalsPK.Length == this.ordinalsFK.Length);
			
			this.Filter = new Predicate<Object>(DoFilter);

			children.ListChanged += new System.ComponentModel.ListChangedEventHandler(children_ListChanged);
		}

		private void children_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
		{
			Refresh();
		}

		public DataView DataView
		{
			get
			{
				return (DataView)this.SourceCollection;
			}
		}

		public new DataRowView AddNew()
		{
			DataView children = (DataView)this.SourceCollection;
			DataRowView child = children.AddNew();

			for (int i = 0; i < this.ordinalsFK.Length; ++i)
				child[this.ordinalsFK[i]] = this.parent[this.ordinalsPK[i]];

			if (children.Table.Columns.Contains("Position"))
				child["Position"] = AdoUtils.GetNextPosition(children, "Position");

			child.EndEdit();
			return child;
		}

		private int[] GetOrdinals(DataColumn[] columns)
		{
			int[] ordinals = new int[columns.Length];
			for (int i = 0; i < ordinals.Length; ++i)
			{
				ordinals[i] = columns[i].Ordinal;
			}
			return ordinals;
		}

		private bool DoFilter(Object item)
		{
			DataRowView child = item as DataRowView;
			return child != null ? DoFilter(child) : false;
		}

		private bool DoFilter(DataRowView child)
		{
			for (int i = 0; i < this.ordinalsPK.Length; ++i)
				if (!parent[this.ordinalsPK[i]].Equals(child[this.ordinalsFK[i]]))
					return false;
			return true;		
		}
	}
}
