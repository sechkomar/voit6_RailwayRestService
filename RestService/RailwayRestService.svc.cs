using System;
using System.Collections.Generic;
using System.Web;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ServiceModel.Web;

namespace RestService
{
    public class RailwayRestService : IRailwayRestService
    {

        // --- methods and classes for CheckUser ---
        private string CreateBadCheckResponse(string checkResult)
        {
            JObject badResult = new JObject
            {
                ["result"] = checkResult
            };
            return badResult.ToString();

        }

        private Tuple<string, string> GetUserInfo(string name, string token)
        {
            var ctx = HttpContext.Current;
            String ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip) || ip.ToLower() == "unknown")
            {
                ip = ctx.Request.ServerVariables["REMOTEADDR"];
            }
            String agent = HttpContext.Current.Request.UserAgent;
            return Tuple.Create(ip, agent);
        }

        private class CheckResponse
        {
            public string d;
        }

        public string CheckToken(string name, string token, string ip, string userAgent)
        {
            string paymentServiceAddress = "http://payment-service-uni.apphb.com/PaymentRest.svc";
            var client = new RestClient(paymentServiceAddress);

            var checkRequest = new RestRequest("checkPayment", Method.GET);
            checkRequest.AddParameter("name", name);
            checkRequest.AddParameter("useragent", userAgent);
            checkRequest.AddParameter("token", token);
            checkRequest.AddParameter("ip", ip);


            var response = client.Execute<CheckResponse>(checkRequest);
            return response.Data.d;
        }

        private string CheckUser(string name, string token)
        {
            var info = GetUserInfo(name, token);
            var ip = info.Item1;
            var userAgent = info.Item2;


            // --- TODO change back ---
            return "true";
            //return CheckToken(name, token, ip, userAgent);
        }


        // --- methods for GetPossibleRoutes ---

        private List<Route> GetRoutesListFromDB()
        {
            //string dbServiceAddress = "10.160.64.86:63282";
            //var client = new RestClient(dbServiceAddress);
            //var routesRequest = new RestRequest("GetDestination", Method.GET);

            //IRestResponse routesResponse = client.Execute(routesRequest);
            //var routesRawString = routesResponse.Content;

            string routesRawString = "[{\"route_from\" : \"Minsk\", \"route_to\" : \"Moscow\"}," +
                "{\"route_from\" : \"Krakow\", \"route_to\" : \"Gomel\"}]";

            List<Route> routes = JsonConvert.DeserializeObject<List<Route>>(routesRawString);
            return routes;
        }

        public string GetPossibleRoutes(string name, string token)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token); // false, true or expired

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                var routesList = GetRoutesListFromDB();

                // some actions maybe later

                return JsonConvert.SerializeObject(routesList).ToString(); //json with routes
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return CreateBadCheckResponse(checkResult); // "result" : ("false" | "expired")
            }
        }


        //--- methods for GetRouteDepartureTimes ---

        private string GetTimesListFromDB(string routeFrom, string routeTo)
        {
            return "";
        }

        public string GetRouteDepartureTimes(string name, string token, string routeFrom, string routeTo)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token); // false, true or expired

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                var timesList = GetTimesListFromDB(routeFrom, routeTo);
                return "times list";
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return CreateBadCheckResponse(checkResult); // "result" : ("false" | "expired")
            }
        }


        // --- methods for BuyTicket ---

        private string BuyTicketFromDB(string routeFrom, string routeTo)
        {
            return "";

        }

        public string BuyTicket(string name, string token, string routeFrom, string routeTo, string dateTime)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token); // false, true or expired

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                var ticketResult = BuyTicketFromDB(routeFrom, routeTo);
                return "ticket result";
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return CreateBadCheckResponse(checkResult); // "result" : ("false" | "expired")
            }
        }


    }
}
