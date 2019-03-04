using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BLE.Client.Data;
using RestSharp;

namespace BLE.Client.Droid
{
    class ApiClient : IApiClient
    {
        static string HOST = "http://118.69.37.147:8080";
        static readonly string BASE_URL = "demo/api/";
        static string KHO_URI = BASE_URL + "kho/tracuuton";
        static string XUATKHO_URI = BASE_URL + "kho/xuat";
        static string KIEMKHO_URI = BASE_URL + "kho/kiem";
        static string RESETKHO_URI = BASE_URL + "kho/resetkiemkhos";



        private RestClient restClient = null;
        
        public ApiClient()
        {
            this.restClient = new RestClient(HOST);
        }

        public List<KhoResultDTO> GetKhoResults()
        {
            var request = new RestRequest(KHO_URI, DataFormat.Json);

            var response = restClient.Get<List<KhoResultDTO>>(request);
            return response.Data;

        }

        public KhoDTO XuatKho(XuatkhoDTO dto)
        {
            var request = new RestRequest(XUATKHO_URI + "/" + dto.Rfid, DataFormat.Json);
            request.AddJsonBody(dto);
            var response = restClient.Put<KhoDTO>(request);
            return response.Data;
        }

        public bool KiemOk(string rfid)
        {
            var request = new RestRequest(KIEMKHO_URI + "/" + rfid, DataFormat.Json);
            try
            {
                var response = restClient.Put(request);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }


        }

        public bool ResetKiem()
        {
            var request = new RestRequest(RESETKHO_URI, DataFormat.Json);
            try
            {
                var response = restClient.Post(request);
                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}