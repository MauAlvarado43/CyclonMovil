
using System.Text.RegularExpressions;

namespace Cyclon.Utils {
    class RegexValidation {
        public static bool checkLength(string text) { return text.Length <= 50; }
        public static bool checkEmail(string email) { return (new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase).IsMatch(email)); }
        public static bool checkWords(string text) { return (new Regex(@"^([A-Za-zÁÉÍÓÚáéíóú\\xF1\\xD1]*(\s)?[A-Za-zÁÉÍÓÚáéíóú\\xF1\\xD1]+)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase).IsMatch(text)); }
        public static string replaceSpaces(string text) { return Regex.Replace(text, @"\s+", " "); }
    }
}
