using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SHGui {
    public class SHProto {
        internal const string authURL = "/api/v2/auth?username={0}&password={1}";
        internal const string createURL = "/api/v2/create?username={0}&password={1}";
        internal const string changeURL = "/api/v2/auth?username={0}&password={1}&newpassword={2}";
        internal const string deleteURL = "/api/v2/delete?username={0}&password={1}";

        internal const string pushURL = "/api/v1/push?username={0}&password={1}&destination={2}&data={3}";
        internal const string pushRIDURL = "/api/v1/push?username={0}&password={1}&destination={2}&responseid={3}&data={4}";
        internal const string pullURL = "/api/v1/pull?username={0}&password={1}&offset={2}&count={3}";
        internal const string flushURL = "/api/v1/flush?username={0}&password={1}";
        internal const string lastURL = "/api/v1/last?username={0}&password={1}";

        internal const string apiV1 = "/api/v1";
        internal const string apiV2 = "/api/v2";

        const int API_V1_VER = 1;
        const int API_V2_VER = 1;

        public string ServerAddr { private set; get; }

        public SHProto(string serverAddr) {
            this.ServerAddr = serverAddr;
        }

        public string MakeGET(string url) {
            return Get("http://" + ServerAddr + url);
        }

        public string Get(string uri) {
            HttpClient client = new HttpClient();
            var responseT = client.GetAsync(uri);
            responseT.Wait();
            var contentT = responseT.Result.Content.ReadAsStringAsync();
            contentT.Wait();
            return contentT.Result;
        }

        public SecureResult RequestAuth(Credentials credentials) {
            string result = MakeGET(string.Format(authURL, credentials.username, credentials.password));
            return JsonConvert.DeserializeObject<SecureResult>(result);
        }
        public SecureResult RequestCreate(Credentials credentials) {
            string result = MakeGET(string.Format(createURL, credentials.username, credentials.password));
            return JsonConvert.DeserializeObject<SecureResult>(result);
        }
        public SecureResult RequestChange(Credentials credentials, string newpass) {
            string result = MakeGET(string.Format(changeURL, credentials.username, credentials.password, newpass));
            return JsonConvert.DeserializeObject<SecureResult>(result);
        }
        public SecureResult RequestDelete(Credentials credentials) {
            string result = MakeGET(string.Format(deleteURL, credentials.username, credentials.password));
            return JsonConvert.DeserializeObject<SecureResult>(result);
        }

        public TicketResult RequestPush(Credentials credentials, int destination, string data, int? rid) {
            string result = rid.HasValue ? 
                MakeGET(string.Format(pushRIDURL, credentials.username, credentials.password, destination, rid, data)) :
                MakeGET(string.Format(pushURL, credentials.username, credentials.password, destination, data));
            return JsonConvert.DeserializeObject<TicketResult>(result);
        }
        public TicketResult RequestPull(Credentials credentials, int offset, int lenght) {
            string result = MakeGET(string.Format(pullURL, credentials.username, credentials.password, offset, lenght));
            return JsonConvert.DeserializeObject<TicketResult>(result);
        }
        public TicketServiceResult RequestFlush(Credentials credentials) {
            string result = MakeGET(string.Format(flushURL, credentials.username, credentials.password));
            return JsonConvert.DeserializeObject<TicketServiceResult>(result);
        }
        public TicketServiceResult RequestLast(Credentials credentials) {
            string result = MakeGET(string.Format(lastURL, credentials.username, credentials.password));
            return JsonConvert.DeserializeObject<TicketServiceResult>(result);
        }

        public ApiVersionResult RequestAPIV1() {
            string result = MakeGET(apiV1);
            return JsonConvert.DeserializeObject<ApiVersionResult>(result);
        }
        public ApiVersionResult RequestAPIV2() {
            string result = MakeGET(apiV2);
            return JsonConvert.DeserializeObject<ApiVersionResult>(result);
        }
    }

    public struct Credentials {
        public string username, password;
    }
    public struct Ticket {
        public uint GlobalID, SourceID, DestinationID, TicketID, ResponseID;
        public string Data;
        public DateTime Date;
    }

    public struct TicketResult {
        public bool ok;
        public short status;
        public Ticket[] tickets;
    }
    public struct TicketServiceResult {
        public bool ok;
        public short status;
        public long count;
    }

    public struct SecureResult {
        public bool ok;
        public short status;
        public int ID;
    }

    public struct ApiVersionResult {
        public bool ok;
        public short status;
        public int version;
    }
}
