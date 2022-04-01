using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ProtectionOfInfo.WebApp.TelegramBot
{
    public interface IHandlerUpdateTelegramService
    {
        public Task EchoAsync(Update update);
        public void SendAllClientMessage(string text);
        public void SendAllClientImage(byte[] data, string fileName, string extension, string caption);
        public void SendAllClientFile(byte[] data, string fileName, string extension, string caption);
    }
}