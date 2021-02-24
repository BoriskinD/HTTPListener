using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace Listener
{
    public class Sign
    {
        public X509Certificate2 SignerCert { get; private set; }

        public Sign()
        { }

        public static bool VerifyMsg(byte[] encodedSignedCms, out string result)
        {
            SignedCms signedCms = new SignedCms();

            result = "Проверка подписи прошла успешно";

            try
            {
                // Декодируем сообщение.
                signedCms.Decode(encodedSignedCms);
            }
            catch (Exception ex) { 
                result = ex.Message;
                return false;
            }

            if (signedCms.SignerInfos.Count == 0)
            {
                result = "Файл не подписан";
                return false;
            }

            bool valid = true;
            SignerInfoEnumerator enumerator = signedCms.SignerInfos.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SignerInfo current = enumerator.Current;
                //SignerCert = current.Certificate;
                try
                {
                    // Используем проверку подписи и стандартную 
                    // процедуру проверки сертификата: построение цепочки, 
                    // проверку цепочки, и необходимых расширений для данного 
                    // сертификата.
                    current.CheckSignature(false);
                }
                catch (System.Security.Cryptography.CryptographicException ex)
                {
                    valid = false;
                    result = ex.Message;
                }
                // При наличии соподписей проверяем соподписи.
                if (current.CounterSignerInfos.Count > 0)
                {
                    SignerInfoEnumerator coenumerator = current.CounterSignerInfos.GetEnumerator();
                    while (coenumerator.MoveNext())
                    {
                        SignerInfo cosigner = coenumerator.Current;
                        try
                        {
                            // Используем проверку подписи и стандартную 
                            // процедуру проверки сертификата: построение цепочки, 
                            // проверку цепочки, и необходимых расширений для данного 
                            // сертификата.
                            cosigner.CheckSignature(false);
                        }
                        catch (System.Security.Cryptography.CryptographicException ex)
                        {
                            valid = false;
                            result = ex.Message;
                        }
                    }
                }
            }
            //Console.WriteLine();
            if (valid)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

