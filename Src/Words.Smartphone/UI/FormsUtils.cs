using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LinguaSpace.Words.UI
{
    class FormsUtils
    {
        protected FormsUtils()
        {
        }

        public static void SelectItem(ListView view, int index)
        {
            Debug.Assert(0 <= index && index < view.Items.Count);
            ListViewItem item = view.Items[index];
            item.Selected = true;
            item.Focused = true;
        }

        public static int GetSelectionIndex(ListView view)
        {
            int index = -1;
            if (view.SelectedIndices.Count > 0)
            {
                index = view.SelectedIndices[0];
            }
            return index;
        }

        public static ListViewItem GetSelectedItem(ListView view)
        {
            ListViewItem item = null;
            int index = GetSelectionIndex(view);
            if (index >= 0)
            {
                item = view.Items[index];
            }
            return item;
        }
    }
}