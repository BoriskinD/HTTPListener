using System.IO;
using System.Net;
using System.Text;

namespace Listener
{
    class Listener
    {
        //флаг текущего состояния прослушки
        bool isListening;

        HttpListener listener;

        public Listener()
        {
            listener = new HttpListener();
            isListening = false;
        }

        public string prefix { get; set; }

        public void StartListen()
        {
            listener.Start();
            isListening = true;
        }

        public void StopListen()
        {
            listener.Stop();
            isListening = false;
        }

        public void Listen()
        {
            //добавить адрес прослушки
            listener.Prefixes.Add(prefix);
            StartListen();

            while (isListening)
            {
                //получить доступ к объектам запроса
                HttpListenerContext context = listener.GetContext();

                //описывает входящий запрос
                HttpListenerRequest incomingRequest = context.Request;

                //поток с данными запроса
                Stream requestStream = incomingRequest.InputStream;

                //массив байтов данных запроса
                byte[] data = new byte[incomingRequest.ContentLength64];

                //записать данные с потока в массив
                requestStream.Read(data, 0, data.Length);
                requestStream.Close();

                //проверить подпись
                _ = Sign.VerifyMsg(data, out string result);

                //массив байтов содержащий результат проверки подписи
                byte[] verifyResult = Encoding.Default.GetBytes(result);

                //сформировать объект ответа на запрос
                HttpListenerResponse outputResponse = context.Response;

                //задать длину данных для ответа
                outputResponse.ContentLength64 = verifyResult.Length;

                //поток для ответа
                Stream responseStream = outputResponse.OutputStream;

                //записать ответ в поток
                responseStream.Write(verifyResult, 0, verifyResult.Length);

                //отправить ответ и закрыть объект HttpListenerResponse
                outputResponse.Close();
            }
        }
    }
}
