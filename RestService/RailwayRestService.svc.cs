﻿using System;
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
        private string dbServiceAddress = "";
        private string paymentServiceAddress = "http://payment-service-uni.apphb.com/PaymentRest.svc";

        // --- methods and classes for CheckUser ---

        //private string CreateBadCheckResponse(string checkResult)
        //{
        //    JObject badResult = new JObject
        //    {
        //        ["result"] = checkResult
        //    };
        //    return badResult.ToString();

        //}

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
            //var client = new RestClient(dbServiceAddress);
            //var routesRequest = new RestRequest("GetDestination", Method.GET);

            //var routesResponse = client.Execute<List<Route>>(routesRequest);
            // return routesResponse.Data; // check it

            // TODO CHANGE BACK

            string routesRawString = "[{\"route_from\" : \"Minsk\", \"route_to\" : \"Moscow\"}," +
                "{\"route_from\" : \"Krakow\", \"route_to\" : \"Gomel\"}]";

            List<Route> routes = JsonConvert.DeserializeObject<List<Route>>(routesRawString);
            return routes;
        }

        public List<Route> GetPossibleRoutes(string name, string token)
        {
            //    var cl = new ServicesRegister.RegServ1();
            //    string[] methodsList = { "GetPossibleRoutes", "GetRouteDepartureTimes", "BuyTicket" };

            //    cl.addServiceWithMethods("RailwayRestService", methodsList);


            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token); // false, true or expired

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                var routesList = GetRoutesListFromDB();

                // some actions maybe later

                return routesList;
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;
            }
        }

        // --- methods and fields for GetTimesByDate ---

        private Dictionary<string, List<string>> currentScheduleDict;

        private List<string> GetTimesFromCurrentSheduleDict(string date)
        {
            if (currentScheduleDict.ContainsKey(date))
            {
                return currentScheduleDict[date];
            }
            return null;
        }

        public List<string> GetTimesByDate(string name, string token, string date)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token);

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                var currentTimes = GetTimesFromCurrentSheduleDict(date);
                return currentTimes;
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;
            }
        }

        //--- methods for GetRouteDepartureTimes ---

        private Dictionary<string, List<string>> GetTimesDictFromDB(string routeFrom, string routeTo)
        {
            var client = new RestClient(dbServiceAddress);

            string relativeRequest = "/" + routeFrom + "/" + routeTo;
            var timesRequest = new RestRequest("GetDestination" + relativeRequest, Method.GET);

            var timesDict = client.Execute<Dictionary<string, List<string>>>(timesRequest);
            return timesDict.Data;

        }


        public List<string> GetRouteDepartureDates(string name, string token, string routeFrom, string routeTo)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token);

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                currentScheduleDict = GetTimesDictFromDB(routeFrom, routeTo);
                
                return new List<string>(currentScheduleDict.Keys);
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;
            }
        }



        // --- methods for BuyTicket ---



        private TicketResponse BuyTicketFromDB(string routeFrom, string routeTo, string dateTime)
        {
            var client = new RestClient(dbServiceAddress);
            string relativeRequest = "/" + routeFrom + "/" + routeTo + "/" + dateTime;
            var buyTicketRequest = new RestRequest("GetDestination" + relativeRequest, Method.GET);

            var buyTicketResponse = client.Execute<TicketResponse>(buyTicketRequest);
            return buyTicketResponse.Data;

        }

        public TicketResponse BuyTicket(string name, string token, string routeFrom, string routeTo, string dateTime)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            var checkResult = CheckUser(name, token);

            if (checkResult.ToLower() == "true")
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                var ticketResult = BuyTicketFromDB(routeFrom, routeTo, dateTime);
                return ticketResult; // TODO CHANGE THIS
            }
            else
            {
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return null;
            }
        }

    }
}
