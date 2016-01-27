using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Ngonzalez.Util
{
    public class ApiValidation :IApiValidation
    {
        public T CleanParameter<T>(T data) where T : IConvertible
        {
            var temp = AntiXSS.Sanitize(data.ToString(CultureInfo.InvariantCulture));

            return ((T)(Convert.ChangeType(temp, typeof(T))));
        }

        public bool ValidateText(string text, int length)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var valid = new Regex(@"^([a-z.A-ZÑñ_áéíóú0-9\s]+)$", RegexOptions.None);
            return (valid.IsMatch(text) && text.Length <= length);
        }

        public bool ValidateDate(string date)
        {
            if (string.IsNullOrWhiteSpace(date)) return false;
            var valid = new Regex(@"^([0][1-9]|[12][0-9]|3[01])(/|-)(0[1-9]|1[012])\2(\d{4})$", RegexOptions.IgnoreCase);
            return (valid.IsMatch(date));
        }

        public bool ValidateMail(string text, int length)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var valid = new Regex(@"(^[0-9a-zA-Z]+(?:[._][0-9a-zA-Z]+)*)@([0-9a-zA-Z]+(?:[._-][0-9a-zA-Z]+)*\.[0-9a-zA-Z]{2,3})$", RegexOptions.None);
            return (valid.IsMatch(text) && text.Length <= length);
        }

        public bool ValidatePhone(string text, int length)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var valid = new Regex(@"^(?:\+|-)?\d+$", RegexOptions.None);
            return (valid.IsMatch(text) && text.Length <= length);
        }

        public bool IsNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            var valid = new Regex(@"^[0-9]+$", RegexOptions.None);
            return (valid.IsMatch(text));
        }

        public bool ValidateRut(string rut)
        {
            var valid = new Regex(@"^0*(\d{1,3}(\.?\d{3})*)\-?([\dkK])$", RegexOptions.IgnoreCase);
            if (string.IsNullOrWhiteSpace(rut) || !valid.IsMatch(rut)) return false;
            rut = rut.Replace("-", "");
            string body = rut.Substring(0, rut.Length - 1);
            string digit = rut.Substring(rut.Length - 1, 1);

            int numberRut;
            if (int.TryParse(body, out numberRut))
            {
                return (CheckDigit(numberRut).ToUpper() == digit.ToUpper());
            }
            return false;
        }

        private string CheckDigit(int rut)
        {
            int cont = 2;
            int collector = 0;
            while (rut != 0)
            {
                int multiplier = (rut % 10) * cont;
                collector = collector + multiplier;
                rut = rut / 10;
                cont = cont + 1;
                if (cont == 8)
                {
                    cont = 2;
                }
            }

            int digit = 11 - (collector % 11);
            string digitRut = digit.ToString(CultureInfo.InvariantCulture).Trim();
            if (digit == 10)
            {
                digitRut = "K";
            }
            if (digit == 11)
            {
                digitRut = "0";
            }
            return (digitRut);
        }

    }
}

