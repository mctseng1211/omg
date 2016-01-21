using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO.Compression;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace omg.Models
{
    public class TrafficStats
    {
        public async Task<string> PostJSON(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                Uri _url = new Uri(url);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _url);
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var buffer = await response.Content.ReadAsByteArrayAsync();
                var byteArray = buffer.ToArray();
                var responseString = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                return responseString;
            }
        }

        public async Task<string> PostGZJSON(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                Uri _url = new Uri(url);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _url);
                request.Headers.Add("Accept", "application/json");
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var byteArray = await response.Content.ReadAsByteArrayAsync();
                //maybe decompress async 
                var unzipByteArray = Decompress(byteArray);
                var responseString = System.Text.Encoding.UTF8.GetString(unzipByteArray, 0, unzipByteArray.Length);

                return responseString;
            }
        }

        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.

            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }



        public async Task<string> GetModel(string url, string modelName, object sarea)
        {

            var urlJson = await PostGZJSON(url);

            switch (modelName)
            {
                case "youbike":
                    dynamic json = JValue.Parse(Convert.ToString(urlJson));
                    string rawValStr = Convert.ToString(json.retVal);
                    parseYouBike(rawValStr, Convert.ToString(sarea), "");
                    return rawValStr;
                case "mrt":
                    return "";
                case "bus":
                    //dynamic json1 = JValue.Parse(Convert.ToString(jsonstr));
                    //string rawValStr1 = Convert.ToString(json1.retVal);
                    //parseYouBike(rawValStr1, Convert.ToString(sarea), "");
                    JObject estimateJObj = JObject.Parse(Convert.ToString(urlJson));
                    IList<JToken> estimateList = estimateJObj["BusInfo"].Children().ToList();
                    var routejson = await PostGZJSON("http://data.taipei/bus/ROUTE");
                    parseBus(estimateList);
                    return "";
                default:
                    return "";
            }

        }

        public void parseYouBike(string jsonStr, string sarea, string sna){
            ybStationsDict = JsonConvert.DeserializeObject<Dictionary<string, YBInfo>>(jsonStr);

            foreach (KeyValuePair<string, YBInfo> item in ybStationsDict) {
                debugStr += item.Value.sarea + item.Value.ar;
                if (item.Value.sarea == sarea) {
                    ybStationList.Add(item.Value);
                }
            }
        }

        public void parseBus(IList<JToken> estList)
        {
            foreach (JToken estToken in estList)
            {
                EstimateInfo estInfo = JsonConvert.DeserializeObject<EstimateInfo>(estToken.ToString());
                estInfoList.Add(estInfo);
            }
        }

        public class EstimateInfo
        {
            public string StopID { get; set; }
            public string GoBack { get; set; }
            public string RouteID { get; set; }
            public string EstimateTime { get; set; }
        }





        public class YBInfo
        {
            public string sno { get; set; }
            public string sna { get; set; }
            public string tot { get; set; }
            public string sbi { get; set; }
            public string sarea { get; set; }
            public string mday { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
            public string ar { get; set; }
            public string sareaen { get; set; }
            public string snaen { get; set; }
            public string aren { get; set; }
            public string bemp { get; set; }
            public string act { get; set; }
        }


        public string GetDebugStr() {
            return debugStr;
        }

        public Dictionary<string, YBInfo> GetYBStationDict() {
            return ybStationsDict;
        }

        public List<YBInfo> GetYBList() {
            return ybStationList;
        }


        public List<EstimateInfo> GetEstimateList()
        {
            return estInfoList;
        }
        Dictionary<string, YBInfo> ybStationsDict;
        List<YBInfo> ybStationList = new List<YBInfo>();

        List<EstimateInfo> estInfoList = new List<EstimateInfo>();


        public static string debugStr = "debug";

    }
}
