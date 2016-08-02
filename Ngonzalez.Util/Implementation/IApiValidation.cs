using System;
using System.Collections.Generic;

namespace Ngonzalez.Util
{
    public interface IApiValidation
    {
        T CleanParameter<T>(T data) where T : IConvertible;
        bool ValidateText(string text, int length);
        bool ValidateDate(string date);
        bool ValidateMail(string text, int length);
        bool ValidatePhone(string text, int length);
        bool IsNumber(string text);
        bool ValidateRut(string rut);
        string SanitizeFileName(string input);
        bool CheckFileExtension(string fileName, List<string> extension);
    }
}