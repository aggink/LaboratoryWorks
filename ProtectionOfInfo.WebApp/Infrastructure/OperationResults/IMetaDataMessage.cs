namespace ProtectionOfInfo.WebApp.Infrastructure.OperationResults
{
    public interface IMetaDataMessage : IHaveDataObject
    {
        string Message { get; }
        object DataObject { get; }
    }
}
