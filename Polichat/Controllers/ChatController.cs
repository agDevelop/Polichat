using Newtonsoft.Json;
using Polichat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Polichat.Controllers
{
    public class ChatController : Controller
    {
        DBContext db = new DBContext();

        [HttpPost]
        public string Test(string name)
        {
            return $"Hello, {name}";
        }

        [HttpGet]
        public string Test()
        {
            return "Hello";
        }


        /// <summary>
        /// Вход
        /// </summary>
        /// <param name="user">Пользователь в формате JSON</param>
        /// <returns>Код авторизации 1 - успешно, 0 - отказ, 2 - с ошибкой, 4 - такого логина нет</returns>
        [HttpPost]
        public string Login(string user)
        {
            try
            {
                User deserializedUser = JsonConvert.DeserializeObject<User>(user);

                if (db.Users.Count(usr => usr.Login.Equals(deserializedUser.Login)) == 0)
                {

                    return JsonConvert.SerializeObject("4");

                }
                else
                {
                    User userToLogin = db.Users.FirstOrDefault(usr => usr.Login == deserializedUser.Login);

                    if (userToLogin.Password == deserializedUser.Password)
                    {
                        return JsonConvert.SerializeObject("1");
                    }
                }
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject("2");
            }

            return JsonConvert.SerializeObject("0");
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="user">Пользователь в формате JSON</param>
        /// <returns>Код регистрации 1 - успешно, 0 - отказ, 2 - с ошибкой, 3 - такой логин уже есть</returns>
        [HttpPost]
        public string Register(string user)
        {
            try
            {
                User deserializedUser = JsonConvert.DeserializeObject<User>(user);

                if (db.Users.Count(usr => usr.Login.Equals(deserializedUser.Login)) == 0)
                {
                    if (!string.IsNullOrEmpty(deserializedUser.Login) && !string.IsNullOrEmpty(deserializedUser.Password) && !string.IsNullOrEmpty(deserializedUser.Email))
                    {

                        db.Users.Add(deserializedUser);

                        db.SaveChanges();

                        return JsonConvert.SerializeObject("1");
                    }
                }
                else
                {
                    return JsonConvert.SerializeObject("3");
                }
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject("2");
            }

            return JsonConvert.SerializeObject("0");
        }

        /// <summary>
        /// Возвращает список сообщений
        /// </summary>
        /// <param name="index">индекс элемента, с которого делать выборку</param>
        /// <param name="user">Экземпляр класса User. Пользователь, получающий сообщения</param>
        /// <returns>Сериализованный список сообщений закрытый типом  Message. 0 - отказ, 2 - ошибка, 4 - такого логина нет</returns>
        [HttpPost]
        public string GetMessages(int index, string user)
        {
            try
            {
                User deserializedUser = JsonConvert.DeserializeObject<User>(user);

                if (db.Users.Count(usr => usr.Login.Equals(deserializedUser.Login)) == 0)
                {

                    return JsonConvert.SerializeObject("4");

                }
                else
                {
                    User userToLogin = db.Users.FirstOrDefault(usr => usr.Login == deserializedUser.Login);

                    if (userToLogin.Password == deserializedUser.Password)
                    {
                        List<Message> messages = db.Messages.ToList();

                        List<Message> messagesSlice = messages.Skip(index).ToList();

                        return JsonConvert.SerializeObject(messagesSlice);

                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }

            return JsonConvert.SerializeObject("0");
        }

        /// <summary>
        /// Возвращает код результата добавления сообщения
        /// </summary>
        /// <param name="message">Экземпляр класса Message</param>
        /// <param name="user">Экземпляр класса User. Пользователь, отправляющий сообщения</param>
        /// <returns>1 - успешно, 0 - отказ, 2 - ошибка, 4 - такого логина нет</returns>
        [HttpPost]
        public string SendMessage(string message, string user)
        {
            try
            {
                User deserializedUser = JsonConvert.DeserializeObject<User>(user);

                if (db.Users.Count(usr => usr.Login.Equals(deserializedUser.Login)) == 0)
                {
                    return JsonConvert.SerializeObject("4");
                }
                else
                {
                    User userToLogin = db.Users.FirstOrDefault(usr => usr.Login == deserializedUser.Login);

                    if (userToLogin.Password == deserializedUser.Password)
                    {
                        Message deserializedMessage = JsonConvert.DeserializeObject<Message>(message);

                        deserializedMessage.Added = DateTime.Now;

                        db.Messages.Add(deserializedMessage);

                        db.SaveChanges();

                        return JsonConvert.SerializeObject("1");
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }

            return JsonConvert.SerializeObject("0");
        }
    }
}