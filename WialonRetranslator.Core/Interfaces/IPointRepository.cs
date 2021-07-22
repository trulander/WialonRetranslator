namespace WialonRetranslator.DataAccess.Repository
{
    public interface IPointRepository
    {
        void Create(Core.Models.Point point);
        Core.Models.Point Read(int pointId);
        Core.Models.Point[] Read();
        void Delete(int pointId);
    }
}