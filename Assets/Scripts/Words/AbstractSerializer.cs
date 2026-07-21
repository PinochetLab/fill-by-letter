namespace Words
{
    public abstract class AbstractSerializer<T>
    {
        public abstract string Serialize(T t);
        
        public abstract T Deserialize(string s);
    }
}