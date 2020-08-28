using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cyclon.Utils {
    public class RestService {

        private static readonly HttpClient client = new HttpClient();
        public static string URL = "http://192.168.100.170:3000";

        public static async Task<string> GET(string URL_TARGET) {
            try {
                var response = await client.GetAsync(URL + URL_TARGET);
                var res = await response.Content.ReadAsStringAsync();
                return res;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static async Task<string> POST(FormUrlEncodedContent content, string URL_TARGET) {
            try {
                var response = await client.PostAsync(URL + URL_TARGET, content);
                var res = await response.Content.ReadAsStringAsync();
                return res;
            } catch (Exception e) {
                return null;
            }
        }
    }
}
