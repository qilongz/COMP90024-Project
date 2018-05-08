namespace TwitterUtil.Extraction
{
    public interface IExtractor<in TS, out TV>
    {
        TV Extract(TS src);
        bool IsGeo(TS src);
    }
}