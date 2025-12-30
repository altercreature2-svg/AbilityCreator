using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class  InnerListWrapper<T> : IEquatable<InnerListWrapper<T>>
{
    public List<T> innerList = new List<T>();

    public InnerListWrapper(List<T> list)
    {
        innerList = list;
    }
    public bool Equals(InnerListWrapper<T> obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return base.ToString();
    }
}

[System.Serializable]
public class ListOfListsWrapper<T> : IEquatable<ListOfListsWrapper<T>>
{
    public List<InnerListWrapper<T>> listOfLists = new List<InnerListWrapper<T>>();
    public bool Equals(ListOfListsWrapper<T> obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return base.ToString();
    }
}
