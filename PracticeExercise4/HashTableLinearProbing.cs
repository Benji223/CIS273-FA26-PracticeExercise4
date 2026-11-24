namespace PracticeExercise4;

public class HashTableLinearProbing<K, V> : IHashTable<K, V>
{
    private HashEntry<K, V>[] hashEntries;

    private int initialCapacity = 13;

    public readonly double MAX_LOAD_FACTOR = 0.6;

    public HashTableLinearProbing()
    {
        hashEntries = new HashEntry<K, V>[initialCapacity];
        for (int i = 0; i < hashEntries.Length; i++)
        {
            hashEntries[i] = new HashEntry<K, V>();
        }
    }

    private int count;
    public int Count => count;

    public double LoadFactor => (double)count / hashEntries.Length;

    public bool Add(K key, V value)
    {
        if (Update(key, value))
        {
            return true;
        }

        if ((count + 1) / (double)hashEntries.Length > MAX_LOAD_FACTOR)
        {
            Resize();
        }

        int index = Hash(key) % hashEntries.Length;
        int startIndex = index;
        do
        {
            if (hashEntries[index].State != HashEntryState.Full)
            {
                hashEntries[index] = new HashEntry<K, V>(key, value);
                count++;
                return false;
            }
            index = (index + 1) % hashEntries.Length;
        } while (index != startIndex);

        return false;
    }

    // If key is present, update its value and return true; otherwise return false.
    private bool Update(K key, V value)
    {
        int index = Hash(key) % hashEntries.Length;
        int startIndex = index;
        do
        {
            var entry = hashEntries[index];
            if (entry.State == HashEntryState.EmptySinceStart)
            {
                return false;
            }
            if (entry.State == HashEntryState.Full &&
                EqualityComparer<K>.Default.Equals(entry.Key, key))
            {
                entry.Value = value;
                return true;
            }
            index = (index + 1) % hashEntries.Length;
        } while (index != startIndex);
        return false;
    }

    public bool ContainsKey(K key)
    {
        int index = Hash(key) % hashEntries.Length;
        int startIndex = index;
        do
        {
            var entry = hashEntries[index];
            if (entry.State == HashEntryState.EmptySinceStart)
            {
                return false;
            }
            if (entry.State == HashEntryState.Full &&
                EqualityComparer<K>.Default.Equals(entry.Key, key))
            {
                return true;
            }
            index = (index + 1) % hashEntries.Length;
        } while (index != startIndex);
        return false;
    }

    public bool ContainsValue(V value)
    {
        foreach (var entry in hashEntries)
        {
            if (entry.State == HashEntryState.Full &&
                EqualityComparer<V>.Default.Equals(entry.Value, value))
            {
                return true;
            }
        }
        return false;
    }

    public V Get(K key)
    {
        int index = Hash(key) % hashEntries.Length;
        int startIndex = index;
        do
        {
            var entry = hashEntries[index];
            if (entry.State == HashEntryState.EmptySinceStart)
            {
                throw new KeyNotFoundException();
            }
            if (entry.State == HashEntryState.Full &&
                EqualityComparer<K>.Default.Equals(entry.Key, key))
            {
                return entry.Value;
            }
            index = (index + 1) % hashEntries.Length;
        } while (index != startIndex);
        throw new KeyNotFoundException();
    }

    public List<K> GetKeys()
    {
        var keys = new List<K>();
        foreach (var entry in hashEntries)
        {
            if (entry.State == HashEntryState.Full)
            {
                keys.Add(entry.Key);
            }
        }
        return keys;
    }

    public List<V> GetValues()
    {
        var values = new List<V>();
        foreach (var entry in hashEntries)
        {
            if (entry.State == HashEntryState.Full)
            {
                values.Add(entry.Value);
            }
        }
        return values;
    }

    public bool Remove(K key)
    {
        int index = Hash(key) % hashEntries.Length;
        int startIndex = index;
        do
        {
            var entry = hashEntries[index];
            if (entry.State == HashEntryState.EmptySinceStart)
            {
                return false;
            }
            if (entry.State == HashEntryState.Full &&
                EqualityComparer<K>.Default.Equals(entry.Key, key))
            {
                entry.Clear();
                count--;
                return true;
            }
            index = (index + 1) % hashEntries.Length;
        } while (index != startIndex);
        return false;
    }

    private int Hash(K key)
    {
        // clear the sign bit (Math.Abs throws on int.MinValue)
        return key.GetHashCode() & 0x7FFFFFFF;
    }

    private void Resize()
    {
        int newSize = NextPrime(2 * hashEntries.Length);
        var newBuckets = new HashEntry<K, V>[newSize];
        var oldBuckets = hashEntries;

        // initialize the new buckets array 
        for (int i = 0; i < newBuckets.Length; i++)
        {
            newBuckets[i] = new HashEntry<K, V>();
        }

        hashEntries = newBuckets;
        count = 0;

        // re-add all the old buckets into the new array
        foreach (var bucket in oldBuckets)
        {
            if (bucket.State == HashEntryState.Full)
            {
                Add(bucket.Key, bucket.Value);
            }
        }
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n < 4) return true;           // 2 and 3
        if (n % 2 == 0) return false;

        for (int d = 3; (long)d * d <= n; d += 2)
        {
            if (n % d == 0) return false;
        }
        return true;
    }

    private static int NextPrime(int n)
    {
        if (n <= 2) return 2;
        int candidate = (n % 2 == 0) ? n + 1 : n;   // only odd numbers can be prime past 2
        while (!IsPrime(candidate))
        {
            candidate += 2;
        }
        return candidate;
    }
}
