using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using Supplier_AzureTBLRModel.ViewModels;
using Supplier_AzureTBLRModel.Entities;

namespace DeleteSupplierFunc_AzureTBL
{
    public static class Function1
    {
        [FunctionName("DeleteSupplier")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string id = req.Query["id"];
                if (string.IsNullOrEmpty(id))
                {
                    return new OkObjectResult(new ResponseModel() { Data = "Id was not supplied", Success = false });
                }
                CloudStorageAccount storageAcc = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=tablestorage1;AccountKey=mvdDaDEswE16qPIWefkIjcpiNjpvC8GbXEglDBjrMKItK0QsFXFxr0SwNSjIdzdKeDrShIZ6abHw+AStfRWs5A==;EndpointSuffix=core.windows.net");

                //// create table client
                CloudTableClient tblclient = storageAcc.CreateCloudTableClient(new TableClientConfiguration());

                // get customer table
                CloudTable cloudTable = tblclient.GetTableReference("Supplier");

                TableOperation retrieveOperationForDelete = TableOperation.Retrieve<Supplier>(id, id);
                TableResult retrievedResultForDelete = cloudTable.Execute(retrieveOperationForDelete);
                Supplier supplier = (Supplier)retrievedResultForDelete.Result;
                if (supplier == null)
                {
                    throw new Exception("Supplier Not Found");

                }

                TableOperation deleteTableOperation = TableOperation.Delete(supplier);
                TableResult deleteOrMergeTableResult = await cloudTable.ExecuteAsync(deleteTableOperation);



                return new OkObjectResult(new ResponseModel() { Data = supplier, Success = true });
            }
            catch (Exception e)
            {
                return new OkObjectResult(new ResponseModel() { Data = e.Message, Success = true });
            }
        }
    }
}
