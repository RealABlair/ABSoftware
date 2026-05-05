using System;
using System.Collections;
using System.Collections.Generic;

namespace ABSoftware
{
    public class ABDictionary<Key, Value> : IEnumerable<KeyValuePair<Key, Value>>
    {
        private ArrayList<Element>[] buckets;

        public int Size { get; private set; }

        public float LoadRatio { get; private set; }

        public ABDictionary() : this(0.75f) { }

        public ABDictionary(float loadRatio)
        {
            buckets = new ArrayList<Element>[64];
            Size = 0;
            this.LoadRatio = loadRatio;
        }

        public Value this[Key key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        ArrayList<Element> GetBucket(int hash)
        {
            int index = Math.Abs(hash) % buckets.Length;

            if (buckets[index] == null)
                buckets[index] = new ArrayList<Element>();

            return buckets[index];
        }

        int GetBucketIndex(int hash)
        {
            return Math.Abs(hash) % buckets.Length;
        }

        void ControlSize()
        {
            if (Size > buckets.Length * LoadRatio)
            {
                var oldBuckets = buckets;
                buckets = new ArrayList<Element>[oldBuckets.Length * 2];
                Size = 0;

                for (int i = 0; i < oldBuckets.Length; i++)
                {
                    if (oldBuckets[i] == null) continue;

                    for (int j = 0; j < oldBuckets[i].Size; j++)
                    {
                        Element element = oldBuckets[i][j];
                        Add(element.key, element.value);
                    }
                }
            }
        }

        public void Add(Key key, Value value)
        {
            if (key == null)
                throw new Exception("Supplied key is null.");

            int hash = key.GetHashCode();

            ArrayList<Element> bucket = GetBucket(hash);

            if (bucket.Contains(e => { return e.key.Equals(key); }))
                throw new Exception("Trying to add a duplicate.");
            else
            {
                bucket.Add(new Element(hash, key, value));
                Size++;

                ControlSize();
            }
        }

        public bool TryAdd(Key key, Value value)
        {
            if (key == null)
                return false;

            int hash = key.GetHashCode();

            ArrayList<Element> bucket = GetBucket(hash);

            if (bucket.Contains(e => { return e.key.Equals(key); }))
                return false;
            else
            {
                bucket.Add(new Element(hash, key, value));
                Size++;

                ControlSize();
                return true;
            }
        }

        public Value Get(Key key)
        {
            if (key == null)
                throw new Exception($"Supplied key was null.");

            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets[GetBucketIndex(hash)];

            if (bucket == null)
                return default;

            for (int i = 0; i < bucket.Size; i++)
            {
                if (bucket[i].key.Equals(key))
                    return bucket[i].value;
            }

            throw new Exception($"Key {key} wasn't found.");
        }

        public bool TryGet(Key key, out Value value)
        {
            if(key == null)
            {
                value = default;
                return false;
            }

            int hash = key.GetHashCode();

            ArrayList<Element> bucket = buckets[GetBucketIndex(hash)];

            if(bucket == null)
            {
                value = default;
                return false;
            }

            for (int i = 0; i < bucket.Size; i++)
            {
                if (bucket[i].key.Equals(key))
                {
                    value = bucket[i].value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void Set(Key key, Value value)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = GetBucket(hash);

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
            ControlSize();
        }

        public void Remove(Key key)
        {
            int hash = key.GetHashCode();

            ArrayList<Element> bucket = GetBucket(hash);

            Size -= bucket.RemoveIf(e => { return e.key.Equals(key); });
        }

        public void Clear()
        {
            Array.Clear(buckets, 0, buckets.Length);
            Size = 0;
        }

        public bool Contains(Key key)
        {
            return TryGet(key, out _);
        }

        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
        {
            for (int i = 0; i < buckets.Length; i++)
            {
                var bucket = buckets[i];
                if (bucket == null) continue;

                for (int j = 0; j < bucket.Size; j++)
                    yield return new KeyValuePair<Key, Value>(bucket[j].key, bucket[j].value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct Element
        {
            public readonly int hash;
            public readonly Key key;
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
