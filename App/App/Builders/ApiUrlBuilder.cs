using System;
using App.Enums;

namespace App.Builders
{
	public class ApiUrlBuilder
	{
		private string httpSecurity = "s";
        private string ApiServerAddress = "localhost";
        private string ApiPortNumber = "7242";
		private string _urlType = "";
		private string _entities = "";
		private string _request = "";
		private string _method = "";
		private string _parameter = "";

		public ApiUrlBuilder(UrlTypeEnum urlType)
		{
			if (urlType == UrlTypeEnum.api) _urlType += "/api";
			else if (urlType == UrlTypeEnum.sql) _urlType += "https://sql-server"; // Daha sonra tekrardan yazılacak!
        }


		public ApiUrlBuilder SetHttpSecurity(bool security)
		{
			if (security) httpSecurity = "s";
			else httpSecurity = "";
			return this;
		}


        public ApiUrlBuilder SetServerAddress(string adress)
		{
			ApiServerAddress = adress;
			return this;
		}


		public ApiUrlBuilder SetPortNumber(string portNumber)
        {
            ApiPortNumber = portNumber;
            return this;
        }


        public ApiUrlBuilder SetPortNumber(int portNumber)
        {
            ApiPortNumber = portNumber.ToString();
            return this;
        }


        public ApiUrlBuilder AddEntities(EntitiesEnum entitiesType)
		{
			_entities += "/";
            _entities += entitiesType.ToString();
			return this;
		}


		public ApiUrlBuilder AddRequest(RequestEnum requestType)
		{
            _request += "/";
            _request += requestType.ToString();
			return this;
        }


		public ApiUrlBuilder AddMethod(MethodEnum methodType)
		{
			_method += "/";
			_method += methodType.ToString();
            return this;
		}


		public ApiUrlBuilder AddParameter(string parameter)
		{
			_parameter += "/";
			_parameter += parameter;
			return this;
		}


        public ApiUrlBuilder AddParameter(int parameter)
        {
            _parameter += "/";
            _parameter += parameter.ToString();
            return this;
        }


        public string Build()
		{
			string url = "http" + httpSecurity + "://" + ApiServerAddress + ":" + ApiPortNumber + _urlType + _entities + _request + _method + _parameter;
			return url;
		}
	}
}

