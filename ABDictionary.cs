using System;

namespace ABSoftware
{
    public class ABDictionary<Key, Value>
    {
        private ArrayList<ArrayList<Element>> buckets;

        public int Size { get; private set; }

        public ABDictionary()
        {
            buckets = new ArrayList<ArrayList<Element>>();
            Size = 0;
        }

        public Value this[Key key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public void Add(Key key, Value value)
        {
            if (key == null)
                throw new Exception("Supplied key is null.");

            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                ArrayList<Element> newBucket = new ArrayList<Element>();
                buckets.Add(newBucket);
                newBucket.Add(new Element(hash, key, value));
                Size++;
                return;
            }
            else
            {
                if (bucket.Contains(e => { return e.key.Equals(key); }))
                    throw new Exception("Trying to add a duplicate.");
                else
                {
                    bucket.Add(new Element(hash, key, value));
                    Size++;
                }
            }
        }

        public bool TryAdd(Key key, Value value)
        {
            if (key == null)
                return false;

            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                ArrayList<Element> newBucket = new ArrayList<Element>();
                buckets.Add(newBucket);
                newBucket.Add(new Element(hash, key, value));
                Size++;
                return true;
            }
            else
            {
                if (bucket.Contains(e => { return e.key.Equals(key); }))
                    return false;
                else
                {
                    bucket.Add(new Element(hash, key, value));
                    Size++;
                    return true;
                }
            }
        }

        public Value Get(Key key)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
                return default;
            else
                return bucket.FirstOrDefault(e => { return e.key.Equals(key); }).value;
        }

        public bool TryGet(Key key, out Value value)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = bucket.FirstOrDefault(e => { return e.key.Equals(key); }).value;
                return true;
            }
        }

        public void Set(Key key, Value value)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                ArrayList<Element> newBucket = new ArrayList<Element>();
                buckets.Add(newBucket);
                newBucket.Add(new Element(hash, key, value));
                Size++;
                return;
            }
            for (int i = 0; i < bucket.Size; i++)
            {
                if (bucket[i].key.Equals(key))
                {
                    bucket[i] = new Element(hash, key, value);
                    return;
                }
            }
            bucket.Add(new Element(hash, key, value));
            Size++;
        }

        public void Remove(Key key)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
                return;
            else
            {
                Size -= bucket.RemoveIf(e => { return e.key.Equals(key); });
            }
        }

        public void Clear()
        {
            buckets.Clear();
            Size = 0;
        }

        public bool Contains(Key key)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
                return false;
            else
                return !bucket.FirstOrDefault(e => { return e.key.Equals(key); }).Equals(default(Element));
        }

        private struct Element
        {
            public int hash;
            public Key key;
            public Value value;

            public Element(int hash, Key key, Value value)
            {
                this.hash = hash;
                this.key = key;
                this.value = value;
            }
        }
    }
}
