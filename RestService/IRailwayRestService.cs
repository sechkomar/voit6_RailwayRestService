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
        List<Route> GetPossibleRoutes(string name, string token);


        [WebGet(UriTemplate = "/GetRouteDepartureDates?name={name}&token={token}&route_from={routeFrom}&route_to={routeTo}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetRouteDepartureDates(string name, string token, string routeFrom, string routeTo);

        [WebGet(UriTemplate = "/GetTimesByDate?name={name}&token={token}&date={date}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetTimesByDate(string name, string token, string date);


        [WebGet(UriTemplate = "/BuyTicket?name={name}&token={token}&route_from={routeFrom}&route_to={routeTo}&time={dateTime}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        TicketResponse BuyTicket(string name, string token, string routeFrom, string routeTo, string dateTime);


        //[WebGet(UriTemplate = "/AddToServicesRegister?name={serviceName}&={token}")]
        //[OperationContract]
        //void AddToServicesRegister(string name, string token);

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

    [DataContract]
    public class TicketResponse
    {
        [DataMember]
        public int ResponseCode { get; set; }


        [DataMember]
        public string ResponseMessage { get; set; }
    }
}
