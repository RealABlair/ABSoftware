using System;

namespace ABSoftware
{
    public class ElementPool
    {
        object[] pool;

        public int Size { get { return pool.Length; } }

        public ElementPool()
        {
            pool = new object[0];
        }

        public T AddElement<T>()
        {
            return (T)AddElement(typeof(T));
        }

        public T AddElement<T>(T instance)
        {
            Array.Resize(ref pool, pool.Length + 1);
            pool[pool.Length - 1] = instance;

            return instance;
        }

        public object AddElement(Type type)
        {
            object obj = type.GetConstructor(new Type[0]).Invoke(new object[0]);

            Array.Resize(ref pool, pool.Length + 1);
            pool[pool.Length - 1] = obj;

            return obj;
        }

        public T GetElement<T>()
        {
            return (T)GetElement(typeof(T));
        }

        public object GetElement(Type type)
        {
            for (int i = 0; i < pool.Length; i++)
                if (pool[i].GetType().Equals(type))
                    return pool[i];

            return null;
        }

        public T[] GetElements<T>()
        {
            object[] elements = GetElements(typeof(T));
            T[] t = new T[elements.Length];
            for (int i = 0; i < elements.Length; i++)
                t[i] = (T)elements[i];

            return t;
        }

        public object[] GetElements(Type type)
        {
            object[] elements = new object[0];
            for (int i = 0; i < pool.Length; i++)
                if (pool[i].GetType().Equals(type))
                {
                    Array.Resize(ref elements, elements.Length + 1);
                    elements[elements.Length - 1] = pool[i];
                }
            return elements;
        }

        public bool RemoveElement<T>(T element)
        {
            for (int i = 0; i < pool.Length; i++)
                if (pool[i].Equals(element))
                    return RemoveElementAt(i);

            return false;
        }

        public bool RemoveElementAt(int index)
        {
            if (index >= pool.Length || index < 0)
                return false;

            Array.Copy(pool, index + 1, pool, index, pool.Length - index - 1);
            Array.Resize(ref pool, pool.Length - 1);

            return true;
        }
    }
}
