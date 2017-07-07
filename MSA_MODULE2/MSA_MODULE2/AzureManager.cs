using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using MSA_MODULE2.Model;
using System.Threading.Tasks;

namespace MSA_MODULE2
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<CognitiveAccuracyDbModel> notHotDogTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://msaModule2WhatIsThis.azurewebsites.net");
            this.notHotDogTable = this.client.GetTable<CognitiveAccuracyDbModel>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        //public async Task<List<CognitiveAccuracyDbModel>> GetAccuracyInfo()
        //{
        //    return await this.notHotDogTable.ToListAsync();
        //}
    }

}
