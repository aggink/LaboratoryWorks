using AutoMapper;
using ProtectionOfInfo.WebApp.Data.Entities.DictionaryEntities;
using ProtectionOfInfo.WebApp.Infrastructure.OperationResults;
using ProtectionOfInfo.WebApp.ViewModels.DictionaryViewModels;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProtectionOfInfo.WebApp.Infrastructure.Services.DictionaryApiService
{
    public class DictionaryApi : IDictionaryApi
    {
        private const string urlApi = "https://api.dictionaryapi.dev/api/v2/entries/";
        private readonly IMapper _mapper;
        public DictionaryApi(IMapper mapper) 
        {
            _mapper = mapper;
        }

        private async Task<string?> GetJsonDictionaryAsync<T>(OperationResult<T> operation, string word, string code)
        {
            var resultUrl = urlApi + code + "/" + word;

            var request = (HttpWebRequest)WebRequest.Create(resultUrl);
            if (request == null)
            {
                operation.AddError("Ошибка при формировании url");
                return null;
            }

            request.Method = "GET";
            request.ContentType = "application/json";

            var response = (HttpWebResponse)await request.GetResponseAsync();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream);

            string json = await reader.ReadToEndAsync();

            response.Close();
            return json;
        }

        public async Task<OperationResult<Dictionary>> GetWordDefinitionsAsync(string word, string code)
        {
            var operation = OperationResult.CreateResult<Dictionary>();

            try
            {
                var json = await GetJsonDictionaryAsync<Dictionary>(operation, word, code);
                if(json == null)
                {
                    return operation;
                }

                var dictionary = JsonSerializer.Deserialize<Dictionary[]>(json);
                if (dictionary == null)
                {
                    var error = JsonSerializer.Deserialize<ErrorDictionary>(json);
                    if(error == null)
                    {
                        operation.AddError("В процессе десериализации json файла произошла ошибка");
                        return operation;
                    }

                    if (error.Title != null && error.Message != null && error.Resolution != null)
                    {
                        operation.AppendLog(new string[]
                        {
                            error.Title,
                            error.Message,
                            error.Resolution
                        });
                    }

                    operation.AddError($"Ошибка в запросе url - { urlApi + code + "/" + word }");
                    return operation;
                }

                operation.Result = dictionary[0]!;
                operation.AddSuccess("Десериализация файла json завершилась успешно");
            }
            catch (WebException ex)
            {
                var status = ex.Status;
                if(status == WebExceptionStatus.ProtocolError)
                {
                    var httpResponse = (HttpWebResponse)ex.Response!;
                    operation.AddError($"Статус кода ошибки: { (int)httpResponse.StatusCode } - { httpResponse.StatusCode }");
                }        
            }
            catch(Exception ex)
            {
                operation.AddError(ex.Message);
            }

            return operation;
        }

        public async Task<OperationResult<string>> GetJsonWordDefinitionsAsync(string word, string code)
        {
            var operation = OperationResult.CreateResult<string>();

            var dictionary = await GetWordDefinitionsAsync(word, code);
            operation.AppendLog(dictionary.Logs);
            if (!dictionary.Ok)
            {
                operation.MetaData = dictionary.MetaData;
                operation.Exception = dictionary.Exception;
                return operation;
            }

            var dictionaryViewModel = _mapper.Map<DictionaryViewModel>(dictionary.Result);

            try
            {
                string json = JsonSerializer.Serialize<DictionaryViewModel>(dictionaryViewModel);
                if (json == null)
                {
                    operation.AddError("Сериализация объекта завершилась с ошибкой");
                    return operation;
                }

                operation.Result = json;
                operation.AddSuccess("Cериализация файла json завершилась успешно");
            }
            catch (Exception ex)
            {
                operation.AddError(ex.Message);
            }

            return operation;
        }
    }
}
