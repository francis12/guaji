﻿namespace IntelligentPlanning.ExDataGridView
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Forms;

    public class TreeGridNodeCollection : IList<TreeGridNode>, ICollection<TreeGridNode>, IEnumerable<TreeGridNode>, IList, ICollection, IEnumerable
    {
        internal List<TreeGridNode> _list;
        internal TreeGridNode _owner;

        internal TreeGridNodeCollection(TreeGridNode owner)
        {
            this._owner = owner;
            this._list = new List<TreeGridNode>();
        }

        public TreeGridNode Add(string text)
        {
            TreeGridNode item = new TreeGridNode();
            this.Add(item);
            item.Cells[0].Value = text;
            return item;
        }

        public TreeGridNode Add(params object[] values)
        {
            TreeGridNode item = new TreeGridNode();
            this.Add(item);
            int num = 0;
            if (values.Length > item.Cells.Count)
            {
                throw new ArgumentOutOfRangeException("values");
            }
            foreach (object obj2 in values)
            {
                item.Cells[num].Value = obj2;
                num++;
            }
            return item;
        }

        public void Add(TreeGridNode item)
        {
            item.BaseTGV = this._owner.BaseTGV;
            if (item._CheckState == CheckState.Indeterminate)
            {
                if (item.BaseTGV.CheckDefult)
                {
                    item._CheckState = CheckState.Checked;
                }
                else
                {
                    item._CheckState = CheckState.Unchecked;
                }
            }
            bool hasChildren = this._owner.HasChildren;
            item.Owner = this;
            this._list.Add(item);
            this._owner.AddChildNode(item);
            if (!(hasChildren || !this._owner.IsSited))
            {
                this._owner.BaseTGV.InvalidateRow(this._owner.RowIndex);
            }
        }

        public TreeGridNode Add(string text, bool pCheck)
        {
            TreeGridNode item = new TreeGridNode();
            this.Add(item);
            if (pCheck)
            {
                item._CheckState = CheckState.Checked;
            }
            else
            {
                item._CheckState = CheckState.Unchecked;
            }
            item.Cells[0].Value = text;
            return item;
        }

        public void Clear()
        {
            this._owner.ClearNodes();
            this._list.Clear();
        }

        public bool Contains(TreeGridNode item) => 
            this._list.Contains(item);

        public void CopyTo(TreeGridNode[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEnumerator<TreeGridNode> GetEnumerator() => 
            this._list.GetEnumerator();

        public int IndexOf(TreeGridNode item) => 
            this._list.IndexOf(item);

        public void Insert(int index, TreeGridNode item)
        {
            item.BaseTGV = this._owner.BaseTGV;
            item.Owner = this;
            this._list.Insert(index, item);
            this._owner.InsertChildNode(index, item);
        }

        public bool Remove(TreeGridNode item)
        {
            this._owner.RemoveChildNode(item);
            item.BaseTGV = null;
            return this._list.Remove(item);
        }

        public void RemoveAt(int index)
        {
            TreeGridNode node = this._list[index];
            this._owner.RemoveChildNode(node);
            node.BaseTGV = null;
            this._list.RemoveAt(index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        int IList.Add(object value)
        {
            TreeGridNode item = value as TreeGridNode;
            this.Add(item);
            return item.Index;
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value) => 
            this.Contains(value as TreeGridNode);

        int IList.IndexOf(object item) => 
            this.IndexOf(item as TreeGridNode);

        void IList.Insert(int index, object value)
        {
            this.Insert(index, value as TreeGridNode);
        }

        void IList.Remove(object value)
        {
            this.Remove(value as TreeGridNode);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        public int Count =>
            this._list.Count;

        public bool IsReadOnly =>
            false;

        public TreeGridNode this[int index]
        {
            get => 
                this._list[index];
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        int ICollection.Count =>
            this.Count;

        bool ICollection.IsSynchronized
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            this.IsReadOnly;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
    }
}

