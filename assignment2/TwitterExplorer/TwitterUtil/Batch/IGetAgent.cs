namespace TwitterUtil.Batch
{
    public interface IGetAgent<T>
    {
        string TgtLocation { get; }

        void Initialise(IGetAgent<T> src);
        void Get(string line);
    }
}