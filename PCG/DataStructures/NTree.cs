using System;
using System.Collections.Generic;

namespace PCG.DataStructures
{
    public class NTree<T>
    {
        private readonly T _data;
        private readonly int _size;
        private readonly LinkedList<NTree<T>> _children;

        public NTree(T data,int size)
        {
            if (size <= 0)
            {
                throw new Exception($"Invalid size of {nameof(NTree<T>)}");
            }

            _size = size;
            _data = data;
            _children = new LinkedList<NTree<T>>();
        }

        public void AddChild(T data)
        {
            if (_children.Count >= _size)
            {
                Console.WriteLine("Couldn't add another child to the tree");
                return;
            }

            _children.AddFirst(new NTree<T>(data, _size));
        }

        public NTree<T> GetChild(int i)
        {
            foreach (NTree<T> n in _children)
                if (--i == 0)
                    return n;
            return null;
        }

        public void Traverse(Action<T> visitor)
        {
            visitor(_data);
            foreach (var kid in _children)
                kid.Traverse(visitor);
        }
    }
}