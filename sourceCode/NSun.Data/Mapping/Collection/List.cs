using System;
using System.Collections.Generic;

namespace NSun.Data.Collection
{
    [Serializable]
    public class List<T> : System.Collections.Generic.List<T> where T : class
    {
        private Helper.IModelHelper ModelHelp { get; set; }

        public List(DBQuery db)
        {
            ModelHelp = db.ModelHelp; //new Helper.ModelHelper(db);
        }

        public T this[int index, bool isproxy = true]
        {
            get
            {
                if (isproxy)
                {
                    if (ModelHelp.DB.Db.IsUseRelation)
                    {
                        return ModelHelp.CreateProxy<T>(base[index]);
                    }
                }
                return base[index];
            }
            set { base[index] = value; }
        }

        //重点修改
        public new T this[int index]
        {
            get { return this[index, true]; }
            set { this[index, true] = value; }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            var enumitem = base.GetEnumerator();
            while (enumitem.MoveNext())
            {
                if (ModelHelp.DB.Db.IsUseRelation)
                    yield return ModelHelp.CreateProxy(enumitem.Current);
                else
                    yield return enumitem.Current;
            }
        }
    }
}

#if Debug

    //public class List<T> : System.Collections.Generic.IList<T>, System.Collections.IList where T : class
    //{
    //    private System.Collections.Generic.List<T> list;

    //    private NSun.Data.Systems.ModelHelper ModelHelp { get; set; }

    //    public List(DBQuery db)
    //    {
    //        list = new System.Collections.Generic.List<T>();
    //        ModelHelp = new Systems.ModelHelper(db);
    //    }

    //    public int IndexOf(T item)
    //    {
    //        return list.IndexOf(item);
    //    }

    //    public void Insert(int index, T item)
    //    {
    //        list.Insert(index, item);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        list.RemoveAt(index);
    //    }

    //    //重点修改
    //    public T this[int index]
    //    {
    //        get
    //        {
    //            if (ModelHelp.DB.Db.IsUseRelation)
    //            {
    //                return ModelHelp.CreateProxy<T>(list[index]);
    //            }
    //            return list[index];
    //        }
    //        set { list[index] = value; }
    //    }

    //    public void Add(T item)
    //    {
    //        list.Add(item);
    //    }

    //    public void Clear()
    //    {
    //        list.Clear();
    //    }

    //    public bool Contains(T item)
    //    {
    //        return list.Contains(item);
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        list.CopyTo(array, arrayIndex);
    //    }

    //    public int Count
    //    {
    //        get { return list.Count; }
    //    }

    //    public bool IsReadOnly
    //    {
    //        get { return false; }
    //    }

    //    public bool Remove(T item)
    //    {
    //        return list.Remove(item);
    //    }

    //    //重点修改
    //    public IEnumerator<T> GetEnumerator()
    //    {

    //        foreach (T item in list)
    //        {
    //            if (ModelHelp.DB.Db.IsUseRelation)
    //            {
    //                yield return ModelHelp.CreateProxy(item);
    //            }
    //            else
    //            {
    //                yield return item;
    //            }
    //        }
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    #region IList 成员

    //    public int Add(object value)
    //    {
    //        return ((System.Collections.IList)list).Add(value);
    //    }

    //    public bool Contains(object value)
    //    {
    //        return ((System.Collections.IList)list).Contains(value);
    //    }

    //    public int IndexOf(object value)
    //    {
    //        return ((System.Collections.IList)list).IndexOf(value);
    //    }

    //    public void Insert(int index, object value)
    //    {
    //        ((System.Collections.IList)list).Insert(index, value);
    //    }

    //    public bool IsFixedSize
    //    {
    //        get { return ((System.Collections.IList)list).IsFixedSize; }
    //    }

    //    public void Remove(object value)
    //    {
    //        ((System.Collections.IList)list).Remove(value);
    //    }

    //    object System.Collections.IList.this[int index]
    //    {
    //        get
    //        {
    //            if (ModelHelp.DB.Db.IsUseRelation)
    //            {
    //                return ModelHelp.CreateProxy(((System.Collections.IList)list)[index]);
    //            }
    //            return ((System.Collections.IList)list)[index];
    //        }
    //        set
    //        {
    //            ((System.Collections.IList)list)[index] = value;
    //        }
    //    }

    //    #endregion

    //    #region ICollection 成员

    //    public void CopyTo(Array array, int index)
    //    {
    //        ((System.Collections.IList)list).CopyTo(array, index);
    //    }

    //    public bool IsSynchronized
    //    {
    //        get { return ((System.Collections.IList)list).IsSynchronized; }
    //    }

    //    public object SyncRoot
    //    {
    //        get { return ((System.Collections.IList)list).SyncRoot; }
    //    }

    //    #endregion
    //} 
#endif
