using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ProtectionOfInfo.WebApp.Infrastructure.OperationResults
{
    [Serializable]
    public abstract class OperationResult
    {
        public readonly IList<string> _logs = new List<string>();
        public string ActivityId { get; set; }
        public MetaData MetaData { get; set; }
        public Exception Exception { get; set; }
        public IEnumerable<string> Logs => _logs;

        protected OperationResult()
        {
            ActivityId = Generate(11);
        }

        public static OperationResult<T> CreateResult<T>(T result)
        {
            return CreateResult(result, null);
        }

        public static OperationResult<T> CreateResult<T>()
        {
            return CreateResult<T>(default(T), null);
        }

        public static OperationResult<TResult> CreateResult<TResult>(TResult result, Exception? error)
        {
            var operation = new OperationResult<TResult>
            {
                Result = result
            };
            return operation;
        }

        private static string Generate(int size)
        {
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            var data = new byte[1];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                //Заполняет массив байтов криптостойкой последовательностью случайных ненулевых значений.
                crypto.GetNonZeroBytes(data);
                data = new byte[size];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(size);
            foreach (var item in data)
            {
                result.Append(chars[item % (chars.Length)]);
            }

            return result.ToString();
        }

        public void AppendLog(string messageLog)
        {
            if (string.IsNullOrEmpty(messageLog)) return;

            if(messageLog.Length > 500)
            {
                _logs.Add($"{messageLog.Substring(0, 500)}");
            }

            _logs.Add(messageLog);
        }

        public void AppendLog(IEnumerable<string> messageLogs)
        {
            if (messageLogs is null) return;

            foreach (var messageLog in messageLogs)
            {
                AppendLog(messageLog);
            }
        }
    }
    
    [Serializable]
    public class OperationResult<T> : OperationResult
    {
        public T Result { get; set; }
        public bool Ok
        {
            get
            {
                if(MetaData is null)
                {
                    return Exception == null && Result != null;
                }

                return Exception == null && Result != null && MetaData?.Type != MetaDataType.Error;
            }
        }
    }
}
