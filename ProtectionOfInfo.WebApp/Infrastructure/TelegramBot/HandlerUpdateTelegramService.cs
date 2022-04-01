using Calabonga.UnitOfWork;
using ProtectionOfInfo.WebApp.Data;
using ProtectionOfInfo.WebApp.Data.DbContexts;
using ProtectionOfInfo.WebApp.Data.Entities.TelegramEntities;
using ProtectionOfInfo.WebApp.Infrastructure.Services.CurrencyService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProtectionOfInfo.WebApp.TelegramBot
{
    /// <summary>
    ///  // aggink: update summary - 27.03.2022 21:37:31
    ///  https://github.com/TelegramBots/Telegram.Bot.Examples/tree/df370a105c33c135f3e0cc0b1ad3d9ed8d138f5c
    /// </summary>
    public class HandlerUpdateTelegramService : IHandlerUpdateTelegramService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUnitOfWork<TelegramDbContext> _telegtamDbContext;
        private readonly ICurrencyService _currencyService;

        private static List<UserTelegram> Users = new List<UserTelegram>();

        public HandlerUpdateTelegramService(
            ITelegramBotClient botClient, 
            IUnitOfWork<TelegramDbContext> telegramDbContext,
            ICurrencyService currencyService)
        {
            _botClient = botClient;
            _telegtamDbContext = telegramDbContext;
            _currencyService = currencyService;
        }

        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),
                _ => UnkmownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception ex)
            {
                await HandlerErrorAsync(ex);
            }
        }

        #region BotOnMessageReceived


        private async Task BotOnMessageReceived(Message message)
        {
            if(message.Type != MessageType.Text)
            {
                throw new Exception("Не является текстом");
            }
            
            
            switch(message.Text!.Split(' ')[0])
            {
                case "/start":
                    await SendStart(message);
                    break;
                case "/select_photo":
                    await SendInlineKeyboard(message);
                    break;
                case "/random_photo":
                    await SendImage(message);
                    break;
                case "/exchange_rates":
                    await SendReplyKeyboard(message);
                    break;
                default:
                    if (!await CheckExchangeRate(message))
                    {
                        await Usage(message);
                    }
                    break;
            }
        }

        private async Task<Message> Usage(Message message)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            const string usage = "Используйте:\n" +
                                 "/start - хехехе\n" +
                                 "/select_photo - выбрать фото\n" +
                                 "/random_photo - получить рандомное фото\n" +
                                 "/exchange_rates - просмотреть курс валют\n";

            return await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove());
        }

        private async Task<bool> CheckExchangeRate(Message message)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var valute = _currencyService.GetValute(message.Text!);
            if (valute != null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"{valute.Nominal} {valute.Name} - {valute.Value} руб.",
                    replyMarkup: new ReplyKeyboardRemove());

                return true;
            }

            return false;
        }

        private async Task<Message> SendStart(Message message)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var user = Users.FirstOrDefault(x => x.ChatId == message.Chat.Id);
            if (user == null)
            {
                Users.Add(new UserTelegram()
                {
                    Id = Guid.NewGuid(),
                    ChatId = message.Chat.Id,
                    NickName = message.Chat!.Username ?? message.Chat!.FirstName!
                });
            }

            await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Чёрт",
                replyMarkup: new ReplyKeyboardRemove());

            return await Usage(message);
        }

        private async Task<Message> SendInlineKeyboard(Message message)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var fileRepository = _telegtamDbContext.GetRepository<FileTelegram>();
            var images = await fileRepository.GetAllAsync(predicate: x => x.IsImage && x.IsPublication);

            var inKeyboardBtn = new List<List<InlineKeyboardButton>>();
            var _inKeyboardBtn = new List<InlineKeyboardButton>();
            for(int i = 0, j = 0; i < images.Count(); i++, j++)
            {
                if (j == 3)
                {
                    inKeyboardBtn.Add(new List<InlineKeyboardButton>(_inKeyboardBtn));
                    _inKeyboardBtn.Clear();
                    j = 0;
                }
                _inKeyboardBtn.Add(InlineKeyboardButton.WithCallbackData(images[i].Value!, images[i].Id.ToString()));
            }
            inKeyboardBtn.Add(new List<InlineKeyboardButton>(_inKeyboardBtn));


            return await _botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Сделайте выбор",
                replyMarkup: new InlineKeyboardMarkup(inKeyboardBtn));
        }

        private async Task<Message> SendReplyKeyboard(Message message)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var valutes = _currencyService.GetAllValute();
            if(valutes == null)
            {
                return await _botClient.SendTextMessageAsync(
                    chatId: message!.Chat.Id,
                    text: $"Ошибка при обработке запроса");
            }

            var keyboardBtns = new List<List<KeyboardButton>>();
            var _keyboardBtns = new List<KeyboardButton>();
            for (int i = 0, j = 0; i < valutes.Count(); i++, j++)
            {
                if (j == 3)
                {
                    keyboardBtns.Add(new List<KeyboardButton>(_keyboardBtns));
                    _keyboardBtns.Clear();
                    j = 0;
                }
                _keyboardBtns.Add(new KeyboardButton(valutes[i].Name));
            }
            keyboardBtns.Add(new List<KeyboardButton>(_keyboardBtns));

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(keyboardBtns)
            {
                ResizeKeyboard = true
            };

            return await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Сделайте выбор",
                                                  replyMarkup: replyKeyboardMarkup);
        }

        private async Task<Message> SendImage(Message message)
        {
            await _botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

            var fileRepository = _telegtamDbContext.GetRepository<FileTelegram>();
            var images = await fileRepository.GetAllAsync(predicate: x => x.IsImage && x.IsPublication);
            if (images == null && images!.Count == 0)
            {
                return await _botClient.SendTextMessageAsync(
                    chatId: message!.Chat.Id,
                    text: $"Ошибка при обработке запроса");

            }
            var random = new Random();
            var i = random.Next(0, images.Count);

            using (var fileStream = new MemoryStream(images[i].Data))
            {
                return await _botClient.SendPhotoAsync(
                    chatId: message!.Chat.Id,
                    photo: new InputOnlineFile(fileStream, images[i].FileName + images[i].Extension),
                    caption: images[i].Value);
            }
        }

        #endregion

        #region Send

        public void SendAllClientMessage(string text)
        {
            Parallel.For(0, Users.Count, async (i) =>
            {
                await _botClient.SendChatActionAsync(Users[i].ChatId, ChatAction.Typing);

                await _botClient.SendTextMessageAsync(
                    chatId: Users[i].ChatId,
                    text: text,
                    replyMarkup: new ReplyKeyboardRemove());
            });
        }

        public void SendAllClientFile(byte[] data, string fileName, string extension, string caption)
        {
            Parallel.For(0, Users.Count, async (i) =>
            {
                await _botClient.SendChatActionAsync(Users[i].ChatId, ChatAction.UploadDocument);

                using (var fileStream = new MemoryStream(data))
                {
                    await _botClient.SendDocumentAsync(
                        chatId: Users[i].ChatId,
                        document: new InputOnlineFile(fileStream, fileName + extension),
                        caption: caption);
                }
            });
        }

        public void SendAllClientImage(byte[] data, string fileName, string extension, string caption)
        {
            Parallel.For(0, Users.Count, async (i) =>
            {
                await _botClient.SendChatActionAsync(Users[i].ChatId, ChatAction.UploadPhoto);

                using (var fileStream = new MemoryStream(data))
                {
                    await _botClient.SendPhotoAsync(
                        chatId: Users[i].ChatId,
                        photo: new InputOnlineFile(fileStream, fileName + extension),
                        caption: caption);
                }
            });
        }

        #endregion

        /// <summary>
        ///  // aggink: update summary - 28.03.2022 20:27:08
        ///  // ответ от действий метода - SendInlineKeyboard
        ///  // Этот объект представляет входящий запрос обратного вызова от кнопки обратного вызова на встроенной клавиатуре. 
        ///  // Если кнопка, вызвавшая запрос, была прикреплена к сообщению, отправленному ботом, поле сообщения будет присутствовать. 
        ///  // Если кнопка была прикреплена к сообщению, отправленному через бота (в режиме inline ), поле inline_message_id будет присутствовать. 
        ///  // Будет присутствовать ровно одно из полей data или game_short_name.
        /// </summary>
        /// <param name="callbackQuery"></param>
        /// <returns></returns>
        private async Task<Message> BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Получено {callbackQuery.Data}");

            await _botClient.SendChatActionAsync(callbackQuery.Message!.Chat.Id, ChatAction.UploadPhoto);

            var fileRepository = _telegtamDbContext.GetRepository<FileTelegram>();
            var image = await fileRepository.GetFirstOrDefaultAsync(predicate: x => x.IsImage && x.IsPublication && x.Id.ToString() == callbackQuery.Data);
            if (image == null)
            {
                return await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message!.Chat.Id,
                    text: $"Ошибка при обработке запроса");

            }

            using (var fileStream = new MemoryStream(image.Data))
            {
                return await _botClient.SendPhotoAsync(
                    chatId: callbackQuery.Message!.Chat.Id,
                    photo: new InputOnlineFile(fileStream, image.FileName + image.Extension),
                    caption: image.Value);
            }
        }


        private Task UnkmownUpdateHandlerAsync(Update update)
        {
            return Task.CompletedTask;
        }

        private Task HandlerErrorAsync(Exception ex)
        {
            var ErrorMessage = ex switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => ex.ToString()
            };

            return Task.CompletedTask;
        }
    }
}