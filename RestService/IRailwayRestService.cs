using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RestService
{
    [ServiceContract]
    public interface IRailwayRestService
    {

        [WebGet(UriTemplate = "/GetPossibleRoutes?name={name}&token={token}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetPossibleRoutes(string name, string token);


        [WebGet(UriTemplate = "/GetRouteDepartureTimes?name={name}&token={token}&route_from={routeFrom}&route_to={routeTo}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        string GetRouteDepartureTimes(string name, string token, string routeFrom, string routeTo);


        [WebGet(UriTemplate = "/BuyTicket?name={name}&token={token}&route_from={routeFrom}&route_to={routeTo}&time={dateTime}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        string BuyTicket(string name, string token, string routeFrom, string routeTo, string dateTime);


        //// TODO: delete from interface
        //[WebGet(UriTemplate = "/CheckRequest?name={name}&useragent={useragent}&token={token}&ip={ip}"
        //    , RequestFormat = WebMessageFormat.Json,
        //    ResponseFormat = WebMessageFormat.Json)]
        //    //)]
        //[OperationContract]
        //string CheckToken(string name, string token, string ip, string userAgent);


    }

    [DataContract]
    public class Route
    {
        [DataMember]
        public string route_to { get; set; }

        [DataMember]
        public string route_from { get; set; }
    
    }
}
