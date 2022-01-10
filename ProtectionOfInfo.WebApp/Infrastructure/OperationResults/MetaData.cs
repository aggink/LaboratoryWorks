using System;

namespace ProtectionOfInfo.WebApp.Infrastructure.OperationResults
{
    [Serializable]
    public class MetaData : IMetaDataMessage
    {
        private readonly OperationResult _sourse;
        public MetaDataType Type { get; }
        public string Message { get; }
        public object DataObject { get; private set; }

        public MetaData()
        {
            Type = MetaDataType.Info;
        }

        public MetaData(OperationResult sourse, string message) : this()
        {
            _sourse = sourse;
            Message = message;
        }

        public MetaData(OperationResult source, string message, MetaDataType type = MetaDataType.Info)
        {
            Type = type;
            _sourse = source;
            Message = message;
        }

        public void AddData(object data)
        {
            if(data is Exception exception && _sourse.MetaData == null)
            {
                _sourse.MetaData = new MetaData(_sourse, exception.Message);
            }
            else
            {
                _sourse.MetaData.DataObject = data;
            }
        }
    }
}
