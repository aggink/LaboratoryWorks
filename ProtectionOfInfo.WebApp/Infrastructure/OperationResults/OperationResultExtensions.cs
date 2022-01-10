using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.OperationResults
{
    public static class OperationResultExtensions
    {
        public static IHaveDataObject AddInfo(this OperationResult source, string message)
        {
            source.AppendLog(message);
            source.MetaData = new MetaData(source, message);
            return source.MetaData;
        }

        public static IHaveDataObject AddSuccess(this OperationResult source, string message)
        {
            source.AppendLog(message);
            source.MetaData = new MetaData(source, message, MetaDataType.Success);
            return source.MetaData;
        }

        public static IHaveDataObject AddWarning(this OperationResult source, string message)
        {
            source.AppendLog(message);
            source.MetaData = new MetaData(source, message, MetaDataType.Warning);
            return source.MetaData;
        }

        public static IHaveDataObject AddError(this OperationResult source, string message)
        {
            source.AppendLog(message);
            source.MetaData = new MetaData(source, message, MetaDataType.Error);
            return source.MetaData;
        }

        public static IHaveDataObject AddError(this OperationResult source, Exception exception)
        {
            source.Exception = exception;
            source.MetaData = new MetaData(source, exception!.Message, MetaDataType.Error);
            if (exception != null)
            {
                source.AppendLog(exception.Message);
            }
            return source.MetaData;
        }


        public static IHaveDataObject AddError(this OperationResult source, string message, Exception exception)
        {
            source.Exception = exception;
            source.MetaData = new MetaData(source, message, MetaDataType.Error);
            if (!string.IsNullOrEmpty(message))
            {
                source.AppendLog(message);
            }

            if (exception != null)
            {
                source.AppendLog(exception.Message);
            }

            return source.MetaData;
        }

        public static string GetMetadataMessages(this OperationResult source)
        {
            if (source == null) throw new ArgumentNullException();

            var sb = new StringBuilder();
            if (source.MetaData != null)
            {
                sb.AppendLine($"{source.MetaData.Message}");
            }

            if (!source.Logs.Any()) return sb.ToString();
            source.Logs.ToList().ForEach(x => sb.AppendLine($"Log: {x}"));
            return sb.ToString();

        }
    }
}
